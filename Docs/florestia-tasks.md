# Florestia · Live Task Tracker

Fonte única de tudo que está pendente, em curso ou concluído no projeto.

**Como usar:**
- `- [ ]` = pendente · `- [x]` = concluído
- Marcar uma task como feita: editar `[ ]` para `[x]` neste arquivo
- Adicionar nova task: criar nova linha no cluster certo, ou novo cluster se for tema novo
- Esse arquivo é a fonte; o Trello é o espelho operacional

**Última geração:** este markdown foi gerado a partir do script `scripts/florestia_trello.py`. Edições manuais NÃO são reaplicadas pelo script — depois desta geração inicial, edite o markdown direto.

---

## 📅 Próximas entregas oficiais (datas adiadas)

| Data | Artefato | Cluster Trello |
|---|---|---|
| **18/05/26** | Protótipo · fotos e/ou vídeo dos resultados | `H01.1` + `H01.2` (em Em Curso) |
| **22/05/26** | Relato de Experiência (versão final) | `RE01` (em Em Curso) |
| **25/05/26** | Validação do MVP com mentor | `MV01` (Sprint MVP) — apresentação + smoke test + slide H01 (`H01.3`–`H01.5`) |

> Sprint MVP originalmente datado 19/05; validação real adiada para 25/05. O intervalo efetivo agora é 11–25/05.

---

## 🎯 Sprint MVP (11-25/05)

### A01 · Audit + reescrita de todos os textos do jogo
> **Labels:** mentor · kid-friendly · conteúdo · educação | **Status:** ✅ Concluído · ver `Docs/florestia-glossario-kid-friendly.md`

- [x] **A01.1** Listar todos os strings da UI (HUD, botões, modais, EndScreen)
- [x] **A01.2** Reescrever conceitos adultos: 'margem', 'estoque', 'saldo', 'custo fixo'
- [x] **A01.3** Reescrever mensagens educacionais da EndScreen
- [x] **A01.4** Reescrever feedback do mercado (acceptLine / rejectLine atuais)
- [x] **A01.5** Validar com o teste de 10 segundos da auditoria de UX por idade

### A02 · Refactor de textos no código
> **Labels:** mentor · kid-friendly · código | **Dono:** vanni | **Due:** 2026-05-15 | **Depende:** A01

- [x] **A02.1** Aplicar strings novas em HUDController, MarketUIController, EndScreenController
- [x] **A02.2** Usar 'intermediadora' em todo o código e docs
- [x] **A02.3** Remover notação % do HUD do mercado (manter sinal por cor)
- [ ] **A02.4** Renomear SerializedFields para nomes mais legíveis se ajudar leitura do código

### A03 · Tutorial inicial in-game
> **Labels:** mentor · kid-friendly · código · educação | **Dono:** vanni | **Due:** 2026-05-17 | **Depende:** A01, C01

- [x] **A03.1** Tela de boas-vindas no primeiro launch
- [x] **A03.2** Tooltip contextual primeira ação de plantar
- [x] **A03.3** Tooltip contextual primeira ação de regar
- [x] **A03.4** Tooltip contextual primeira ação de colher
- [x] **A03.5** Tooltip contextual primeira ida ao mercado
- [x] **A03.6** Botão 'pular tutorial' sempre disponível
- [x] **A03.7** Flag de 'tutorial completo' no save

### A04 · MarketScene kid-first
> **Labels:** mentor · kid-friendly · código | **Dono:** vanni | **Due:** 2026-05-16 | **Depende:** A01, C02

- [x] **A04.1** Substituir slider de quantidade por stepper ◀ [N] ▶
- [x] **A04.2** Remover percentual da margem (manter R$ + cor)
- [x] **A04.3** Botão vender com label dinâmico ('Vender 4 mandiocas')
- [x] **A04.4** Retrato do comprador dominando 1/3 esquerdo com balão de fala
- [x] **A04.5** Animação thumbs-up/down no resultado da venda
- [x] **A04.6** Pilha de moedas visual para o total
- [x] **A04.7** Eliminar runtime fallback EnsureQuantitySlider

