# Florestia · Trello · Dois Marcos: MVP 19/05 + Feira 01–07/06

Tradução do quadro de mentoria de 11/05/26 + roadmap anterior em estrutura Trello.

- **🎯 MVP 19/05** — POC para o mentor. Adaptação do jogo existente para ser kid-friendly + mecânicas educacionais novas em versão hardcoded + Modelo C v2 + slide. Não é rebuild.
- **🌳 Feira 01–07/06** — Produto polido para o júri Amazon Hacking. Inclui WebGL, audio, dashboard da intermediadora funcional, validação pedagógica, sprite art, estande.

**Datas em dias corridos** (trabalha no FDS).

> **Regra geral de execução:** todo código é do `vanni`. Os outros (`kadu`, `tex`, `biel`, `luiz`, `pm`) produzem conteúdo, spec, pesquisa, slide, assets, ou estratégia — nunca código. Cards de implementação dependem dos cards de conteúdo correspondentes (sinalizado com **depende:** abaixo).

---

## Critério de corte entre os dois marcos

| Pergunta | MVP 19/05 | Feira |
|---|---|---|
| Mentor precisa ver isso na sala 19/05? | ✅ | — |
| Quebra se não estiver pronto para o júri? | — | ✅ |
| Exige logística externa (escola, pedagogos, hardware-alvo)? | — | ✅ |
| Modelo C / spec / documento? | ✅ | — |
| Arte/áudio/animação de polimento? | — | ✅ |
| Demonstrável em Unity Editor com laptop? | ✅ | — |
| Exige WebGL build ou produto distribuível? | — | ✅ |
| É implementação funcional do dashboard da intermediadora? | Mock visual ✅ | Funcional ✅ |

Empate → vence o card que mais conecta o jogo ao contexto educacional da Vila Jutaí.

---

## Estrutura do Board

**Nome sugerido:** `Florestia · Amazon Hacking 2026`

### Listas

1. **📋 Backlog** — sem marco
2. **🎯 Sprint MVP (11–19/05)** — pra apresentação do mentor
3. **🌳 Sprint Feira (20/05–07/06)** — pro júri
4. **🔧 Em Curso** — alguém está executando agora
5. **🔍 Em Validação** — aguardando review/teste interno
6. **✅ Concluído** — entregue na branch principal

### Labels

| Label | Uso |
|---|---|
| `mentor` | Pedido direto VH / Fabio |
| `kid-friendly` | Adaptação para 8–11 anos |
| `código` | Implementação no jogo (vanni) |
| `conteúdo` | Texto, diálogo, mensagem (não-código) |
| `educação` | Camada pedagógica / BNCC |
| `modelo-c` | Canvas v2 |
| `pesquisa` | Leitura / estudo / validação externa |
| `apresentação` | Slide / pitch / demo |
| `polimento` | Arte / áudio / build / balance |
| `feira` | Específico do estande |

### Membros

`vanni` (Giovanni · todo código) · `kadu` · `tex` · `biel` · `luiz` · `pm`

### Standup diário (começando 11/05)

- O que fechei desde ontem
- O que vou pegar hoje
- Bloqueios (especialmente: vanni esperando conteúdo de quem)

---

## 🎯 Sprint MVP (11–19/05)

### Bloco A · Refactor kid-friendly do jogo existente

> Adaptação, não rebuild. Cada card pega features que já funcionam e ajusta linguagem, layout, ritmo e conceito para a criança de 8–11.

#### A01 · Audit + reescrita de todos os textos do jogo

- **Labels:** `mentor` `kid-friendly` `conteúdo` `educação`
- **Dono:** biel + kadu (escrevem) — vanni implementa em A02
- **Due:** 14/05
- **Subtasks:**
  - [ ] Listar todos os strings da UI (HUD, botões, modais, EndScreen)
  - [ ] Reescrever conceitos adultos para faixa 8–11 ("margem", "estoque", "saldo", "custo fixo")
  - [ ] Reescrever mensagens educacionais da EndScreen
  - [ ] Reescrever feedback do mercado (acceptLine / rejectLine atuais)
  - [ ] Validar com a checklist do teste de 10 segundos da auditoria de UX por idade

#### A02 · Refactor de textos no código

