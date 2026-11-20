# Triagem codificada v4

Gerado por `dotnet run scripts/triagem_v4.cs -- relatorio` a partir dos arquivos de `dados/execucao_v4_2026-11-19/triagem_v4`. Livro de códigos e regras: `protocolo/codigos_v4.md`.

## Fluxo

| Passo | Registros |
|---|---:|
| Identificados (únicos, consultas E1–E8) | 738 |
| Excluídos por termos de título (X1, X2) | 42 |
| Etapa 1 — codificados por título e resumo | 696 de 696 |
| X — excluídos na etapa 1 | 406 |
| C — contexto (não seguem) | 59 |
| A — núcleo provável | 22 |
| B — núcleo incerto | 165 |
| D — incerto sem base (C2 ou C3 ≠ S; não seguem) | 42 |
| P — comparador possível | 2 |
| Texto integral obtido (A+B+P) | 189 de 189 |
| Etapa 2 — codificados com texto integral | 189 |
| **NUCLEO — C1–C4 = S, C5 = S e C6 = S** | **12** |
| PENDENTE — C5 = S e C6 = S, algum de C1–C4 = I | 2 |
| COMPARADOR_GERATIVO — C1–C4 = S e C7 = S | 1 |
| ESCRITOR_COMPACTO — C1–C4 = S e C5 = S, sem C6 = S | 8 |
| AVALIACAO_ESTADO — C1–C4 = S e C6 = S, sem C5 = S | 52 |
| MEMORIA_ESTRUTURADA — C1–C4 = S, nenhum de C5–C7 = S | 70 |
| EXCLUIDO_AUSENCIA — código E conferido ou algum de C1–C4 = N | 8 |
| EXCLUIDO_NAO_DEMONSTRADO — algum de C1–C4 = I, fora de PENDENTE | 36 |

### Motivos na etapa 1

Contagem de registros por critério (um registro pode ter mais de um).

| Grupo | Critério | Registros |
|---|---|---:|
| C | C5–C7=N | 36 |
| C | C4=N | 28 |
| C | C3=N | 8 |
| D | C2=I | 25 |
| D | C3=I | 25 |
| X | C2=N | 343 |
| X | E9 | 124 |
| X | E7 | 123 |
| X | E4 | 79 |
| X | E2 | 38 |
| X | E6 | 21 |
| X | E5 | 17 |
| X | C1=N | 15 |
| X | E3 | 9 |
| X | E10 | 8 |

## Núcleo (selecionados para a síntese)

Valores efetivos da etapa 2: `S` somente com trecho literal conferido no texto salvo. O núcleo exige C1 a C4 = S, C5 = S e C6 = S.

