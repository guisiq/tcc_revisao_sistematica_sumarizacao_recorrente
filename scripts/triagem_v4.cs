#!/usr/bin/env dotnet
// Triagem codificada v4: o modelo atribui códigos (C1–C7, E2–E10) e o script classifica por regras fixas.
// Uso (a partir da raiz do repositório):
//   dotnet run scripts/triagem_v4.cs -- resumo   [--estimar] [--limite-creditos <n>] [--paralelo <n>] [--max-requisicoes <n>]
//   dotnet run scripts/triagem_v4.cs -- textos   (baixa o texto integral dos grupos A, B e P da etapa 1)
//   dotnet run scripts/triagem_v4.cs -- texto    [--estimar] [--limite-creditos <n>] [--paralelo <n>] [--max-requisicoes <n>]
//   dotnet run scripts/triagem_v4.cs -- relatorio [--saida <pasta>]   (offline e determinístico)
// Parâmetros: protocolo/triagem_v4.json; livro de códigos e regras: protocolo/codigos_v4.md.
#:package GitHub.Copilot.SDK@1.0.31
#:package AngleSharp@1.10.5
#:package PdfPig@0.1.16
#:property Nullable=enable
#:property PublishAot=false
#:property NoWarn=GHCP001

using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using AngleSharp.Html.Parser;
using GitHub.Copilot;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

Console.OutputEncoding = Encoding.UTF8;
const double TETO_ABSOLUTO = 1000;
const double LIMITE_MINIMO_SESSAO = 30;
const int MAX_TENTATIVAS = 3;
var ci = CultureInfo.InvariantCulture;

var comando = args.Length > 0 && !args[0].StartsWith("--") ? args[0] : "";
var a = Args.Ler(args);
var prot = JsonNode.Parse(File.ReadAllText(a.Get("protocolo", "protocolo/triagem_v4.json")))!;
var dados = Path.GetFullPath(a.Get("dados", (string)prot["dados"]!));
var dirTri = Path.Combine(dados, "triagem_v4");
var dirTextos = Path.Combine(dirTri, "textos");
var arqEtapa1 = Path.Combine(dirTri, "etapa1.jsonl");
var arqEtapa2 = Path.Combine(dirTri, "etapa2.jsonl");
var arqTextos = Path.Combine(dirTextos, "manifesto.jsonl");
var livro = File.ReadAllText((string)prot["livro_codigos"]!);
var roteiro = Entre(livro, "<!-- roteiro:inicio -->", "<!-- roteiro:fim -->");
var saidaJson = new JsonSerializerOptions { Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping };
var jsonOpts = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

// Registros submetidos à triagem: candidatos que passaram pela exclusão por termos de título.
var chavesTriagem = File.ReadLines(Path.Combine(dados, (string)prot["chaves_triagem"]!)).Select(l => l.Trim()).Where(l => l.Length > 0).ToHashSet();
var candidatos = File.ReadLines(Path.Combine(dados, "candidatos.jsonl")).Where(l => l.Trim().Length > 0)
    .Select(l => JsonNode.Parse(l)!.AsObject()).ToList();
var registros = candidatos.Where(o => chavesTriagem.Contains((string)o["key"]!))
    .OrderBy(o => (string)o["key"]!, StringComparer.Ordinal).ToDictionary(o => (string)o["key"]!);
if (registros.Count != chavesTriagem.Count) throw new InvalidOperationException($"Chaves sem registro: {chavesTriagem.Count - registros.Count}.");

switch (comando)
{
    case "resumo": return await Etapa1();
    case "textos": return await Textos();
    case "texto": return await Etapa2();
    case "relatorio": return Relatorio();
    default:
        Console.Error.WriteLine("Comando: resumo | textos | texto | relatorio");
        return 1;
}

// ---------- Etapa 1: título e resumo ----------
async Task<int> Etapa1()
{
    var e1 = prot["etapa1"]!;
    var porReq = int.Parse(a.Get("por-requisicao", e1["registros_por_requisicao"]!.ToString()));
    var feitas = LerJsonl<Cod1>(arqEtapa1).Select(c => c.Key!).ToHashSet();
    var pend = registros.Keys.Where(k => !feitas.Contains(k)).ToList();
    var pedidos = new List<Pedido>();
    for (int i = 0; i < pend.Count; i += porReq)
    {
        var bloco = pend.Skip(i).Take(porReq).ToList();
        pedidos.Add(new Pedido($"etapa1_{i / porReq:D3}", bloco,
            rest => "Codifique os registros abaixo (título e resumo) segundo o roteiro. Devolva exatamente um item por chave em `codificacoes`. "
                + "Os campos c1 a c7 recebem somente S, I ou N. `exclusoes` lista os códigos E2 a E10 que forem objeto principal do estudo, ou []. "
                + "`nota` justifica em até 200 caracteres, em português.\n\n"
                + string.Join("\n", rest.Select(k => new JsonObject { ["key"] = k, ["title"] = (string)registros[k]["title"]!, ["abstract"] = (string)registros[k]["abstract"]! }.ToJsonString(saidaJson))),
            bloco.Sum(k => ((string)registros[k]["title"]!).Length + ((string)registros[k]["abstract"]!).Length + 60) + 400,
            bloco.Count * (int)e1["tokens_saida_estimados_por_registro"]!));
    }
    Console.WriteLine($"Etapa 1: {pend.Count} registros pendentes de {registros.Count}, em {pedidos.Count} requisições.");
    return await RodarLlm<Resp1>("etapa1", arqEtapa1, pedidos, (double)e1["limite_creditos"]!, (resp, rest) =>
    {
        var ok = new List<(string, string)>();
        foreach (var c in resp?.Codificacoes ?? [])
        {
            if (c.Key is null || !rest.Contains(c.Key) || ok.Any(x => x.Item1 == c.Key) || !Normalizar1(c))
            {
                Console.Error.WriteLine($"  resposta inválida ou chave desconhecida: {JsonSerializer.Serialize(c, saidaJson)}");
                continue;
            }
            ok.Add((c.Key, JsonSerializer.Serialize(c, saidaJson)));
        }
        return ok;
    });
}

