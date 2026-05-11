# Florestia · Trello · Dois Marcos: MVP 19/05 + Feira 01–07/06

Tradução do quadro de mentoria de 11/05/26 + roadmap anterior em estrutura Trello.
Dois marcos com escopos distintos:

- **🎯 MVP 19/05** (8 dias úteis): prova do conceito educacional para o mentor. Escopo defensável, não completo.
- **🌳 Feira 01–07/06** (≈ 13 dias úteis pós-MVP): produto polido frente ao júri Amazon Hacking. Inclui distribuição, polimento, validação pedagógica e identidade do estande.

---

## Critério de corte entre os dois marcos

| Pergunta | Se sim → MVP 19/05 | Se sim → Feira |
|---|---|---|
| O mentor precisa ver isso na sala 19/05? | ✅ | — |
| Quebra se não estiver pronto para o júri? | — | ✅ |
| Exige logística externa (escola, pedagogos, build em hardware-alvo)? | — | ✅ |
| Modelo C / spec / documento? | ✅ (entrega de aprendizado) | — |
| Arte/áudio/animação de polimento? | — | ✅ |
| Demonstrável em laptop com Unity Editor? | ✅ | — |
| Exige WebGL build ou produto distribuível? | — | ✅ |

Em caso de empate, vence o card que conecta mais o jogo ao contexto educacional da Vila Jutaí (caixa central do quadro do mentor: *"Diferencial é focar no contexto educacional do jogo"*).

---

## Estrutura do Board

**Nome sugerido:** `Florestia · Amazon Hacking 2026`

### Listas (colunas, em ordem)

1. **📋 Backlog** — escopo do projeto sem marco definido
2. **🎯 Sprint MVP (12–19/05)** — entra no que o mentor vai ver
3. **🌳 Sprint Feira (20/05–07/06)** — entra no produto final para o júri
4. **🔧 Em Curso** — alguém pegou e está executando agora
5. **🔍 Em Validação** — pronto, aguardando review/teste interno
6. **✅ Concluído** — entregue na branch principal

### Labels

| Label | Uso |
|---|---|
| `mentor` | Pedido direto de VH / Fabio na rodada 11/05 |
| `ux-kid-first` | Interface adaptada para 8–11 anos |
| `mecânica` | Código / lógica de jogo |
| `educação` | Camada pedagógica / BNCC |
| `modelo-c` | Atualização do canvas v2 |
| `pesquisa` | Leitura / estudo / validação externa |
| `apresentação` | Slide / pitch / demo |
| `polimento` | Arte / áudio / build / balance |
| `feira` | Trabalho específico para o estande Amazon Hacking |

### Membros

`vanni` · `kadu` · `tex` · `biel` · `luiz` · `pm`

### Standup

Diário, começando 11/05. Formato:
- O que fechei desde ontem
- O que vou pegar hoje
- Bloqueios

---

## 🎯 Sprint MVP (até 19/05)

### Mentor — pedidos diretos VH/Fabio (versão MVP)

| # | Card | Labels | Dono | Due |
|---|---|---|---|---|
| M01 | **Refatorar terminologia "ADMIN" → "intermediadora"** no jogo, Modelo C e relato | `mentor` `educação` | vanni | 13/05 |
| M02 | **Mudar conceitos / termos do HUD para entendimento infantil** — margem %, "Estoque", "Resumo Noturno" etc.; lista exaustiva auditada | `mentor` `ux-kid-first` `educação` | vanni | 14/05 |
| M03 | **Simplificar nível da educação financeira para 8–11 anos** — auditar cada texto/feedback em busca de jargão adulto | `mentor` `educação` `ux-kid-first` | vanni + biel | 15/05 |
| M04 | **Mostrar insights de aprendizado financeiro e da agricultura em-jogo** (não só na EndScreen) — versão básica: pop-up curto após eventos-chave | `mentor` `educação` `mecânica` | biel | 16/05 |
| M05a | **Refinar diálogos dos compradores · v1 MVP** — remover tom pretensioso, sem precisar ainda da representatividade plena (essa fica para Feira) | `mentor` `educação` | kadu | 17/05 |
| M06 | **Spec do segundo cliente técnico "ADMIN" (dashboard professora)** — escopo, métricas exibidas, wireframe. *Implementação fica para Feira.* | `mentor` `mecânica` `educação` | vanni + pm | 16/05 |
| M07 | **Slide para apresentação do MVP (mentor-facing)** — gifs e sprites do jogo, fundamentação BNCC, status das mudanças | `mentor` `apresentação` | kadu + tex | 18/05 |

