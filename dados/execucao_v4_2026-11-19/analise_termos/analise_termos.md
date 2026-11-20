# Análise cruzada de termos para exclusão na busca

- Protocolo de busca: `protocolo_v4.json` (SHA-256 `b36061e8a42ee7f67cdbc1218f73772d813cf3fac05ebf3220a5f428c07b0176`).
- Exclusões adotadas: `exclusoes_v4.json` (SHA-256 `f0df9bd43a5980d7b963c9dfd1a574832f705e890ac3ca4d6ab2f16a8cccb487`).
- Rótulos: `execucao_focada_estrita_2026-11-12`; snapshot dos controles SHA-256 `990090508946d042bfe93cf5753ebef33b916509f9c66f9f8c04decbfea06334`; pool v4: 738 registros.
- Grupos: POS = incluídos na revisão anterior ∪ controles (12); NEG_LLM = EXCLUDE da triagem (54); NEG_REV = excluídos na revisão manual (9); INCERTO = CONTEXT/CORE sem revisão (85).
- Regra de candidatura: termo (palavra ou bigrama sem palavras vazias) ausente de todo POS, presente em ≥ 3 documentos NEG e não mais frequente em INCERTO do que em NEG. Contagem por documento; `pool_v4` conta o termo nos registros da nova coleta (sem controles).
- Regra de adoção: o termo deve constar da lista de candidatos (ou ser prefixo com curinga de um candidato), denotar um critério E2–E9 sem outro sentido usual em PLN e não remover nenhum POS. Exclusões aplicam-se ao título; em teste, termos aplicados ao resumo removeram estudos de memória textual que só mencionavam o termo de passagem. A validação é executada pelo script.

## Candidatos examinados e rejeitados