// ---------- Texto integral ----------
async Task<int> Textos()
{
    var t = prot["texto_integral"]!;
    var intervalo = TimeSpan.FromSeconds((double)t["intervalo_segundos"]!);
    var grupos = GruposEtapa1();
    var alvo = grupos.Where(g => g.Value.grupo is "A" or "B" or "P").Select(g => g.Key).OrderBy(k => k, StringComparer.Ordinal).ToList();
    var manifesto = LerJsonl<JsonObject>(arqTextos).GroupBy(o => (string)o["key"]!).ToDictionary(g => g.Key, g => g.First());
    // Já obtido = registrado no manifesto e com o arquivo presente; registrados sem arquivo são reconstituídos e conferidos pelo SHA-256.
    var feitos = manifesto.Where(kv => File.Exists(Path.Combine(dirTextos, (string)kv.Value["arquivo"]!))).Select(kv => kv.Key).ToHashSet();
    Directory.CreateDirectory(dirTextos);
    using var http = new HttpClient { Timeout = TimeSpan.FromMinutes(2) };
    http.DefaultRequestHeaders.UserAgent.ParseAdd("tcc-revisao-sistematica/4.0 (mapeamento academico)");
    var ultimo = DateTime.MinValue;
    async Task<HttpResponseMessage> Get(string url)
    {
        var espera = ultimo + intervalo - DateTime.UtcNow;
        if (espera > TimeSpan.Zero) await Task.Delay(espera);
        ultimo = DateTime.UtcNow;
        return await http.GetAsync(url);
    }
    Console.WriteLine($"Texto integral: {alvo.Count} estudos (A, B e P); {alvo.Count(feitos.Contains)} já obtidos.");
    int falhas = 0;
    foreach (var k in alvo.Where(k => !feitos.Contains(k)))
    {
        var r = registros[k];
        var id = (string)r["id"]!; var versao = (string)r["versao"]!;
        string? texto = null, fonte = null, url = null, shaBruto = null;
        url = ((string)t["url_html"]!).Replace("{id}", id).Replace("{versao}", versao);
        using (var resp = await Get(url))
        {
            if (resp.IsSuccessStatusCode)
            {
                var html = await resp.Content.ReadAsStringAsync();
                if (html.Contains("ltx_document"))
                {
                    texto = TextoHtml(html); fonte = "html";
                    shaBruto = Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(html)));
                }
            }
        }
        if (texto is null || texto.Length < 2000)
        {
            url = ((string)t["url_pdf"]!).Replace("{id}", id).Replace("{versao}", versao);
            using var resp = await Get(url);
            if (resp.IsSuccessStatusCode)
            {
                var bytes = await resp.Content.ReadAsByteArrayAsync();
                try { texto = TextoPdf(bytes); fonte = "pdf"; shaBruto = Convert.ToHexStringLower(SHA256.HashData(bytes)); }
                catch (Exception ex) { Console.Error.WriteLine($"  {k}: PDF ilegível ({ex.Message})"); texto = null; }
            }
        }
        if (texto is null || texto.Length < 2000)
        {
            falhas++;
            Console.Error.WriteLine($"  {k}: texto integral não obtido.");
            continue;
        }
        var arq = $"{id}{versao}.txt";
        var shaTexto = Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(texto)));
        File.WriteAllText(Path.Combine(dirTextos, arq), texto, new UTF8Encoding(false));
        if (manifesto.TryGetValue(k, out var reg))
        {
            var igual = (string)reg["sha256_texto"]! == shaTexto;
            if (!igual) falhas++;
            Console.WriteLine($"  {k}: reconstituído ({fonte}); SHA-256 {(igual ? "confere" : "DIVERGE do manifesto")}");
            continue;
        }
        File.AppendAllText(arqTextos, new JsonObject
        {
            ["key"] = k, ["arquivo"] = arq, ["fonte"] = fonte, ["url"] = url, ["caracteres"] = texto.Length,
            ["sha256_texto"] = shaTexto,
            ["sha256_original"] = shaBruto, ["data_utc"] = DateTime.UtcNow.ToString("o"),
        }.ToJsonString() + "\n");
        Console.WriteLine($"  {k}: {fonte}, {texto.Length:N0} caracteres");
    }
    return falhas > 0 ? 2 : 0;
}

static string TextoHtml(string html)
{
    var doc = new HtmlParser().ParseDocument(html);
    var art = doc.QuerySelector("article.ltx_document") ?? doc.Body!;
    foreach (var e in art.QuerySelectorAll(".ltx_bibliography, nav, .ltx_page_footer, script, style, button").ToList()) e.Remove();
    foreach (var m in art.QuerySelectorAll("math").ToList())
        m.Replace(doc.CreateTextNode(" " + (m.GetAttribute("alttext") ?? m.TextContent) + " "));
    foreach (var e in art.QuerySelectorAll("td, th").ToList()) e.After(doc.CreateTextNode(" | "));
    foreach (var e in art.QuerySelectorAll("p, h1, h2, h3, h4, h5, h6, li, tr, figcaption, br, .ltx_para, .ltx_title, .ltx_listingline, .ltx_caption").ToList())
        e.After(doc.CreateTextNode("\n"));
    return Limpar(art.TextContent);
}

static string TextoPdf(byte[] bytes)
{
    using var pdf = PdfDocument.Open(bytes);
    var sb = new StringBuilder();
    foreach (var p in pdf.GetPages()) sb.AppendLine(ContentOrderTextExtractor.GetText(p));
    var linhas = Limpar(sb.ToString()).Split('\n').ToList();
    // Remove a bibliografia (do último título "References" até o apêndice seguinte, se houver).
    var refs = linhas.FindLastIndex(l => Regex.IsMatch(l.Trim(), @"^(\d+\.?\s*)?(references|bibliography)$", RegexOptions.IgnoreCase));
    if (refs > linhas.Count * 0.3)
    {
        var apendice = linhas.FindIndex(refs + 1, l => Regex.IsMatch(l.Trim(), @"^(appendix|appendices|supplementary)\b|^A\s+[A-Z][a-z]+( [A-Za-z]+){0,6}$", RegexOptions.IgnoreCase));
        linhas.RemoveRange(refs, (apendice > refs ? apendice : linhas.Count) - refs);
    }
    return string.Join("\n", linhas);
}

