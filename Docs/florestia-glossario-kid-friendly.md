# Florestia · Glossário Kid-Friendly (8–11 anos)

Auditoria do vocabulário da UI do Florestia consolidando o que foi implementado
nos commits do MVP 19/05. Materializa o cluster **A01** do Sprint MVP e fixa o
glossário que o time de conteúdo (kadu, biel) usa pra expandir falas e perguntas
futuras.

**Princípio reitor (Modelo C §3.4):** para a faixa 8–11 anos, evitar termos
adultos do mundo financeiro. Usar "sobra" no lugar de "lucro", "dinheiro" no
lugar de "saldo", "sacola" no lugar de "estoque". A palavra técnica entra apenas
quando a criança já operou o conceito.

---

## A01.1 · Inventário de strings da UI

Strings da interface organizadas por tela. Todos extraídos do código do dia
2026-05-18.

### HUD (FarmScene)

| Slot | String atual |
|---|---|
| Dinheiro | `Dinheiro: R$50,00` |
| Dia | `Dia 1 de 15` |
| Energia | `Energia: 20/20` |
| Timer | `04:32` (cor vermelha quando perto da noite) |
| Fase do dia | `Manhã` · `Tarde` · `Anoitecer` (HUDController) |
| Coaching plantio | `Vou plantar Cacau. Custa R$6. Demora 4 dias pra colher.` |
| Coaching regar | `Vou regar a plantação` |
| Coaching colher | `Vou colher o que tá pronto` |
| Proporcionalidade | `Você tem 4 pés de cacau · gastou R$24 · pode render até R$64` |
| Dica do dia | mostrada no início do dia 2+ via A06.3 |

### Mercado Noturno (MarketScene)

| Slot | String atual |
|---|---|
| Título da banca | `Na sacola: 4` (1 → `Na sacola: 1`) |
| Bloco quantidade | `Quantos vender:` · stepper `◀ N ▶` · `Total: R$28,00` |
| Custo unitário | `Custou: R$6,00` |
| Preço pedido | `Seu preço: R$15,00` |
| Margem positiva | `Sobra de R$9,00` (verde) |
| Margem negativa | `Faltam R$3,00` (vermelho) |
| Margem zerada | `Empata: nem sobra nem falta` |
| Botão vender | `Vender 4 cacaus` · dinâmico |
| Botão sem comprador | `Escolha um comprador` |
| Botão sem estoque | `Você não colheu nada` |

### Resumo Noturno (modal antes do AdvanceDay)

| Slot | String atual |
|---|---|
| Título | `Fim do dia 3` |
| Vazio | `Você não vendeu nada hoje.` |
| Linha de venda | `4 × Cacau a R$15,00 = R$60,00` |
| Total ganho | `Você ganhou: R$60,00` |
| Custo do dia | `Custo do sustento: −R$2,00` |
| Saldo do dia | `Dinheiro no fim do dia: R$108,00` |

### Dia Concluído (B04.2, exibido antes do resumo)

| Slot | String atual |
|---|---|
| Título | `Dia 3 concluído!` |
| Corpo | `Você cuidou da roça e chegou ao fim do dia. Agora vamos ver o que aconteceu.` |

### Fluxo Educacional (DailyEducationFlow)

| Slot | String atual |
|---|---|
| Cabeçalho | `Pergunta 1 de 4 · Custo` |
| Pergunta Custo | `Você plantou 4 pés de cacau. Cada um custou R$6,00. Quanto você gastou no total?` |
| Pergunta Receita | `Você vendeu 4 pés de cacau por R$15,00 cada. Quanto recebeu no total?` |
| Pergunta Sobra | `Você gastou R$24,00 e recebeu R$60,00. Quanto sobrou?` |
| Explicação | `4 × R$6,00 = R$24,00.` (gerada a partir dos valores) |
| Curiosidade · Cacau | `O cacau é uma fruta brasileira. Sua semente vira chocolate depois de seca e torrada.` |
| Botão certo | `Isso aí! {explanação}` |
| Botão errado | `A resposta era: {certa}. {explanação}` |
| Avançar | `Próxima pergunta` → `Curiosidade` → `Próximo dia` |

### EndScreen

| Slot | String atual |
|---|---|
| Dinheiro final | `Dinheiro no fim: R$87,00` |
| Falência | `Acabou o dinheiro. Tenta de novo!` |
| Sobreviveu | `Você terminou os 15 dias!` |
| Lucrou | `Você plantou, vendeu e sobrou dinheiro!` |
| Melhor cultura | `O que mais te rendeu: Açaí` |
| Marker no gráfico | `↓ aqui acabou o dinheiro` (se houve virada) |
| Tip contextual | gerada por `GetContextualTip()` — varia por padrão de comportamento |
| Botão restart | `JOGAR DE NOVO` (78pt, dominante) |
| Eixo do gráfico | dias 1–15 + linha de zero |

### Tutorial Inicial (TutorialController)