| Termo | Motivo |
|---|---|
| scene, scenes | Polissêmico: no título removeu Scene-Aware Memory Discrimination (memória textual pessoal com avaliação direta da escrita). |
| geometric | Polissêmico (sentido matemático): removeu Agent-Dice (atualização de conhecimento). |
| optimizer, gradients | Polissêmico: otimizador de prompts e memória de agentes otimizada; não denota memória paramétrica. |
| world models, world | Removeria Masked Diffusion Language Models ... Text-Based World Models (possível comparador) e títulos com real-world. |
| gpu, kernel, residual, attention, transformer | Mencionados de passagem em estudos de memória textual (ex.: avaliação de memória com deprecação, memória pessoal gerida pelo kernel). |
| spatial, image, frames, reconstruction | Aparecem em estudos multimodais cuja memória pode ser textual; deixados para a triagem codificada. |
| sumi | Nome próprio (SUMI/Sumi), não denota critério. |
| demais candidatos genéricos (preserving, short, tokens, due, ...) | Não denotam critério de exclusão; refletem só o tamanho pequeno da amostra rotulada. |
| qualquer termo aplicado ao resumo | Em teste (X2/X3 em ti+abs), 112 remoções incluíram estudos de memória textual (Danus, SAGE, MemEmo, Structured State Reconciliation, Don't Retrain, Align). |

## Validação das exclusões adotadas

| Id | Código | Campo | Termos | POS | NEG_LLM | NEG_REV | INCERTO | Pool v4 |
|---|---|---|---|---:|---:|---:|---:|---:|
| X1 | E4 | ti | video, manipulation, motion, rendering, geometry, visual generation | 0 | 6 | 0 | 2 | 39 |
| X2 | E9 | ti | quantum | 0 | 2 | 0 | 0 | 3 |

Efeito conjunto (um documento conta uma vez):

- POS: 0 de 12 removidos.
- NEG_LLM: 8 de 54 removidos.
- NEG_REV: 0 de 9 removidos.
- INCERTO: 2 de 85 removidos.
- Pool v4: 42 de 738 removidos; restam 696.

Justificativas:

- **X1 (E4, ti)**: Vídeo, manipulação robótica, movimento, renderização, geometria e geração visual no título indicam modalidade não textual ou domínio físico como objeto principal (E4). Candidatos com 0 POS.
- **X2 (E9, ti)**: Computação ou física quântica no título: fora do tema (E9). Candidato com 0 POS.

### Registros removidos do pool v4

| Chave | Rótulo anterior | Exclusões | Título |
|---|---|---|---|
| `arxiv:2509.00259` | novo | X2 | Quantum-Optimized Selective State Space Model for Efficient Time Series Prediction |
| `arxiv:2511.12027` | novo | X1 | GCAgent: Long-Video Understanding via Schematic and Narrative Episodic Memory |
| `arxiv:2511.20424` | novo | X1 | Planar Josephson junctions for sensors and electronics:Different geometry, new functionality |
| `arxiv:2511.22364` | novo | X1 | BINDER: Instantly Adaptive Mobile Manipulation with Open-Vocabulary Commands |
| `arxiv:2512.07344` | novo | X1 | Venus: An Efficient Edge Memory-and-Retrieval System for VLM-based Online Video Understanding |
| `arxiv:2512.10252` | novo | X1 | GDKVM: Echocardiography Video Segmentation via Spatiotemporal Key-Value Memory with Gated Delta Rule |
| `arxiv:2512.19539` | novo | X1 | StoryMem: Multi-shot Long Video Storytelling with Memory |
| `arxiv:2512.24638` | novo | X1 | Resolving State Ambiguity in Robot Manipulation via Adaptive Working Memory Recoding |
| `arxiv:2601.12076` | novo | X1 | CroBIM-V: Memory-Quality Controlled Remote Sensing Referring Video Object Segmentation |
| `arxiv:2603.07690` | novo | X1 | FrameVGGT: Coherence-Preserving Memory for Bounded Streaming Geometry |
| `arxiv:2603.19571` | novo | X1 | CurveStream: Boosting Streaming Video Understanding in MLLMs via Curvature-Aware Hierarchical Visual Memory Management |
| `arxiv:2605.06185` | novo | X1 | Event-Causal RAG: A Retrieval-Augmented Generation Framework for Long Video Reasoning in Complex Scenarios |
| `arxiv:2605.07897` | novo | X1 | Semantic-Aware Adaptive Visual Memory for Streaming Video Understanding |
| `arxiv:2605.18556` | novo | X1 | Key-Gram: Extensible World Knowledge for Embodied Manipulation |
| `arxiv:2605.23610` | NEG_LLM | X1 | EM-Vid: Training-Free Entity-Centric Memory for Efficient and Consistent Multi-Shot Video Generation |
| `arxiv:2606.00576` | novo | X1 | Dynamic Resilient Spatio-Semantic Memory with Hybrid Localization for Mobile Manipulation |
| `arxiv:2606.05624` | novo | X1 | KV-Control: Parameter-Efficient K/V Injection for Trajectory-Controlled Text-to-Motion |
| `arxiv:2606.07512` | novo | X1 | MemDreamer: Decoupling Perception and Reasoning for Long Video Understanding via Hierarchical Graph Memory and Agentic Retrieval Mechanism |
| `arxiv:2606.10363` | NEG_LLM | X1 | HiMem-WAM: Hierarchical Memory-Gated World Action Models for Robotic Manipulation |
| `arxiv:2606.10671` | novo | X1 | FadeMem: Distance-Aware Memory Consolidation for Autoregressive Video Diffusion |
| `arxiv:2606.16353` | novo | X1 | What Should a Streaming Video Model Remember? |
| `arxiv:2606.16449` | novo | X1 | PermaVid: Consistent Video Generation Across Edits via Disentangled Context Memory |
| `arxiv:2606.27821` | NEG_LLM | X2 | Parameter-Efficient Quantum-Inspired Fast Weight Programmers for Traffic-Matrix Forecasting |
| `arxiv:2606.31837` | novo | X2 | Automatic quantum function parallelization and memory management in Qrisp |
| `arxiv:2607.13298` | INCERTO | X1 | FOLIO: Focused Semantic Memory for Streaming Video Understanding |
| `arxiv:2608.11017` | novo | X1 | R4DSG: Relative 4D Scene Graph Memory for Object-Centric Question Answering in Long Egocentric Video |
| `arxiv:2609.04131` | novo | X1 | Beyond Retrieval: Progressive Latent Memory Evolution for Streaming Video Understanding |
| `arxiv:2609.07047` | novo | X1 | MEMOBench: A Process Level Memory Benchmark for Robotic Manipulation |
| `arxiv:2609.28798` | novo | X1 | OCC4M: Object-Centric 4D Memory for Spatiotemporal Reasoning in Long-Horizon Manipulation |
| `arxiv:2609.34502` | NEG_LLM | X1 | SubjectAnchor: Subject-Aware Memory-to-Video for Multi-Shot Storytelling |
| `arxiv:2609.37035` | INCERTO | X1 | Watch-Think-Interact: Bootstrapping Long-Horizon Multi-Turn Streaming Video Reasoning with Reinforcement Learning |
| `arxiv:2609.37583` | novo | X1 | RoboHarn-Evo: Evolving Hierarchical Physical Knowledge for Self-Improving Robotic Manipulation |
| `arxiv:2610.06672` | novo | X1 | VideoTapestry: Query-Adaptive Memory Refinement for Multi-Agent Long-Video Understanding |
| `arxiv:2610.10183` | novo | X1 | VideoEvolve: Co-Evolving Memory and Retrieval for Long Video Understanding |
| `arxiv:2610.15512` | novo | X1 | Structured Stateful Relational Scripts with a High-Quality Dataset for Multi-Shot Audio-Visual Video Generation |
| `arxiv:2610.23904` | novo | X1 | LVMH: A Meta-Optimization Harness for Long-Video Agent Memory |
| `arxiv:2610.26468` | novo | X1 | YUTELA: A Scalable Edge Cloud AIoT System for Multi-Site Video Anomaly Detection with Scene Specific Adaptation |
| `arxiv:2610.27917` | novo | X1 | EventSync: Event-Driven Memory Updates for History-Dependent Manipulation |
| `arxiv:2610.32317` | novo | X1 | Organizing Temporal Evidence in Hyperbolic Space for Video Moment Retrieval |
| `arxiv:2610.32784` | novo | X1 | Video-MemComp: Taylor-Guided Sensitivity-Aware Memory Management for Streaming Video Understanding |
| `arxiv:2611.01858` | novo | X1 | Learning-to-Memorize: Dynamic Context Management for Long-Horizon Autoregressive Video Generation |
| `arxiv:2611.23950` | novo | X1 | VOL-Agent: Programming Your Film via Video Orchestration Language Agentic System |

## Candidatos (todos)

| Campo | Termo | POS | NEG_LLM | NEG_REV | INCERTO | Pool v4 | Códigos nos NEG |
|---|---|---:|---:|---:|---:|---:|---|
| ti | generation | 0 | 7 | 0 | 2 | 29 | E4:5, E2:1, E9:1 |
| ti | world | 0 | 5 | 1 | 0 | 20 | E4:4, E9:1, revisão:1 |
| ti | open | 0 | 4 | 0 | 0 | 10 | E9:3, E6:1 |
| ti | video | 0 | 4 | 0 | 2 | 27 | E4:4 |
| ti | modeling | 0 | 3 | 0 | 0 | 8 | E4:3 |
| ti | sumi | 0 | 3 | 0 | 0 | 1 | E9:2, E4:1 |
| ti | small language | 0 | 2 | 1 | 2 | 3 | E6:1, E9:1, revisão:1 |
| ti | hierarchical | 0 | 3 | 0 | 3 | 15 | E9:2, E4:1 |
| ti | large | 0 | 1 | 2 | 3 | 37 | revisão:2, E9:1 |
| ti | long context | 0 | 1 | 2 | 3 | 14 | revisão:2, E4:1 |
| ti+abs | preserving | 0 | 12 | 1 | 11 | 97 | E9:7, E4:3, E2:1, E6:1, revisão:1 |
| ti+abs | short | 0 | 11 | 1 | 3 | 54 | E4:5, E9:5, E3:1, revisão:1 |
| ti+abs | tokens | 0 | 10 | 2 | 8 | 117 | E4:5, E9:5, revisão:2 |
| ti+abs | video | 0 | 10 | 1 | 2 | 57 | E4:10, revisão:1 |
| ti+abs | attention | 0 | 9 | 1 | 2 | 87 | E4:5, E9:3, E2:1, revisão:1 |
| ti+abs | dynamics | 0 | 9 | 1 | 2 | 41 | E9:7, E4:2, revisão:1 |
| ti+abs | parameter | 0 | 8 | 2 | 4 | 56 | E9:6, revisão:2, E4:1, E6:1 |
| ti+abs | due | 0 | 9 | 1 | 6 | 73 | E9:4, E4:2, E2:1, E3:1, E6:1, revisão:1 |
| ti+abs | natural | 0 | 8 | 1 | 7 | 36 | E9:8, revisão:1 |
| ti+abs | shared | 0 | 7 | 2 | 7 | 77 | E9:5, revisão:2, E2:1, E4:1 |
| ti+abs | generated | 0 | 7 | 2 | 8 | 40 | E4:4, E9:2, revisão:2, E6:1 |
| ti+abs | neural | 0 | 7 | 1 | 1 | 37 | E9:4, E4:2, E2:1, revisão:1 |
| ti+abs | bank | 0 | 5 | 3 | 2 | 24 | E4:4, revisão:3, E9:1 |
| ti+abs | second | 0 | 5 | 3 | 3 | 34 | E9:3, revisão:3, E3:1, E4:1 |
| ti+abs | objective | 0 | 8 | 0 | 4 | 41 | E4:4, E9:3, E2:1 |
| ti+abs | structural | 0 | 7 | 1 | 4 | 48 | E9:4, E4:2, E3:1, revisão:1 |
| ti+abs | loss | 0 | 6 | 2 | 5 | 37 | E9:3, revisão:2, E2:1, E4:1, E6:1 |
| ti+abs | preserve | 0 | 8 | 0 | 5 | 50 | E4:4, E9:4 |
| ti+abs | content | 0 | 7 | 1 | 6 | 77 | E9:4, E4:3, revisão:1 |
| ti+abs | output | 0 | 8 | 0 | 7 | 40 | E9:7, E4:1 |
| ti+abs | including | 0 | 7 | 1 | 8 | 82 | E9:5, E3:1, E4:1, revisão:1 |
| ti+abs | features | 0 | 7 | 0 | 0 | 32 | E9:4, E4:3 |
| ti+abs | transformer | 0 | 6 | 1 | 0 | 32 | E9:4, E4:2, revisão:1 |
| ti+abs | global | 0 | 5 | 2 | 1 | 44 | E9:3, E4:2, revisão:2 |
| ti+abs | point | 0 | 6 | 1 | 1 | 39 | E9:4, E4:2, revisão:1 |
| ti+abs | scene | 0 | 7 | 0 | 1 | 33 | E4:7 |
| ti+abs | identify | 0 | 6 | 1 | 2 | 38 | E9:4, E3:1, E4:1, revisão:1 |
| ti+abs | spatial | 0 | 7 | 0 | 2 | 22 | E4:7 |
| ti+abs | stream | 0 | 4 | 3 | 4 | 22 | E9:3, revisão:3, E4:1 |
| ti+abs | dual | 0 | 6 | 1 | 5 | 44 | E9:4, E4:2, revisão:1 |
| ti+abs | modeling | 0 | 7 | 0 | 5 | 57 | E9:4, E4:3 |
| ti+abs | competitive | 0 | 7 | 0 | 7 | 31 | E9:4, E4:3 |
| ti+abs | constraints | 0 | 7 | 0 | 7 | 63 | E4:3, E9:3, E2:1 |
| ti+abs | gpu | 0 | 5 | 1 | 0 | 34 | E2:2, E9:2, E6:1, revisão:1 |
| ti+abs | gradients | 0 | 6 | 0 | 0 | 7 | E9:5, E2:1 |
| ti+abs | propagation | 0 | 5 | 1 | 0 | 19 | E9:3, E4:2, revisão:1 |
| ti+abs | sumi | 0 | 6 | 0 | 0 | 1 | E9:4, E4:2 |
| ti+abs | addition | 0 | 6 | 0 | 1 | 12 | E9:3, E4:2, E2:1 |
| ti+abs | computational | 0 | 5 | 1 | 1 | 69 | E4:2, E6:2, E9:1, revisão:1 |
| ti+abs | dependent | 0 | 5 | 1 | 1 | 42 | E4:3, E9:2, revisão:1 |
| ti+abs | jointly | 0 | 6 | 0 | 2 | 43 | E9:3, E4:2, E6:1 |
| ti+abs | learns | 0 | 5 | 1 | 2 | 33 | E4:3, E9:2, revisão:1 |
| ti+abs | network | 0 | 5 | 1 | 2 | 35 | E9:3, E2:1, E4:1, revisão:1 |
| ti+abs | particularly | 0 | 6 | 0 | 2 | 28 | E4:3, E9:2, E6:1 |
| ti+abs | target | 0 | 5 | 1 | 2 | 71 | E9:3, E4:2, revisão:1 |
| ti+abs | difficult | 0 | 4 | 2 | 3 | 33 | E4:2, E9:2, revisão:2 |
| ti+abs | entities | 0 | 5 | 1 | 3 | 24 | E4:3, E9:2, revisão:1 |
| ti+abs | recurrent | 0 | 6 | 0 | 3 | 30 | E9:3, E4:2, E2:1 |
| ti+abs | like | 0 | 6 | 0 | 4 | 37 | E4:3, E9:3 |
| ti+abs | read | 0 | 3 | 3 | 4 | 26 | revisão:3, E2:1, E4:1, E9:1 |
| ti+abs | representations | 0 | 6 | 0 | 4 | 66 | E4:3, E9:3 |
| ti+abs | general | 0 | 6 | 0 | 5 | 52 | E9:5, E4:1 |
| ti+abs | matching | 0 | 6 | 0 | 5 | 29 | E9:3, E2:1, E4:1, E6:1 |
| ti+abs | stable | 0 | 5 | 1 | 5 | 51 | E9:3, E4:1, E6:1, revisão:1 |
| ti+abs | stage | 0 | 5 | 1 | 5 | 50 | E4:3, E9:2, revisão:1 |
| ti+abs | dense | 0 | 5 | 1 | 6 | 44 | E4:2, E9:2, E2:1, revisão:1 |
| ti+abs | environments | 0 | 6 | 0 | 6 | 59 | E9:3, E4:2, E2:1 |
| ti+abs | long context | 0 | 3 | 3 | 6 | 48 | revisão:3, E9:2, E4:1 |
| ti+abs | natural language | 0 | 5 | 1 | 6 | 21 | E9:5, revisão:1 |
| ti+abs | open source | 0 | 6 | 0 | 6 | 30 | E4:2, E9:2, E3:1, E6:1 |
| ti+abs | prior | 0 | 5 | 1 | 6 | 100 | E4:2, E9:2, E2:1, revisão:1 |
| ti+abs | robust | 0 | 6 | 0 | 6 | 59 | E4:3, E9:3 |
| ti+abs | engine | 0 | 3 | 2 | 0 | 24 | E9:2, revisão:2, E2:1 |
| ti+abs | manipulation | 0 | 5 | 0 | 0 | 18 | E4:4, E9:1 |
| ti+abs | motion | 0 | 5 | 0 | 0 | 16 | E9:3, E4:2 |
| ti+abs | correct | 0 | 4 | 1 | 1 | 36 | E9:3, E4:1, revisão:1 |
| ti+abs | forgetting | 0 | 5 | 0 | 1 | 45 | E9:3, E4:2 |
| ti+abs | image | 0 | 5 | 0 | 1 | 29 | E4:4, E9:1 |
| ti+abs | prompting | 0 | 5 | 0 | 1 | 15 | E4:2, E9:2, E6:1 |
| ti+abs | reconstruction | 0 | 5 | 0 | 1 | 24 | E4:3, E9:2 |
| ti+abs | samples | 0 | 5 | 0 | 1 | 15 | E9:4, E4:1 |
| ti+abs | short term | 0 | 4 | 1 | 1 | 22 | E4:2, E3:1, E9:1, revisão:1 |
| ti+abs | third | 0 | 5 | 0 | 1 | 11 | E9:3, E3:1, E4:1 |
| ti+abs | allowing | 0 | 5 | 0 | 2 | 30 | E9:3, E4:2 |
| ti+abs | class | 0 | 4 | 1 | 2 | 31 | E9:2, E2:1, E6:1, revisão:1 |
| ti+abs | compare | 0 | 5 | 0 | 2 | 23 | E9:5 |
| ti+abs | encode | 0 | 5 | 0 | 2 | 18 | E9:3, E4:2 |
| ti+abs | prediction | 0 | 5 | 0 | 2 | 44 | E9:4, E4:1 |
| ti+abs | remaining | 0 | 5 | 0 | 2 | 19 | E4:3, E9:2 |
| ti+abs | suite | 0 | 4 | 1 | 2 | 16 | E4:2, E6:1, E9:1, revisão:1 |
| ti+abs | decoding | 0 | 5 | 0 | 3 | 42 | E9:4, E4:1 |
| ti+abs | horizons | 0 | 5 | 0 | 3 | 17 | E4:4, E9:1 |
| ti+abs | retaining | 0 | 5 | 0 | 3 | 34 | E4:2, E9:2, E2:1 |
| ti+abs | sequence | 0 | 5 | 0 | 3 | 47 | E9:4, E4:1 |
| ti+abs | top | 0 | 3 | 2 | 3 | 27 | E4:2, revisão:2, E9:1 |
| ti+abs | benefits | 0 | 4 | 1 | 4 | 16 | E4:2, E9:2, revisão:1 |
| ti+abs | effectively | 0 | 4 | 1 | 4 | 49 | E9:2, E4:1, E6:1, revisão:1 |
| ti+abs | form | 0 | 4 | 1 | 4 | 48 | E4:3, E9:1, revisão:1 |
| ti+abs | generate | 0 | 4 | 1 | 4 | 35 | E4:3, E9:1, revisão:1 |
| ti+abs | matched | 0 | 5 | 0 | 4 | 43 | E9:4, E4:1 |
| ti+abs | view | 0 | 4 | 1 | 4 | 29 | E4:3, E3:1, revisão:1 |
| ti+abs | against | 0 | 5 | 0 | 5 | 54 | E9:4, E4:1 |
| ti+abs | conditions | 0 | 5 | 0 | 5 | 46 | E9:3, E4:2 |
| ti+abs | every | 0 | 3 | 2 | 5 | 76 | E9:3, revisão:2 |
| ti+abs | rate | 0 | 4 | 1 | 5 | 49 | E6:2, E2:1, E4:1, revisão:1 |
| ti+abs | response | 0 | 2 | 3 | 5 | 34 | revisão:3, E6:1, E9:1 |
| ti+abs | scalable | 0 | 5 | 0 | 5 | 51 | E9:3, E4:2 |
| ti+abs | significantly | 0 | 4 | 1 | 5 | 62 | E4:2, E6:1, E9:1, revisão:1 |
| ti+abs | allows | 0 | 3 | 1 | 0 | 21 | E4:2, E9:1, revisão:1 |
| ti+abs | effects | 0 | 3 | 1 | 0 | 15 | E9:3, revisão:1 |
| ti+abs | families | 0 | 4 | 0 | 0 | 18 | E9:2, E4:1, E6:1 |
| ti+abs | geometric | 0 | 4 | 0 | 0 | 16 | E4:4 |
| ti+abs | linear | 0 | 4 | 0 | 0 | 25 | E9:2, E2:1, E4:1 |
| ti+abs | neural network | 0 | 3 | 1 | 0 | 9 | E2:1, E4:1, E9:1, revisão:1 |
| ti+abs | predicted | 0 | 4 | 0 | 0 | 10 | E4:2, E6:1, E9:1 |
| ti+abs | pretrained | 0 | 3 | 1 | 0 | 26 | E9:2, E4:1, revisão:1 |
| ti+abs | scenes | 0 | 4 | 0 | 0 | 12 | E4:4 |
| ti+abs | structural memory | 0 | 4 | 0 | 0 | 6 | E9:2, E3:1, E4:1 |
| ti+abs | utilizes | 0 | 2 | 2 | 0 | 9 | revisão:2, E4:1, E6:1 |
| ti+abs | captures | 0 | 4 | 0 | 1 | 15 | E9:3, E4:1 |
| ti+abs | depth | 0 | 3 | 1 | 1 | 22 | E9:2, E4:1, revisão:1 |
| ti+abs | gradient | 0 | 4 | 0 | 1 | 19 | E9:4 |
| ti+abs | instructions | 0 | 3 | 1 | 1 | 16 | E4:1, E6:1, E9:1, revisão:1 |
| ti+abs | learn | 0 | 3 | 1 | 1 | 38 | E9:2, E4:1, revisão:1 |
| ti+abs | numerical | 0 | 4 | 0 | 1 | 11 | E9:4 |
| ti+abs | object | 0 | 3 | 1 | 1 | 25 | E4:3, revisão:1 |
| ti+abs | previous | 0 | 4 | 0 | 1 | 22 | E4:2, E9:2 |
| ti+abs | program | 0 | 4 | 0 | 1 | 10 | E9:4 |
| ti+abs | proprietary | 0 | 4 | 0 | 1 | 12 | E4:2, E6:1, E9:1 |
| ti+abs | recipe | 0 | 4 | 0 | 1 | 13 | E9:3, E4:1 |
| ti+abs | refine | 0 | 3 | 1 | 1 | 16 | E9:2, E4:1, revisão:1 |
| ti+abs | reverse | 0 | 3 | 1 | 1 | 9 | E4:2, E9:1, revisão:1 |
| ti+abs | semantics | 0 | 4 | 0 | 1 | 28 | E9:3, E4:1 |
| ti+abs | several | 0 | 4 | 0 | 1 | 23 | E9:4 |
| ti+abs | unlike | 0 | 4 | 0 | 1 | 26 | E4:2, E9:2 |
| ti+abs | dedicated | 0 | 4 | 0 | 2 | 27 | E9:2, E2:1, E4:1 |
| ti+abs | despite | 0 | 4 | 0 | 2 | 37 | E9:3, E4:1 |
| ti+abs | diagnostic | 0 | 3 | 1 | 2 | 20 | E4:2, E9:1, revisão:1 |
| ti+abs | flow | 0 | 4 | 0 | 2 | 22 | E9:3, E4:1 |
| ti+abs | generative | 0 | 3 | 1 | 2 | 27 | E4:2, E9:1, revisão:1 |
| ti+abs | independent | 0 | 4 | 0 | 2 | 36 | E4:2, E9:2 |
| ti+abs | memory bank | 0 | 2 | 2 | 2 | 14 | E4:2, revisão:2 |
| ti+abs | relational | 0 | 4 | 0 | 2 | 24 | E9:3, E3:1 |
| ti+abs | relationships | 0 | 4 | 0 | 2 | 18 | E4:2, E9:2 |
| ti+abs | serving | 0 | 2 | 2 | 2 | 28 | revisão:2, E2:1, E9:1 |
| ti+abs | simulated | 0 | 4 | 0 | 2 | 10 | E4:3, E9:1 |
| ti+abs | structures | 0 | 3 | 1 | 2 | 32 | E9:2, E4:1, revisão:1 |
| ti+abs | wise | 0 | 3 | 1 | 2 | 33 | E4:2, E9:1, revisão:1 |
| ti+abs | back | 0 | 4 | 0 | 3 | 17 | E4:2, E9:2 |
| ti+abs | encoding | 0 | 4 | 0 | 3 | 16 | E4:2, E9:2 |
| ti+abs | evaluations | 0 | 4 | 0 | 3 | 51 | E9:2, E2:1, E4:1 |
| ti+abs | format | 0 | 3 | 1 | 3 | 17 | E9:2, E4:1, revisão:1 |
| ti+abs | frames | 0 | 4 | 0 | 3 | 23 | E4:3, E9:1 |
| ti+abs | observed | 0 | 4 | 0 | 3 | 28 | E4:3, E9:1 |
| ti+abs | offers | 0 | 3 | 1 | 3 | 30 | E9:3, revisão:1 |
| ti+abs | post | 0 | 3 | 1 | 3 | 31 | E4:1, E6:1, E9:1, revisão:1 |
| ti+abs | already | 0 | 4 | 0 | 4 | 26 | E9:3, E4:1 |
| ti+abs | applies | 0 | 4 | 0 | 4 | 18 | E9:3, E4:1 |
| ti+abs | because | 0 | 2 | 2 | 4 | 48 | revisão:2, E4:1, E9:1 |
| ti+abs | continuous | 0 | 4 | 0 | 4 | 59 | E4:2, E9:2 |
| ti+abs | dataset | 0 | 3 | 1 | 4 | 41 | E9:2, E4:1, revisão:1 |
| ti+abs | enhanced | 0 | 3 | 1 | 4 | 23 | E4:3, revisão:1 |
| ti+abs | implementation | 0 | 4 | 0 | 4 | 23 | E9:2, E2:1, E3:1 |
| ti+abs | parallel | 0 | 4 | 0 | 4 | 51 | E9:2, E2:1, E6:1 |
| ti+abs | topology | 0 | 4 | 0 | 4 | 8 | E9:3, E2:1 |
| ti+abs | approaches typically | 0 | 2 | 1 | 0 | 9 | E4:1, E9:1, revisão:1 |
| ti+abs | branching | 0 | 2 | 1 | 0 | 8 | E2:1, E9:1, revisão:1 |
| ti+abs | classes | 0 | 3 | 0 | 0 | 12 | E9:3 |
| ti+abs | computational cost | 0 | 3 | 0 | 0 | 13 | E4:2, E6:1 |
| ti+abs | describe | 0 | 3 | 0 | 0 | 10 | E9:3 |
| ti+abs | detection | 0 | 3 | 0 | 0 | 30 | E9:2, E4:1 |
| ti+abs | encoder | 0 | 3 | 0 | 0 | 18 | E4:2, E9:1 |
| ti+abs | entity states | 0 | 2 | 1 | 0 | 6 | E4:1, E9:1, revisão:1 |
| ti+abs | estimation | 0 | 3 | 0 | 0 | 11 | E4:2, E9:1 |
| ti+abs | explicit persistent | 0 | 2 | 1 | 0 | 2 | E4:2, revisão:1 |
| ti+abs | extracts | 0 | 1 | 2 | 0 | 18 | revisão:2, E4:1 |
| ti+abs | follows | 0 | 2 | 1 | 0 | 6 | E4:1, E9:1, revisão:1 |
| ti+abs | geometry | 0 | 3 | 0 | 0 | 14 | E4:2, E9:1 |
| ti+abs | greater | 0 | 3 | 0 | 0 | 7 | E9:2, E4:1 |
| ti+abs | infer | 0 | 2 | 1 | 0 | 8 | E4:1, E9:1, revisão:1 |
| ti+abs | insight | 0 | 2 | 1 | 0 | 17 | E4:1, E9:1, revisão:1 |
| ti+abs | insufficient | 0 | 3 | 0 | 0 | 21 | E4:3 |
| ti+abs | least | 0 | 2 | 1 | 0 | 8 | E9:2, revisão:1 |
| ti+abs | metric | 0 | 3 | 0 | 0 | 8 | E4:2, E6:1 |
| ti+abs | minute | 0 | 3 | 0 | 0 | 4 | E4:2, E9:1 |
| ti+abs | model families | 0 | 3 | 0 | 0 | 5 | E9:2, E6:1 |
| ti+abs | movement | 0 | 2 | 1 | 0 | 6 | E2:1, E6:1, revisão:1 |
| ti+abs | negative | 0 | 2 | 1 | 0 | 18 | E4:1, E9:1, revisão:1 |
| ti+abs | networks | 0 | 3 | 0 | 0 | 28 | E9:2, E2:1 |
| ti+abs | neural networks | 0 | 3 | 0 | 0 | 16 | E9:2, E2:1 |
| ti+abs | optimizer | 0 | 3 | 0 | 0 | 4 | E9:3 |
| ti+abs | performance gains | 0 | 2 | 1 | 0 | 9 | E9:2, revisão:1 |
| ti+abs | placement | 0 | 2 | 1 | 0 | 10 | E2:1, E9:1, revisão:1 |
| ti+abs | position | 0 | 3 | 0 | 0 | 20 | E4:2, E2:1 |
| ti+abs | properties | 0 | 3 | 0 | 0 | 21 | E9:2, E4:1 |
| ti+abs | purely | 0 | 2 | 1 | 0 | 9 | E4:1, E9:1, revisão:1 |
| ti+abs | quantum | 0 | 3 | 0 | 0 | 4 | E9:3 |
| ti+abs | rendering | 0 | 2 | 1 | 0 | 7 | E4:2, revisão:1 |
| ti+abs | simulations | 0 | 3 | 0 | 0 | 3 | E9:3 |
| ti+abs | start | 0 | 3 | 0 | 0 | 14 | E9:2, E4:1 |
| ti+abs | subject | 0 | 2 | 1 | 0 | 7 | E4:2, revisão:1 |
| ti+abs | technical | 0 | 3 | 0 | 0 | 10 | E9:2, E4:1 |
| ti+abs | throughput | 0 | 2 | 1 | 0 | 45 | E2:1, E6:1, revisão:1 |
| ti+abs | time training | 0 | 3 | 0 | 0 | 1 | E4:2, E9:1 |
| ti+abs | video generation | 0 | 3 | 0 | 0 | 7 | E4:3 |
| ti+abs | visual generation | 0 | 2 | 1 | 0 | 3 | E4:2, revisão:1 |
| ti+abs | worker | 0 | 2 | 1 | 0 | 4 | E2:1, E9:1, revisão:1 |
| ti+abs | world models | 0 | 2 | 1 | 0 | 7 | E4:2, revisão:1 |
| ti+abs | world state | 0 | 2 | 1 | 0 | 3 | E4:2, revisão:1 |
| ti+abs | worlds | 0 | 2 | 1 | 0 | 5 | E4:2, revisão:1 |
| ti+abs | abstract | 0 | 2 | 1 | 1 | 8 | E4:1, E9:1, revisão:1 |
| ti+abs | acquisition | 0 | 2 | 1 | 1 | 7 | E4:1, E9:1, revisão:1 |
| ti+abs | anchoring | 0 | 2 | 1 | 1 | 8 | E4:2, revisão:1 |
| ti+abs | application | 0 | 3 | 0 | 1 | 23 | E3:1, E6:1, E9:1 |
| ti+abs | classical | 0 | 3 | 0 | 1 | 7 | E9:3 |
| ti+abs | communication | 0 | 2 | 1 | 1 | 19 | E9:2, revisão:1 |
| ti+abs | consistent improvements | 0 | 3 | 0 | 1 | 7 | E4:3 |
| ti+abs | correction | 0 | 3 | 0 | 1 | 20 | E9:3 |
| ti+abs | entry | 0 | 2 | 1 | 1 | 11 | E4:1, E9:1, revisão:1 |
| ti+abs | establishing | 0 | 3 | 0 | 1 | 16 | E9:2, E6:1 |
| ti+abs | example | 0 | 2 | 1 | 1 | 8 | E9:2, revisão:1 |
| ti+abs | executable | 0 | 2 | 1 | 1 | 19 | E9:2, revisão:1 |
| ti+abs | executing | 0 | 3 | 0 | 1 | 6 | E4:2, E2:1 |
| ti+abs | formats | 0 | 2 | 1 | 1 | 3 | E9:2, revisão:1 |
| ti+abs | gate | 0 | 2 | 1 | 1 | 28 | E4:1, E9:1, revisão:1 |
| ti+abs | helps | 0 | 2 | 1 | 1 | 12 | E4:1, E9:1, revisão:1 |
| ti+abs | identity | 0 | 3 | 0 | 1 | 23 | E4:3 |
| ti+abs | inference latency | 0 | 3 | 0 | 1 | 9 | E9:2, E4:1 |
| ti+abs | kernel | 0 | 3 | 0 | 1 | 21 | E2:2, E9:1 |
| ti+abs | lexical | 0 | 3 | 0 | 1 | 8 | E9:3 |
| ti+abs | machine | 0 | 3 | 0 | 1 | 8 | E9:3 |
| ti+abs | nature | 0 | 3 | 0 | 1 | 16 | E9:2, E4:1 |
| ti+abs | number | 0 | 3 | 0 | 1 | 21 | E9:2, E6:1 |
| ti+abs | poses | 0 | 3 | 0 | 1 | 9 | E4:2, E9:1 |
| ti+abs | reads | 0 | 2 | 1 | 1 | 12 | E4:1, E9:1, revisão:1 |
| ti+abs | replay | 0 | 3 | 0 | 1 | 23 | E9:3 |
| ti+abs | residual | 0 | 3 | 0 | 1 | 14 | E9:3 |
| ti+abs | resulting | 0 | 3 | 0 | 1 | 31 | E4:1, E6:1, E9:1 |
| ti+abs | shape | 0 | 2 | 1 | 1 | 4 | E4:1, E9:1, revisão:1 |
| ti+abs | side | 0 | 3 | 0 | 1 | 30 | E9:2, E4:1 |
| ti+abs | solution | 0 | 2 | 1 | 1 | 29 | E6:1, E9:1, revisão:1 |
| ti+abs | studying | 0 | 2 | 1 | 1 | 7 | E4:1, E9:1, revisão:1 |
| ti+abs | temporal context | 0 | 3 | 0 | 1 | 3 | E4:3 |
| ti+abs | transform | 0 | 3 | 0 | 1 | 4 | E9:2, E4:1 |
| ti+abs | type | 0 | 3 | 0 | 1 | 45 | E9:3 |
| ti+abs | why | 0 | 1 | 2 | 1 | 9 | revisão:2, E9:1 |
| ti+abs | analyze | 0 | 3 | 0 | 2 | 15 | E9:2, E3:1 |
| ti+abs | autoregressive models | 0 | 3 | 0 | 2 | 10 | E9:3 |
| ti+abs | aware memory | 0 | 3 | 0 | 2 | 25 | E4:3 |
| ti+abs | case | 0 | 2 | 1 | 2 | 28 | E9:2, revisão:1 |
| ti+abs | depend | 0 | 2 | 1 | 2 | 15 | E9:2, revisão:1 |
| ti+abs | drift | 0 | 1 | 2 | 2 | 27 | revisão:2, E4:1 |
| ti+abs | efficiently | 0 | 3 | 0 | 2 | 23 | E3:1, E4:1, E9:1 |
| ti+abs | emph | 0 | 3 | 0 | 2 | 13 | E4:2, E9:1 |
| ti+abs | executes | 0 | 2 | 1 | 2 | 9 | E9:2, revisão:1 |
| ti+abs | exploration | 0 | 3 | 0 | 2 | 28 | E4:2, E9:1 |
| ti+abs | extracted | 0 | 2 | 1 | 2 | 9 | E4:1, E9:1, revisão:1 |
| ti+abs | feature | 0 | 3 | 0 | 2 | 24 | E4:2, E9:1 |
| ti+abs | fine tuned | 0 | 2 | 1 | 2 | 9 | E9:2, revisão:1 |
| ti+abs | finite | 0 | 3 | 0 | 2 | 14 | E9:3 |
| ti+abs | generalization | 0 | 3 | 0 | 2 | 32 | E4:3 |
| ti+abs | high level | 0 | 3 | 0 | 2 | 16 | E2:1, E4:1, E9:1 |
| ti+abs | highly | 0 | 3 | 0 | 2 | 32 | E4:2, E2:1 |
| ti+abs | layers | 0 | 3 | 0 | 2 | 32 | E9:2, E2:1 |
| ti+abs | pre | 0 | 2 | 1 | 2 | 46 | E4:2, revisão:1 |
| ti+abs | programs | 0 | 2 | 1 | 2 | 8 | E6:1, E9:1, revisão:1 |
| ti+abs | proposes | 0 | 3 | 0 | 2 | 29 | E9:2, E6:1 |
| ti+abs | refines | 0 | 2 | 1 | 2 | 9 | E4:1, E9:1, revisão:1 |
| ti+abs | relation | 0 | 2 | 1 | 2 | 17 | E9:2, revisão:1 |
| ti+abs | represent | 0 | 3 | 0 | 2 | 14 | E9:3 |
| ti+abs | result | 0 | 2 | 1 | 2 | 32 | E4:1, E9:1, revisão:1 |
| ti+abs | simple | 0 | 3 | 0 | 2 | 32 | E4:2, E9:1 |
| ti+abs | supported | 0 | 1 | 2 | 2 | 14 | revisão:2, E9:1 |
| ti+abs | transitions | 0 | 3 | 0 | 2 | 21 | E9:2, E4:1 |
| ti+abs | translation | 0 | 3 | 0 | 2 | 13 | E4:2, E9:1 |
| ti+abs | tuned | 0 | 2 | 1 | 2 | 18 | E9:2, revisão:1 |
| ti+abs | verification | 0 | 2 | 1 | 2 | 27 | E4:1, E9:1, revisão:1 |
| ti+abs | 3.5 | 0 | 3 | 0 | 3 | 7 | E9:2, E2:1 |
| ti+abs | automated | 0 | 3 | 0 | 3 | 16 | E4:1, E6:1, E9:1 |
| ti+abs | bias | 0 | 3 | 0 | 3 | 19 | E9:2, E2:1 |
| ti+abs | boundary | 0 | 3 | 0 | 3 | 25 | E4:2, E9:1 |
| ti+abs | capacity | 0 | 2 | 1 | 3 | 49 | E4:1, E9:1, revisão:1 |
| ti+abs | combining | 0 | 3 | 0 | 3 | 28 | E9:2, E4:1 |
| ti+abs | context reasoning | 0 | 0 | 3 | 3 | 11 | revisão:3 |
| ti+abs | deep | 0 | 3 | 0 | 3 | 26 | E2:1, E3:1, E6:1 |
| ti+abs | deterministic | 0 | 2 | 1 | 3 | 32 | E2:1, E4:1, revisão:1 |
| ti+abs | discovery | 0 | 3 | 0 | 3 | 16 | E9:2, E3:1 |
| ti+abs | evaluating | 0 | 2 | 1 | 3 | 28 | E4:1, E9:1, revisão:1 |
| ti+abs | explore | 0 | 3 | 0 | 3 | 16 | E3:1, E4:1, E9:1 |
| ti+abs | induced | 0 | 3 | 0 | 3 | 19 | E9:3 |
| ti+abs | mean | 0 | 2 | 1 | 3 | 30 | E9:2, revisão:1 |
| ti+abs | previously | 0 | 2 | 1 | 3 | 22 | E4:1, E9:1, revisão:1 |
| ti+abs | producing | 0 | 3 | 0 | 3 | 14 | E9:2, E4:1 |
| ti+abs | rapid | 0 | 3 | 0 | 3 | 15 | E9:2, E4:1 |
| ti+abs | requirements | 0 | 2 | 1 | 3 | 22 | E4:1, E9:1, revisão:1 |
| ti+abs | robustness | 0 | 3 | 0 | 3 | 38 | E9:2, E4:1 |
| ti+abs | selected | 0 | 2 | 1 | 3 | 29 | E9:2, revisão:1 |
| ti+abs | smaller | 0 | 2 | 1 | 3 | 11 | E6:1, E9:1, revisão:1 |