static string Limpar(string s)
{
    var linhas = s.Normalize(NormalizationForm.FormKC).Replace("\r", "").Split('\n')
        .Select(l => Regex.Replace(l, @"[ \t\u00a0]+", " ").Trim());
    var sb = new StringBuilder();
    bool vazia = false;
    foreach (var l in linhas)
    {
        if (l.Length == 0) { if (!vazia && sb.Length > 0) sb.Append('\n'); vazia = true; continue; }
        sb.Append(l).Append('\n'); vazia = false;
    }
    return sb.ToString().Trim();
}

// ---------- Etapa 2: texto integral ----------
async Task<int> Etapa2()
{
    var e2 = prot["etapa2"]!;
    var max = (int)e2["max_caracteres_texto"]!;
    var grupos = GruposEtapa1();
    var textos = LerJsonl<JsonObject>(arqTextos).ToDictionary(o => (string)o["key"]!);
    var feitas = LerJsonl<Cod2>(arqEtapa2).Select(c => c.Key!).ToHashSet();
    var alvo = grupos.Where(g => g.Value.grupo is "A" or "B" or "P").Select(g => g.Key).OrderBy(k => k, StringComparer.Ordinal).ToList();
    var semTexto = alvo.Where(k => !textos.ContainsKey(k)).ToList();
    if (semTexto.Count > 0) Console.WriteLine($"Aviso: {semTexto.Count} estudos sem texto integral (execute `textos`): {string.Join(", ", semTexto)}");
    var pedidos = new List<Pedido>();
    foreach (var k in alvo.Where(k => textos.ContainsKey(k) && !feitas.Contains(k)))
    {
        var r = registros[k];
        var integral = File.ReadAllText(Path.Combine(dirTextos, (string)textos[k]["arquivo"]!));
        var truncado = integral.Length > max;
        var corpo = truncado ? integral[..max] : integral;
        var prompt = "Codifique o estudo abaixo com base no texto integral, segundo o roteiro. "
            + "Para cada característica c1 a c7, devolva `valor` (S, I ou N) e `trecho`. Quando valor = S, `trecho` deve ser uma cópia literal e contígua "
            + $"do texto (entre {(int)e2["tamanho_minimo_trecho"]!} e {(int)e2["tamanho_maximo_trecho"]!} caracteres, sem reticências nem paráfrase) que sustente o valor; "
            + "sem trecho literal, use I. Em c5, o trecho deve nomear o modelo que escreve ou atualiza o estado e seu tamanho. "
            + "Em `exclusoes`, liste somente os códigos E2 a E10 que forem objeto principal do estudo, cada um com trecho literal; caso contrário, []. "
            + "`nota` resume em até 300 caracteres, em português, o que o estudo faz com a memória ou o estado.\n\n"
            + $"Chave: {k}\nTítulo: {(string)r["title"]!}\nResumo: {(string)r["abstract"]!}\n"
            + $"Texto integral (fonte {(string)textos[k]["fonte"]!}{(truncado ? $"; truncado nos primeiros {max:N0} caracteres" : "")}):\n<<<\n{corpo}\n>>>";
        pedidos.Add(new Pedido($"etapa2_{k}", [k], _ => prompt, prompt.Length, (int)e2["tokens_saida_estimados"]!));
    }
    Console.WriteLine($"Etapa 2: {pedidos.Count} estudos pendentes de {alvo.Count} (A, B e P).");
    return await RodarLlm<Cod2>("etapa2", arqEtapa2, pedidos, (double)e2["limite_creditos"]!, (resp, rest) =>
    {
        if (resp is null || rest.Count != 1 || !Normalizar2(resp))
        {
            Console.Error.WriteLine($"  resposta inválida: {JsonSerializer.Serialize(resp, saidaJson)}");
            return [];
        }
        resp.Key = rest.First();
        return [(resp.Key, JsonSerializer.Serialize(resp, saidaJson))];
    });
}