### A05 · EndScreen kid-first
> **Labels:** mentor · kid-friendly · código | **Dono:** vanni | **Due:** 2026-05-17 | **Depende:** A01, C03, C09

- [x] **A05.1** Hero zone: 3 estados visuais (Falência / Sobreviveu / Lucrou) com ilustração
- [x] **A05.2** Frase única encorajadora por desfecho (de C03)
- [x] **A05.3** Gráfico de barras com rótulos de dia (1-15)
- [x] **A05.4** Linha horizontal de zero no gráfico
- [x] **A05.5** Marker visual no dia da virada (saldo cruzou para negativo)
- [x] **A05.6** Barras coloridas por sinal (vermelho < 0, verde ≥ 0)
- [x] **A05.7** Tip contextual baseado em comportamento do aluno
- [x] **A05.8** Botão 'JOGAR DE NOVO' grande e dominante
- [x] **A05.9** Remover 'Falência! O saldo chegou a zero' em vermelho-sobre-preto

### A06 · Insights e dicas in-game
> **Labels:** mentor · kid-friendly · código · educação | **Dono:** vanni | **Due:** 2026-05-18 | **Depende:** C05

- [x] **A06.1** Pop-up curto após primeira venda lucrativa
- [x] **A06.2** Pop-up curto após primeira venda no prejuízo
- [x] **A06.3** Pop-up no início de cada dia com dica do dia anterior
- [x] **A06.4** Tip contextual na EndScreen baseado em padrão de comportamento

### A07 · Visualização da progressão do dia
> **Labels:** mentor · kid-friendly · código | **Dono:** vanni | **Due:** 2026-05-18

- [x] **A07.1** Indicador visual de fase do dia além do sky gradient
- [x] **A07.2** Texto curto no HUD: 'Manhã' / 'Tarde' / 'Anoitecer'

### A08 · Mercado, feedback visual e polimento pós-review
> **Labels:** mentor · kid-friendly · código · polimento | **Dono:** vanni | **Fonte:** anotações de review em 2026-05-18

- [x] **A08.1** Revamp inicial da MarketScene com stepper, retratos novos e chance de venda
- [x] **A08.2** RNG noturno configurável para aumentar/diminuir chance dos compradores aceitarem
- [x] **A08.3** Substituir sprites dos compradores por retratos neutros e respeitosos
- [x] **A08.4** Desclutter da coluna direita do mercado: preço, quantidade, cards e ações em blocos separados
- [ ] **A08.5** Criar arte de fundo do mercado com céu noturno escuro e original
- [ ] **A08.6** Reavaliar termo/representação do comprador "Atravessador" com foco pedagógico e sem imagem negativa

### A09 · Feedback visual de ações na fazenda
> **Labels:** kid-friendly · código · polimento | **Dono:** vanni | **Fonte:** anotações de review em 2026-05-18

- [x] **A09.1** Adicionar aviso visual quando a planta ficar pronta para colher, possivelmente exclamação
- [x] **A09.2** Adicionar reação visual quando a planta for plantada
- [x] **A09.3** Adicionar reação visual quando a planta for regada
- [x] **A09.4** Adicionar reação visual quando a planta for colhida
- [x] **A09.5** Colocar highlight no tile sob o mouse, não no tile para onde o player olha

### A10 · Assets, fontes, save e controles
> **Labels:** código · polimento · UX | **Dono:** vanni | **Fonte:** anotações de review em 2026-05-18

- [x] **A10.1** Fazer sprite de regador legível no toolbar
- [ ] **A10.2** Ajustar savefiles: reset, troca de perfil, arquivos antigos e fluxo de continuação
- [x] **A10.3** Ajustar tecla ESC: menu de pausa, sair, salvar e voltar
- [x] **A10.4** Explicitar energia e custo de energia das ações antes/depois de agir
- [ ] **A10.5** Integrar fonte da pasta `SDV Fonts/` sem quebrar acentos e fallback TMP

### B01 · Save persistente por aluno
> **Labels:** mentor · kid-friendly · código · educação | **Dono:** vanni | **Status:** ✅ Concluído · commits `5c83754` (fundação) + `b235a49` (picker)