### Mecânica nova exigida pelo mentor (versão MVP)

| # | Card | Labels | Dono | Due |
|---|---|---|---|---|
| ME01 | **Implementar save persistente por aluno** — histórico de plantios, vendas, perguntas. Base para perguntas adaptativas. | `mecânica` `educação` | luiz | 14/05 |
| ME02 | **Pergunta diária no resumo do dia · MVP-do-MVP** — uma pergunta por dia, conteúdo escolhido a partir do que o aluno plantou/vendeu. Banco de perguntas hardcoded por cultura, sem geração procedural (essa fica para Feira). | `mecânica` `educação` `mentor` | vanni + biel | 16/05 |
| ME03 | **Fallback de histórico se o aluno não plantou** — exibe revisão histórica em vez de pergunta nova | `mecânica` `educação` | vanni | 16/05 |
| ME04 | **"Dia" como uma fase com encerramento educacional** — ampliar Resumo Noturno com a pergunta de ME02 | `mecânica` `educação` | biel | 17/05 |

### Sprint 1 easy wins (marcados "II")

| # | Card | Labels | Dono | Due |
|---|---|---|---|---|
| S01 | **Barra "life" de tempo de plantação** — feedback visual da maturação | `mecânica` `ux-kid-first` | luiz | 13/05 |
| S02 | **Melhorar painel de vendas com o vendedor** — refinar o redesenho que já começou | `ux-kid-first` `mecânica` | tex | 13/05 |

### Kid-first UX (do roadmap anterior · entra no MVP)

| # | Card | Labels | Dono | Due |
|---|---|---|---|---|
| UX01 | **Refatorar MarketScene kid-first** — stepper `◀ N ▶`, label dinâmico do botão vender, remover % da margem, buyer face no terço esquerdo | `ux-kid-first` `mecânica` `mentor` | tex + biel | 15/05 |
| UX02 | **Refatorar EndScreen com hero zone** — ilustração por desfecho, frase única, gráfico com rótulos de dia + zero line + marca do dia da virada, tip contextual | `ux-kid-first` `mecânica` `mentor` | vanni | 16/05 |
| UX03 | **Atualizar MarketSceneBuilder e HUDBuilder com layout final** — eliminar os fallbacks runtime que sobrepõem o layout do builder | `ux-kid-first` `mecânica` | tex | 17/05 |

### Modelo C v2 — quebrado por campo

| # | Card | Labels | Dono | Due |
|---|---|---|---|---|
| MC01 | **Modelo C v2 · Cliente** — professora como intermediadora + dashboard ADMIN como cliente técnico paralelo | `modelo-c` | pm | 14/05 |
| MC02 | **Modelo C v2 · Mecanismos de Receita** — Steam vs Governo vs empresas de jogos; recomendação argumentada | `modelo-c` | pm | 15/05 |
| MC03 | **Modelo C v2 · Acompanhamento do Impacto** — indicadores automáticos do jogo + pedagógicos | `modelo-c` `educação` | pm | 16/05 |
| MC04 | **Modelo C v2 · Foco do Impacto / Ações** — representatividade cultural brasileira/jutaí explícita | `modelo-c` `educação` | pm | 17/05 |

### Pesquisa (alimenta cards de mecânica)

| # | Card | Labels | Dono | Due |
|---|---|---|---|---|
| P01 | **Estudo histórico/cultural da Vila Jutaí · preliminar** — perfil quilombola/ribeirinho, cultivos locais. Output MVP: doc de 1-2 páginas com fatos utilizáveis em diálogos. (Versão profunda no Feira.) | `pesquisa` | tex + kadu | 17/05 |
| P02 | **Estudo do formato do resumo do dia** — referências de jogos educativos com feedback diário; valida ME02 | `pesquisa` `educação` | vanni + biel | 15/05 |
| P03 | **Pesquisa em educação financeira infantil · preliminar** — artigos e materiais 8–11 anos; output MVP: lista de conceitos válidos para a faixa etária | `pesquisa` `educação` | luiz + pm | 17/05 |

---

## 🌳 Sprint Feira (20/05 – 07/06)

### Mentor — versões avançadas dos pedidos