// ---------- Execução no Copilot SDK com controle de custo ----------
async Task<int> RodarLlm<T>(string etapa, string saida, List<Pedido> pedidos, double limiteEtapa,
    Func<T?, HashSet<string>, List<(string chave, string json)>> extrair) where T : class
{
    var llm = prot["llm"]!;
    var limite = Math.Min(TETO_ABSOLUTO, Math.Min(limiteEtapa, double.Parse(a.Get("limite-creditos", limiteEtapa.ToString(ci)), ci)));
    var reserva = (double)llm["reserva_por_requisicao"]!;
    var custoMinimo = (double)llm["custo_minimo_por_requisicao"]!;
    var paralelo = Math.Clamp(int.Parse(a.Get("paralelo", "1")), 1, 4);
    var maxReq = int.Parse(a.Get("max-requisicoes", int.MaxValue.ToString()));
    Directory.CreateDirectory(dirTri);
    var razao = Path.Combine(dirTri, "custo_llm.jsonl");
    double gasto = LerJsonl<JsonObject>(razao).Where(o => (string?)o["etapa"] == etapa).Sum(o => (double)o["custo_contabilizado"]!);
    Console.WriteLine($"Créditos já consumidos em {etapa}: {gasto:F2} de {limite:0.##}.");
    if (pedidos.Count == 0) return 0;

    await using var client = new CopilotClient();
    await client.StartAsync();
    var modelos = (await client.ListModelsAsync())
        .Where(m => m.Billing?.TokenPrices?.InputPrice is not null && m.Billing.TokenPrices.OutputPrice is not null)
        .Where(m => m.Policy is null || string.Equals(m.Policy.State.ToString(), "enabled", StringComparison.OrdinalIgnoreCase))
        .Select(m => (m, preco: m.Billing!.TokenPrices!.InputPrice!.Value + m.Billing.TokenPrices.OutputPrice!.Value))
        .ToList();
    if (modelos.Count == 0) throw new InvalidOperationException("Nenhum modelo com preço por token disponível.");
    var menor = modelos.Min(x => x.preco);
    var baratos = modelos.Where(x => x.preco == menor).Select(x => x.m).OrderBy(m => m.Id, StringComparer.Ordinal).ToList();
    var pedido = a.Get("modelo", "");
    var modelo = pedido.Length == 0 ? baratos[0] : baratos.FirstOrDefault(m => m.Id == pedido)
        ?? throw new InvalidOperationException($"Modelo '{pedido}' não está entre os de menor preço ({string.Join(", ", baratos.Select(m => m.Id))}).");
    var tp = modelo.Billing!.TokenPrices!;
    double lote = tp.BatchSize is > 0 ? tp.BatchSize.Value : 1_000_000;
    double CustoTokens(long entrada, long saidaT, long cacheL, long cacheE) =>
        ((entrada - cacheL) * tp.InputPrice!.Value + cacheL * (tp.CacheReadPrice ?? tp.InputPrice!.Value)
         + cacheE * (tp.CacheWritePrice ?? tp.InputPrice!.Value) + saidaT * tp.OutputPrice!.Value) / lote;
    double Estimar(Pedido p) => Math.Max(CustoTokens((roteiro.Length + p.CaracteresEntrada) / 3, p.TokensSaida, 0, 0) * reserva, custoMinimo);
    var estimativa = pedidos.Sum(Estimar);
    Console.WriteLine($"Modelo: {modelo.Id}. Estimativa: {estimativa:F2} créditos sem novas tentativas; até {MAX_TENTATIVAS * estimativa:F2} com {MAX_TENTATIVAS} tentativas.");
    if (gasto + estimativa > limite) Console.WriteLine($"Aviso: a estimativa excede o teto restante ({limite - gasto:F2}); a execução parará ao atingi-lo.");
    if (a.Has("estimar")) { await client.StopAsync(); return 0; }

    File.WriteAllText(Path.Combine(dirTri, $"_execucao_{etapa}.json"), new JsonObject
    {
        ["data_utc"] = DateTime.UtcNow.ToString("o"), ["modelo"] = modelo.Id, ["sdk"] = "GitHub.Copilot.SDK 1.0.31",
        ["roteiro_sha256"] = Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(roteiro))),
        ["livro_codigos_sha256"] = Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(livro))),
        ["limite_creditos"] = limite, ["requisicoes"] = pedidos.Count,
    }.ToJsonString(new JsonSerializerOptions { WriteIndented = true }));

    var trava = new object();
    double reservado = 0;
    bool parar = false;
    int enviadas = 0, aceitosTotal = 0;
    async Task Processar(Pedido p)
    {
        double est = Math.Max(LIMITE_MINIMO_SESSAO, MAX_TENTATIVAS * Estimar(p));
        lock (trava)
        {
            if (parar || enviadas >= maxReq) return;
            if (gasto + reservado + est > limite)
            {
                parar = true;
                Console.Error.WriteLine($"Saldo insuficiente para reservar {est:F2} créditos (gasto {gasto:F2}, reservado {reservado:F2}, teto {limite:0.##}); interrompendo.");
                return;
            }
            reservado += est; enviadas++;
        }
        long entrada = 0, saidaT = 0, cacheL = 0, cacheE = 0; double reportado = 0;
        var restantes = p.Chaves.ToHashSet();
        var aceitas = new List<(string chave, string json)>();
        try
        {
            await using var sessao = await client.CreateSessionAsync(new SessionConfig
            {
                Model = modelo.Id,
                SystemMessage = new SystemMessageConfig { Mode = SystemMessageMode.Replace, Content = roteiro },
                AvailableTools = [],
                SkipCustomInstructions = true,
                OnPermissionRequest = PermissionHandler.ApproveAll,
                SessionLimits = new SessionLimitsConfig { MaxAiCredits = est },
                OnEvent = ev =>
                {
                    if (ev is AssistantUsageEvent u)
                    {
                        Interlocked.Add(ref entrada, u.Data.InputTokens ?? 0);
                        Interlocked.Add(ref saidaT, u.Data.OutputTokens ?? 0);
                        Interlocked.Add(ref cacheL, u.Data.CacheReadTokens ?? 0);
                        Interlocked.Add(ref cacheE, u.Data.CacheWriteTokens ?? 0);
                        lock (trava) reportado += u.Data.Cost ?? 0;
                    }
                    else if (ev is SessionLimitsExhaustedRequestedEvent)
                    {
                        lock (trava) parar = true;
                        Console.Error.WriteLine("Limite de créditos da sessão esgotado no servidor; interrompendo.");
                    }
                },
            });
            for (int tentativa = 1; tentativa <= MAX_TENTATIVAS && restantes.Count > 0 && !parar; tentativa++)
            {
                try
                {
                    var resp = await sessao.SendAndWaitAsync<T>(new MessageOptions { Prompt = p.Prompt(restantes) }, jsonOpts, TimeSpan.FromMinutes(4));
                    foreach (var x in extrair(resp, restantes)) { aceitas.Add(x); restantes.Remove(x.chave); }
                }
                catch (JsonException ex)
                {
                    Console.Error.WriteLine($"  [{p.Rotulo}] JSON inválido na tentativa {tentativa}: {ex.Message.Split('\n')[0]}");
                }
            }
        }
        finally
        {
            var custo = CustoTokens(entrada, saidaT, cacheL, cacheE);
            var contabilizado = Math.Max(custo, reportado);
            lock (trava)
            {
                reservado -= est; gasto += contabilizado; aceitosTotal += aceitas.Count;
                File.AppendAllLines(saida, aceitas.Select(x => x.json));
                File.AppendAllText(razao, new JsonObject
                {
                    ["data_utc"] = DateTime.UtcNow.ToString("o"), ["etapa"] = etapa, ["modelo"] = modelo.Id, ["pedido"] = p.Rotulo,
                    ["registros"] = p.Chaves.Count, ["codificados"] = aceitas.Count,
                    ["tokens_entrada"] = entrada, ["tokens_saida"] = saidaT, ["tokens_cache_leitura"] = cacheL, ["tokens_cache_escrita"] = cacheE,
                    ["custo_tokens"] = Math.Round(custo, 4), ["custo_reportado"] = Math.Round(reportado, 4), ["custo_contabilizado"] = Math.Round(contabilizado, 4),
                }.ToJsonString() + "\n");
                Console.WriteLine($"  [{p.Rotulo}] {aceitas.Count}/{p.Chaves.Count} codificados; custo {contabilizado:F3}; acumulado {gasto:F2}/{limite:0.##}");
            }
        }
    }
    using (var sem = new SemaphoreSlim(paralelo))
        await Task.WhenAll(pedidos.Select(async p =>
        {
            await sem.WaitAsync();
            try { await Processar(p); } finally { sem.Release(); }
        }));
    var pendentes = pedidos.Sum(p => p.Chaves.Count) - aceitosTotal;
    Console.WriteLine($"Concluído {etapa}: {aceitosTotal} codificados, {pendentes} pendentes, {gasto:F2}/{limite:0.##} créditos.");
    await client.StopAsync();
    return pendentes > 0 ? 2 : 0;
}