| Estudo | C1 | C2 | C3 | C4 | C5 | C6 | C7 | Grupo etapa 1 |
|---|---|---|---|---|---|---|---|---|
| [Intrinsic Memory Agents: Heterogeneous Multi-Agent LLM Systems through Structured Contextual Memory](https://arxiv.org/abs/2508.08997v2) | S | S | S | S | S | S | N | B |
| [Learning to Share: Selective Memory for Efficient Parallel Agentic Systems](https://arxiv.org/abs/2602.05965v2) | S | S | S | S | S | S | N | B |
| [Learning to Remember: End-to-End Training of Memory Agents for Long-Context Reasoning](https://arxiv.org/abs/2602.18493v2) | S | S | S | S | S | S | N | B |
| [MemReader: From Passive to Active Extraction for Long-Term Agent Memory](https://arxiv.org/abs/2604.07877v2) | S | S | S | S | S | S | N | A |
| [CMI-Mem: Toward Generalizable Long-Term Memory Management via CMI-Augmented Reinforcement Learning](https://arxiv.org/abs/2607.20553v2) | S | S | S | S | S | S | N | B |
| [AVERT: Audio-Verified Adjudication for Spoken Dialogue State Tracking](https://arxiv.org/abs/2609.01828v1) | S | S | S | S | S | S | N | A |
| [MoM: Memory of Memory](https://arxiv.org/abs/2609.25054v1) | S | S | S | S | S | S | N | A |
| [SpeakerMem-R1: Speaker-Centered Dual-Track Memory for Multi-Party Dialogue](https://arxiv.org/abs/2609.26780v2) | S | S | S | S | S | S | N | B |
| [CoEM: Empowering Long-Context Reasoning with Commit-on-Evidence Memory](https://arxiv.org/abs/2609.36935v2) | S | S | S | S | S | S | N | B |
| [AMU:Admission and Memory Update for Personalized Conversations---Structured Memory with SLM Guided Control](https://arxiv.org/abs/2609.36976v2) | S | S | S | S | S | S | N | B |
| [MemRevise: A Self-Revising Memory for Conversational Search](https://arxiv.org/abs/2610.24672v1) | S | S | S | S | S | S | N | A |
| [Accept Changes, Reject Attacks: Robust Memory Management for LLM Agents](https://arxiv.org/abs/2610.41056v1) | S | S | S | S | S | S | N | B |

## Demais classes da etapa 2

Não entram na síntese; ficam registradas para o fluxograma e para consulta.

| Classe | Estudo | C1 | C2 | C3 | C4 | C5 | C6 | C7 | Motivo | Rebaixados (trecho não conferido; C5 por tamanho) |
|---|---|---|---|---|---|---|---|---|---|---|
| PENDENTE | [PGMem: Tightly Coupled Persona-Memory Graph for Lifelong Personalized Agents](https://arxiv.org/abs/2608.01708v2) | S | S | S | I | S | S | N | não demonstrado: C4=I | C4 |
| PENDENTE | [Beyond Memory Construction: Rethinking Memory Access for LLM-based Conversational Agents](https://arxiv.org/abs/2609.33226v1) | S | S | S | I | S | S | N | não demonstrado: C4=I |  |
| COMPARADOR_GERATIVO | [DiffuPlan: Editable Planning State for Diffusion Language Model Agents](https://arxiv.org/abs/2610.24717v1) | S | S | S | S | I | N | S |  |  |
| ESCRITOR_COMPACTO | [Efficient On-Device Agents via Adaptive Context Management](https://arxiv.org/abs/2511.03728v2) | S | S | S | S | S | N | N |  |  |
| ESCRITOR_COMPACTO | [MemLoRA: Distilling Expert Adapters for On-Device Memory Systems](https://arxiv.org/abs/2512.04763v1) | S | S | S | S | S | N | N |  |  |
| ESCRITOR_COMPACTO | [MemBuilder: Reinforcing LLMs for Long-Term Memory Construction via Attributed Dense Rewards](https://arxiv.org/abs/2601.05488v4) | S | S | S | S | S | I | N |  |  |
| ESCRITOR_COMPACTO | [Mem-T: Densifying Rewards for Long-Horizon Memory Agents](https://arxiv.org/abs/2601.23014v2) | S | S | S | S | S | I | N |  |  |
| ESCRITOR_COMPACTO | [To Know is to Construct: Schema-Constrained Generation for Agent Memory](https://arxiv.org/abs/2604.20117v1) | S | S | S | S | S | N | N |  |  |
| ESCRITOR_COMPACTO | [DimMem: Dimensional Structuring for Efficient Long-Term Agent Memory](https://arxiv.org/abs/2605.15759v3) | S | S | S | S | S | I | N |  |  |
| ESCRITOR_COMPACTO | [ActiveMem: Distributed Active Memory for Long-Horizon LLM Reasoning](https://arxiv.org/abs/2606.10532v1) | S | S | S | S | S | N | N |  |  |
| ESCRITOR_COMPACTO | [GARP: Real-Time Generative Agents on Local Hardware in a Game Engine](https://arxiv.org/abs/2611.11064v1) | S | S | S | S | S | N | N |  |  |
| AVALIACAO_ESTADO | [GraphMed-LT: Patient-Specific Graph Memory with Latent Clinical Thought Refinement for Multi-Turn Medical Conversations](https://arxiv.org/abs/2510.03536v3) | S | S | S | S | N | S | N |  |  |
| AVALIACAO_ESTADO | [D-SMART: Enhancing LLM Dialogue Consistency via Dynamic Structured Memory And Reasoning Tree](https://arxiv.org/abs/2510.13363v1) | S | S | S | S | N | S | N |  |  |
| AVALIACAO_ESTADO | [LightMem: Lightweight and Efficient Memory-Augmented Generation](https://arxiv.org/abs/2510.18866v4) | S | S | S | S | N | S | N |  |  |
| AVALIACAO_ESTADO | [HaluMem: Evaluating Hallucinations in Memory Systems of Agents](https://arxiv.org/abs/2511.03506v3) | S | S | S | S | I | S | N |  |  |
| AVALIACAO_ESTADO | [Beyond Training: Enabling Self-Evolution of Agents with MOBIMEM](https://arxiv.org/abs/2512.15784v1) | S | S | S | S | N | S | N |  |  |
| AVALIACAO_ESTADO | [Agentic Memory: Learning Unified Long-Term and Short-Term Memory Management for Large Language Model Agents](https://arxiv.org/abs/2601.01885v3) | S | S | S | S | I | S | N |  |  |
| AVALIACAO_ESTADO | [Inside Out: Evolving User-Centric Core Memory Trees for Long-Term Personalized Dialogue Systems](https://arxiv.org/abs/2601.05171v2) | S | S | S | S | N | S | N |  |  |
| AVALIACAO_ESTADO | [InfMem: Learning System-2 Memory Control for Long-Context Agent](https://arxiv.org/abs/2602.02704v2) | S | S | S | S | I | S | N |  |  |
| AVALIACAO_ESTADO | [All-Mem: Agentic Lifelong Memory via Dynamic Topology Evolution](https://arxiv.org/abs/2603.19595v2) | S | S | S | S | I | S | N |  |  |
| AVALIACAO_ESTADO | [AlpsBench: An LLM Personalization Benchmark for Real-Dialogue Memorization and Preference Alignment](https://arxiv.org/abs/2603.26680v2) | S | S | S | S | N | S | N |  |  |
| AVALIACAO_ESTADO | [ClawVM: Harness-Managed Virtual Memory for Stateful Tool-Using LLM Agents](https://arxiv.org/abs/2604.10352v1) | S | S | S | S | I | S | N |  |  |
| AVALIACAO_ESTADO | [WorldDB: A Vector Graph-of-Worlds Memory Engine with Ontology-Aware Write-Time Reconciliation](https://arxiv.org/abs/2604.18478v1) | S | S | S | S | I | S | N |  |  |
| AVALIACAO_ESTADO | [Detecting Clinical Discrepancies in Health Coaching Agents: A Dual-Stream Memory and Reconciliation Architecture](https://arxiv.org/abs/2604.27045v1) | S | S | S | S | N | S | N |  |  |
| AVALIACAO_ESTADO | [Learning How and What to Memorize: Cognition-Inspired Two-Stage Optimization for Evolving Memory](https://arxiv.org/abs/2605.00702v1) | S | S | S | S | N | S | N |  |  |
| AVALIACAO_ESTADO | [SAGE: A Self-Evolving Agentic Graph-Memory Engine for Structure-Aware Associative Memory](https://arxiv.org/abs/2605.12061v2) | S | S | S | S | I | S | N |  |  |
| AVALIACAO_ESTADO | [ReacTOD: Bounded Neuro-Symbolic Agentic NLU for Zero-Shot Dialogue State Tracking](https://arxiv.org/abs/2605.19077v1) | S | S | S | S | N | S | N |  |  |
| AVALIACAO_ESTADO | [MemForest: An Efficient Agent Memory System with Hierarchical Temporal Indexing](https://arxiv.org/abs/2605.23986v3) | S | S | S | S | I | S | N |  |  |
| AVALIACAO_ESTADO | [VikingMem: A Memory Base Management System for Stateful LLM-based Applications](https://arxiv.org/abs/2605.29640v3) | S | S | S | S | I | S | N |  |  |
| AVALIACAO_ESTADO | [Scaling Expert Feedback with Reflective Edit Propagation in Compositional Knowledge Bases](https://arxiv.org/abs/2606.05023v1) | S | S | S | S | I | S | N |  |  |
| AVALIACAO_ESTADO | [Organize then Retrieve: Hierarchical Memory Navigation for Efficient Agents](https://arxiv.org/abs/2606.11680v1) | S | S | S | S | I | S | N |  |  |
| AVALIACAO_ESTADO | [PROJECTMEM: A Local-First, Event-Sourced Memory and Judgment Layer for AI Coding Agents](https://arxiv.org/abs/2606.12329v2) | S | S | S | S | I | S | N |  |  |
| AVALIACAO_ESTADO | [A Unified Framework for Context-Aware and Relation-Aware Graph Retrieval-Augmented Generation](https://arxiv.org/abs/2606.18075v1) | S | S | S | S | N | S | N |  |  |
| AVALIACAO_ESTADO | [Are We Ready For An Agent-Native Memory System?](https://arxiv.org/abs/2606.24775v1) | S | S | S | S | I | S | N |  |  |
| AVALIACAO_ESTADO | [When Not to Write Memory: Governing False Promotion from Correlated Agent Traces](https://arxiv.org/abs/2607.02579v1) | S | S | S | S | I | S | N |  |  |
| AVALIACAO_ESTADO | [MRMS: A Multi-Resolution Memory Substrate for Long-Lived AI Agents](https://arxiv.org/abs/2607.04617v1) | S | S | S | S | I | S | N |  |  |
| AVALIACAO_ESTADO | [MemOps: Benchmarking Lifecycle Memory Operations in Long-Horizon Conversations](https://arxiv.org/abs/2607.12893v1) | S | S | S | S | N | S | N |  |  |
| AVALIACAO_ESTADO | [A Self-Evolving Agent for Longitudinal Personal Health Management](https://arxiv.org/abs/2607.13940v2) | S | S | S | S | N | S | N |  |  |
| AVALIACAO_ESTADO | [Presentation, Not Mechanism: A Render Confound in Deprecation-Aware Memory Evaluation](https://arxiv.org/abs/2607.16019v1) | S | S | S | S | I | S | N |  |  |
| AVALIACAO_ESTADO | [Mi-Memory: A Lifecycle Memory Framework for Personal AI](https://arxiv.org/abs/2607.18975v1) | S | S | S | S | I | S | N |  |  |
| AVALIACAO_ESTADO | [MemTX: Transactional Belief Commit for Stateful Agent Memory](https://arxiv.org/abs/2607.23929v2) | S | S | S | S | N | S | N |  |  |
| AVALIACAO_ESTADO | [TARL: Transaction-Aware Reliable Ledgers for Executable Memory Management in Long-Term Agents](https://arxiv.org/abs/2608.03699v5) | S | S | S | S | I | S | N |  |  |
| AVALIACAO_ESTADO | [LatticeMind: A Conflict-Aware Memory Primitive for Multi-Agent Systems](https://arxiv.org/abs/2608.08236v1) | S | S | S | S | N | S | N |  |  |
| AVALIACAO_ESTADO | [From Faulty Memories to Corrected Actions: Dependency-Guided Rollback Repair for Memory-Augmented Agents](https://arxiv.org/abs/2608.10502v2) | S | S | S | S | N | S | N |  |  |
| AVALIACAO_ESTADO | [Self-Correcting Long-Horizon Search Agents via Tree-Structured Memory](https://arxiv.org/abs/2608.10676v1) | S | S | S | S | N | S | N |  |  |
| AVALIACAO_ESTADO | [rEDMRec: Distilling Large Language Model Reasoning into an Editable Experience Memory for Recommendation](https://arxiv.org/abs/2608.18952v1) | S | S | S | S | I | S | N |  |  |
| AVALIACAO_ESTADO | [EvoWiki: Incremental State Overwriting and Traceable Question Answering for Cross-Meeting Knowledge Evolution](https://arxiv.org/abs/2608.23265v1) | S | S | S | S | I | S | N |  |  |
| AVALIACAO_ESTADO | [AtomRec: Evolving Atomic Memory for Agentic Recommendation](https://arxiv.org/abs/2609.04882v1) | S | S | S | S | I | S | N |  |  |
| AVALIACAO_ESTADO | [ROAM: Robust Organization of Atomic Memories for Agents through Semantic Relations](https://arxiv.org/abs/2609.09778v1) | S | S | S | S | I | S | N |  |  |
| AVALIACAO_ESTADO | [MACE: Memory-Agent Co-Evolution with Adaptive Memory Graphs for Multi-Agent Systems](https://arxiv.org/abs/2609.21533v1) | S | S | S | S | I | S | N |  |  |
| AVALIACAO_ESTADO | [AutoViewMem: Self-Configuring Orthogonal Views for Conversational Long-Term Memory](https://arxiv.org/abs/2609.21940v1) | S | S | S | S | N | S | N |  |  |
| AVALIACAO_ESTADO | [Governed AI-Agent Coordination for Dementia Care: Architecture, Safety Contracts, and Evidence-Derived Workflow Verification](https://arxiv.org/abs/2609.25956v2) | S | S | S | S | I | S | N |  |  |
| AVALIACAO_ESTADO | [Not All Memories Are Equal: Hierarchical Collaborative Memory for Validity-Aware Retrieval in LLM Agents](https://arxiv.org/abs/2609.30289v1) | S | S | S | S | I | S | N |  |  |
| AVALIACAO_ESTADO | [ARSM: Auto-Regressive State Machine for Agentic Reasoning Compression](https://arxiv.org/abs/2609.32852v1) | S | S | S | S | N | S | N |  |  |
| AVALIACAO_ESTADO | [TRACE: Governing Memory Validity in Evolving Multi-Agent Systems](https://arxiv.org/abs/2609.33517v1) | S | S | S | S | N | S | N |  |  |
| AVALIACAO_ESTADO | [HGP:An on-device personalized agent memory via hybrid graph storage](https://arxiv.org/abs/2610.10071v1) | S | S | S | S | I | S | N |  |  |
| AVALIACAO_ESTADO | [Test-Time Memory Construction](https://arxiv.org/abs/2610.13348v1) | S | S | S | S | N | S | N |  |  |
| AVALIACAO_ESTADO | [MARS: Memory-Aware Routing for Session-Level LLM Assistants](https://arxiv.org/abs/2610.23583v1) | S | S | S | S | N | S | N |  | C5(tamanho) |
| AVALIACAO_ESTADO | [SLICE-Mem: Salience-Gated Lightweight Information Curation for Efficient LLM Agent Memory](https://arxiv.org/abs/2610.33015v1) | S | S | S | S | N | S | N |  |  |
| AVALIACAO_ESTADO | [MedGraph: Maintaining Patient State with Graph Memory for Long-Term Medical Dialogue](https://arxiv.org/abs/2611.08433v1) | S | S | S | S | N | S | N |  |  |
| AVALIACAO_ESTADO | [TransMem: Transition-Aware Retrieval for Evolving Personal Memory](https://arxiv.org/abs/2611.10683v1) | S | S | S | S | N | S | N |  |  |
| AVALIACAO_ESTADO | [RoadState: A Domain-Prior-Enhanced Vision-Language System for Industrial-Grade Lane-Level Map Updating](https://arxiv.org/abs/2611.10746v1) | S | S | S | S | N | S | N |  |  |
| AVALIACAO_ESTADO | [MemEmo: Evaluating Emotion in Memory Systems](https://arxiv.org/abs/2611.20733v1) | S | S | S | S | I | S | N |  |  |
| MEMORIA_ESTRUTURADA | [TRAIL: Joint Inference and Refinement of Knowledge Graphs with Large Language Models](https://arxiv.org/abs/2508.04474v1) | S | S | S | S | N | N | N |  |  |
| MEMORIA_ESTRUTURADA | [RCR-Router: Efficient Role-Aware Context Routing for Multi-Agent LLM Systems with Structured Memory](https://arxiv.org/abs/2508.04903v5) | S | S | S | S | I | N | N |  |  |
| MEMORIA_ESTRUTURADA | [Memory-R1: Enhancing Large Language Model Agents to Manage and Utilize Memories via Reinforcement Learning](https://arxiv.org/abs/2508.19828v5) | S | S | S | S | I | N | N |  |  |
| MEMORIA_ESTRUTURADA | [Text2Mem: A Unified Memory Operation Language for Memory Operating System](https://arxiv.org/abs/2509.11145v2) | S | S | S | S | I | I | N |  |  |
| MEMORIA_ESTRUTURADA | [COSMIR: Chain Orchestrated Structured Memory for Iterative Reasoning over Long Context](https://arxiv.org/abs/2510.04568v1) | S | S | S | S | N | N | N |  |  |
| MEMORIA_ESTRUTURADA | [Learning from Supervision with Semantic and Episodic Memory: A Reflective Approach to Agent Adaptation](https://arxiv.org/abs/2510.19897v3) | S | S | S | S | N | N | N |  |  |
| MEMORIA_ESTRUTURADA | [MGA: Memory-Driven GUI Agent for Observation-Centric Interaction](https://arxiv.org/abs/2510.24168v3) | S | S | S | S | N | I | N |  | C5(tamanho) |
| MEMORIA_ESTRUTURADA | [ENGRAM: Effective, Lightweight Memory Orchestration for Conversational Agents](https://arxiv.org/abs/2511.12960v2) | S | S | S | S | I | N | N |  |  |
| MEMORIA_ESTRUTURADA | [TeleMem: Building Long-Term and Multimodal Memory for Agentic AI](https://arxiv.org/abs/2601.06037v4) | S | S | S | S | N | N | N |  |  |
| MEMORIA_ESTRUTURADA | [MemWeaver: Weaving Hybrid Memories for Traceable Long-Horizon Agentic Reasoning](https://arxiv.org/abs/2601.18204v1) | S | S | S | S | N | I | N |  |  |
| MEMORIA_ESTRUTURADA | [GLOVE: Global Verifier for LLM Memory-Environment Realignment](https://arxiv.org/abs/2601.19249v2) | S | S | S | S | N | N | N |  |  |
| MEMORIA_ESTRUTURADA | [AMA: Adaptive Memory via Multi-Agent Collaboration](https://arxiv.org/abs/2601.20352v4) | S | S | S | S | N | N | N |  |  |
| MEMORIA_ESTRUTURADA | [Darwinian Memory: A Training-Free Self-Regulating Memory System for GUI Agent Evolution](https://arxiv.org/abs/2601.22528v1) | S | S | S | S | N | N | N |  |  |
| MEMORIA_ESTRUTURADA | [Live-Evo: Online Evolution of Agentic Memory from Continuous Feedback](https://arxiv.org/abs/2602.02369v1) | S | S | S | S | I | N | N |  |  |
| MEMORIA_ESTRUTURADA | [Panini: Continual Learning in Token Space via Structured Memory](https://arxiv.org/abs/2602.15156v1) | S | S | S | S | N | I | N |  |  |
| MEMORIA_ESTRUTURADA | [Your Code Agent Can Grow Alongside You with Structured Memory](https://arxiv.org/abs/2603.13258v1) | S | S | S | S | N | N | N |  |  |
| MEMORIA_ESTRUTURADA | [D-MEM: Dopamine-Gated Agentic Memory via Reward Prediction Error Routing](https://arxiv.org/abs/2603.14597v1) | S | S | S | S | I | I | N |  |  |
| MEMORIA_ESTRUTURADA | [MERIT: Memory-Enhanced Retrieval for Interpretable Knowledge Tracing](https://arxiv.org/abs/2603.22289v1) | S | S | S | S | N | N | N |  |  |
| MEMORIA_ESTRUTURADA | [AnalogAgent: Self-Improving Analog Circuit Design Automation with LLM Agents](https://arxiv.org/abs/2603.23910v1) | S | S | S | S | I | N | N |  |  |
| MEMORIA_ESTRUTURADA | [GUIDE: Guided Updates for In-context Decision Evolution in LLM-Driven Spacecraft Operations](https://arxiv.org/abs/2603.27306v3) | S | S | S | S | N | N | N |  |  |
| MEMORIA_ESTRUTURADA | [Drawing on Memory: Dual-Trace Encoding Improves Cross-Session Recall in LLM Agents](https://arxiv.org/abs/2604.12948v1) | S | S | S | S | N | N | N |  |  |
| MEMORIA_ESTRUTURADA | [APEX-MEM: Agentic Semi-Structured Memory with Temporal Reasoning for Long-Term Conversational AI](https://arxiv.org/abs/2604.14362v1) | S | S | S | S | N | I | N |  | C6 |
| MEMORIA_ESTRUTURADA | [BOOKMARKS: Efficient Active Storyline Memory for Role-playing](https://arxiv.org/abs/2605.14169v2) | S | S | S | S | N | N | N |  |  |
| MEMORIA_ESTRUTURADA | [Auto-Dreamer: Learning Offline Memory Consolidation for Language Agents](https://arxiv.org/abs/2605.20616v1) | S | S | S | S | N | N | N |  |  |
| MEMORIA_ESTRUTURADA | [The Model Is Not the Product: A Dual-Pillar Architecture for Local-First Psychological Coaching](https://arxiv.org/abs/2605.24411v1) | S | S | S | S | I | N | N |  |  |
| MEMORIA_ESTRUTURADA | [eMEM: A Hybrid Spatio-Temporal Memory System For Embodied Agents](https://arxiv.org/abs/2606.03374v2) | S | S | S | S | N | N | N |  |  |
| MEMORIA_ESTRUTURADA | [Temporal Order Matters for Agentic Memory: Segment Trees for Long-Horizon Agents](https://arxiv.org/abs/2606.04555v1) | S | S | S | S | I | N | N |  |  |
| MEMORIA_ESTRUTURADA | [PersonaTree: Structured Lifecycle Memory for Person Understanding in LLM Agents](https://arxiv.org/abs/2606.04780v1) | S | S | S | S | I | N | N |  |  |
| MEMORIA_ESTRUTURADA | [MemToolAgent: Leveraging Memory for Tool Using Agents Based on Environment and User Feedback](https://arxiv.org/abs/2606.07909v2) | S | S | S | S | I | N | N |  |  |
| MEMORIA_ESTRUTURADA | [REAL: A Reasoning-Enhanced Graph Framework for Long-Term Memory Management of LLMs](https://arxiv.org/abs/2606.10694v1) | S | S | S | S | N | N | N |  |  |
| MEMORIA_ESTRUTURADA | [G-Long: Graph-Enhanced Memory Management for Efficient Long-Term Dialogue Agents](https://arxiv.org/abs/2606.13115v1) | S | S | S | S | N | I | N |  |  |
| MEMORIA_ESTRUTURADA | [DYNA : Dynamic Episodic Memory Networks for Augmenting Large Language Models with Temporal Knowledge Graphs in Continuous Learning](https://arxiv.org/abs/2606.15778v1) | S | S | S | S | N | I | N |  |  |
| MEMORIA_ESTRUTURADA | [AtomMem: Building Simple and Effective Memory System for LLM Agents via Atomic Facts](https://arxiv.org/abs/2606.19847v2) | S | S | S | S | N | N | N |  |  |
| MEMORIA_ESTRUTURADA | [LedgerAgent: Structured State for Policy-Adherent Tool-Calling Agents](https://arxiv.org/abs/2606.20529v1) | S | S | S | S | N | N | N |  |  |
| MEMORIA_ESTRUTURADA | [ISM:Self-Improving Strategy Memory for Continual Mathematical Reasoning](https://arxiv.org/abs/2606.31191v3) | S | S | S | S | N | N | N |  |  |
| MEMORIA_ESTRUTURADA | [ECHO: Prune To Act, Trace To Learn With Selective Turn Memory In Agentic RL](https://arxiv.org/abs/2606.31650v5) | S | S | S | S | I | N | N |  |  |
| MEMORIA_ESTRUTURADA | [Imprint: Online Memory Compression for Long-Horizon Egocentric QA](https://arxiv.org/abs/2607.00696v1) | S | S | S | S | N | I | N |  |  |
| MEMORIA_ESTRUTURADA | [Danus: Orchestrating Mathematical Reasoning Agents with Fact-Graph Memory](https://arxiv.org/abs/2607.06447v2) | S | S | S | S | N | I | N |  |  |
| MEMORIA_ESTRUTURADA | [Track, Rank, Crack: Epistemic Working Memory Scales Multi-Hop Reasoning in Language Agents](https://arxiv.org/abs/2607.12267v1) | S | S | S | S | N | N | N |  |  |
| MEMORIA_ESTRUTURADA | [Memory as a Controlled Process: Learned Adaptive Memory Management for LLM Agents](https://arxiv.org/abs/2607.13591v3) | S | S | S | S | N | N | N |  |  |
| MEMORIA_ESTRUTURADA | [ZifaMem: Structured Memory for Persona, Preference, and Emotional Continuity in AI Companions](https://arxiv.org/abs/2607.17564v1) | S | S | S | S | I | N | N |  |  |
| MEMORIA_ESTRUTURADA | [SF-AMS: Strategic Forgetting for Structured Memory in LLM Agent](https://arxiv.org/abs/2607.22562v1) | S | S | S | S | N | N | N |  |  |
| MEMORIA_ESTRUTURADA | [Co-Evolving Graph and Text Memory for Training-Free Multi-Hop Question Answering](https://arxiv.org/abs/2607.23278v1) | S | S | S | S | N | I | N |  |  |
| MEMORIA_ESTRUTURADA | [MemLens: A Value-Aware Memory Management System with Interactive Analytics for LLM-based Agents](https://arxiv.org/abs/2607.25992v1) | S | S | S | S | I | N | N |  |  |
| MEMORIA_ESTRUTURADA | [A Graph-Native Bitemporal Memory Store for Conversational AI Agents](https://arxiv.org/abs/2607.26520v1) | S | S | S | S | N | N | N |  |  |
| MEMORIA_ESTRUTURADA | [OptGraph: Large Language Models Enhanced Evolutionary Optimization Via Graph Retrieval-Augmented Generation](https://arxiv.org/abs/2607.27918v1) | S | S | S | S | N | N | N |  |  |
| MEMORIA_ESTRUTURADA | [LeanMem: Simple and Efficient Long-Term Memory for LLM Agents](https://arxiv.org/abs/2608.03463v1) | S | S | S | S | N | I | N |  |  |
| MEMORIA_ESTRUTURADA | [Caching for the Future: Scrub Jay Episodic Memory Principles for Agent Memory Systems](https://arxiv.org/abs/2608.04746v1) | S | S | S | S | N | I | N |  |  |
| MEMORIA_ESTRUTURADA | [Hierarchical Graph Memory for LLM Agents with Path-level Localization and Rewrite](https://arxiv.org/abs/2608.05095v1) | S | S | S | S | I | I | N |  |  |
| MEMORIA_ESTRUTURADA | [TEPA: Revoking Stale Memories for Conflict-Robust Language Agents](https://arxiv.org/abs/2608.07429v2) | S | S | S | S | I | N | N |  |  |
| MEMORIA_ESTRUTURADA | [SodaMem: Evidence-Grounded Temporal Graph Memory for LLM Agents](https://arxiv.org/abs/2608.08055v1) | S | S | S | S | I | N | N |  |  |
| MEMORIA_ESTRUTURADA | [EvoGraph-Mem: Failure-Aware Editable Graph Memory for Long-Term Language Agents](https://arxiv.org/abs/2608.11248v1) | S | S | S | S | I | N | N |  |  |
| MEMORIA_ESTRUTURADA | [LycheeMemory V2: Efficient Long-Term Memory for LLM Agents via Semantic Segment-Level Consolidation](https://arxiv.org/abs/2608.12990v2) | S | S | S | S | I | N | N |  |  |
| MEMORIA_ESTRUTURADA | [Learning What to Share and What to Personalize: Hierarchical Strategy Co-Evolution for Agent Memory](https://arxiv.org/abs/2608.25329v1) | S | S | S | S | N | N | N |  |  |
| MEMORIA_ESTRUTURADA | [Making Prospective Memory SLM-Shaped: Typed Intention Stores for Small-Model Agents](https://arxiv.org/abs/2609.01272v1) | S | S | S | S | I | I | N |  |  |
| MEMORIA_ESTRUTURADA | [Continual Graph Memory for Adaptive Recommendation under Intent Drift](https://arxiv.org/abs/2609.04651v1) | S | S | S | S | I | I | N |  |  |
| MEMORIA_ESTRUTURADA | [When Does Memory Help? A Cost-Aware Evaluation of Long-Term Memory in Tool-Using LLM Agents](https://arxiv.org/abs/2609.05441v1) | S | S | S | S | I | I | N |  |  |
| MEMORIA_ESTRUTURADA | [Kernel-Managed Shared Memory for System-Wide Personalization](https://arxiv.org/abs/2609.10144v3) | S | S | S | S | I | N | N |  |  |
| MEMORIA_ESTRUTURADA | [What Should an Agent Forget? Separating What Is Stored from What Is Used](https://arxiv.org/abs/2609.10263v1) | S | S | S | S | I | N | N |  |  |
| MEMORIA_ESTRUTURADA | [JustMem: Just-Enough Memory Access for Long-Term Conversations](https://arxiv.org/abs/2609.19877v1) | S | S | S | S | I | I | N |  |  |
| MEMORIA_ESTRUTURADA | [Jev-Mem: System-One-Controlled Agentic Memory for Efficient AI Agents](https://arxiv.org/abs/2609.23986v1) | S | S | S | S | I | N | N |  |  |
| MEMORIA_ESTRUTURADA | [Shared Worlds, Private Minds: Structured Memory for Long-Form Writing as World Creation](https://arxiv.org/abs/2609.32401v1) | S | S | S | S | N | N | N |  |  |
| MEMORIA_ESTRUTURADA | [Decoupling Memory from Context: Structured Memory for Token-Efficient Test-Time Continual Learning](https://arxiv.org/abs/2610.02687v2) | S | S | S | S | N | N | N |  |  |
| MEMORIA_ESTRUTURADA | [MINDSET: Energy-based Memory Evolution for Long Conversational Agents](https://arxiv.org/abs/2610.08586v3) | S | S | S | S | I | N | N |  |  |
| MEMORIA_ESTRUTURADA | [SkillForge: Co-Evolving Skills and Agents via Dynamic Skill Lifecycles](https://arxiv.org/abs/2610.09832v2) | S | S | S | S | N | I | N |  |  |
| MEMORIA_ESTRUTURADA | [Gated Memory: Admission-Controlled Memory Formation for Conversational AI](https://arxiv.org/abs/2610.11270v1) | S | S | S | S | N | N | N |  |  |
| MEMORIA_ESTRUTURADA | [Event-Centric Memory with Query-Aware Graph Augmentation for Long-Term Conversational Agents](https://arxiv.org/abs/2610.11920v1) | S | S | S | S | N | I | N |  |  |
| MEMORIA_ESTRUTURADA | [SEPM: Self-Evolving Procedural Memory Grounded in Task Graphs for Multi-Agent Collaboration](https://arxiv.org/abs/2610.14515v1) | S | S | S | S | I | N | N |  |  |
| MEMORIA_ESTRUTURADA | [EvolveMAS: A Unified Self-Evolution Framework for Decentralized Multi-Agent Systems](https://arxiv.org/abs/2610.34450v1) | S | S | S | S | I | I | N |  |  |
| MEMORIA_ESTRUTURADA | [Evolving Memory, Learning to Meta-Control: Test-Time Adaptation for Experience-augmented LLM Agents](https://arxiv.org/abs/2611.01747v1) | S | S | S | S | I | N | N |  |  |
| EXCLUIDO_AUSENCIA | [FSFM: A Biologically-Inspired Framework for Selective Forgetting of Agent Memory](https://arxiv.org/abs/2604.20300v2) | S | I | S | S | N | I | N | E3 |  |
| EXCLUIDO_AUSENCIA | [Supersede: Diagnosing and Training the Memory-Update Gap in LLM Agents](https://arxiv.org/abs/2606.27472v1) | S | S | S | N | S | I | N | C4=N |  |
| EXCLUIDO_AUSENCIA | [Multi-Head Recurrent Memory Agents](https://arxiv.org/abs/2607.01523v2) | S | S | S | N | N | S | N | C4=N |  |
| EXCLUIDO_AUSENCIA | [The Sleeping Agent: What Gist-Based Context Compression Loses and Why](https://arxiv.org/abs/2608.11775v1) | S | S | S | N | I | S | N | C4=N |  |
| EXCLUIDO_AUSENCIA | [Affective Agent: On-Device Personalized Intervention Reasoning for Wearable Systems](https://arxiv.org/abs/2609.12322v1) | S | N | S | S | S | I | N | C2=N |  |
| EXCLUIDO_AUSENCIA | [Beyond Downstream QA: Rethinking How We Evaluate Agent Memory](https://arxiv.org/abs/2610.22209v1) | N | S | S | S | N | S | N | C1=N |  |
| EXCLUIDO_AUSENCIA | [Agentic Knowledge-Model Co-Evolution via Consistency Regularization](https://arxiv.org/abs/2610.30790v1) | S | S | S | N | I | S | N | C4=N |  |
| EXCLUIDO_AUSENCIA | [FIRME: Registry-Anchored Memory Graphs for Evolving, Heterogeneous Investor Knowledge](https://arxiv.org/abs/2610.40482v1) | S | N | S | S | N | S | N | C2=N |  |
| EXCLUIDO_NAO_DEMONSTRADO | [MemoTime: Memory-Augmented Temporal Knowledge Graph Enhanced Large Language Model Reasoning](https://arxiv.org/abs/2510.13614v3) | S | S | S | I | N | N | N | não demonstrado: C4=I |  |
| EXCLUIDO_NAO_DEMONSTRADO | [MemRec: Collaborative Memory-Augmented Agentic Recommender System](https://arxiv.org/abs/2601.08816v3) | I | S | S | I | N | I | N | não demonstrado: C1=I C4=I | C1 |
| EXCLUIDO_NAO_DEMONSTRADO | [Factored Reasoning with Inner Speech and Persistent Memory for Evidence-Grounded Human-Robot Interaction](https://arxiv.org/abs/2602.00675v1) | S | S | S | I | I | N | N | não demonstrado: C4=I | C4 |
| EXCLUIDO_NAO_DEMONSTRADO | [Canvas-of-Thought: Grounding Reasoning via Mutable Structured States](https://arxiv.org/abs/2602.10494v1) | S | S | I | S | N | I | N | não demonstrado: C3=I | C3 |
| EXCLUIDO_NAO_DEMONSTRADO | [Scene-Aware Memory Discrimination: Deciding Which Personal Knowledge Stays](https://arxiv.org/abs/2602.11607v1) | S | S | I | I | N | S | N | não demonstrado: C3=I C4=I | C3 |
| EXCLUIDO_NAO_DEMONSTRADO | [HyMem: Hybrid Memory Architecture with Dynamic Retrieval Scheduling](https://arxiv.org/abs/2602.13933v2) | S | S | I | S | N | I | N | não demonstrado: C3=I |  |
| EXCLUIDO_NAO_DEMONSTRADO | [MemEmo: Evaluating Emotion in Memory Systems of Agents](https://arxiv.org/abs/2602.23944v1) | S | I | S | S | I | S | N | não demonstrado: C2=I |  |
| EXCLUIDO_NAO_DEMONSTRADO | [Adaptive Memory Admission Control for LLM Agents](https://arxiv.org/abs/2603.04549v1) | S | S | S | I | I | S | N | não demonstrado: C4=I |  |
| EXCLUIDO_NAO_DEMONSTRADO | [Hybrid Self-evolving Structured Memory for GUI Agents](https://arxiv.org/abs/2603.10291v1) | S | I | S | S | N | I | N | não demonstrado: C2=I |  |
| EXCLUIDO_NAO_DEMONSTRADO | [MemArchitect: A Policy Driven Memory Governance Layer](https://arxiv.org/abs/2603.18330v1) | S | S | S | I | I | N | N | não demonstrado: C4=I |  |
| EXCLUIDO_NAO_DEMONSTRADO | [REVERE: Reflective Evolving Research Engineer](https://arxiv.org/abs/2603.20667v2) | S | S | S | I | N | S | N | não demonstrado: C4=I |  |
| EXCLUIDO_NAO_DEMONSTRADO | [Memory Bear AI Memory Science Engine for Multimodal Affective Intelligence: A Technical Report](https://arxiv.org/abs/2603.22306v1) | S | I | S | S | I | N | N | não demonstrado: C2=I |  |
| EXCLUIDO_NAO_DEMONSTRADO | [TSUBASA: Improving Long-Horizon Personalization via Evolving Memory and Self-Learning with Context Distillation](https://arxiv.org/abs/2604.07894v1) | S | S | I | S | I | I | N | não demonstrado: C3=I | C3 C5 |
| EXCLUIDO_NAO_DEMONSTRADO | [From Recall to Forgetting: Benchmarking Long-Term Memory for Personalized Agents](https://arxiv.org/abs/2604.20006v1) | S | I | S | S | I | I | N | não demonstrado: C2=I |  |
| EXCLUIDO_NAO_DEMONSTRADO | [StructMem: Structured Memory for Long-Horizon Behavior in LLMs](https://arxiv.org/abs/2604.21748v1) | S | S | S | I | I | S | N | não demonstrado: C4=I |  |
| EXCLUIDO_NAO_DEMONSTRADO | [ContextWeaver: Selective and Dependency-Structured Memory Construction for LLM Agents](https://arxiv.org/abs/2604.23069v2) | S | S | S | I | N | I | N | não demonstrado: C4=I | C4 |
| EXCLUIDO_NAO_DEMONSTRADO | [STALE: Can LLM Agents Know When Their Memories Are No Longer Valid?](https://arxiv.org/abs/2605.06527v1) | I | S | I | S | N | I | N | não demonstrado: C1=I C3=I | C1 C3 C6 |
| EXCLUIDO_NAO_DEMONSTRADO | [WorldMemArena: Evaluating Multimodal Agent Memory Through Action-World Interaction](https://arxiv.org/abs/2605.29341v2) | S | S | S | I | I | S | N | não demonstrado: C4=I |  |
| EXCLUIDO_NAO_DEMONSTRADO | [SAGE: A Novelty Gate for Efficient Memory Evolution in Agentic LLMs](https://arxiv.org/abs/2605.30711v2) | S | S | S | I | I | I | N | não demonstrado: C4=I |  |
| EXCLUIDO_NAO_DEMONSTRADO | [Auto-Configuring Scientific Simulators with Lightweight Coding-Agent Adapters](https://arxiv.org/abs/2606.09774v2) | S | S | S | I | I | I | N | não demonstrado: C4=I |  |
| EXCLUIDO_NAO_DEMONSTRADO | [Detect, Remask, Repair: Diffusion Editing for Faithful Summarization of Evolving Contexts](https://arxiv.org/abs/2606.12807v2) | S | I | I | I | N | S | S | não demonstrado: C2=I C3=I C4=I |  |
| EXCLUIDO_NAO_DEMONSTRADO | [Episodic-to-Semantic Consolidation Without Identity Drift](https://arxiv.org/abs/2607.01988v1) | S | I | S | S | N | S | N | não demonstrado: C2=I |  |
| EXCLUIDO_NAO_DEMONSTRADO | [Retain or Consolidate? Budget-Dependent Operator Selection for Language Agent Memory](https://arxiv.org/abs/2607.17545v2) | S | S | I | I | N | I | N | não demonstrado: C3=I C4=I |  |
| EXCLUIDO_NAO_DEMONSTRADO | [Profile-Graph Memory for LLM Agents: Implicit Cross-Entity Traversal through Narrative Profiles](https://arxiv.org/abs/2607.19359v1) | S | S | S | I | I | N | N | não demonstrado: C4=I |  |
| EXCLUIDO_NAO_DEMONSTRADO | [When Memory Updates but Behavior Does Not: Repairing Implicit Stale Dependencies in Personalized Agent Responses](https://arxiv.org/abs/2608.01619v1) | S | S | I | S | N | S | N | não demonstrado: C3=I |  |
| EXCLUIDO_NAO_DEMONSTRADO | [CoEvo-Mem: Co-Evolving Retrieval Policy and Memory Bank for LLM Agents](https://arxiv.org/abs/2608.01739v1) | S | S | I | I | N | S | N | não demonstrado: C3=I C4=I | C3 C4 |
| EXCLUIDO_NAO_DEMONSTRADO | [Dual-Layer Agentic Memory with Fast Write Routing and Slow Consolidation](https://arxiv.org/abs/2608.22215v2) | S | S | S | I | I | S | N | não demonstrado: C4=I |  |
| EXCLUIDO_NAO_DEMONSTRADO | [Beyond Scaling: Self-Evolving LLM Agents for Hardware Kernel Optimization via an Experience-Driven Workflow and Experience Graph Memory](https://arxiv.org/abs/2608.25570v1) | S | S | I | S | N | N | N | não demonstrado: C3=I | C3 |
| EXCLUIDO_NAO_DEMONSTRADO | [Retrieval-Driven Memory Reconsolidation for Long-Term LLM Agents](https://arxiv.org/abs/2609.16053v1) | S | I | S | S | N | N | N | não demonstrado: C2=I | C2 |
| EXCLUIDO_NAO_DEMONSTRADO | [RoutePrism: Tracing Construction Order Effects in Agent Memory](https://arxiv.org/abs/2609.34160v1) | S | I | S | S | N | S | N | não demonstrado: C2=I |  |
| EXCLUIDO_NAO_DEMONSTRADO | [Stale, Misattributed, or Late: Where Personal Memory Fails Before Generation](https://arxiv.org/abs/2610.10265v1) | S | I | I | S | N | I | N | não demonstrado: C2=I C3=I | C3 C6 |
| EXCLUIDO_NAO_DEMONSTRADO | [From Retrieval to Reconstruction: Constructing Evolvable Cognitive Memory for Long-Term Dialogue](https://arxiv.org/abs/2610.11314v1) | S | S | S | I | N | I | N | não demonstrado: C4=I |  |
| EXCLUIDO_NAO_DEMONSTRADO | [Can Agents Forget When Asked? A Benchmark for Memory Unlearning in LLM Agents](https://arxiv.org/abs/2610.18052v1) | S | S | S | I | N | I | N | não demonstrado: C4=I | C4 C6 |
| EXCLUIDO_NAO_DEMONSTRADO | [Hy-Memory-30B-A3B: Attributed Reinforcement Learning for Long-Term Memory Writing](https://arxiv.org/abs/2610.22196v1) | S | I | S | S | S | N | N | não demonstrado: C2=I |  |
| EXCLUIDO_NAO_DEMONSTRADO | [TrackMem: Tracking Evolving Beliefs in Long-Horizon Financial Dialogue](https://arxiv.org/abs/2611.02001v1) | S | I | S | S | N | S | N | não demonstrado: C2=I |  |
| EXCLUIDO_NAO_DEMONSTRADO | [DoGNAVY-Exploit: Harnessing Parallel Open-Weight Agents for Stage-Aware Vulnerability Exploitation](https://arxiv.org/abs/2611.04196v1) | S | I | S | S | I | N | N | não demonstrado: C2=I |  |

## Controles

Os controles orientaram a construção das consultas; a recuperação deles não é validação independente.

| Controle | Recuperado | Após termos | Grupo etapa 1 | Classe final | Motivo |
|---|---|---|---|---|---|
| AlpsBench | sim | sim | A  | AVALIACAO_ESTADO |  |
| MemReader | sim | sim | A  | NUCLEO |  |
| DimMem | sim | sim | A  | ESCRITOR_COMPACTO |  |
| Prospective Intention Store (PIS) | sim | sim | B  | MEMORIA_ESTRUTURADA |  |
| AMU | sim | sim | B  | NUCLEO |  |
| MemLoRA | sim | sim | B  | ESCRITOR_COMPACTO |  |
| CMI-Mem | sim | sim | B  | NUCLEO |  |
| PGMem | sim | sim | B  | PENDENTE | não demonstrado: C4=I |
| The Bitter Lesson of Diffusion Language Models | sim | sim | X E10 C2=N | — |  |
| DiffuPlan | sim | sim | P  | COMPARADOR_GERATIVO |  |
| Intrinsic Memory Agents | sim | sim | B  | NUCLEO |  |
| Supersede | sim | sim | B  | EXCLUIDO_AUSENCIA | C4=N |

## Evidências dos selecionados

### Intrinsic Memory Agents: Heterogeneous Multi-Agent LLM Systems through Structured Contextual Memory

`arxiv:2508.08997` · NUCLEO · texto: html (https://arxiv.org/html/2508.08997v2)

Propõe Intrinsic Memory Agents: memórias textuais específicas por agente, atualizadas por LLM a partir das saídas de cada turno, com template genérico. Avalia em PDDL, FEVER e ALFWorld e em estudo de caso de pipeline de dados.

- **C1**: “This paper introduces Intrinsic Memory Agents, a novel framework that addresses these limitations through agent-specific memories that evolve intrinsically with agent outputs.”
- **C2**: “The memory update function f_{\text{memory\_update}} is implemented as a prompted LLM operation.”
- **C3**: “Then with the generated output O_{n,m} and the previous memory M_{n,m-1} , we update the slot content using a memory update function,”
- **C4**: “Figure 5: Manually generated prompt for the LLM agent in the PDDL task.”
- **C5**: “Llama-3.2-3b as the underlying LLM.”
- **C6**: “The Intrinsic Memory system shows consistent improvement on all metrics compared to the baseline Autogen.”

### Learning to Share: Selective Memory for Efficient Parallel Agentic Systems

`arxiv:2602.05965` · NUCLEO · texto: html (https://arxiv.org/html/2602.05965v2)

Propõe o Learning to Share (LTS), memória compartilhada textual global entre equipes paralelas de agentes LLM. Um controlador leve (Qwen3-0.6B, treinado por RL com recompensa de uso) decide quais passos entram na memória, reduzindo computação redundante e tempo de execução no AssistantBench e GAIA.

- **C1**: “we propose Learning to Share (LTS), a learned shared-memory mechanism for parallel agentic frameworks”
- **C2**: “The memory bank stores intermediate agent steps as textual key–value pairs, where each entry consists of a concise natural-language summary that serves as a retrieval key”
- **C3**: “a lightweight controller that decides whether intermediate agent steps should be added to memory or not”
- **C4**: “The memory bank stores intermediate agent steps as textual key–value pairs, where each entry consists of a concise natural-language summary”
- **C5**: “The proposed memory controller is implemented as a lightweight causal transformer based on the Qwen3-0.6B architecture”
- **C6**: “We report the percentage of steps admitted to memory, the memory recall rate, and the fraction of recalled memories used by a different team.”

### Learning to Remember: End-to-End Training of Memory Agents for Long-Context Reasoning

`arxiv:2602.18493` · NUCLEO · texto: html (https://arxiv.org/html/2602.18493v2)

Propõe UMA, agente que mantém um Memory Bank estruturado por operações CRUD em fases sequenciais de manutenção, treinado com Task-Stratified GRPO para memória reutilizável em múltiplas sessões de QA; avalia em TTL/AR e no Ledger-QA.

- **C1**: “We introduce Unified Memory Agent (UMA) for a one-to-many setting: query-agnostic external memory is constructed once from a stream and reused across multiple future QA sessions.”
- **C2**: “A single policy maintains a structured Memory Bank through CRUD operations and answers using both the Memory Bank and raw context.”
- **C3**: “A single policy maintains a structured Memory Bank through CRUD operations and answers using both the Memory Bank and raw context.”
- **C4**: “A single policy maintains a structured Memory Bank through CRUD operations and answers using both the Memory Bank and raw context.”
- **C5**: “The policy model is initialized from Qwen/Qwen3-4B-Instruct-2507”
- **C6**: “Among them, 37 cases (64.9%) are memory-formation errors, including incorrect writes, incomplete aggregation, or stale/conflicting state entries, while 20 cases”

### MemReader: From Passive to Active Extraction for Long-Term Agent Memory

`arxiv:2604.07877` · NUCLEO · texto: html (https://arxiv.org/html/2604.07877v2)

Propõe MemReader, que trata a extração de memória como gestão ativa: o MemReader-4B (ReAct, SFT e GRPO) decide adicionar, bufferizar, buscar ou ignorar entradas num estado de memória textual; o 0.6B faz extração JSON por destilação. Avalia em LOCOMO, LongMemEval e HaluMem.

- **C1**: “we introduce the MemReader family for active long-term memory extraction in agent systems”
- **C2**: “add_memory: write directly into the long-term memory store when the information is valuable and complete.”
- **C3**: “a single conversation may involve multi-turn buffer-to-add chains, so the data must support cross-turn consistency”
- **C4**: “Return a single valid JSON object with the following structure:”
- **C5**: “MemReader-4B is built on Qwen3-4B”
- **C6**: “evaluates retrieval, update, and QA ability, with metrics such as recall, precision, hallucination rate, and omission rate”

### CMI-Mem: Toward Generalizable Long-Term Memory Management via CMI-Augmented Reinforcement Learning

`arxiv:2607.20553` · NUCLEO · texto: html (https://arxiv.org/html/2607.20553v2)

Propõe CMI-Mem, RL com recompensa híbrida (QA + Informação Mútua Condicional) para treinar um gerenciador de memória que atualiza um estado estruturado em quatro slots (core, episódico, semântico, procedural) a cada sessão. Avalia em LongMemEval, LoCoMo e MemoryAgentBench.

- **C1**: “We propose CMI-Mem, a lightweight RL memory manager with a hybrid reward.”
- **C2**: “The memory store at time t is a structured object”
- **C3**: “At session t , each agent observes the context C_{t} and the relevant slice of the existing memory M_{t-1} , then emits an action a_{t}”
- **C4**: “a core profile holding stable user attributes, an episodic log of timestamped events, a semantic store of factual knowledge, and a procedural store of routines and workflows.”
- **C5**: “The policy backbone is Qwen3-4B-Instruct-2507 35”
- **C6**: “Table 3: Ablation results on LongMemEval.”

### AVERT: Audio-Verified Adjudication for Spoken Dialogue State Tracking

`arxiv:2609.01828` · NUCLEO · texto: html (https://arxiv.org/html/2609.01828v1)

Propõe AVERT, que adjudica valores de slots de estado de diálogo falado combinando concordância entre turnos com um verificador treinado sobre áudio e três operadores restritos (vote, add, swap) sobre o estado textual JSON atualizado por turno, sem retreinar o modelo base.

- **C1**: “We present AVERT, which scores each candidate value by combining cross-turn agreement with a trained audio-conditioned verifier”
- **C2**: “Conditioned on the current-turn audio and the dialogue history as text, R emits at each turn a JSON object with the ASR transcript and a first-pass state.”
- **C3**: “The state also accumulates across turns, so an early error persists and degrades later predictions.”
- **C4**: “a set of slot-value pairs (s,v) over a schema of slots S”
- **C5**: “E is a separate OLMo-2-1B (Team OLMo et al., 2025) with LoRA (Hu et al., 2022), fine-tuned once on SpokenWOZ to map A_{t} and the first-pass state to a corrected state.”
- **C6**: “We evaluate with joint goal accuracy (JGA), the fraction of turns whose predicted state matches the gold state exactly, with no missing or extra pair.”

### MoM: Memory of Memory

`arxiv:2609.25054` · NUCLEO · texto: html (https://arxiv.org/html/2609.25054v1)

Propõe Memory of Memory e P-Mem: grafo de proveniência tipado que compromete o valor atual na escrita e preserva valores substituídos. Operações tipadas (accept, contest, reject, revoke, resolve) atualizam o estado; avaliado em LongMemEval, ALFWorld-Revision e MemoryAgentBench.

- **C1**: “We instantiate MoM as Provenant Memory (P-Mem), a typed provenance graph whose active frontier exposes one current value per resolved key while displaced values are retained as provenance;”
- **C2**: “A cell renders as one line, key = value, with support, alternatives, or history appended on demand.”
- **C3**: “Arrival order fixes the direction of supersession at write time, so the reader never reconstructs it at query time.”
- **C4**: “The extractor prompt asks the 4B model for a JSON list of {key, value} objects.”
- **C5**: “The annotator is the frozen 4B model at temperature 0, emitting schema-constrained JSON”
- **C6**: “At 262k the current value survives for 99% of the 8,445 gold keys (84% in a cell whose key also matches)”

### SpeakerMem-R1: Speaker-Centered Dual-Track Memory for Multi-Party Dialogue

`arxiv:2609.26780` · NUCLEO · texto: html (https://arxiv.org/html/2609.26780v2)

Propõe SpeakerMem-R1, memória dual-track com mensagens verbatim rotuladas por falante e estados derivados em visões por pessoa e grupo, escrita incrementalmente por um Writer Qwen2.5-3B treinado com RL (GRPO) e consultada por evidências combinadas.

- **C1**: “To address both, we propose SpeakerMem-R1: its dual-track memory stores speaker-labeled verbatim messages and derived states organized into person-level and group-level views”
- **C2**: “The Writer reads each local segment and the current heads of the derived layers, returns Add, Update, or Noop, and deterministic code validates the action, adds provenance, and writes System 2.”
- **C3**: “Update cites an existing entry_id, appends a new node while reusing the owner, source, and layer coordinates of the record identified by that entry_id, and connects the states through links/superseded_by without overwriting history”
- **C4**: “owner is whom the record concerns; source is who stated or observed the information.”
- **C5**: “We train a locally deployable Qwen2.5-3B Writer with SpeakerLevenshtein and speaker-conditioned GRPO”
- **C6**: “SpeakerLevenshtein combines token-level F1 with a normalized sequence-matching rate, rather than standard edit distance.”

### CoEM: Empowering Long-Context Reasoning with Commit-on-Evidence Memory

`arxiv:2609.36935` · NUCLEO · texto: html (https://arxiv.org/html/2609.36935v2)

Propõe CoEM, agente de memória recorrente que mantém memória textual limitada em duas partes: fatos comprometidos e um conjunto pendente de trechos literais. Política aprendida por RL decide promover, manter ou descartar evidências; verificador NLI congelado aceita fatos somente se suportados pela fonte.

- **C1**: “we introduce Commit-on-Evidence Memory (CoEM), a recurrent memory agent that learns when to commit evidence”
- **C2**: “CoEM carries all retained information in the strictly bounded recurrent textual memory.”
- **C3**: “As new context arrives, a learned policy revisits each pending excerpt and decides whether to promote it to the committed memory, retain it for further consideration, or discard it.”
- **C4**: “Each fact consumes one promotion attempt from every source record it uses”
- **C5**: “We evaluate CoEM using Qwen3.5-4B and Qwen3.5-9B backbones on HotpotQA, 2WikiMultiHopQA, and MuSiQue.”
- **C6**: “Calibrated DeBERTaV3-small achieves 97.2% faithfulness at 4.3 ms/pair, versus Qwen3.5-9B’s 97.5% at 295.0 ms/pair”

### AMU:Admission and Memory Update for Personalized Conversations---Structured Memory with SLM Guided Control

`arxiv:2609.36976` · NUCLEO · texto: html (https://arxiv.org/html/2609.36976v2)

Propõe AMU, framework de controle da escrita de memória pessoal em conversas: filtro estruturado decide o que entra e um SLM decide se o registro é armazenado, descartado como duplicata ou fundido como atualização. Avaliado em benchmark sintético controlado.

- **C1**: “we present AMU: Admission and Memory Update for Personalized Conversations, an SLM-guided (Small language model guided) structured framework for writing-time memory control.”
- **C2**: “The resulting memories are kept as structured records and serialized into plain-text memory context for downstream RAG-based response generation.”
- **C3**: “Separate records are inserted, duplicate records are discarded, and update-related records are fused into the current reusable memory state.”
- **C4**: “AMU maintains personalized memories as structured and updateable records rather than an append-only log of user utterances.”
- **C5**: “Qwen3.5-2B is used as the default SLM for memory management unless otherwise specified.”
- **C6**: “Action Acc., defined as the percentage of memory-writing turns whose predicted action matches the gold action label.”

### MemRevise: A Self-Revising Memory for Conversational Search

`arxiv:2610.24672` · NUCLEO · texto: html (https://arxiv.org/html/2610.24672v1)

Propõe MemRevise, uma memória de grafo revisável por turno, escrita por um modelo de 3B destilado de memórias-professor; operações de adicionar, abandonar e reativar tópicos mantêm fatos. A busca combina a consulta do escritor e a memória. Também cria o benchmark ConvRevise.

- **C1**: “We also propose MemRevise, which combines a revisable memory graph, a 3B writer distilled from teacher memories, and a search guided by the memory.”
- **C2**: “The writer reads each turn once and adds a node that records its topic, its facts and the operations it performs, such as adding, abandoning or reactivating a topic.”
- **C3**: “A 3B writer, distilled from teacher-generated memories, updates the memory with one call per turn and produces the search query”
- **C4**: “Each node also records the earlier turns it depends on and the topics it contradicts, forming a graph over the conversation.”
- **C5**: “a Ministral-3-3B (Liu et al., 2026) model with a LoRA adapter (Hu et al., 2021) is trained to reproduce them.”
- **C6**: “On the development sets, the writer applies Reactivate on 88% of immediate returns, Abandon on 96% of corrections, and Revise on 98% of value revisions”

### Accept Changes, Reject Attacks: Robust Memory Management for LLM Agents

`arxiv:2610.41056` · NUCLEO · texto: html (https://arxiv.org/html/2610.41056v1)

Propõe formalização de robustez de memória de agentes LLM (desatualização e envenenamento) com métrica ECA e receita one-shot de SFT de um gerenciador de memória (Qwen3-4B) treinado com episódios sintetizados sobre uma conversa; memória textual com operações ADD/UPDATE/DELETE/NOOP e leitura seletiva.

- **C1**: “We improve robustness with a simple one-shot recipe that fine-tunes the manager on episodes synthesized around a single conversation with inserted changes and attacks.”
- **C2**: “Memory managers are either prompted, as in Mem0 and A-Mem”
- **C3**: “A memory manager decides how each input changes this memory and which records the agent acts on.”
- **C4**: “The first part is a list of operations, as in Mem0 and Memory-R1 (Chhikara et al., 2025; Yan et al., 2026): ADD a record from o_{t} , UPDATE or DELETE a retrieved record, or NOOP.”
- **C5**: “Every manager in the main text uses Qwen3-4B (Yang et al., 2025a) as its backbone”
- **C6**: “ECA scores the answer and also checks the records the manager selects, so one score covers both problems.”

## Custo

etapa1: 36 requisições, 38,00 créditos; etapa2: 189 requisições, 191,29 créditos.