| # | Card | Labels | Dono | Notas |
|---|---|---|---|---|
| F-M05b | **Diálogos dos compradores · v2 Feira** — versão com representatividade cultural plena, falas alinhadas ao estudo de P01 | `mentor` `educação` `feira` | kadu | Depende de P01-avançado |
| F-M06b | **Implementar dashboard ADMIN funcional** — métricas reais, visualizações, exportação simples | `mentor` `mecânica` `educação` `feira` | luiz + pm | Spec já fechada em M06 |
| F-M07b | **Slide para o júri Amazon Hacking** — narrativa diferente do mentor: contexto territorial, demonstração de impacto, ODS 1 e 4, próximos passos | `mentor` `apresentação` `feira` | kadu + tex + vanni | Deck completo, não só update |

### Mecânica · polimento da pergunta adaptativa

| # | Card | Labels | Dono | Notas |
|---|---|---|---|---|
| F-ME02b | **Pergunta diária · v2 Feira** — banco de perguntas expandido, lógica adaptativa baseada em desempenho passado (acertou 3× a margem do açaí → pergunta sobe de dificuldade), insights culturais alternando | `mecânica` `educação` `feira` | vanni + biel | Versão MVP em ME02 |
| F-ME05 | **Histórico longitudinal por aluno** — extensão do save para alimentar o dashboard ADMIN com dados ao longo de múltiplas sessões | `mecânica` `educação` `feira` | luiz | Extensão de ME01 |

### Polimento técnico

| # | Card | Labels | Dono | Notas |
|---|---|---|---|---|
| F-B01 | **Build WebGL** — testar em Chromebook real | `polimento` `feira` | tex | Crítico para distribuição em escola |
| F-B02 | **Audio mínimo** — ambient da roça (loop curto) + 4-5 SFX (plantar/regar/colher/vender/recusar) | `polimento` `feira` | biel | Diretórios em Assets/Audio existem vazios |
| F-B03 | **Frames adicionais de walkUp e walkSide** | `polimento` `feira` | kadu | Atualmente 1 frame, animação estática |
| F-B04 | **Decisão de balanceamento stamina** — atualizar GDD ou assets (3/2/3 vs 1/1/1) | `polimento` `educação` `feira` | pm + vanni | Doc + asset edit |
| F-B05 | **Visualização da progressão do dia** — sky gradient já existe, complementar com indicadores visuais de fase do dia (manhã / tarde / fim de tarde) | `mentor` `ux-kid-first` `feira` | biel | Pedido VH |
| F-B06 | **Roupas do personagem** — variação visual mínima ou conjuntos por dia | `polimento` `mentor` `feira` | kadu | Marcado ✓ no quadro, validar e completar |

### Pesquisa avançada e validação externa

| # | Card | Labels | Dono | Notas |
|---|---|---|---|---|
| F-P01b | **Estudo histórico/cultural · v2 profundo** — fontes primárias se possível, parcerias com lideranças locais | `pesquisa` `feira` | tex + kadu | Extensão de P01 |
| F-P03b | **Pesquisa educação financeira infantil · v2 profundo** — review sistemático, output em artigo curto | `pesquisa` `educação` `feira` | luiz + pm | Extensão de P03 |
| F-VAL01 | **Marcar e realizar reunião com profissional pedagogo** — validação do modelo de ensino, captura de feedback | `pesquisa` `feira` | grupo todo | Dono específico: definir no standup 12/05 |
| F-VAL02 | **Aplicação piloto curta na escola Vila Jutaí** — se logisticamente viável antes da Feira | `pesquisa` `educação` `feira` | grupo todo | Depende de comunicação com a escola |

### Modelo de negócio (continuação pós-MVP)

| # | Card | Labels | Dono | Notas |
|---|---|---|---|---|
| F-MC05 | **Abordagem a empresas de jogos** — listar potenciais parceiros, draft de proposta | `modelo-c` `mentor` `feira` | pm | Pedido VH |
| F-MC06 | **Plano de continuidade pós-Amazon-Hacking** — Sec. Municipal Educação Moju, Prefeitura, próximos passos institucionais | `modelo-c` `feira` | pm | Depende de MC02 |

### Feira-específico (estande, materiais, demo)

