#!/usr/bin/env dotnet
// Análise cruzada de termos: contrasta estudos excluídos com incluídos/controles para derivar
// candidatos a termos de exclusão da busca e valida as exclusões adotadas no protocolo.
// dotnet run scripts/analise_termos.cs -- analisar --protocolo json --exclusoes json --rotulos pasta --dados pasta
#:property Nullable=enable
#:property PublishAot=false
#:property InvariantGlobalization=false

using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;

Console.OutputEncoding = Encoding.UTF8;
if (args.Length == 0 || args[0] != "analisar")
{
    Console.Error.WriteLine("Uso: analise_termos.cs analisar --protocolo json --exclusoes json --rotulos pasta --dados pasta");
    return 2;
}
var valores = new Dictionary<string, string>(StringComparer.Ordinal);
for (int i = 1; i < args.Length; i += 2)
{
    if (i + 1 >= args.Length || args[i] is not ("--protocolo" or "--exclusoes" or "--rotulos" or "--dados") || !valores.TryAdd(args[i][2..], args[i + 1]))
        throw new ArgumentException($"Opção inválida ou repetida: {args[i]}");
}
foreach (var obrigatoria in new[] { "protocolo", "exclusoes", "rotulos", "dados" })
    if (!valores.ContainsKey(obrigatoria)) throw new ArgumentException($"Informe --{obrigatoria}.");

var analise = new Analise(Path.GetFullPath(valores["protocolo"]), Path.GetFullPath(valores["exclusoes"]), Path.GetFullPath(valores["rotulos"]), Path.GetFullPath(valores["dados"]));
await analise.GarantirControles();
return analise.Executar();