- **Labels:** `mentor` `kid-friendly` `código`
- **Dono:** vanni
- **Depende:** A01
- **Due:** 15/05
- **Subtasks:**
  - [ ] Aplicar strings novas em HUDController, MarketUIController, EndScreenController
  - [x] Usar "intermediadora" em todo o código e docs
  - [ ] Remover notação `%` do HUD do mercado (manter sinal por cor)
  - [ ] Renomear `educationalLabel`, `marginLabel`, `stockLabel` para nomes mais legíveis se ajudar leitura do código

#### A03 · Tutorial inicial in-game

- **Labels:** `mentor` `kid-friendly` `código` `educação`
- **Dono:** vanni implementa, conteúdo de roteiro vem de biel + kadu
- **Depende:** A01 (texto)
- **Due:** 17/05
- **Subtasks:**
  - [ ] Roteiro do tutorial: 4–6 passos cobrindo plantar, regar, colher, ir ao mercado, vender, encerrar dia (kadu + biel)
  - [ ] Tela de boas-vindas no primeiro launch
  - [ ] Tooltips contextuais nas primeiras ações de cada tipo
  - [ ] Botão "pular" sempre disponível
  - [ ] Flag de "tutorial completo" no save

#### A04 · MarketScene kid-first (do roadmap anterior, ajustado)

- **Labels:** `mentor` `kid-friendly` `código`
- **Dono:** vanni
- **Depende:** A01 (diálogos e termos) + C02 (diálogos novos de comprador)
- **Due:** 16/05
- **Subtasks:**
  - [x] Substituir TMP_Dropdown por 3 botões de cultura
  - [ ] Substituir slider de quantidade por stepper `◀ [N] ▶`
  - [ ] Remover percentual `+426%` da margem (manter R$ + cor)
  - [ ] Botão vender com label dinâmico ("Vender 4 mandiocas")
  - [ ] Retrato do comprador dominando 1/3 esquerdo com balão de fala
  - [ ] Animação thumbs-up/down no resultado da venda
  - [ ] Pilha de moedas visual para o total (substituir texto numérico bruto)
  - [ ] Eliminar runtime fallback `EnsureQuantitySlider` que sobrepõe o builder

#### A05 · EndScreen kid-first (do roadmap anterior, ajustado)

- **Labels:** `mentor` `kid-friendly` `código`
- **Dono:** vanni
- **Depende:** A01 (mensagens) + C05 (ilustrações)
- **Due:** 17/05
- **Subtasks:**
  - [ ] Hero zone: 3 estados visuais (Falência / Sobreviveu / Lucrou) — ilustração + frase única encorajadora
  - [ ] Gráfico de barras com rótulos de dia (1–15) e linha horizontal de zero
  - [ ] Marker visual no dia da virada (quando saldo cruzou para negativo)
  - [ ] Barras coloridas por sinal (vermelho < 0, verde ≥ 0) — corrige bug atual
  - [ ] Tip contextual baseado em comportamento do aluno (ver A06)
  - [ ] Botão "JOGAR DE NOVO" grande, dominante na tela
  - [ ] Remover "Falência! O saldo chegou a zero" em vermelho-sobre-preto

#### A06 · Insights e dicas in-game

- **Labels:** `mentor` `kid-friendly` `código` `educação`
- **Dono:** vanni implementa, roteiro do conteúdo vem de biel
- **Depende:** C05 (roteiro de insights por evento)
- **Due:** 18/05
- **Subtasks:**
  - [ ] Pop-up curto após primeira venda lucrativa
  - [ ] Pop-up curto após primeira venda no prejuízo
  - [ ] Pop-up no início de cada dia com dica do dia anterior
  - [ ] Tip contextual na EndScreen baseado em padrão de comportamento (sempre vendeu para Atravessador? só plantou Mandioca? vendeu abaixo do custo?)

#### A07 · Visualização da progressão do dia

- **Labels:** `mentor` `kid-friendly` `código`
- **Dono:** vanni
- **Due:** 18/05
- **Subtasks:**
  - [ ] Indicador visual claro de fase do dia (manhã / tarde / fim de tarde) além do sky gradient já existente
  - [ ] Sino ou som quando se aproxima da noite (asset por luiz)
  - [ ] Texto curto no HUD: "Manhã" / "Tarde" / "Anoitecer"

### Bloco B · Mecânica nova: save por aluno + pergunta diária

#### B01 · Save persistente por aluno

