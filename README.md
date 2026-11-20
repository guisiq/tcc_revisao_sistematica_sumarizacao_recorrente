# Busca sistemática: sumarização recorrente com memória estruturada (2025–2026)

Artefatos da busca sistemática que fundamenta o quadro de trabalhos relacionados
do Trabalho de Conclusão de Curso de Guilherme Henrique Siqueira Lopes
(Bacharelado em Sistemas de Informação, Unitins, 2026; orientador: Alex Coelho). O repositório contém somente o necessário
para auditar e reproduzir a busca: protocolo, *snapshots* das respostas da API,
decisões da triagem, revisão final e *scripts*.

**Versão v2.1.0 (vigente):** a seleção usada no artigo vem da
[execução v4](#execução-v4-busca-por-facetas-exclusões-por-termos-e-triagem-codificada)
(busca de 19/11/2026, triagem codificada e classes por perfil). As seções
seguintes descrevem a execução de 12/11/2026, publicada na v1.0.0 e mantida
para auditoria.

- [Relatório da triagem v4](resultados/v4/triagem_v4.md)
- [Inventário dos 696 registros triados na v4 (CSV)](resultados/v4/triagem_v4.csv)
- [Núcleo da v4 (RIS, para o Zotero)](resultados/v4/selecionados_v4.ris)

Execução de 12/11/2026 (v1.0.0):

- [Relatório e metodologia](resultados/mapeamento_focado_trabalhos_relacionados_2025_2026.md)
- [Inventário dos 158 candidatos (CSV)](resultados/mapeamento_focado_inventario_2025_2026.csv)
- [Estudos selecionados (RIS, para o Zotero)](resultados/mapeamento_focado_selecionados_2025_2026.ris)

## Estrutura

```text
protocolo/   protocolo_focado_estrito.json (execução final), roteiro_triagem_focado.md,
             triagem_llm.json, protocolos do piloto e do ensaio
dados/       execucao_focada_estrita_2026-11-12/  execução final
             execucao_focada_ensaio_2026-11-12/   ensaio, sem a consulta F6
             execucao_focada_2026-11-12/          piloto, primeira versão das consultas
scripts/     busca_focada.cs (coleta, preparação, relatório), triagem.cs (triagem por LLM)
resultados/  relatório, inventário e RIS gerados por busca_focada.cs
```

Cada pasta de execução contém o `manifesto.json` (hash do protocolo, totais,
paginação e hash de cada resposta Atom), os *snapshots* `arxiv/*.xml`,
`candidatos.jsonl`, as decisões do modelo em `decisoes/` e o registro de custo
`custo_llm.jsonl`. A execução final contém também `revisao_final.json`.

## Procedimento

O procedimento é um mapeamento sistematizado e limitado, inspirado em
Petersen et al. (2015): planejamento (protocolo), busca, triagem de título e
resumo, revisão e extração.

1. **Busca:** seis consultas (F1–F6) na API do arXiv, combinando facetas nos
   campos título e resumo, com janela pela data do primeiro depósito. Em
   12/11/2026, OpenAlex e Semantic Scholar responderam HTTP 429 e não integram a
   coleta; o recorte cobre apenas o arXiv.
2. **Triagem:** 158 títulos e resumos classificados como CORE, CONTEXT ou
   EXCLUDE pelo modelo `claude-haiku-5.5` via GitHub Copilot SDK 1.0.31, com o
   [roteiro](protocolo/roteiro_triagem_focado.md), 10 registros por requisição e
   teto de 160 créditos. Resultado: 13 CORE, 91 CONTEXT e 54 EXCLUDE.
3. **Revisão final:** todos os CORE e seis CONTEXT escolhidos por proximidade
   foram examinados pelo autor; correções e comparadores são justificados
   individualmente em
   [`revisao_final.json`](dados/execucao_focada_estrita_2026-11-12/revisao_final.json).
   Foram selecionados dez estudos: oito do núcleo (três eram CONTEXT corrigidos
   após consulta ao texto) e dois comparadores.

**Limitações:** fonte única (arXiv); a F6 foi acrescentada após inspecionar um
estudo de referência não recuperado (refinamento exploratório); a verificação de
textos integrais foi parcial; não houve avaliador humano independente.

**Custo:** o piloto foi interrompido após 33 créditos e não alimenta a
classificação final; a execução final consumiu 16 créditos. Total: 49 créditos.

## Reprodução

Requer o .NET SDK 10 (os *scripts* usam `dotnet run` de arquivo único). Execute
na raiz do repositório:

```powershell
# Regenera o relatório, o CSV e o RIS em resultados/, sem rede e sem chamadas ao modelo.
# Falha se o protocolo ou algum snapshot divergir dos hashes do manifesto.
dotnet run scripts\busca_focada.cs -- offline
```

Para uma nova execução (requer rede e, na triagem, acesso ao GitHub Copilot):

```powershell
Copy-Item protocolo\protocolo_focado_estrito.json protocolo\protocolo_focado_nova_execucao.json
# Edite inicio e fim na cópia.
dotnet run scripts\busca_focada.cs -- coletar --dados dados\nova_execucao --protocolo protocolo\protocolo_focado_nova_execucao.json
dotnet run scripts\busca_focada.cs -- preparar --dados dados\nova_execucao --protocolo protocolo\protocolo_focado_nova_execucao.json

# Prévia de custo e triagem.
dotnet run scripts\triagem.cs -- --dados dados\nova_execucao --alvo principal --roteiro protocolo\roteiro_triagem_focado.md --por-requisicao 10 --paralelo 1 --limite-creditos 160 --estimar
dotnet run scripts\triagem.cs -- --dados dados\nova_execucao --alvo principal --roteiro protocolo\roteiro_triagem_focado.md --por-requisicao 10 --paralelo 1 --limite-creditos 160

# Após registrar a revisão de cada CORE em dados\nova_execucao\revisao_final.json:
dotnet run scripts\busca_focada.cs -- relatorio --dados dados\nova_execucao --protocolo protocolo\protocolo_focado_nova_execucao.json --saida resultados\nova_execucao
```

Cada item de `revisao_final.json` tem `key`, `incluir`, `motivo` e `verificacao`
(`R` para título/resumo; `T` para texto integral, com `fonte` apontando a versão
do arXiv registrada no *snapshot*). Itens incluídos têm `tipo` (`NUCLEO` ou
`COMPARADOR`) e os campos `representacao`, `atualizacao`, `modelos`,
`avaliacao` e `paradigmas`. O *script* falha se faltar decisão CORE, campo
obrigatório ou referência à versão verificada.

### Execução v4: busca por facetas, exclusões por termos e triagem codificada

O protocolo [`protocolo/protocolo_v4.json`](protocolo/protocolo_v4.json) busca
por facetas (memória estruturada, atualização, escritor compacto, avaliação do
estado, paradigma gerativo) na janela 2025-07-01 a 2026-11-19, com 12
controles: os 10 incluídos na execução anterior, Intrinsic Memory Agents e
Supersede. Os controles orientaram o vocabulário das consultas; sua recuperação
não é validação independente. O arXiv aplica curingas ao índice radicalizado,
por isso os prefixos param no radical (`consolid*`, `templat*`, `convers*`,
`distil*`); prefixos mais longos, como `consolidat*`, quase não recuperam nada.

As exclusões por termos ficam em
[`protocolo/exclusoes_v4.json`](protocolo/exclusoes_v4.json) e são derivadas
pela análise cruzada de termos entre os rótulos da execução anterior
(incluídos/controles × excluídos). Um termo é candidato quando não aparece em
nenhum positivo, aparece em ao menos três excluídos e não é mais frequente
entre os CONTEXT não revisados do que entre os excluídos. É adotado apenas se
denota um código E2–E9 sem outro sentido usual em PLN; aplica-se ao título.
O *script* falha se um termo adotado não constar da lista de candidatos ou
remover algum controle.

```powershell
dotnet run scripts\busca_focada.cs -- coletar --protocolo protocolo\protocolo_v4.json --dados dados\execucao_v4_2026-11-19
dotnet run scripts\analise_termos.cs -- analisar --protocolo protocolo\protocolo_v4.json --exclusoes protocolo\exclusoes_v4.json --rotulos dados\execucao_focada_estrita_2026-11-12 --dados dados\execucao_v4_2026-11-19
```

A análise grava em `dados\execucao_v4_2026-11-19\analise_termos\` o relatório
`analise_termos.md` (candidatos, termos rejeitados com motivo, efeito por grupo
e registros removidos), `candidatos_exclusao.csv`, o *snapshot*
`controles.xml` e `chaves_apos_exclusao.txt`, que é a entrada da triagem.

#### Triagem codificada v4

O modelo não decide a inclusão: ele atribui valores S/I/N às características
C1–C7 e códigos de exclusão E2–E10, definidos no livro de códigos
[`protocolo/codigos_v4.md`](protocolo/codigos_v4.md). O *script*
[`scripts/triagem_v4.cs`](scripts/triagem_v4.cs) classifica os registros por
regras fixas. Os códigos E7 (memória interna do modelo) e E10 (saída
estruturada estática) foram separados de E9 pela análise cruzada das
exclusões da execução anterior. Os parâmetros estão em
[`protocolo/triagem_v4.json`](protocolo/triagem_v4.json).

1. **Etapa 1 (título e resumo, 20 registros por requisição):** cada registro
   recebe um grupo: A, B, P (seguem para o texto integral), C, D ou X (encerram
   na etapa 1).
2. **Texto integral:** usa a mesma versão do arXiv. O *script* prefere o HTML
   e recorre ao PDF quando não há HTML; remove a bibliografia e grava o texto e
   o manifesto com SHA-256.
3. **Etapa 2 (uma requisição por artigo, até 100 mil caracteres):** cada `S`
   exige um trecho literal. O relatório confere o trecho no texto salvo e
   rebaixa a `I` todo `S` sem trecho encontrado. Um C5 = S cujo trecho só
   cita tamanhos de modelo acima de 4,5B passa a `N`. Só então o estudo é
   classificado.
4. **Classes por perfil:** o **núcleo** exige C1–C4 = S, C5 = S (escritor
   compacto) e C6 = S (avaliação direta do estado). Os demais estudos lidos no
   texto integral recebem uma classe própria (pendente, comparador gerativo,
   escritor compacto, avaliação do estado, memória estruturada ou excluídos),
   registrada no relatório (ver [`codigos_v4.md`](protocolo/codigos_v4.md)).
   Resultado: 12 estudos no núcleo.

Os textos integrais (`triagem_v4/textos/*.txt`) não são versionados, por
respeito às licenças dos artigos; o manifesto com URL, versão e SHA-256 de
cada texto está em `textos/manifesto.jsonl`, e o comando `textos` os
reconstitui antes do `relatorio`.

```powershell
dotnet run scripts\triagem_v4.cs -- resumo [--estimar]
dotnet run scripts\triagem_v4.cs -- textos
dotnet run scripts\triagem_v4.cs -- texto [--estimar] [--paralelo 4]
dotnet run scripts\triagem_v4.cs -- relatorio
```

`resumo` e `texto` retomam a partir das codificações gravadas em
`dados\execucao_v4_2026-11-19\triagem_v4\` (`etapa1.jsonl`, `etapa2.jsonl`)
e registram o custo em `custo_llm.jsonl`. `relatorio` não consulta a rede nem
o modelo; gera `resultados\v4\triagem_v4.md`, `triagem_v4.csv` (todos os
registros, com códigos, grupo, motivo e classe final) e `selecionados_v4.ris`.

## Origem

Os artefatos foram extraídos da pasta de trabalho do TCC sem alterar dados,
protocolos ou decisões. Diferenças em relação à origem:

- `busca_focada.cs` grava em `resultados/` (antes, na pasta pai) e cita
  Prodanov e Freitas (2013) em texto, sem link para arquivo externo; o CSV e o
  RIS regenerados são idênticos aos originais, e o relatório difere apenas
  nesses caminhos e na citação.
- `triagem.cs` lê, por padrão, `protocolo/triagem_llm.json`, cópia literal da
  seção `triagem` do protocolo do mapeamento amplo usado na execução original,
  e usa como padrão a pasta e o roteiro da execução final.
- Os lotes (`dados/**/lotes/`) são derivados e regenerados por `preparar`.

## Referências

PETERSEN, K.; VAKKALANKA, S.; KUZNIARZ, L. Guidelines for conducting systematic
mapping studies in software engineering: an update. *Information and Software
Technology*, v. 64, p. 1–18, 2015. DOI: 10.1016/j.infsof.2015.03.007.

PRODANOV, C. C.; FREITAS, E. C. *Metodologia do trabalho científico*: métodos e
técnicas da pesquisa e do trabalho acadêmico. 2. ed. Novo Hamburgo: Feevale, 2013.

## Licença

[MIT](LICENSE). Os *snapshots* em `dados/**/arxiv/` reproduzem metadados
retornados pela API do arXiv e seguem os termos de uso do arXiv.