// ---------- Regras fixas ----------
static bool Valor(string? v) => v is "S" or "I" or "N";
static bool CodigoE(string? c) => c is "E2" or "E3" or "E4" or "E5" or "E6" or "E7" or "E8" or "E9" or "E10";
static string? Up(string? s) => s?.Trim().ToUpperInvariant();

static bool Normalizar1(Cod1 c)
{
    (c.C1, c.C2, c.C3, c.C4, c.C5, c.C6, c.C7) = (Up(c.C1), Up(c.C2), Up(c.C3), Up(c.C4), Up(c.C5), Up(c.C6), Up(c.C7));
    c.Exclusoes = (c.Exclusoes ?? []).Select(x => Up(x)!).Distinct().OrderBy(x => x, StringComparer.Ordinal).ToList();
    return new[] { c.C1, c.C2, c.C3, c.C4, c.C5, c.C6, c.C7 }.All(Valor) && c.Exclusoes.All(CodigoE)
        && !string.IsNullOrWhiteSpace(c.Nota) && c.Nota.Length <= 400;
}

static bool Normalizar2(Cod2 c)
{
    var ev = c.Caracteristicas();
    if (ev.Any(e => e is null)) return false;
    foreach (var e in ev) e!.Valor = Up(e.Valor);
    c.Exclusoes = (c.Exclusoes ?? []).Where(x => x is not null).ToList();
    foreach (var x in c.Exclusoes) x.Codigo = Up(x.Codigo);
    return ev.All(e => Valor(e!.Valor)) && c.Exclusoes.All(x => CodigoE(x.Codigo)) && !string.IsNullOrWhiteSpace(c.Nota);
}

// Etapa 1 → grupo (A, B, P, C ou X) e motivo.
static (string grupo, string motivo) Grupo1(Cod1 c)
{
    var falhas = new List<string>();
    if (c.Exclusoes!.Count > 0) falhas.AddRange(c.Exclusoes);
    if (c.C1 == "N") falhas.Add("C1=N");
    if (c.C2 == "N") falhas.Add("C2=N");
    if (falhas.Count > 0) return ("X", string.Join(" ", falhas));
    string[] extra = [c.C5!, c.C6!, c.C7!];
    if (c.C3 == "S" && c.C4 == "S" && extra.Contains("S")) return ("A", "");
    // B exige C2=S e C3=S; sem essa base, o incerto forma D e não segue para o texto integral.
    if (c.C3 != "N" && c.C4 != "N" && extra.Any(v => v != "N"))
        return c.C2 == "S" && c.C3 == "S" ? ("B", "") : ("D", string.Join(" ", new[] { ("C2", c.C2), ("C3", c.C3) }.Where(x => x.Item2 != "S").Select(x => $"{x.Item1}={x.Item2}")));
    if (c.C7 != "N" && c.C3 != "N") return ("P", "");
    if (c.C3 == "N") falhas.Add("C3=N");
    if (c.C4 == "N") falhas.Add("C4=N");
    if (extra.All(v => v == "N")) falhas.Add("C5–C7=N");
    return ("C", string.Join(" ", falhas));
}

Dictionary<string, (string grupo, string motivo, Cod1 cod)> GruposEtapa1()
{
    var cods = LerJsonl<Cod1>(arqEtapa1).GroupBy(c => c.Key!).ToDictionary(g => g.Key, g => g.First());
    var faltam = registros.Keys.Count(k => !cods.ContainsKey(k));
    if (faltam > 0) Console.WriteLine($"Aviso: {faltam} registros ainda sem codificação na etapa 1.");
    return cods.Where(kv => registros.ContainsKey(kv.Key)).ToDictionary(kv => kv.Key, kv => { var (g, m) = Grupo1(kv.Value); return (g, m, kv.Value); });
}

static string Norm(string s)
{
    var sb = new StringBuilder();
    foreach (var ch in s.Normalize(NormalizationForm.FormKC).ToLowerInvariant())
        sb.Append(char.IsLetterOrDigit(ch) ? ch : ' ');
    return Regex.Replace(sb.ToString(), " +", " ").Trim();
}