- **Labels:** `mentor` `kid-friendly` `código` `educação`
- **Dono:** vanni
- **Due:** 14/05
- **Subtasks:**
  - [ ] Identificação simples de aluno (nome digitado no boot, sem login)
  - [ ] Estrutura de dados: por aluno × plantios, vendas, perguntas respondidas, decisões, dia atual
  - [ ] Salvar a cada ação relevante (extensão do `SaveSystem` atual)
  - [ ] Carregar ao iniciar; menu pra trocar de aluno
  - [ ] Reset functionality

#### B02 · Pergunta diária no fim do dia (MVP-do-MVP, hardcoded)

- **Labels:** `mentor` `kid-friendly` `código` `educação`
- **Dono:** vanni
- **Depende:** B01 + C08 (banco de perguntas)
- **Due:** 16/05
- **Subtasks:**
  - [ ] Hook no fim do dia (modal antes do `AdvanceDay`)
  - [ ] Banco de perguntas hardcoded por cultura (vem do card C08)
  - [ ] UI da pergunta: enunciado + 3 alternativas, ou input numérico
  - [ ] Feedback visual: certo (verde + parabéns) / errado (sem punição, mostra a resposta certa)
  - [ ] Salvar resposta no histórico do aluno
  - [ ] Escolha simples: pergunta baseada na última cultura plantada/vendida

#### B03 · Fallback de histórico

- **Labels:** `mentor` `kid-friendly` `código` `educação`
- **Dono:** vanni
- **Depende:** B01
- **Due:** 17/05
- **Subtasks:**
  - [ ] Detectar "não plantou hoje"
  - [ ] Mostrar revisão histórica em vez de pergunta nova: gráfico mini do saldo, melhor venda da semana, etc.

#### B04 · "Dia" como fase com encerramento educacional

- **Labels:** `kid-friendly` `código` `educação`
- **Dono:** vanni
- **Depende:** B02
- **Due:** 17/05
- **Subtasks:**
  - [ ] Resumo Noturno (já existe) + Pergunta (B02) + dica contextual = fluxo único de "fechar fase"
  - [ ] Tela "Dia X concluído!" antes do resumo, com mood encorajador
  - [ ] Transição visual de fim de dia → início do próximo dia

### Bloco C · Conteúdo (texto, diálogo, roteiro, ilustração)

> Todos os cards C são pré-requisitos para os cards A e B do vanni.

| # | Card | Labels | Dono | Due |
|---|---|---|---|---|
| C01 | **Roteiro do tutorial inicial** — 4–6 passos cobrindo plantar/regar/colher/mercado/vender/encerrar com linguagem 8–11 | `conteúdo` `educação` | kadu + biel | 13/05 |
| C02 | **Reescrita dos diálogos dos compradores (v1 MVP)** — remover tom pretensioso, manter três personalidades distintas (Atravessador / Feirante / Comprador Direto). Versão profunda com representatividade plena vem na Feira. | `mentor` `conteúdo` `educação` | kadu | 14/05 |
| C03 | **Mensagens educacionais da EndScreen reescritas** — três variantes (Falência / Sobreviveu / Lucrou) em linguagem encorajadora | `mentor` `conteúdo` `educação` `kid-friendly` | biel | 14/05 |
| C04 | **Audit + glossário de termos do HUD** — lista de cada string atual com substituto kid-friendly | `mentor` `conteúdo` `kid-friendly` | biel | 13/05 |
| C05 | **Roteiro de insights pop-up in-game** — quando dispara, o que diz, mood do tom | `mentor` `conteúdo` `educação` | biel | 15/05 |
| C06 | **Spec + wireframe do dashboard da intermediadora (para mock)** — métricas a exibir, layout em papel | `mentor` `conteúdo` `educação` | pm | 14/05 |
| C07 | **Spec do mecanismo de pergunta diária** — formato (múltipla escolha vs aberta), distribuição por cultura, política de feedback | `mentor` `conteúdo` `educação` | biel + vanni | 13/05 |
| C08 | **Banco inicial de perguntas hardcoded** — 5 perguntas por cultura para o MVP, sem ainda lógica adaptativa | `mentor` `conteúdo` `educação` | biel + luiz | 15/05 |
| C09 | **Ilustrações da Hero zone da EndScreen** — 3 ilustrações (Falência / Sobreviveu / Lucrou) em estilo Stardew | `kid-friendly` `polimento` | kadu (curadoria) | 16/05 |

### Bloco D · Modelo C v2 (vários cards, conforme você pediu)