| # | Card | Labels | Dono | Due |
|---|---|---|---|---|
| F-EST01 | **Plano de demonstração presencial no estande** — fluxo de demo de 2-5 minutos para visitante casual, plus demo aprofundada para júri | `apresentação` `feira` | tex + vanni | 30/05 |
| F-EST02 | **Materiais visuais do estande** — banner, folder, identidade gráfica (paleta Florestia já existe) | `apresentação` `feira` | kadu + biel | 02/06 |
| F-EST03 | **Setup técnico do estande** — laptop(s) com WebGL/standalone build, energia, conectividade redundante, save anônimo pra cada visitante | `feira` | tex + luiz | 31/05 |
| F-EST04 | **Q&A prep para o júri** — antecipar perguntas (impacto, escalabilidade, sustentabilidade financeira, alinhamento BNCC) e treinar respostas | `apresentação` `feira` | grupo todo | 04/06 |
| F-EST05 | **Iteração com feedback do mentor 19/05** — pegar críticas da apresentação do MVP, priorizar 3 ajustes de maior retorno antes da Feira | `feira` `mentor` | grupo todo | 21/05 |

---

## 📋 Backlog (sem marco · futuro pós-Feira)

| # | Card | Labels | Notas |
|---|---|---|---|
| BL01 | Sistema de NPCs com diálogo (GDD §10 v2) | `mecânica` `polimento` | Fora de escopo do MVP e da Feira |
| BL02 | Expansão do grid além de 6×6 | `mecânica` `polimento` | Pós-Feira |
| BL03 | Múltiplas save slots por dispositivo | `mecânica` | Pós-Feira |
| BL04 | Deterioração de estoque | `mecânica` | Pós-Feira |
| BL05 | Sistema de clima (chuva como bônus de crescimento) | `mecânica` | Pós-Feira |
| BL06 | Implementação em escolas além da Vila Jutaí | `educação` | Depende do resultado da Feira |

---

## ✅ Concluído (Sprint MVP)

Já feito ou parcial — confirmar com a equipe antes de mover:

- Substituir TMP_Dropdown por três botões de cultura (✓ código + builder)
- Resumo Noturno modal versão básica (perguntas vêm em ME02/M02)
- Multi-quantity sell (slider funciona, vira stepper em UX01)
- HUD math coaching labels (preview + proporcionalidade)
- EDD compliance PDF (12 páginas, Kami)
- Verificar "jogo Fácil" ✓ e "jogo Intuitivo" ✓ (marcados no quadro — validar com equipe)

---

## Snapshot de atribuições (11/05)

### Sprint MVP

| Pessoa | Cards MVP | Foco |
|---|---|---|
| **vanni** | M01, M02, M03, M06, ME02, ME03, UX02, P02 | Refactor terminologia, EndScreen, perguntas |
| **biel** | M03, M04, ME02, ME04, UX01, P02 | MarketScene, perguntas, insights in-game |
| **kadu** | M05a, M07, P01 | Diálogos, slide, estudo cultural |
| **tex** | S02, M07, UX01, UX03, P01 | Painel vendas, slide, MarketScene |
| **luiz** | ME01, S01, P03 | Save persistente, life bar, pesquisa infantil |
| **pm** | M06, MC01-MC04, P03 | Modelo C v2 inteiro + spec dashboard + pesquisa |

> pm está pesado. Considerar mover P03 inteiramente para luiz e pm assumir só os Modelo C cards.

### Sprint Feira (preview)

| Pessoa | Foco principal |
|---|---|
| **vanni** | Perguntas adaptativas v2, slide júri, narrativa |
| **biel** | Audio, sky gradient, perguntas adaptativas v2 |
| **kadu** | Diálogos v2, walk frames, materiais do estande |
| **tex** | WebGL build, plano de demo, setup técnico estande |
| **luiz** | Dashboard ADMIN funcional, histórico longitudinal |
| **pm** | Dashboard ADMIN, plano de continuidade, pesquisa v2 |

---

## Notas de processo

- **Diferencial é focar no contexto educacional do jogo** (caixa central do quadro do mentor) — critério de desempate sempre que dois cards competirem.
- **Pergunta diária baseada no histórico** é a mecânica nova de maior risco. Ter uma versão hardcoded até 16/05 e iterar; a versão adaptativa de verdade fica para a Feira.
- **Dashboard ADMIN**: spec no MVP (M06), implementação na Feira (F-M06b). Não deixar virar bloqueio na sprint MVP.
- **Feedback do mentor 19/05** entra na Sprint Feira via F-EST05 — reservar um dia (21/05) só para reagendar prioridades pós-apresentação.
- **Tickets "II"** = easy wins, não prioridade alta no sentido tradicional. São fáceis, não necessariamente mais importantes.