// ---------- Relatório (offline, determinístico) ----------
int Relatorio()
{
    var e2 = prot["etapa2"]!;
    var minTrecho = (int)e2["tamanho_minimo_trecho"]!;
    var saidaDir = Path.GetFullPath(a.Get("saida", (string)prot["resultados"]!));
    Directory.CreateDirectory(saidaDir);
    var grupos = GruposEtapa1();
    var textos = LerJsonl<JsonObject>(arqTextos).ToDictionary(o => (string)o["key"]!);
    var cod2 = LerJsonl<Cod2>(arqEtapa2).GroupBy(c => c.Key!).ToDictionary(g => g.Key, g => g.First());
    var normCache = new Dictionary<string, string>();

    // Etapa 2 → valores efetivos (S só com trecho literal verificado), exclusões verificadas e classe final.
    var final = new Dictionary<string, Final>();
    foreach (var (k, c) in cod2.OrderBy(kv => kv.Key, StringComparer.Ordinal))
    {
        if (!textos.TryGetValue(k, out var tx)) continue;
        if (!normCache.TryGetValue(k, out var alvoNorm))
        {
            var bruto = File.ReadAllText(Path.Combine(dirTextos, (string)tx["arquivo"]!));
            alvoNorm = normCache[k] = Norm((string)registros[k]["title"]! + " " + (string)registros[k]["abstract"]! + " " + bruto);
        }
        bool Confere(string? trecho) { var n = Norm(trecho ?? ""); return n.Length >= minTrecho && alvoNorm.Contains(n); }
        var ev = c.Caracteristicas();
        var efetivo = ev.Select(e => e!.Valor == "S" && !Confere(e.Trecho) ? "I" : e!.Valor!).ToArray();
        var rebaixados = ev.Select((e, i) => (e, i)).Where(x => x.e!.Valor == "S" && efetivo[x.i] == "I").Select(x => $"C{x.i + 1}").ToList();
        var excl = c.Exclusoes!.Where(x => Confere(x.Trecho)).Select(x => x.Codigo!).Distinct().OrderBy(x => x, StringComparer.Ordinal).ToList();
        var exclNaoConf = c.Exclusoes!.Where(x => !Confere(x.Trecho)).Select(x => x.Codigo!).Distinct().OrderBy(x => x, StringComparer.Ordinal).ToList();
        // Classes por perfil: o núcleo exige a base (C1–C4 = S), C5 = S e C6 = S.
        // Cada estudo recebe a primeira classe cujo critério atende, na ordem abaixo.
        string classe, motivo = "";
        bool S(int i) => efetivo[i - 1] == "S";
        var baseNaoS = Enumerable.Range(1, 4).Where(i => !S(i)).Select(i => $"C{i}={efetivo[i - 1]}").ToList();
        var baseN = Enumerable.Range(1, 4).Where(i => efetivo[i - 1] == "N").Select(i => $"C{i}=N").ToList();
        if (excl.Count > 0 || baseN.Count > 0) { classe = "EXCLUIDO_AUSENCIA"; motivo = string.Join(" ", excl.Concat(baseN)); }
        else if (baseNaoS.Count == 0 && S(5) && S(6)) classe = "NUCLEO";
        else if (S(5) && S(6)) { classe = "PENDENTE"; motivo = "não demonstrado: " + string.Join(" ", baseNaoS); }
        else if (baseNaoS.Count > 0) { classe = "EXCLUIDO_NAO_DEMONSTRADO"; motivo = "não demonstrado: " + string.Join(" ", baseNaoS); }
        else if (S(7)) classe = "COMPARADOR_GERATIVO";
        else if (S(5)) classe = "ESCRITOR_COMPACTO";
        else if (S(6)) classe = "AVALIACAO_ESTADO";
        else classe = "MEMORIA_ESTRUTURADA";
        final[k] = new Final(efetivo, rebaixados, excl, exclNaoConf, classe, motivo, c);
    }

    // CSV de todos os registros triados.
    string Csv(string? s) => "\"" + (s ?? "").Replace("\"", "\"\"") + "\"";
    var csv = new StringBuilder("chave,titulo,url,consultas,e1_c1,e1_c2,e1_c3,e1_c4,e1_c5,e1_c6,e1_c7,e1_exclusoes,grupo_etapa1,motivo_etapa1,nota_etapa1,fonte_texto,e2_c1,e2_c2,e2_c3,e2_c4,e2_c5,e2_c6,e2_c7,e2_rebaixados,e2_exclusoes,classe_final,motivo_final,nota_etapa2\n");
    foreach (var k in registros.Keys)
    {
        var r = registros[k];
        grupos.TryGetValue(k, out var g);
        final.TryGetValue(k, out var f);
        var c1 = g.cod;
        string[] v1 = c1 is null ? new string[7] : [c1.C1!, c1.C2!, c1.C3!, c1.C4!, c1.C5!, c1.C6!, c1.C7!];
        string[] v2 = f?.Efetivo ?? new string[7];
        var classe = f?.Classe ?? (g.grupo switch { null => "SEM_CODIGO", "X" => "EXCLUIDO_ETAPA1", "C" => "CONTEXTO", "D" => "INCERTO_SEM_BASE", _ => textos.ContainsKey(k) ? "PENDENTE_ETAPA2" : "SEM_TEXTO" });
        csv.AppendLine(string.Join(",", new[] { Csv(k), Csv((string)r["title"]!), Csv((string)r["url"]!), Csv(string.Join(" ", r["consultas"]!.AsArray().Select(x => (string)x!))) }
            .Concat(v1.Select(Csv)).Concat([Csv(c1 is null ? "" : string.Join(" ", c1.Exclusoes!)), Csv(g.grupo), Csv(g.motivo), Csv(c1?.Nota),
                Csv(textos.TryGetValue(k, out var t) ? (string)t["fonte"]! : "")])
            .Concat(v2.Select(Csv)).Concat([Csv(f is null ? "" : string.Join(" ", f.Rebaixados)), Csv(f is null ? "" : string.Join(" ", f.Exclusoes)),
                Csv(classe), Csv(f?.Motivo), Csv(f?.Cod.Nota)])));
    }
    File.WriteAllText(Path.Combine(saidaDir, "triagem_v4.csv"), csv.ToString(), new UTF8Encoding(true));

    // RIS dos selecionados.
    var selecionados = final.Where(kv => kv.Value.Classe == "NUCLEO").OrderBy(kv => kv.Key, StringComparer.Ordinal).ToList();
    var ris = new StringBuilder();
    foreach (var (k, f) in selecionados)
    {
        var r = registros[k];
        ris.Append("TY  - JOUR\n").Append($"TI  - {(string)r["title"]!}\n");
        foreach (var au in r["authors"]!.AsArray()) ris.Append($"AU  - {(string)au!}\n");
        ris.Append($"PY  - {((string)r["date"]!)[..4]}\nDA  - {(string)r["date"]!}\nJO  - arXiv\nUR  - {(string)r["url"]!}\nN1  - triagem v4: {f.Classe}\nER  - \n\n");
    }
    File.WriteAllText(Path.Combine(saidaDir, "selecionados_v4.ris"), ris.ToString(), new UTF8Encoding(false));

    // Relatório em Markdown.
    var nCand = candidatos.Count;
    var md = new StringBuilder();
    md.AppendLine("# Triagem codificada v4").AppendLine();
    md.AppendLine("Gerado por `dotnet run scripts/triagem_v4.cs -- relatorio` a partir dos arquivos de `" + Rel(dirTri) + "`. Livro de códigos e regras: `protocolo/codigos_v4.md`.").AppendLine();
    md.AppendLine("## Fluxo").AppendLine();
    var cont1 = grupos.Values.GroupBy(g => g.grupo).ToDictionary(g => g.Key, g => g.Count());
    int G(string x) => cont1.GetValueOrDefault(x);
    var abp = G("A") + G("B") + G("P");
    var comTexto = grupos.Count(g => g.Value.grupo is "A" or "B" or "P" && textos.ContainsKey(g.Key));
    var cont2 = final.Values.GroupBy(f => f.Classe).ToDictionary(g => g.Key, g => g.Count());
    md.AppendLine("| Passo | Registros |").AppendLine("|---|---:|");
    md.AppendLine($"| Identificados (únicos, consultas E1–E8) | {nCand} |");
    md.AppendLine($"| Excluídos por termos de título (X1, X2) | {nCand - registros.Count} |");
    md.AppendLine($"| Etapa 1 — codificados por título e resumo | {grupos.Count} de {registros.Count} |");
    md.AppendLine($"| X — excluídos na etapa 1 | {G("X")} |");
    md.AppendLine($"| C — contexto (não seguem) | {G("C")} |");
    md.AppendLine($"| A — núcleo provável | {G("A")} |");
    md.AppendLine($"| B — núcleo incerto | {G("B")} |");
    md.AppendLine($"| D — incerto sem base (C2 ou C3 ≠ S; não seguem) | {G("D")} |");
    md.AppendLine($"| P — comparador possível | {G("P")} |");
    md.AppendLine($"| Texto integral obtido (A+B+P) | {comTexto} de {abp} |");
    md.AppendLine($"| Etapa 2 — codificados com texto integral | {final.Count} |");
    foreach (var (cl, desc) in ClassesV4.Lista)
        md.AppendLine(cl == "NUCLEO" ? $"| **{cl} — {desc}** | **{cont2.GetValueOrDefault(cl)}** |" : $"| {cl} — {desc} | {cont2.GetValueOrDefault(cl)} |");
    md.AppendLine();

    md.AppendLine("### Motivos na etapa 1").AppendLine();
    md.AppendLine("Contagem de registros por critério (um registro pode ter mais de um).").AppendLine();
    md.AppendLine("| Grupo | Critério | Registros |").AppendLine("|---|---|---:|");
    foreach (var (grp, crit, n) in grupos.Values.Where(g => g.grupo is "X" or "C" or "D")
        .SelectMany(g => g.motivo.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(m => (g.grupo, m)))
        .GroupBy(x => x).Select(x => (x.Key.grupo, x.Key.m, x.Count())).OrderBy(x => x.grupo, StringComparer.Ordinal).ThenByDescending(x => x.Item3).ThenBy(x => x.m, StringComparer.Ordinal))
        md.AppendLine($"| {grp} | {crit} | {n} |");
    md.AppendLine();

    md.AppendLine("## Núcleo (selecionados para a síntese)").AppendLine();
    md.AppendLine("Valores efetivos da etapa 2: `S` somente com trecho literal conferido no texto salvo. "
        + "O núcleo exige C1 a C4 = S, C5 = S e C6 = S.").AppendLine();
    md.AppendLine("| Estudo | C1 | C2 | C3 | C4 | C5 | C6 | C7 | Grupo etapa 1 |").AppendLine("|---|---|---|---|---|---|---|---|---|");
    foreach (var (k, f) in selecionados)
        md.AppendLine($"| [{Md((string)registros[k]["title"]!)}]({(string)registros[k]["url"]!}) | {string.Join(" | ", f.Efetivo)} | {grupos[k].grupo} |");
    md.AppendLine();

    md.AppendLine("## Demais classes da etapa 2").AppendLine();
    md.AppendLine("Não entram na síntese; ficam registradas para o fluxograma e para consulta.").AppendLine();
    md.AppendLine("| Classe | Estudo | C1 | C2 | C3 | C4 | C5 | C6 | C7 | Motivo | Rebaixados por falta de trecho |").AppendLine("|---|---|---|---|---|---|---|---|---|---|---|");
    var ordemClasse = ClassesV4.Lista.Select((c, i) => (c.Classe, i)).ToDictionary(x => x.Classe, x => x.i);
    foreach (var (k, f) in final.Where(kv => kv.Value.Classe != "NUCLEO").OrderBy(kv => ordemClasse[kv.Value.Classe]).ThenBy(kv => kv.Key, StringComparer.Ordinal))
        md.AppendLine($"| {f.Classe} | [{Md((string)registros[k]["title"]!)}]({(string)registros[k]["url"]!}) | {string.Join(" | ", f.Efetivo)} | {f.Motivo} | {string.Join(" ", f.Rebaixados)} |");
    md.AppendLine();

    // Diagnóstico dos controles (não é validação independente: os controles orientaram as consultas).
    md.AppendLine("## Controles").AppendLine();
    md.AppendLine("Os controles orientaram a construção das consultas; a recuperação deles não é validação independente.").AppendLine();
    md.AppendLine("| Controle | Recuperado | Após termos | Grupo etapa 1 | Classe final | Motivo |").AppendLine("|---|---|---|---|---|---|");
    var protBusca = JsonNode.Parse(File.ReadAllText((string)prot["protocolo_busca"]!))!;
    var chavesCand = candidatos.Select(o => (string)o["key"]!).ToHashSet();
    foreach (var ctl in protBusca["controles"]!.AsArray())
    {
        var k = "arxiv:" + (string)ctl!["id_arxiv"]!;
        grupos.TryGetValue(k, out var g);
        final.TryGetValue(k, out var f);
        md.AppendLine($"| {Md((string)ctl["nome"]!)} | {(chavesCand.Contains(k) ? "sim" : "não")} | {(registros.ContainsKey(k) ? "sim" : "não")} | {g.grupo ?? "—"} {g.motivo} | {f?.Classe ?? "—"} | {f?.Motivo ?? ""} |");
    }
    md.AppendLine();

    md.AppendLine("## Evidências dos selecionados").AppendLine();
    foreach (var (k, f) in selecionados)
    {
        md.AppendLine($"### {Md((string)registros[k]["title"]!)}").AppendLine();
        md.AppendLine($"`{k}` · {f.Classe} · texto: {(string)textos[k]["fonte"]!} ({(string)textos[k]["url"]!})").AppendLine();
        md.AppendLine(Md(f.Cod.Nota ?? "")).AppendLine();
        var ev = f.Cod.Caracteristicas();
        for (int i = 0; i < 7; i++)
            if (f.Efetivo[i] == "S") md.AppendLine($"- **C{i + 1}**: “{Md(ev[i]!.Trecho ?? "")}”");
        md.AppendLine();
    }

    var custos = LerJsonl<JsonObject>(Path.Combine(dirTri, "custo_llm.jsonl")).GroupBy(o => (string)o["etapa"]!)
        .OrderBy(g => g.Key, StringComparer.Ordinal).Select(g => $"{g.Key}: {g.Count()} requisições, {g.Sum(o => (double)o["custo_contabilizado"]!):F2} créditos");
    md.AppendLine("## Custo").AppendLine().AppendLine(string.Join("; ", custos) + ".");
    File.WriteAllText(Path.Combine(saidaDir, "triagem_v4.md"), md.ToString(), new UTF8Encoding(false));

    Console.WriteLine($"Etapa 1: {string.Join(", ", new[] { "A", "B", "D", "P", "C", "X" }.Select(x => $"{x}={G(x)}"))}; etapa 2: {string.Join(", ", cont2.OrderBy(x => x.Key, StringComparer.Ordinal).Select(x => $"{x.Key}={x.Value}"))}.");
    Console.WriteLine($"Saída: {saidaDir}");
    return 0;
}