| # | Card | Labels | Dono | Due |
|---|---|---|---|---|
| D01 | **Modelo C v2 · Cliente** — professora intermediadora + dashboard da intermediadora como cliente técnico paralelo | `modelo-c` | pm | 14/05 |
| D02 | **Modelo C v2 · Mecanismos de Receita** — Steam vs Governo vs empresas de jogos, com recomendação argumentada | `modelo-c` | pm | 15/05 |
| D03 | **Modelo C v2 · Acompanhamento do Impacto** — indicadores automáticos do jogo + pedagógicos | `modelo-c` `educação` | pm | 16/05 |
| D04 | **Modelo C v2 · Foco do Impacto / Ações** — incluir representatividade brasileira/jutaí explícita | `modelo-c` `educação` | pm | 17/05 |

### Bloco E · Implementação mock do dashboard da intermediadora

#### E01 · Mock do dashboard da intermediadora para MVP

- **Labels:** `mentor` `kid-friendly` `código` `educação`
- **Dono:** vanni
- **Depende:** C06 (spec) + D01 (Modelo C cliente)
- **Due:** 18/05
- **Subtasks:**
  - [ ] Tela acessível por flag ou senha simples (não é jogo, é vista para professora)
  - [ ] Layout estático com dados dummy: nome do aluno, dia atual, saldo, melhor cultura, % de acerto nas perguntas
  - [ ] Visualização de gráfico de barras de saldo (reaproveitar o componente da EndScreen)
  - [ ] Header indicando que é a "visão da intermediadora"
  - [ ] Sem persistência real — só prova de UI. Funcional vem na Feira.

### Bloco F · Pesquisa (preliminar, output em 1–2 páginas)

| # | Card | Labels | Dono | Due |
|---|---|---|---|---|
| F01 | **Estudo histórico/cultural Vila Jutaí · preliminar** — perfil quilombola/ribeirinho, cultivos locais, fatos utilizáveis em diálogos. Versão profunda na Feira. | `pesquisa` | tex + kadu | 17/05 |
| F02 | **Estudo do formato do resumo do dia** — referências de jogos educativos com feedback diário; valida B02 e B04 | `pesquisa` `educação` | vanni + biel | 14/05 |
| F03 | **Pesquisa em educação financeira infantil · preliminar** — artigos e materiais 8–11 anos; output: lista de conceitos válidos para a faixa etária | `pesquisa` `educação` | luiz + pm | 17/05 |

### Bloco G · Sprint 1 self-feedback (easy wins do quadro, "II")

#### G01 · Barra "life" de tempo de plantação

- **Labels:** `kid-friendly` `código`
- **Dono:** vanni
- **Due:** 13/05
- **Subtasks:**
  - [ ] Barra fina sobre cada `CropSlot` ocupado
  - [ ] Preenchimento conforme `daysPlanted / growthDays`
  - [ ] Esconde quando crop está ready (já tem o sprite final)

#### G02 · Painel de vendas com vendedor (refinamento)

- **Labels:** `kid-friendly` `código`
- **Dono:** vanni
- **Due:** 14/05
- **Notas:** parte do trabalho já foi feito em commits anteriores — refinar baseado nos screenshots que mostraram o painel travado.
- **Subtasks:**
  - [ ] Garantir que após `SelectCropByIndex` o painel atualiza stock + preço + quantidade corretamente
  - [ ] Sell button label dinâmico (relacionado com A04)
  - [ ] Garantir que vender 0 unidades é bloqueado

### Bloco H · Apresentação MVP

#### H01 · Slide para apresentação do MVP (mentor-facing)

- **Labels:** `mentor` `apresentação`
- **Dono:** kadu + tex (conteúdo + assets) — vanni grava gifs do jogo
- **Due:** 18/05
- **Subtasks:**
  - [ ] Gifs curtos das mecânicas novas (plantio, mercado kid-first, pergunta diária, EndScreen)
  - [ ] Sprites e ilustrações da Hero zone na apresentação
  - [ ] Slide de fundamentação BNCC (referenciar EDD compliance PDF já existente)
  - [ ] Slide do Modelo C v2 (resumo dos 4 campos atualizados)
  - [ ] Slide do que vem na Feira (preview do escopo)

---

## 🌳 Sprint Feira (20/05 – 07/06)

### Bloco A-Feira · Versões avançadas dos cards MVP

