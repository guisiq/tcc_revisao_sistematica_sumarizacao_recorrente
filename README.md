# Busca sistemática: sumarização recorrente com memória estruturada (2025–2026)

Artefatos da busca sistemática que fundamenta o quadro de trabalhos relacionados
do Trabalho de Conclusão de Curso de Guilherme Henrique Siqueira Lopes
(Bacharelado em Sistemas de Informação, Unitins, 2026; orientador: Alex Coelho). O repositório contém somente o necessário
para auditar e reproduzir a busca: protocolo, *snapshots* das respostas da API,
decisões da triagem, revisão final e *scripts*.

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