- [x] **B01.1** Identificação simples de aluno (nome digitado no boot, sem login)
- [x] **B01.2** Estrutura de dados por aluno: plantios, vendas, perguntas, decisões, dia atual
- [x] **B01.3** Salvar a cada ação relevante (extensão do SaveSystem)
- [x] **B01.4** Carregar ao iniciar; menu pra trocar de aluno
- [x] **B01.5** Reset functionality

### B02 · Pergunta diária no fim do dia (versão hardcoded)
> **Labels:** mentor · kid-friendly · código · educação | **Dono:** vanni | **Due:** 2026-05-16 | **Depende:** B01, C07, C08

- [x] **B02.1** Hook no fim do dia (modal antes do AdvanceDay)
- [x] **B02.2** Carregar banco de perguntas hardcoded por cultura (de C08)
- [x] **B02.3** UI da pergunta: enunciado + 3 alternativas, ou input numérico
- [x] **B02.4** Feedback visual: certo (verde + parabéns) / errado (mostra a certa, sem punição)
- [x] **B02.5** Salvar resposta no histórico do aluno
- [x] **B02.6** Escolha simples: pergunta baseada na última cultura plantada/vendida

### B03 · Fallback de histórico se não plantou
> **Labels:** mentor · kid-friendly · código · educação | **Dono:** vanni | **Due:** 2026-05-17 | **Depende:** B01

- [x] **B03.1** Detectar 'não plantou hoje'
- [x] **B03.2** Mostrar revisão histórica: mini-gráfico, melhor venda da semana

### B04 · Dia como fase com encerramento educacional
> **Labels:** kid-friendly · código · educação | **Dono:** vanni | **Due:** 2026-05-17 | **Depende:** B02

- [x] **B04.1** Encadeamento: Resumo Noturno → Pergunta → dica → fechar fase
- [x] **B04.2** Tela 'Dia X concluído!' antes do resumo, com mood encorajador
- [x] **B04.3** Transição visual de fim de dia → início do próximo dia

### C01 · Roteiro do tutorial inicial
> **Labels:** conteúdo · educação | **Dono:** kadu,biel | **Due:** 2026-05-13

- [x] **C01.1** Roteiro passo 1: chegar à roça e plantar
- [x] **C01.2** Roteiro passo 2: regar a plantação
- [x] **C01.3** Roteiro passo 3: colher
- [x] **C01.4** Roteiro passo 4: ir ao mercado pela ponte
- [x] **C01.5** Roteiro passo 5: vender com slider de preço
- [x] **C01.6** Roteiro passo 6: encerrar o dia + pergunta

### C02 · Diálogos dos compradores (v1 MVP)
> **Labels:** mentor · conteúdo · educação | **Dono:** kadu | **Due:** 2026-05-14

- [x] **C02.1** Escrever Atravessador: tom rápido, ofertas baixas mas certas
- [x] **C02.2** Escrever Feirante Local: tom amigável, preços medianos
- [x] **C02.3** Escrever Comprador Direto: tom respeitoso, paga bem mas raro
- [x] **C02.4** Validar vocabulário 8-11, sem jargão financeiro

### C03 · Mensagens educacionais da EndScreen
> **Labels:** mentor · conteúdo · educação · kid-friendly | **Dono:** biel | **Due:** 2026-05-14

- [x] **C03.1** Escrever variante Falência: encorajadora, sugere estratégia
- [x] **C03.2** Escrever variante Sobreviveu: validador, sugere upgrade
- [x] **C03.3** Escrever variante Lucrou: parabéns + próximo desafio implícito

### C04 · Audit + glossário de termos do HUD
> **Labels:** mentor · conteúdo · kid-friendly | **Dono:** biel | **Due:** 2026-05-13

- [x] **C04.1** Listar todos os strings do HUDController
- [x] **C04.2** Listar strings do MarketUIController
- [x] **C04.3** Listar strings da EndScreenController
- [x] **C04.4** Para cada string, propor substituto kid-friendly 8-11

### C05 · Roteiro de insights pop-up in-game
> **Labels:** mentor · conteúdo · educação | **Dono:** biel | **Due:** 2026-05-15