| # | Card | Labels | Dono | Notas |
|---|---|---|---|---|
| FA01 | **Diálogos dos compradores v2 — representatividade plena** | `mentor` `conteúdo` `educação` `feira` | kadu (escreve) + vanni (integra) | Depende de F-F01 (estudo histórico profundo) |
| FA02 | **Pergunta diária v2 — adaptativa** | `código` `educação` `feira` | vanni | Banco expandido, lógica baseada em desempenho passado; depende de C08-v2 (biel expande banco) |
| FA03 | **Dashboard da intermediadora funcional** | `mentor` `código` `educação` `feira` | vanni | Implementa a spec C06, com dados reais do save B01 |
| FA04 | **Histórico longitudinal por aluno** | `código` `educação` `feira` | vanni | Extensão do save B01 para múltiplas sessões |
| FA05 | **Hero zone EndScreen v2 — ilustrações finais** | `polimento` `feira` | kadu (curadoria/produção) + vanni (integra) | C09 entrega versão MVP, FA05 a final |

### Bloco F-B · Polimento técnico

| # | Card | Labels | Dono | Notas |
|---|---|---|---|---|
| FB01 | **Build WebGL para Chromebook** | `código` `feira` | vanni | Testar em hardware real (Chromebook) |
| FB02 | **Integração de audio** | `código` `polimento` `feira` | vanni (integra) — luiz cura assets | Subtasks: ambient da roça (loop curto), SFX plant/water/harvest/sell/reject, música mercado |
| FB03 | **Frames adicionais de walkUp e walkSide** | `polimento` `feira` | kadu (sprites) + vanni (integra) | Atualmente 1 frame, animação estática |
| FB04 | **Decisão e implementação de balanceamento de stamina** | `código` `educação` `feira` | pm + vanni (decisão) → vanni (asset edit) | 3/2/3 vs 1/1/1; atualizar GDD se mudar |
| FB05 | **Smoke test de 1 partida completa** | `código` `feira` | vanni | Jogar 15 dias seguidos, anotar bugs |
| FB06 | **Suporte completo a mouse para interação** — jogo atualmente é WASD + E; criança 8–11 em Chromebook se dá melhor com mouse. Mantém WASD para movimento, mouse passa a controlar plantio/rega/colheita | `kid-friendly` `código` `feira` | vanni | Subtasks: hover destaca tile, click interage, cursor muda por ferramenta, testar touchpad Chromebook |

### Bloco F-C · Pesquisa avançada e validação externa

| # | Card | Labels | Dono | Notas |
|---|---|---|---|---|
| FC01 | **Estudo histórico/cultural v2 profundo** | `pesquisa` `feira` | tex + kadu | Fontes primárias, parcerias com lideranças se viável |
| FC02 | **Pesquisa educação financeira infantil v2 profundo** | `pesquisa` `educação` `feira` | luiz + pm | Review sistemático, output em artigo curto |
| FC03 | **Reunião com profissional pedagogo** — validação do modelo de ensino | `pesquisa` `mentor` `feira` | grupo todo (definir dono no standup) | Marcar reunião + iterar com feedback |
| FC04 | **Aplicação piloto curta na escola Vila Jutaí** | `pesquisa` `educação` `feira` | grupo todo | Depende de logística com a escola |

### Bloco F-D · Modelo C / negócio continuação

| # | Card | Labels | Dono | Notas |
|---|---|---|---|---|
| FD01 | **Abordagem a empresas de jogos** | `modelo-c` `mentor` `feira` | pm | Lista de potenciais + draft de proposta |
| FD02 | **Plano de continuidade pós-Amazon Hacking** | `modelo-c` `feira` | pm | Sec. Municipal Educação Moju, Prefeitura, próximos passos |

### Bloco F-E · Estande, materiais e demo da Feira

| # | Card | Labels | Dono | Due |
|---|---|---|---|---|
| FE01 | **Plano de demonstração presencial** — fluxo de 2-5 min para visitante + demo aprofundada para júri | `apresentação` `feira` | tex + vanni | 30/05 |
| FE02 | **Materiais visuais do estande** — banner, folder, identidade gráfica | `apresentação` `feira` | kadu + biel | 02/06 |
| FE03 | **Setup técnico do estande** — laptop(s) com build, energia, conectividade redundante | `feira` | tex (logística) + vanni (build) | 31/05 |
| FE04 | **Q&A prep para o júri** — antecipar perguntas e treinar respostas | `apresentação` `feira` | grupo todo | 04/06 |
| FE05 | **Iteração com feedback do mentor pós-19/05** — priorizar 3 ajustes de maior retorno antes da Feira | `feira` `mentor` | grupo todo | 21/05 |
| FE06 | **Slide para o júri Amazon Hacking** — narrativa diferente do mentor: contexto territorial, demonstração de impacto, ODS 1 e 4, próximos passos | `mentor` `apresentação` `feira` | kadu + tex + vanni | 03/06 |