static string Md(string s) => s.Replace("|", "\\|").Replace("\n", " ");
string Rel(string p) => Path.GetRelativePath(Directory.GetCurrentDirectory(), p).Replace('\\', '/');

static List<T> LerJsonl<T>(string arq) => !File.Exists(arq) ? [] :
    File.ReadLines(arq).Where(l => l.Trim().Length > 0).Select(l => JsonSerializer.Deserialize<T>(l, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!).ToList();

static string Entre(string s, string ini, string fim)
{
    int i = s.IndexOf(ini, StringComparison.Ordinal), j = s.IndexOf(fim, StringComparison.Ordinal);
    if (i < 0 || j < i) throw new InvalidOperationException("Marcadores do roteiro não encontrados no livro de códigos.");
    return s[(i + ini.Length)..j].Trim();
}

static class ClassesV4
{
    public static readonly (string Classe, string Descricao)[] Lista =
    [
        ("NUCLEO", "C1–C4 = S, C5 = S e C6 = S"),
        ("PENDENTE", "C5 = S e C6 = S, algum de C1–C4 = I"),
        ("COMPARADOR_GERATIVO", "C1–C4 = S e C7 = S"),
        ("ESCRITOR_COMPACTO", "C1–C4 = S e C5 = S, sem C6 = S"),
        ("AVALIACAO_ESTADO", "C1–C4 = S e C6 = S, sem C5 = S"),
        ("MEMORIA_ESTRUTURADA", "C1–C4 = S, nenhum de C5–C7 = S"),
        ("EXCLUIDO_AUSENCIA", "código E conferido ou algum de C1–C4 = N"),
        ("EXCLUIDO_NAO_DEMONSTRADO", "algum de C1–C4 = I, fora de PENDENTE"),
    ];
}

record Pedido(string Rotulo, List<string> Chaves, Func<IReadOnlyCollection<string>, string> Prompt, int CaracteresEntrada, int TokensSaida);
record Final(string[] Efetivo, List<string> Rebaixados, List<string> Exclusoes, List<string> ExclusoesNaoConferidas, string Classe, string Motivo, Cod2 Cod);

class Resp1
{
    [JsonPropertyName("codificacoes")] public List<Cod1> Codificacoes { get; set; } = [];
}

class Cod1
{
    [JsonPropertyName("key")] public string? Key { get; set; }
    [JsonPropertyName("c1")] public string? C1 { get; set; }
    [JsonPropertyName("c2")] public string? C2 { get; set; }
    [JsonPropertyName("c3")] public string? C3 { get; set; }
    [JsonPropertyName("c4")] public string? C4 { get; set; }
    [JsonPropertyName("c5")] public string? C5 { get; set; }
    [JsonPropertyName("c6")] public string? C6 { get; set; }
    [JsonPropertyName("c7")] public string? C7 { get; set; }
    [JsonPropertyName("exclusoes")] public List<string>? Exclusoes { get; set; }
    [JsonPropertyName("nota")] public string? Nota { get; set; }
}

class Evidencia
{
    [JsonPropertyName("valor")] public string? Valor { get; set; }
    [JsonPropertyName("trecho")] public string? Trecho { get; set; }
}

class Exclusao
{
    [JsonPropertyName("codigo")] public string? Codigo { get; set; }
    [JsonPropertyName("trecho")] public string? Trecho { get; set; }
}

class Cod2
{
    [JsonPropertyName("key")] public string? Key { get; set; }
    [JsonPropertyName("c1")] public Evidencia? C1 { get; set; }
    [JsonPropertyName("c2")] public Evidencia? C2 { get; set; }
    [JsonPropertyName("c3")] public Evidencia? C3 { get; set; }
    [JsonPropertyName("c4")] public Evidencia? C4 { get; set; }
    [JsonPropertyName("c5")] public Evidencia? C5 { get; set; }
    [JsonPropertyName("c6")] public Evidencia? C6 { get; set; }
    [JsonPropertyName("c7")] public Evidencia? C7 { get; set; }
    [JsonPropertyName("exclusoes")] public List<Exclusao>? Exclusoes { get; set; }
    [JsonPropertyName("nota")] public string? Nota { get; set; }
    public Evidencia?[] Caracteristicas() => [C1, C2, C3, C4, C5, C6, C7];
}

class Args
{
    readonly Dictionary<string, string> v = new();
    public static Args Ler(string[] a)
    {
        var r = new Args();
        for (int i = 0; i < a.Length; i++)
            if (a[i].StartsWith("--"))
                r.v[a[i][2..]] = i + 1 < a.Length && !a[i + 1].StartsWith("--") ? a[++i] : "true";
        return r;
    }
    public string Get(string k, string padrao) => v.TryGetValue(k, out var s) ? s : padrao;
    public bool Has(string k) => v.ContainsKey(k);
}