- [x] **C05.1** Pop-up primeira venda com sobra: parabéns + nome do conceito (sobra)
- [x] **C05.2** Pop-up primeira venda com falta: sem culpa + explicação curta
- [x] **C05.3** Pop-up início do dia 2+: dica do dia anterior baseada em comportamento

### C06 · Spec + wireframe do dashboard da intermediadora
> **Labels:** mentor · conteúdo · educação | **Dono:** pm | **Due:** 2026-05-14

- [ ] **C06.1** Listar métricas relevantes para a professora
- [ ] **C06.2** Wireframe em papel ou Figma simples
- [ ] **C06.3** Definir hierarquia de informação
- [ ] **C06.4** Entregar doc + imagem do wireframe

### C07 · Spec do mecanismo de pergunta diária
> **Labels:** mentor · conteúdo · educação | **Dono:** biel,vanni | **Due:** 2026-05-13

- [x] **C07.1** Decidir formato: múltipla escolha (3 alternativas) ou input numérico
- [x] **C07.2** Definir política de escolha da pergunta (random vs adaptativo)
- [x] **C07.3** Definir política de feedback: certo / errado
- [x] **C07.4** Definir estrutura do banco de perguntas (campos)

### C08 · Banco inicial de perguntas hardcoded
> **Labels:** mentor · conteúdo · educação | **Dono:** biel,luiz | **Due:** 2026-05-15

- [x] **C08.1** 5 perguntas Mandioca (custo, margem, multiplicação)
- [x] **C08.2** 5 perguntas Cacau
- [x] **C08.3** 5 perguntas Açaí
- [x] **C08.4** Estruturar cada pergunta: enunciado + 3 alternativas + correta + explicação

### C09 · Ilustrações da Hero zone da EndScreen
> **Labels:** kid-friendly · polimento | **Dono:** kadu | **Due:** 2026-05-16

- [ ] **C09.1** Ilustração Falência: agricultor pensativo, sem desespero
- [ ] **C09.2** Ilustração Sobreviveu: agricultor neutro com colheita modesta
- [ ] **C09.3** Ilustração Lucrou: agricultor com pilha de moedas e colheita farta

### D01 · Modelo C v2 · Cliente
> **Labels:** modelo-c | **Dono:** pm | **Due:** 2026-05-14

- [ ] **D01.1** Revisar texto atual do campo Cliente
- [ ] **D01.2** Articular dois clientes técnicos (criança jogador + professora dashboard)
- [ ] **D01.3** Atualizar Modelo C v2 no formato visual do canvas

### D02 · Modelo C v2 · Mecanismos de Receita
> **Labels:** modelo-c | **Dono:** pm | **Due:** 2026-05-15

- [ ] **D02.1** Analisar viabilidade Steam (monetização direta)
- [ ] **D02.2** Analisar viabilidade Governo (Sec. Municipal, Prefeitura, MEC)
- [ ] **D02.3** Analisar viabilidade empresas de jogos (parceria)
- [ ] **D02.4** Escrever recomendação argumentada

### D03 · Modelo C v2 · Acompanhamento do Impacto
> **Labels:** modelo-c · educação | **Dono:** pm | **Due:** 2026-05-16

- [ ] **D03.1** Listar indicadores automáticos do jogo (tempo, margem, padrão de comprador, % acerto)
- [ ] **D03.2** Listar indicadores pedagógicos (provas antes/depois, participação, feedback)
- [ ] **D03.3** Definir critérios de sucesso
- [ ] **D03.4** Atualizar canvas Modelo C

### D04 · Modelo C v2 · Foco do Impacto / Ações
> **Labels:** modelo-c · educação | **Dono:** pm | **Due:** 2026-05-17

- [ ] **D04.1** Revisar texto atual de Foco do Impacto
- [ ] **D04.2** Adicionar representatividade brasileira/jutaí
- [ ] **D04.3** Revisar Ações de Impacto Positivo correspondentes
- [ ] **D04.4** Atualizar canvas Modelo C

### E01 · Mock do dashboard da intermediadora
> **Labels:** mentor · kid-friendly · código · educação | **Dono:** vanni | **Due:** 2026-05-18 | **Depende:** C06, D01