| Slot | String atual |
|---|---|
| Boas-vindas | `Você vai plantar, regar, colher e vender para cuidar do dinheiro da família. Eu vou te guiar nos primeiros passos.` |
| Plantio | `Escolha uma semente nos botões de baixo e clique em um canteiro vazio para plantar.` |
| Após plantar | `Agora escolha o regador e clique na plantação. Planta regada cresce no fim do dia.` |
| Após regar | `Quando a barra da planta encher, escolha a ferramenta de colher e pegue sua produção.` |
| Após colher | `Agora atravesse a ponte ou espere o dia acabar para ir ao mercado vender.` |
| No mercado | `Escolha uma cultura, converse com um comprador e teste o preço. Se vender por mais do que gastou, sobra dinheiro.` |
| Botão skip | `Pular tutorial` |
| Botões avançar | `Entendi` · `Vou regar` · `Certo` · `Ir vender` · `Terminar tutorial` |

### Picker de Aluno

| Slot | String atual |
|---|---|
| Título | `Quem está jogando?` |
| Subtítulo | `Escolha seu nome ou crie um novo aluno` |
| Input placeholder | `Seu nome` |
| Criar | `Começar` |
| Reset | `Reiniciar progresso` |
| Apagar | `Apagar este aluno` |

---

## A01.2 · Glossário antes / depois

Tabela canônica. Quando criar texto novo, usar a coluna direita.

| Termo adulto (evitar) | Substituição kid-friendly | Onde aparece |
|---|---|---|
| Saldo | Dinheiro · Dinheiro no fim · Dinheiro no fim do dia | HUD, EndScreen, Resumo |
| Saldo final | Dinheiro no fim | EndScreen |
| Lucro | Sobra | Mercado · Curiosidades · Tips |
| Prejuízo | Falta · Faltam R$X | Mercado · EndScreen |
| Margem | Sobra · Faltam · Empata | Mercado |
| Margem positiva | Sobra de R$X | Mercado |
| Margem negativa | Faltam R$X | Mercado |
| Margem zerada | Empata: nem sobra nem falta | Mercado |
| Estoque | Na sacola | Mercado |
| Custo (substantivo) | Custou | Mercado |
| Custo fixo | Custo do sustento | Resumo Noturno |
| Receita | Você ganhou | Resumo Noturno |
| Falência | Acabou o dinheiro | EndScreen |
| Investimento | Gastou | HUD coaching |
| Cultura mais rentável | O que mais te rendeu | EndScreen |
| Quantidade | Quantos · Qtd · stepper visual | Mercado |
| Vender N unidades | Vender N {cultura no plural correto} | Mercado |
| Painel de vendas | Mercado · banca | Mercado |
| ADMIN do jogo | Intermediadora · professora | Modelo C, docs |
| Notação % | (removida do HUD; só por cor) | Mercado |
| `−R$Y (-20%)` | `Faltam R$Y` | Mercado |
| `+R$X (+150%)` | `Sobra de R$X` | Mercado |

### Plurais corretos por cultura

A função `PluralCropName()` no HUDController e `PluralizeCrop()` no
MarketUIController padroniza:

| Singular | Plural |
|---|---|
| pé de mandioca | pés de mandioca |
| pé de cacau | pés de cacau |
| açaizeiro | açaizeiros |

E no botão de venda (mais coloquial):

| Singular | Plural |
|---|---|
| mandioca | mandiocas |
| cacau | cacaus |
| açaí | açaís |

---

## A01.3 · Mensagens da EndScreen (antes/depois)

| Desfecho | Antes (clinical) | Depois (encorajador) |
|---|---|---|
| Falência | `Falência! O saldo chegou a zero.` (vermelho-sobre-preto) | `Acabou o dinheiro. Tenta de novo!` (laranja-rosa) |
| Sobreviveu | `Você sobreviveu! Dá pra melhorar.` | `Você terminou os 15 dias!` (dourado) |
| Lucrou | `Você lucrou! Ótima gestão.` | `Você plantou, vendeu e sobrou dinheiro!` (verde) |

### Tips educacionais (antes/depois)

| Caso | Antes (genérico) | Depois (contextual em `GetContextualTip()`) |
|---|---|---|
| Falência | `Dica: venda sempre acima do custo da semente para não ter prejuízo.` | `Não desanima! Plantar variedade e cobrar mais alto ajuda o dinheiro durar.` ou contextual baseada em padrão real |
| Lucro alto | `Você dominou a precificação! Margem = Preço − Custo. É assim na roça de verdade.` | `Você terminou com R${gm.Balance:F0}! Bom plano de plantio e venda.` |
| Sobreviveu | `Dica: Açaí tem margem de R$18 por unidade. Vale imobilizar capital por 6 dias?` | `O Açaí demora 6 dias mas paga bem. Tente plantar mais açaí na próxima vez!` |
| Vendeu abaixo do custo | (sem tip) | `Você vendeu Cacau por R$5,00, mas pagou R$6,00 na semente. Tente cobrar mais alto na próxima!` |
| Só plantou uma cultura | (sem tip) | `Você só plantou Mandioca. Da próxima, tenta misturar as três culturas pra ver qual rende mais!` |
| Vendeu muito ao Atravessador | (sem tip) | `O Atravessador paga menos. Tente o Feirante ou o Comprador Direto na próxima vez!` |

