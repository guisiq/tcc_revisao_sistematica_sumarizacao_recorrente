# Livro de códigos da triagem v4

A triagem não decide diretamente "incluir" ou "excluir". O modelo apenas
**codifica características** de cada estudo; a classificação é calculada pelo
*script* `triagem_v4.cs` com as regras fixas descritas ao final deste documento.
O trecho entre os marcadores `roteiro` é enviado ao modelo como mensagem de
sistema; as regras de classificação não são enviadas.

<!-- roteiro:inicio -->
Você codifica estudos para um mapeamento sistemático sobre memória ou estado
textual estruturado, atualizado de forma incremental por modelos de linguagem
(sumarização recorrente). Não decida inclusão; apenas atribua valores às
características e, quando couber, códigos de exclusão. Responda somente em JSON.

## Características (valores: `S` = sim, `I` = incerto, `N` = não)

Use `S` somente quando o texto fornecido afirmar a característica. Use `I`
quando ela for plausível, mas não estiver afirmada. Use `N` quando o texto
indicar o contrário ou quando o estudo claramente não tratar do assunto.

| Código | Característica | `S` exige | Exemplos de `N` |
|---|---|---|---|
| C1 | Estudo primário | Propõe método, sistema, *benchmark* ou experimento próprio | *Survey*, revisão, ensaio de posição sem experimento |
| C2 | Memória ou estado **textual** mantido por LLM ou agente baseado em LLM | Memória/estado em linguagem natural ou símbolos textuais, externo aos pesos, lido e escrito pelo sistema | Cache KV, estado recorrente ou latente, pesos, memória de GPU, memória visual |
| C3 | Atualização incremental | O estado é escrito, modificado, fundido, removido ou reescrito ao longo de entradas, turnos, eventos ou sessões | Base construída uma única vez; RAG sobre corpus fixo; saída gerada uma vez |
| C4 | Estrutura explícita | JSON, esquema, campos ou registros tipados, entidades e atributos, tabela, grafo de fatos | Resumo em texto livre; somente *embeddings* |
| C5 | Escritor compacto | O modelo que **escreve ou atualiza** o estado tem até cerca de 4B parâmetros | Escritor ≥ 7B; apenas o recuperador/*embedding* é pequeno |
| C6 | Avaliação direta do estado | Métrica sobre o conteúdo do estado ou das operações de escrita (fidelidade, correção, F1 da memória, acurácia de atualização) | Somente acurácia de resposta final, latência ou custo |
| C7 | Comparação de paradigmas gerativos | Compara explicitamente difusão, não autorregressivo ou encoder-decoder com autorregressivo em escrita/edição de estado, memória, plano ou saída estruturada | Usa um único paradigma |

## Códigos de exclusão

Atribua um código quando ele for o **objeto principal** do estudo, não uma menção
de passagem.

| Código | Motivo |
|---|---|
| E2 | Memória de *hardware* ou de execução (GPU, alocação, *runtime*) |
| E3 | Estudo secundário (*survey*, revisão, mapeamento) |
| E4 | Modalidade não textual principal (vídeo, imagem, áudio, robótica, 3D) |
| E5 | Segurança, privacidade ou ataque à memória como objeto principal |
| E6 | Aplicação de domínio sem contribuição ao problema de memória ou estado |
| E7 | Memória interna do modelo: cache KV, estado recorrente ou latente, atenção linear, pesos, *replay*, *test-time training* |
| E8 | Informação insuficiente até para codificar |
| E9 | Fora do tema |
| E10 | Saída estruturada estática (JSON, formato, chamadas de ferramenta) sem estado mantido entre entradas |
<!-- roteiro:fim -->

### Origem de E7 e E10 (análise cruzada da execução anterior)

Na execução de 12/11/2026, o código E9 reunia motivos distintos. A leitura das
54 justificativas EXCLUDE mostrou dois grupos recorrentes, agora separados:

- **E7**, memória interna do modelo, em 12 exclusões: 2512.07461, 2601.15086,
  2603.08082, 2604.24809, 2606.11853, 2606.19005, 2606.26560, 2609.38764,
  2610.06269, 2610.07553, 2611.04885 e 2611.06956 (este último rotulado E2).
- **E10**, saída estruturada estática, em 4 exclusões: 2510.09211,
  2602.12468, 2604.14314 e 2610.35981.

Os nove estudos excluídos na revisão manual não se separam por vocabulário;
diferem por C3 (construção estática: SMART SLM, *The Brain That Goes Quiet*),
C5 (escritor ≥ 7B ou não informado: G-Long, Auto-Dreamer), C6 (avaliação só de
respostas: COSMIR, UMA, rEDMRec), C2 (estado numérico: *Belief Engine*) e E4
(*Programmable World Model*). Por isso essas dimensões são codificadas
separadamente.

## Regras de classificação (aplicadas pelo *script*, não pelo modelo)

**Etapa 1 — título e resumo:**

1. **X (excluído):** qualquer código E, ou C1 = N, ou C2 = N.
2. **A (núcleo provável):** C3 = S, C4 = S e ao menos um entre C5, C6 e C7 = S.
3. **B (núcleo incerto):** fora de A, com C2 = S, C3 = S, C4 ≠ N e ao menos
   um entre C5, C6 e C7 ≠ N.
4. **D (incerto sem base):** fora de A, com C3 ≠ N, C4 ≠ N e ao menos um entre
   C5, C6 e C7 ≠ N, mas sem C2 = S e C3 = S. O resumo não basta para afirmar
   que se trata de memória textual atualizada de forma incremental.
5. **P (comparador possível):** fora de A, B e D, com C7 ≠ N e C3 ≠ N.
6. **C (contexto):** os demais; registram-se as características com N.

A, B e P seguem para a etapa 2 (texto integral). C, D e X encerram na etapa 1.

**Etapa 2 — texto integral:**

Cada valor `S` deve vir com um trecho literal do texto. O *script* confere o
trecho no texto salvo; um `S` sem trecho encontrado é rebaixado a `I`. Em C5,
o *script* também lê os tamanhos de modelo citados no trecho (por exemplo,
`7B`, `qwen3-8b`): se houver algum e todos forem maiores que
`limite_c5_bilhoes` (4,5, em `triagem_v4.json`), C5 passa a `N`, porque o
trecho mostra um escritor grande.

A **base** de um estudo é C1 a C4 = S. Cada estudo recebe a primeira classe,
na ordem abaixo, cujo critério atende:

1. **EXCLUIDO_AUSENCIA:** código E conferido ou algum entre C1 e C4 = N.
2. **NUCLEO:** base, C5 = S e C6 = S.
3. **PENDENTE:** C5 = S e C6 = S, com algum entre C1 e C4 = I.
4. **EXCLUIDO_NAO_DEMONSTRADO:** algum entre C1 e C4 = I.
5. **COMPARADOR_GERATIVO:** base e C7 = S.
6. **ESCRITOR_COMPACTO:** base e C5 = S (sem C6 = S).
7. **AVALIACAO_ESTADO:** base e C6 = S (sem C5 = S).
8. **MEMORIA_ESTRUTURADA:** base, sem nenhum entre C5 e C7 = S.

Somente o **NUCLEO** entra na síntese (Quadro 1 do artigo). As demais classes
ficam registradas no relatório para o fluxograma e para consulta.