- [x] **E01.1** Tela acessível por flag ou senha simples
- [x] **E01.2** Layout estático com dados dummy: aluno, dia, saldo, melhor cultura, % acerto
- [x] **E01.3** Reaproveitar componente de gráfico de barras da EndScreen
- [x] **E01.4** Header 'visão da intermediadora'
- [x] **E01.5** Sem persistência real (funcional vem na Feira)

### F01 · Estudo histórico/cultural Vila Jutaí · preliminar
> **Labels:** pesquisa | **Dono:** tex,kadu | **Due:** 2026-05-17

- [ ] **F01.1** Perfil quilombola/ribeirinho da Vila Jutaí
- [ ] **F01.2** Cultivos locais e práticas agrícolas
- [ ] **F01.3** Dinâmicas econômicas (atravessadores, feiras)
- [ ] **F01.4** Entregar doc curto (1-2 páginas)

### F02 · Estudo do formato do resumo do dia
> **Labels:** pesquisa · educação | **Dono:** vanni,biel | **Due:** 2026-05-14

- [ ] **F02.1** Pesquisar pelo menos 3 jogos com feedback diário
- [ ] **F02.2** Anotar o que funciona e o que não funciona
- [ ] **F02.3** Entregar doc curto com recomendações

### F03 · Pesquisa em educação financeira infantil · preliminar
> **Labels:** pesquisa · educação | **Dono:** luiz,pm | **Due:** 2026-05-17

- [ ] **F03.1** Ler pelo menos 3 artigos sobre educação financeira infantil
- [ ] **F03.2** Listar conceitos adequados 8-11
- [ ] **F03.3** Listar conceitos a evitar
- [ ] **F03.4** Entregar doc curto

### G01 · Barra 'life' de tempo de plantação
> **Labels:** kid-friendly · código | **Dono:** vanni | **Status:** ✅ Concluído · commit `178b384`

- [x] **G01.1** Barra fina sobre cada CropSlot ocupado
- [x] **G01.2** Preenchimento conforme daysPlanted / growthDays
- [x] **G01.3** Esconde quando crop está ready

### G02 · Painel de vendas com vendedor (refinamento)
> **Labels:** kid-friendly · código | **Dono:** vanni | **Status:** ✅ Concluído · commit `9fb758a`

- [x] **G02.1** Garantir update do painel após SelectCropByIndex
- [x] **G02.2** Sell button label dinâmico (relacionado com A04)
- [x] **G02.3** Bloquear vender 0 unidades

### H01 · Slide para apresentação do MVP (mentor-facing)
> **Labels:** mentor · apresentação | **Dono:** kadu,tex,vanni | **Due:** ver datas adiadas no topo

- [ ] **H01.1** Gravar gifs/vídeo das mecânicas (plantio, mercado, pergunta, EndScreen) · **due 18/05** · artefato Protótipo
- [ ] **H01.2** Coletar sprites e ilustrações da Hero zone · **due 18/05** · artefato Protótipo
- [ ] **H01.3** Slide de fundamentação BNCC (referenciar EDD compliance PDF) · **due 25/05** · validação MVP
- [ ] **H01.4** Slide do Modelo C v2 (resumo dos 4 campos) · **due 25/05** · validação MVP
- [ ] **H01.5** Slide do preview da Feira · **due 25/05** · validação MVP

### MV01 · Validação do MVP com mentor
> **Labels:** mentor · apresentação | **Dono:** grupo todo | **Due:** 2026-05-25 | **Status:** novo cluster pós-adiamento

- [ ] **MV01.1** Smoke test end-to-end no jogo: tutorial → plantio → mercado → resumo + perguntas → EndScreen sem erros vermelhos
- [ ] **MV01.2** Anexar versão final do Modelo C v2 (entregue pelo pm) ao deck
- [ ] **MV01.3** Anexar gifs/vídeo (H01.1) e sprites (H01.2) ao deck
- [ ] **MV01.4** Ensaiar a apresentação ao menos 1x antes do dia 25/05
- [ ] **MV01.5** Validação propriamente dita com mentor no 25/05