---

## A01.4 · Diálogos dos compradores (antes/depois)

Os três compradores do Modelo C com falas reescritas em commit `96bf961`:

| Comprador | Aceita | Recusa |
|---|---|---|
| **Atravessador** | "Levo agora, mas pago pouco." | "Por esse preço eu não levo." |
| **Feirante Local** | "Gostei! Dá pra vender na feira." | "Hoje não consigo pagar isso." |
| **Comprador Direto** | "Combinado! Pago melhor pela sua colheita." | "Esse preço ficou alto pra mim hoje." |

**Caracterização preservada** (Modelo C §5):
- Atravessador: rápido, paga pouco (max R$5/R$12/R$20)
- Feirante: amigável, preços medianos (max R$7/R$15/R$26)
- Comprador Direto: respeitoso, paga bem mas raro (max R$9/R$18/R$30)

---

## A01.5 · Validação · teste de 10 segundos

Critério: **uma criança de 8 anos, sozinha, em sala de aula, entende o que a
tela está dizendo em 10 segundos sem um adulto explicando?**

| Tela | Veredito | Justificativa |
|---|---|---|
| HUD do FarmScene | ✅ | "Dinheiro R$50", "Energia 20/20", "Dia 1 de 15" — sem jargão |
| Coaching de plantio | ✅ | Frase única em primeira pessoa: "Vou plantar Cacau. Custa R$6." |
| Tutorial | ✅ | Linguagem direta: "Escolha uma semente nos botões de baixo e clique em um canteiro vazio" |
| Mercado · stepper | ✅ | `Quantos vender: ◀ 4 ▶` é gestural, dispensa leitura |
| Mercado · margem | ✅ | "Sobra de R$9" + verde, "Faltam R$3" + vermelho |
| Mercado · botão vender | ✅ | "Vender 4 cacaus" — narra a ação que vai acontecer |
| Resumo Noturno | ✅ | "Você ganhou", "Custo do sustento", "Dinheiro no fim do dia" |
| Pergunta de Custo | ✅ | Enunciado narra a situação real do dia que a criança acabou de viver |
| Pergunta de Sobra | ✅ | "Você gastou R$24 e recebeu R$60. Quanto sobrou?" — operação visível |
| Curiosidade cultural | ✅ | Uma frase, primeiro fato concreto sobre a cultura |
| EndScreen Falência | ✅ | "Acabou o dinheiro. Tenta de novo!" — encorajador, sem julgamento |
| EndScreen Lucrou | ✅ | "Você plantou, vendeu e sobrou dinheiro!" — três verbos da ação que ela fez |
| EndScreen tip | ✅ | Cita o que o aluno fez ("Vendeu Cacau por R$5", "Só plantou Mandioca") |
| Picker de aluno | ✅ | "Quem está jogando?" + input "Seu nome" — gestual |
| Botão "JOGAR DE NOVO" | ✅ | All caps, 26pt bold, ação inequívoca |

### Riscos remanescentes para polimento

- **"Custo do sustento"** — palavra `sustento` pode não fazer parte do
  vocabulário cotidiano de toda criança de 8. Validação real em sala de aula
  pode sugerir trocar por `Custo do dia`. Decisão fica com biel + pedagogo no
  card FC03 (Sprint Feira).
- **"Açaizeiro"** — termo correto, mas pouco conhecido fora da Amazônia.
  Considerar fallback "pés de açaí" se as crianças não reconhecerem. Validar
  em F01 (estudo cultural).
- **Operações matemáticas no DailyEducationFlow** sempre mostram duas casas
  decimais (`R$15,00`). Para a faixa 8–9, alternativas inteiras (`R$15`) podem
  ser mais legíveis. Decisão futura.

---

## Histórico de commits que materializaram este glossário

| Cluster | Commit | O que mudou |
|---|---|---|
| A02 | `0c7e831` | Refactor de strings nos controllers principais |
| A04 | `b39d69d` · `cf20154` | Mercado kid-first, "Sobra/Faltam" em vez de "Lucro/Prejuízo" |
| A05 | `750be76` | EndScreen hero zone + frases encorajadoras |
| A06 | `f8c1fe1` | Pop-ups de primeira venda kid-friendly |
| A07 | `143f43b` | "Manhã/Tarde/Anoitecer" no HUD |
| B02/B04/B05 | `b7b2a1d` · `4a0f254` · `309d1d9` | Pergunta diária + curiosidade cultural |
| C02 | `96bf961` | Diálogos dos compradores reescritos |
| A03 | `7e57779` | Tutorial inicial com vocabulário 8-11 |