---

## 📋 Backlog (sem marco · pós-Feira)

| # | Card | Labels | Notas |
|---|---|---|---|
| BL01 | Sistema de NPCs com diálogo (GDD §10) | `código` `polimento` | Fora de escopo |
| BL02 | Expansão do grid além de 6×6 | `código` `polimento` | Pós-Feira |
| BL03 | Múltiplas save slots por dispositivo (além do save-por-aluno do MVP) | `código` | Pós-Feira |
| BL04 | Deterioração de estoque | `código` | Pós-Feira |
| BL05 | Sistema de clima (chuva como bônus de crescimento) | `código` | Pós-Feira |
| BL06 | Implementação em escolas além da Vila Jutaí | `educação` | Depende do resultado da Feira |

---

## ✅ Concluído (Sprint MVP)

Já feito ou parcial — confirmar com a equipe antes de mover:

- TMP_Dropdown → 3 botões de cultura (código + builder)
- Resumo Noturno modal versão básica (perguntas vêm em B02)
- Multi-quantity sell (slider funciona; vira stepper em A04)
- HUD math coaching labels (preview + proporcionalidade — vai ser revisto em A02 para remover %)
- EDD compliance PDF
- "Jogo Fácil" ✓ e "Jogo Intuitivo" ✓ no quadro do mentor — validar com equipe se está realmente OK

---

## Snapshot de atribuições

### Sprint MVP

| Pessoa | Cards | Foco |
|---|---|---|
| **vanni** | A02, A03 (impl), A04, A05, A06 (impl), A07, B01, B02, B03, B04, E01, G01, G02, e gifs para H01 | TODO O CÓDIGO. Refactor kid-friendly + mecânicas novas + mock dashboard. |
| **kadu** | A01 (texto), C01, C02, C09, F01, H01 | Diálogos, roteiro tutorial, estudo cultural, slide |
| **biel** | A01 (texto), C01, C03, C04, C05, C07, C08, F02 | Mensagens educacionais, audit de termos, roteiros, banco de perguntas |
| **tex** | F01, H01 | Estudo cultural com kadu, slide com kadu (gifs/sprites) |
| **luiz** | C08, F03 | Banco de perguntas + pesquisa educação infantil |
| **pm** | C06, D01, D02, D03, D04, F03 | Modelo C v2 inteiro + spec dashboard + pesquisa infantil |

### Sprint Feira (preview)

| Pessoa | Foco principal |
|---|---|
| **vanni** | TODO O CÓDIGO da Feira: WebGL, audio integration, dashboard funcional, pergunta v2 adaptativa, walk frames integration |
| **kadu** | Diálogos v2, walk sprites, materiais estande, slide júri |
| **biel** | Banco de perguntas expandido, materiais estande, audio asset curation |
| **tex** | Plano demo, setup técnico estande, estudo cultural v2 |
| **luiz** | Audio asset curation, pesquisa v2, banco de perguntas v2 |
| **pm** | Dashboard spec final, abordagem empresas de jogos, plano continuidade, pesquisa v2 |

---

## Notas de processo

- **Diferencial é focar no contexto educacional do jogo** (caixa central do quadro) — critério de desempate quando dois cards competem.
- **Vanni é gargalo de código.** Os outros entregam conteúdo cedo (Bloco C com due dates 13–15/05) para vanni implementar 14–18/05. Se algum card C atrasar, o card A correspondente do vanni atrasa também — sinalizar no standup.
- **Pergunta adaptativa é a mecânica nova de maior risco.** MVP é hardcoded (B02). Feira é adaptativa de verdade (FA02). Não tentar fazer a versão adaptativa direto no MVP.
- **Dashboard da intermediadora:** mock no MVP (E01, visual estático com dados dummy). Funcional na Feira (FA03, integrando com B01).
- **Feedback do mentor 19/05** entra na Sprint Feira via FE05 — reservar 21/05 só para reagendar prioridades pós-apresentação.
- **Tickets "II"** = easy wins, não prioridade alta no sentido tradicional. São fáceis, não necessariamente mais importantes.
- **Tutorial (A03)** é card novo a partir do feedback do usuário — não estava no quadro original. Sinalizar no standup que é entregável do MVP.