sealed class Analise
{
    // Parâmetros da regra de candidatura; ficam no relatório para auditoria.
    const int MinimoNegativos = 3;
    static readonly JsonSerializerOptions Json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        PropertyNameCaseInsensitive = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        WriteIndented = true
    };
    static readonly XNamespace Atom = "http://www.w3.org/2005/Atom";
    static readonly UTF8Encoding Utf8 = new(false);
    static readonly HashSet<string> Vazias = new(StringComparer.Ordinal)
    {
        "a","an","the","and","or","of","to","in","on","for","with","by","from","as","at","is","are","was","were","be","been",
        "this","that","these","those","it","its","we","our","their","they","which","while","into","via","than","such","can",
        "not","but","also","both","each","more","most","over","under","across","between","through","without","within","using",
        "based","use","used","new","however","where","when","how","what","whether","only","has","have","had","do","does","may",
        "first","show","shows","propose","proposed","present","introduce","results","approach","method","methods","paper","work"
    };

    readonly string protocoloPath, exclusoesPath, rotulos, dados, pastaSaida, controlesXml;
    readonly Protocolo protocolo;
    readonly List<Exclusao> exclusoes;
    readonly List<Rejeitado> rejeitados;

    public Analise(string protocoloPath, string exclusoesPath, string rotulos, string dados)
    {
        this.protocoloPath = protocoloPath;
        this.exclusoesPath = exclusoesPath;
        this.rotulos = rotulos;
        this.dados = dados;
        pastaSaida = Path.Combine(dados, "analise_termos");
        controlesXml = Path.Combine(pastaSaida, "controles.xml");
        protocolo = JsonSerializer.Deserialize<Protocolo>(File.ReadAllText(protocoloPath, Utf8), Json)
            ?? throw new InvalidDataException("Protocolo vazio.");
        if (protocolo.Controles.Count == 0) throw new InvalidDataException("O protocolo não define controles.");
        var arquivo = JsonSerializer.Deserialize<ArquivoExclusoes>(File.ReadAllText(exclusoesPath, Utf8), Json)
            ?? throw new InvalidDataException("Arquivo de exclusões vazio.");
        exclusoes = arquivo.Exclusoes;
        rejeitados = arquivo.Rejeitados;
        foreach (var e in exclusoes)
            if (!Regex.IsMatch(e.Codigo, @"^E[2-9]$") || e.Campo != "ti" || e.Termos.Count == 0 || string.IsNullOrWhiteSpace(e.Justificativa))
                throw new InvalidDataException($"Exclusão inválida: {e.Id}. Exija código E2–E9, campo ti|abs, termos e justificativa.");
    }

    static string Hash(string path) => Convert.ToHexStringLower(SHA256.HashData(File.ReadAllBytes(path)));
    static string Limpar(string? t) => Regex.Replace(t ?? "", @"\s+", " ").Trim();

    // Snapshot único dos controles (resumos do arXiv); reutilizado nas execuções seguintes.
    public async Task GarantirControles()
    {
        Directory.CreateDirectory(pastaSaida);
        if (File.Exists(controlesXml)) return;
        using var cliente = new HttpClient { Timeout = TimeSpan.FromMinutes(2) };
        cliente.DefaultRequestHeaders.UserAgent.ParseAdd("tcc-mapeamento-focado/1.0");
        string ids = string.Join(",", protocolo.Controles.Select(c => c.IdArxiv));
        string xml = await cliente.GetStringAsync($"https://export.arxiv.org/api/query?id_list={ids}&max_results={protocolo.Controles.Count}");
        File.WriteAllText(controlesXml, xml, Utf8);
        Console.WriteLine($"Snapshot dos controles gravado: {controlesXml}");
    }

    sealed record Doc(string Key, string Titulo, string Resumo, string Grupo, string Codigos);

    List<Doc> CarregarGrupos()
    {
        var candidatos = File.ReadLines(Path.Combine(rotulos, "candidatos.jsonl"), Utf8).Where(l => l.Length > 0)
            .Select(l => JsonSerializer.Deserialize<Registro>(l, Json)!).ToDictionary(r => r.Key, StringComparer.Ordinal);
        var decisoes = File.ReadLines(Path.Combine(rotulos, "decisoes", "llm_focado.jsonl"), Utf8).Where(l => l.Length > 0)
            .Select(l => JsonSerializer.Deserialize<Decisao>(l, Json)!).ToDictionary(d => d.Key, StringComparer.Ordinal);
        var revisoes = JsonSerializer.Deserialize<List<Revisao>>(File.ReadAllText(Path.Combine(rotulos, "revisao_final.json"), Utf8), Json)!
            .ToDictionary(r => r.Key, StringComparer.Ordinal);

        var feed = XDocument.Load(controlesXml).Root!;
        var positivos = new Dictionary<string, Doc>(StringComparer.Ordinal);
        foreach (var entrada in feed.Elements(Atom + "entry"))
        {
            string id = Regex.Replace(entrada.Element(Atom + "id")!.Value.Split('/').Last(), @"v\d+$", "");
            positivos["arxiv:" + id] = new Doc("arxiv:" + id, Limpar(entrada.Element(Atom + "title")?.Value),
                Limpar(entrada.Element(Atom + "summary")?.Value), "POS", "controle");
        }
        var faltando = protocolo.Controles.Where(c => !positivos.ContainsKey("arxiv:" + c.IdArxiv)).ToList();
        if (faltando.Count > 0) throw new InvalidDataException("Controles ausentes no snapshot: " + string.Join(", ", faltando.Select(c => c.IdArxiv)));
        foreach (var r in revisoes.Values.Where(r => r.Incluir))
            if (!positivos.ContainsKey(r.Key)) throw new InvalidDataException($"Incluído {r.Key} não está entre os controles do protocolo.");

        var docs = positivos.Values.ToList();
        foreach (var c in candidatos.Values)
        {
            if (positivos.ContainsKey(c.Key)) continue;
            var d = decisoes.GetValueOrDefault(c.Key) ?? throw new InvalidDataException($"Sem decisão para {c.Key}.");
            string grupo, codigos;
            if (revisoes.TryGetValue(c.Key, out var rev)) { grupo = "NEG_REV"; codigos = "revisão"; }
            else if (d.Decision == "EXCLUDE") { grupo = "NEG_LLM"; codigos = string.Join("+", d.Codes); }
            else { grupo = "INCERTO"; codigos = d.Decision; }
            docs.Add(new Doc(c.Key, c.Title, c.Abstract, grupo, codigos));
        }
        return docs;
    }

    static List<string> Tokens(string texto) =>
        Regex.Matches(texto.ToLowerInvariant(), @"\d+(?:\.\d+)?[a-z]*|[a-z]+").Select(m => m.Value).ToList();

    static HashSet<string> Termos(string texto)
    {
        var t = Tokens(texto);
        var termos = new HashSet<string>(StringComparer.Ordinal);
        for (int i = 0; i < t.Count; i++)
        {
            if (Vazias.Contains(t[i]) || t[i].Length < 3) continue;
            termos.Add(t[i]);
            if (i + 1 < t.Count && !Vazias.Contains(t[i + 1]) && t[i + 1].Length >= 3) termos.Add(t[i] + " " + t[i + 1]);
        }
        return termos;
    }

    // Emula a correspondência de um termo do protocolo: frases, hífen como espaço e curinga final.
    static Regex Padrao(string termo)
    {
        bool curinga = termo.EndsWith('*');
        var partes = termo.TrimEnd('*').ToLowerInvariant().Split([' ', '-'], StringSplitOptions.RemoveEmptyEntries).Select(Regex.Escape);
        return new Regex(@"(?<![a-z0-9])" + string.Join(@"[\s\-]+", partes) + (curinga ? @"[a-z0-9]*" : "") + @"(?![a-z0-9])",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    }

    public static bool Casa(Exclusao e, string titulo, string resumo)
    {
        string alvo = e.Campo == "ti" ? titulo : titulo + " . " + resumo;
        return e.Termos.Any(t => Padrao(t).IsMatch(alvo));
    }
    static bool Casa(Exclusao e, Doc d) => Casa(e, d.Titulo, d.Resumo);

    // Um termo adotado precisa constar da lista de candidatos (ou ser prefixo, com curinga, de um candidato).
    // Exclusão aplicada só ao título pode usar candidato de ti+abs, pois casa um subconjunto dos mesmos documentos.
    static bool Rastreavel(string termo, string campo, IEnumerable<(string Campo, string Termo)> candidatos)
    {
        bool curinga = termo.EndsWith('*');
        string baseTermo = Regex.Replace(termo.TrimEnd('*').ToLowerInvariant(), @"[\s\-]+", " ");
        return candidatos.Any(c => (c.Campo == campo || campo == "ti") &&
            (curinga ? c.Termo.StartsWith(baseTermo, StringComparison.Ordinal) : c.Termo == baseTermo));
    }

    public int Executar()
    {
        var docs = CarregarGrupos();
        string[] grupos = ["POS", "NEG_LLM", "NEG_REV", "INCERTO"];
        var tamanhos = grupos.ToDictionary(g => g, g => docs.Count(d => d.Grupo == g));
        var chavesPos = docs.Where(d => d.Grupo == "POS").Select(d => d.Key).ToHashSet(StringComparer.Ordinal);
        var poolV4 = File.ReadLines(Path.Combine(dados, "candidatos.jsonl"), Utf8).Where(l => l.Length > 0)
            .Select(l => JsonSerializer.Deserialize<Registro>(l, Json)!).ToList();
        var rotuloPorChave = docs.ToDictionary(d => d.Key, d => d.Grupo, StringComparer.Ordinal);

        var linhas = new List<(string Campo, string Termo, Dictionary<string, int> N, int V4, string Codigos)>();
        foreach (var campo in new[] { "ti", "ti+abs" })
        {
            string Texto(string ti, string ab) => campo == "ti" ? ti : ti + " . " + ab;
            var contagem = new Dictionary<string, Dictionary<string, int>>(StringComparer.Ordinal);
            var codigos = new Dictionary<string, Dictionary<string, int>>(StringComparer.Ordinal);
            foreach (var d in docs)
                foreach (var termo in Termos(Texto(d.Titulo, d.Resumo)))
                {
                    if (!contagem.TryGetValue(termo, out var n)) contagem[termo] = n = grupos.ToDictionary(g => g, _ => 0);
                    n[d.Grupo]++;
                    if (d.Grupo.StartsWith("NEG", StringComparison.Ordinal))
                    {
                        if (!codigos.TryGetValue(termo, out var c)) codigos[termo] = c = new(StringComparer.Ordinal);
                        c[d.Codigos] = c.GetValueOrDefault(d.Codigos) + 1;
                    }
                }
            var v4 = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var r in poolV4.Where(r => !chavesPos.Contains(r.Key)))
                foreach (var termo in Termos(Texto(r.Title, r.Abstract))) v4[termo] = v4.GetValueOrDefault(termo) + 1;
            foreach (var (termo, n) in contagem)
            {
                int neg = n["NEG_LLM"] + n["NEG_REV"];
                if (n["POS"] == 0 && neg >= MinimoNegativos && n["INCERTO"] <= neg)
                    linhas.Add((campo, termo, n, v4.GetValueOrDefault(termo), string.Join(", ", codigos[termo]
                        .OrderByDescending(k => k.Value).ThenBy(k => k.Key, StringComparer.Ordinal).Select(k => $"{k.Key}:{k.Value}"))));
            }
        }
        linhas = linhas.OrderBy(l => l.Campo, StringComparer.Ordinal)
            .ThenByDescending(l => l.N["NEG_LLM"] + l.N["NEG_REV"]).ThenBy(l => l.N["INCERTO"]).ThenBy(l => l.Termo, StringComparer.Ordinal).ToList();

        var csv = new StringBuilder("campo,termo,pos,neg_llm,neg_rev,incerto,pool_v4,codigos_neg\n");
        foreach (var l in linhas)
            csv.Append($"{l.Campo},\"{l.Termo}\",{l.N["POS"]},{l.N["NEG_LLM"]},{l.N["NEG_REV"]},{l.N["INCERTO"]},{l.V4},\"{l.Codigos}\"\n");
        File.WriteAllText(Path.Combine(pastaSaida, "candidatos_exclusao.csv"), csv.ToString(), Utf8);

        var b = new StringBuilder();
        void L(string s = "") => b.Append(s).Append('\n');
        L("# Análise cruzada de termos para exclusão na busca");
        L();
        L($"- Protocolo de busca: `{Path.GetFileName(protocoloPath)}` (SHA-256 `{Hash(protocoloPath)}`).");
        L($"- Exclusões adotadas: `{Path.GetFileName(exclusoesPath)}` (SHA-256 `{Hash(exclusoesPath)}`).");
        L($"- Rótulos: `{Path.GetFileName(rotulos)}`; snapshot dos controles SHA-256 `{Hash(controlesXml)}`; pool v4: {poolV4.Count} registros.");
        L($"- Grupos: POS = incluídos na revisão anterior ∪ controles ({tamanhos["POS"]}); NEG_LLM = EXCLUDE da triagem ({tamanhos["NEG_LLM"]}); " +
          $"NEG_REV = excluídos na revisão manual ({tamanhos["NEG_REV"]}); INCERTO = CONTEXT/CORE sem revisão ({tamanhos["INCERTO"]}).");
        L($"- Regra de candidatura: termo (palavra ou bigrama sem palavras vazias) ausente de todo POS, presente em ≥ {MinimoNegativos} documentos NEG " +
          "e não mais frequente em INCERTO do que em NEG. Contagem por documento; `pool_v4` conta o termo nos registros da nova coleta (sem controles).");
        L("- Regra de adoção: o termo deve constar da lista de candidatos (ou ser prefixo com curinga de um candidato), denotar um critério E2–E9 " +
          "sem outro sentido usual em PLN e não remover nenhum POS. Exclusões aplicam-se ao título; em teste, termos aplicados ao resumo removeram " +
          "estudos de memória textual que só mencionavam o termo de passagem. A validação é executada pelo script.");
        L();

        int retorno = 0;
        if (rejeitados.Count > 0)
        {
            L("## Candidatos examinados e rejeitados");
            L();
            L("| Termo | Motivo |");
            L("|---|---|");
            foreach (var r in rejeitados) L($"| {r.Termo} | {r.Motivo} |");
            L();
        }
        L("## Validação das exclusões adotadas");
        L();
        var candidatosChave = linhas.Select(l => (l.Campo, l.Termo)).ToList();
        L("| Id | Código | Campo | Termos | POS | NEG_LLM | NEG_REV | INCERTO | Pool v4 |");
        L("|---|---|---|---|---:|---:|---:|---:|---:|");
        foreach (var e in exclusoes)
        {
            foreach (var t in e.Termos.Where(t => !Rastreavel(t, e.Campo, candidatosChave)))
            {
                Console.Error.WriteLine($"Termo não rastreável à lista de candidatos: {e.Id} {e.Campo} '{t}'.");
                retorno = 1;
            }
            var atingidos = docs.Where(d => Casa(e, d)).ToList();
            int Q(string g) => atingidos.Count(d => d.Grupo == g);
            int noPool = poolV4.Count(r => Casa(e, r.Title, r.Abstract));
            L($"| {e.Id} | {e.Codigo} | {e.Campo} | {string.Join(", ", e.Termos)} | {Q("POS")} | {Q("NEG_LLM")} | {Q("NEG_REV")} | {Q("INCERTO")} | {noPool} |");
            if (Q("POS") > 0) retorno = 1;
        }
        var removidos = docs.Where(d => exclusoes.Any(e => Casa(e, d))).ToList();
        var removidosPool = poolV4.Where(r => exclusoes.Any(e => Casa(e, r.Title, r.Abstract))).ToList();
        L();
        L("Efeito conjunto (um documento conta uma vez):");
        L();
        foreach (var g in grupos) L($"- {g}: {removidos.Count(d => d.Grupo == g)} de {tamanhos[g]} removidos.");
        L($"- Pool v4: {removidosPool.Count} de {poolV4.Count} removidos; restam {poolV4.Count - removidosPool.Count}.");
        if (removidosPool.Any(r => chavesPos.Contains(r.Key))) retorno = 1;
        L();
        L("Justificativas:");
        L();
        foreach (var e in exclusoes) L($"- **{e.Id} ({e.Codigo}, {e.Campo})**: {e.Justificativa}");
        L();
        L("### Registros removidos do pool v4");
        L();
        L("| Chave | Rótulo anterior | Exclusões | Título |");
        L("|---|---|---|---|");
        foreach (var r in removidosPool)
            L($"| `{r.Key}` | {rotuloPorChave.GetValueOrDefault(r.Key, "novo")} | {string.Join(", ", exclusoes.Where(e => Casa(e, r.Title, r.Abstract)).Select(e => e.Id))} | {r.Title.Replace("|", "/")} |");
        if (retorno != 0) L("\n**FALHA: exclusão não rastreável ou que remove positivo.**");
        L();
        L("## Candidatos (todos)");
        L();
        L("| Campo | Termo | POS | NEG_LLM | NEG_REV | INCERTO | Pool v4 | Códigos nos NEG |");
        L("|---|---|---:|---:|---:|---:|---:|---|");
        foreach (var l in linhas)
            L($"| {l.Campo} | {l.Termo} | {l.N["POS"]} | {l.N["NEG_LLM"]} | {l.N["NEG_REV"]} | {l.N["INCERTO"]} | {l.V4} | {l.Codigos} |");

        File.WriteAllText(Path.Combine(pastaSaida, "analise_termos.md"), b.ToString(), Utf8);
        var filtrados = poolV4.Where(r => !removidosPool.Contains(r)).Select(r => r.Key).ToList();
        File.WriteAllLines(Path.Combine(pastaSaida, "chaves_apos_exclusao.txt"), filtrados, Utf8);
        Console.WriteLine($"Candidatos: {linhas.Count}. Pool v4: {poolV4.Count} → {filtrados.Count} após exclusões. Relatório: {Path.Combine(pastaSaida, "analise_termos.md")}");
        if (retorno != 0) Console.Error.WriteLine("Falha na validação das exclusões.");
        return retorno;
    }
}

sealed class Protocolo { public List<Controle> Controles { get; set; } = []; }
sealed class Controle { public string Nome { get; set; } = ""; public string IdArxiv { get; set; } = ""; }
sealed class ArquivoExclusoes { public List<Exclusao> Exclusoes { get; set; } = []; public List<Rejeitado> Rejeitados { get; set; } = []; }
sealed class Rejeitado { public string Termo { get; set; } = ""; public string Motivo { get; set; } = ""; }
sealed class Exclusao
{
    public string Id { get; set; } = "";
    public string Codigo { get; set; } = "";
    public string Campo { get; set; } = "";
    public List<string> Termos { get; set; } = [];
    public string Justificativa { get; set; } = "";
}
sealed class Registro { public string Key { get; set; } = ""; public string Title { get; set; } = ""; public string Abstract { get; set; } = ""; }
sealed class Decisao { public string Key { get; set; } = ""; public string Decision { get; set; } = ""; public List<string> Codes { get; set; } = []; }
sealed class Revisao { public string Key { get; set; } = ""; public bool Incluir { get; set; } }