### RE01 · Relato de Experiência (versão final)
> **Labels:** pesquisa · apresentação · conteúdo · feira | **Dono:** grupo todo | **Due:** 2026-05-22 | **Status:** em Em Curso (originalmente 21/05, adiada 1 dia)

Texto vivo em [Google Doc](https://docs.google.com/document/d/1zKTieMFTJTrKzVOSGtel0M7Qik7llygS/edit). Conteúdo do relato já está em
`references/relato_florestia.pdf` (rascunho); versão final entra no doc compartilhado pelo grupo.

---

## 🌳 Sprint Feira (20/05-07/06)

### FA01 · Diálogos dos compradores v2 — representatividade plena
> **Labels:** mentor · conteúdo · educação · feira | **Dono:** kadu,vanni | **Due:** 2026-06-01 | **Depende:** FC01

- [ ] **FA01** Diálogos dos compradores v2 — representatividade plena
  - Versão profunda com representatividade brasileira/jutaí.

### FA02 · Pergunta diária v2 — adaptativa
> **Labels:** código · educação · feira | **Dono:** vanni | **Due:** 2026-06-03

- [ ] **FA02.1** Banco expandido (10+ perguntas por cultura)
- [ ] **FA02.2** Lógica adaptativa: dificuldade sobe com acertos
- [ ] **FA02.3** Insights culturais alternando com matemáticos
- [ ] **FA02.4** Persistir desempenho por aluno

### FA03 · Dashboard da intermediadora funcional
> **Labels:** mentor · código · educação · feira | **Dono:** vanni | **Due:** 2026-06-05

- [ ] **FA03.1** Conectar com save persistente B01
- [ ] **FA03.2** Métricas reais por aluno
- [ ] **FA03.3** Visualizações: gráfico saldo, % acerto, padrão de compradores
- [ ] **FA03.4** Exportação simples (CSV ou imprimir)

### FA04 · Histórico longitudinal por aluno
> **Labels:** código · educação · feira | **Dono:** vanni | **Due:** 2026-06-04

- [ ] **FA04.1** Estrutura de dados para múltiplas runs por aluno
- [ ] **FA04.2** Agregação de métricas ao longo do tempo
- [ ] **FA04.3** Visualização no dashboard da intermediadora

### FA05 · Hero zone EndScreen v2 — ilustrações finais
> **Labels:** polimento · feira | **Dono:** kadu,vanni | **Due:** 2026-06-02

- [ ] **FA05** Hero zone EndScreen v2 — ilustrações finais
  - Substituir ilustrações MVP (C09) por versão final polida.

### FB01 · Build WebGL para Chromebook
> **Labels:** código · feira | **Dono:** vanni | **Due:** 2026-05-30

- [ ] **FB01.1** Configurar Build Profile WebGL
- [ ] **FB01.2** Resolver issues de compatibility
- [ ] **FB01.3** Testar em Chromebook real
- [ ] **FB01.4** Upload em URL acessível

### FB02 · Integração de audio
> **Labels:** código · polimento · feira | **Dono:** vanni,luiz | **Due:** 2026-06-01

- [ ] **FB02.1** Ambient da roça (loop curto)
- [ ] **FB02.2** SFX plant
- [ ] **FB02.3** SFX water
- [ ] **FB02.4** SFX harvest
- [ ] **FB02.5** SFX sell + reject
- [ ] **FB02.6** Música mercado noturno
- [ ] **FB02.7** Volume controls básicos

### FB03 · Frames adicionais walkUp e walkSide
> **Labels:** polimento · feira | **Dono:** kadu,vanni | **Due:** 2026-05-28

- [ ] **FB03.1** Sprites walkUp 2-frame (kadu)
- [ ] **FB03.2** Sprites walkSide 2-frame (kadu)
- [ ] **FB03.3** Integrar no PlayerController (vanni)

### FB04 · Decisão e implementação de balanceamento de stamina
> **Labels:** código · educação · feira | **Dono:** vanni,pm | **Due:** 2026-05-25

- [ ] **FB04.1** Decisão pm + vanni sobre balanço final (3/2/3 vs 1/1/1)
- [ ] **FB04.2** Atualizar CropData assets se mudar
- [ ] **FB04.3** Atualizar GDD para refletir

### FB05 · Smoke test de 1 partida completa
> **Labels:** código · feira | **Dono:** vanni | **Due:** 2026-06-05

- [ ] **FB05.1** Iniciar run nova
- [ ] **FB05.2** Jogar até o fim (15 dias)
- [ ] **FB05.3** Anotar todos os bugs e issues
- [ ] **FB05.4** Priorizar fixes pré-Feira

### FB06 · Suporte completo a mouse para interação
> **Labels:** kid-friendly · código · feira | **Dono:** vanni | **Due:** 2026-06-02

- [ ] **FB06.1** Mouse hover destaca CropSlot apontado (substitui tile highlight do facing)
- [ ] **FB06.2** Click do mouse executa interação no tile (substitui tecla E)
- [ ] **FB06.3** Cursor muda de ícone conforme ferramenta selecionada no toolbar
- [ ] **FB06.4** Teste de touchpad em Chromebook real (público-alvo)

### FC01 · Estudo histórico/cultural v2 profundo
> **Labels:** pesquisa · feira | **Dono:** tex,kadu | **Due:** 2026-05-28

- [ ] **FC01.1** Contatar lideranças da Vila Jutaí
- [ ] **FC01.2** Pesquisa em fontes primárias se viável
- [ ] **FC01.3** Entregar doc estendido

### FC02 · Pesquisa educação financeira infantil v2 profundo
> **Labels:** pesquisa · educação · feira | **Dono:** luiz,pm | **Due:** 2026-05-30

- [ ] **FC02.1** Ler pelo menos 8 artigos
- [ ] **FC02.2** Análise crítica dos achados
- [ ] **FC02.3** Entregar artigo curto (~2-3 páginas)

### FC03 · Reunião com profissional pedagogo
> **Labels:** pesquisa · mentor · feira | **Dono:** grupo todo | **Due:** 2026-05-25

- [ ] **FC03.1** Identificar pedagogo (CESUPA?)
- [ ] **FC03.2** Marcar reunião
- [ ] **FC03.3** Apresentar o jogo e Modelo C
- [ ] **FC03.4** Capturar feedback
- [ ] **FC03.5** Iterar com base no feedback

### FC04 · Aplicação piloto curta na escola Vila Jutaí
> **Labels:** pesquisa · educação · feira | **Dono:** grupo todo | **Due:** 2026-06-04

- [ ] **FC04.1** Contatar a escola
- [ ] **FC04.2** Marcar visita
- [ ] **FC04.3** Aplicar com pelo menos 1 turma
- [ ] **FC04.4** Capturar dados

### FD01 · Abordagem a empresas de jogos
> **Labels:** modelo-c · mentor · feira | **Dono:** pm | **Due:** 2026-06-02

- [ ] **FD01.1** Listar pelo menos 5 empresas potencialmente alinhadas
- [ ] **FD01.2** Draft de proposta (1 página)
- [ ] **FD01.3** Pesquisar canais de contato

### FD02 · Plano de continuidade pós-Amazon Hacking
> **Labels:** modelo-c · feira | **Dono:** pm | **Due:** 2026-06-04

- [ ] **FD02.1** Definir 3 cenários de continuidade
- [ ] **FD02.2** Para cada cenário, listar próximos passos
- [ ] **FD02.3** Entregar doc para júri da Feira

### FE01 · Plano de demonstração presencial
> **Labels:** apresentação · feira | **Dono:** tex,vanni | **Due:** 2026-05-30

- [ ] **FE01.1** Roteiro demo curta (2-5 min) para visitante casual
- [ ] **FE01.2** Roteiro demo júri (10-15 min)
- [ ] **FE01.3** Ensaio interno

### FE02 · Materiais visuais do estande
> **Labels:** apresentação · feira | **Dono:** kadu,biel | **Due:** 2026-06-02

- [ ] **FE02.1** Banner principal
- [ ] **FE02.2** Folder explicativo
- [ ] **FE02.3** Identidade visual coesa (usar paleta Florestia)

### FE03 · Setup técnico do estande
> **Labels:** feira | **Dono:** tex,vanni | **Due:** 2026-05-31

- [ ] **FE03.1** Laptops com build WebGL/standalone
- [ ] **FE03.2** Energia e cabeamento
- [ ] **FE03.3** Conectividade redundante
- [ ] **FE03.4** Save anônimo por visitante (resetável)

### FE04 · Q&A prep para o júri
> **Labels:** apresentação · feira | **Dono:** grupo todo | **Due:** 2026-06-04

- [ ] **FE04.1** Listar 15 perguntas prováveis
- [ ] **FE04.2** Respostas escritas
- [ ] **FE04.3** Ensaio interno tipo arguição

### FE05 · Iteração com feedback do mentor pós-19/05
> **Labels:** feira · mentor | **Dono:** grupo todo | **Due:** 2026-05-21

- [ ] **FE05.1** Capturar feedback completo na sala 19/05
- [ ] **FE05.2** Priorizar 3 ajustes de maior retorno
- [ ] **FE05.3** Atribuir donos e due dates

### FE06 · Slide para o júri Amazon Hacking
> **Labels:** mentor · apresentação · feira | **Dono:** kadu,tex,vanni | **Due:** 2026-06-03

- [ ] **FE06.1** Slide de abertura: tensão observada na Vila Jutaí
- [ ] **FE06.2** Slide do produto: demo de mecânicas
- [ ] **FE06.3** Slide de fundamentação: BNCC + EDD + Modelo C
- [ ] **FE06.4** Slide de impacto: indicadores + ODS 1 e 4
- [ ] **FE06.5** Slide de continuidade: plano pós-Amazon

---

## 📋 Backlog

### BL01 · Sistema de NPCs com diálogo (GDD §10)
> **Labels:** código · polimento

- [ ] **BL01** Sistema de NPCs com diálogo (GDD §10)
  - Fora de escopo MVP e Feira. Pós-Feira.

### BL02 · Expansão do grid além de 6x6
> **Labels:** código · polimento

- [ ] **BL02** Expansão do grid além de 6x6
  - Pós-Feira.

### BL03 · Múltiplas save slots por dispositivo
> **Labels:** código

- [ ] **BL03** Múltiplas save slots por dispositivo
  - Pós-Feira. O save-por-aluno do MVP cobre o caso principal.

### BL04 · Deterioração de estoque
> **Labels:** código

- [ ] **BL04** Deterioração de estoque
  - Pós-Feira.

### BL05 · Sistema de clima
> **Labels:** código

- [ ] **BL05** Sistema de clima
  - Pós-Feira.

### BL06 · Implementação em escolas além da Vila Jutaí
> **Labels:** educação

- [ ] **BL06** Implementação em escolas além da Vila Jutaí
  - Depende do resultado da Feira.

---

## ✅ Concluído

### DONE01 · TMP_Dropdown → 3 botões de cultura
> **Labels:** código · kid-friendly · mentor | **Dono:** vanni

- [x] **DONE01** TMP_Dropdown → 3 botões de cultura
  - Código + builder atualizados. Eliminou o erro de SetupTemplate.

### DONE02 · Resumo Noturno modal versão básica
> **Labels:** código · educação | **Dono:** vanni

- [x] **DONE02** Resumo Noturno modal versão básica
  - Modal entre mercado e AdvanceDay. Perguntas serão adicionadas em B02.

### DONE03 · Multi-quantity sell (slider)
> **Labels:** código · educação | **Dono:** vanni

- [x] **DONE03** Multi-quantity sell (slider)
  - Slider de quantidade funciona. Vira stepper em A04.

### DONE04 · HUD math coaching labels
> **Labels:** código · educação | **Dono:** vanni

- [x] **DONE04** HUD math coaching labels
  - Preview + proporcionalidade. Vai ser revisto em A02 para remover %.

### DONE05 · EDD compliance PDF
> **Labels:** modelo-c · educação | **Dono:** vanni

- [x] **DONE05** EDD compliance PDF
  - 12 páginas via Kami. Auditoria das 9 habilidades BNCC.

---
