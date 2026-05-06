# Florestia — Game Design Document (MVP)

**Versão:** 0.1  
**Data:** 2026-05-06  
**Equipe:** Carlos Eduardo, Gabriel Mattos, Giovanni Braga, Gustavo Teixeira, Luiz Eduardo, Pedro Miguel  
**Engine:** Unity + C#  
**Plataforma:** PC (Windows/Mac), Android como meta futura

---

## 1. Visão Geral

Florestia é um jogo educativo no estilo Stardew Valley, ambientado na agricultura familiar amazônica de Vila Jutaí (Moju-PA). O jogador assume o papel de um jovem agricultor que precisa administrar sua roça ao longo de 15 dias, tomando decisões reais de plantio, colheita e precificação para terminar o mês "no verde".

**Público-alvo:** 8–11 anos  
**Tema educacional:** Matemática aplicada a decisões econômicas da agricultura familiar (BNCC)  
**Abordagem pedagógica:** Game-based learning + Educação Popular (Freire, 1996) — a matemática emerge da realidade concreta do educando, não de quizzes isolados  
**ODS alinhados:** ODS 1 (Erradicação da Pobreza), ODS 4 (Educação de Qualidade)

---

## 2. Loop Central

```
Manhã → [FARM SCENE: plantar / regar / colher com stamina]
       → Céu escurece + notificação "Vá ao mercado pela ponte"
       → Jogador caminha até a ponte → [MARKET SCENE: definir preço / vender]
       → Resumo noturno + salvamento automático
       → Próximo dia
```

Após 15 dias → **[END SCREEN]**: resumo financeiro completo, resultado (ganhou/perdeu), lição aprendida.

---

## 3. Mundo e Estrutura de Cenas

### Cenas Unity

| Cena          | Descrição                                        |
| ------------- | ------------------------------------------------ |
| `FarmScene`   | Roça diurna: grid 6×6, casinha, ponte ao sul     |
| `MarketScene` | Mercado noturno: feirantes e atravessadores      |
| `EndScreen`   | Resumo dos 15 dias: receita, custos, saldo final |

### Layout da Roça (FarmScene)

- **Grid:** 6×6 tiles plantáveis
- **Casa:** tile fixo no canto superior esquerdo (não plantável)
- **Ponte:** tile de saída ao sul, ativa a transição para MarketScene ao interagir à noite
- **Progressão futura (pós-MVP):** jogador compra tiles adicionais para expandir até 10×10

### Ciclo do Dia

- **Duração:** 5 minutos por dia in-game
- **Início:** manhã — céu claro, timer começa
- **Minuto 4 (80%):** céu começa a escurecer gradualmente
- **Minuto 4:30:** notificação aparece na tela — _"A noite chegou! Vá ao mercado pela ponte."_
- **Ao chegar na ponte:** transição de cena para MarketScene (independentemente do timer)
- **Timer zerado sem ir à ponte:** jogador dorme sem vender — dia avança, estoque retido

---

## 4. Mecânicas da Roça (FarmScene)

### Stamina

- O jogador tem **20 pontos de stamina** por dia
- Cada ação custa stamina:

| Ação            | Custo |
| --------------- | ----- |
| Arar tile       | 1     |
| Plantar semente | 1     |
| Regar tile      | 1     |
| Colher          | 1     |

- Stamina zerada: jogador não consegue mais executar ações — deve ir ao mercado ou aguardar o fim do dia
- **Sem barra de stamina secundária para comer** no MVP (simplificação)

### Culturas (MVP)

| Cultura  | Crescimento | Custo da semente | Preço base de venda | Estratégia                      |
| -------- | ----------- | ---------------- | ------------------- | ------------------------------- |
| Mandioca | 2 dias      | R$3              | R$7                 | Giro rápido, baixa margem       |
| Cacau    | 4 dias      | R$6              | R$16                | Equilíbrio risco/retorno        |
| Açaí     | 6 dias      | R$10             | R$28                | Alto valor, capital imobilizado |

- Crescimento em dias in-game (não em tempo real)
- Cada tile comporta 1 planta
- Planta não colhida no prazo: não estraga no MVP (simplificação)
- **Regra de ouro educacional:** margem bruta = preço de venda − custo da semente. O jogador aprende isso experimentando, não lendo.

---

## 5. Mecânicas do Mercado (MarketScene)

### Fluxo

1. Jogador chega ao mercado com o estoque colhido do dia
2. Para cada cultura no estoque, jogador define **seu preço de venda** (slider ou input numérico)
3. Compradores aparecem em sequência e reagem ao preço definido:
   - **Preço ≤ max do comprador:** compra realizada → dinheiro entra no saldo
   - **Preço > max do comprador:** comprador rejeita com feedback visual + fala curta (ex.: _"Tá caro demais!"_) e vai embora
4. Mercado fecha após todos os compradores passarem
5. Estoque não vendido é retido para o próximo dia (sem deterioração no MVP)

### Tipos de Compradores

| Tipo             | Max price (Mandioca/Cacau/Açaí) | Volume               | Frequência |
| ---------------- | ------------------------------- | -------------------- | ---------- |
| Atravessador     | R$5 / R$12 / R$20               | Alto (compra tudo)   | Alta       |
| Feirante local   | R$7 / R$15 / R$26               | Médio                | Média      |
| Comprador direto | R$9 / R$18 / R$30               | Baixo (1–2 unidades) | Baixa      |

- Jogador não vê o max price — aprende por tentativa e erro ao longo dos 15 dias
- **Tensão educacional:** atravessador aceita quase sempre (preço baixo), comprador direto paga mais mas aparece pouco. Decidir para quem vender é a mecânica da negociação.

---

## 6. Economia do Jogo

| Parâmetro                                    | Valor                         |
| -------------------------------------------- | ----------------------------- |
| Capital inicial                              | R$50                          |
| Custo fixo diário                            | R$2 (alimentação, manutenção) |
| Custo fixo total (15 dias)                   | R$30                          |
| Saldo mínimo para sobreviver sem vender nada | R$20 no dia 15                |
| Condição de vitória                          | Saldo > R$0 ao fim do dia 15  |
| Condição de derrota                          | Saldo < R$0 em qualquer dia   |

**Por que esses números funcionam:** com R$50 e R$2/dia, o jogador tem margem para errar nos primeiros dias mas precisa rentabilizar a roça a partir do dia 7–8. Cria progressão natural de urgência.

---

## 7. Camada Matemática (Hybrid)

O jogador nunca é parado para resolver um exercício. A matemática é **sempre visível e sempre consequente**:

### HUD durante o mercado

```
Custo: R$6    |    Seu preço: R$16    |    Margem: R$10 (+166%)
```

- Custo = custo da semente gasta para produzir aquela unidade
- Margem atualiza em tempo real conforme o slider de preço move
- Cor: verde se margem positiva, vermelho se abaixo do custo

### Resumo noturno (após cada dia de mercado)

```
Dia 3 — Resumo
Vendido:    Mandioca ×4 @ R$7 = R$28
            Cacau ×1 @ R$15 = R$15
Receita:    R$43
Custo fixo: -R$2
Saldo:      R$91
```

### End Screen (dia 15)

- Gráfico de saldo ao longo dos 15 dias (linha)
- Total investido em sementes vs total arrecadado
- Cultura mais rentável
- Mensagem educacional contextualizada (ex.: _"Você aprendeu a calcular margem! Na roça de verdade, isso chama precificação."_)

---

## 8. Arquitetura Unity (Sistemas)

### Track A — Core Systems

- `GameManager` (singleton): estado global, dia atual, saldo, transições de cena
- `DayNightCycle`: timer de 5 min, controla iluminação, dispara notificação e bridge trigger
- `CropSystem`: estado por tile (`Empty → Tilled → Planted → Growing → Ready`), countdown por dia
- `StaminaSystem`: int simples, decrementado por ação, bloqueio quando = 0

### Track B — Economy / UI

- `InventorySystem`: dicionário `{CropType → quantidade}`
- `PricingSystem`: preços base, cálculo de margem em tempo real
- `BuyerSystem`: lista de compradores por noite, lógica de aceitação, feedback
- `HUDController`: saldo, stamina bar, dia atual, notificação noturna
- `MarketUIController`: slider de preço, painel de margem, fila de compradores

### Track C — Art / World

- Tilemap: farm grid (6×6 plantável + casa + ponte), arte pixel com referência Stardew
- Sprites: 3 culturas × 4 estágios de crescimento = 12 sprites
- Compradores: 3 tipos × 1 sprite cada
- Sky gradient: animação de cor (azul claro → laranja → roxo escuro) durante o dia

### Save System

- Salvar após cada noite de mercado
- Formato: JSON local (`/saves/save.json`)
- Dados salvos: dia atual, saldo, inventário, estado de cada tile (cultura + dias restantes)
- Offline-first: sem nenhuma chamada de rede

---

## 9. Direção de Arte

- **Estilo:** pixel art 2D top-down, referência Stardew Valley
- **Paleta:** tons amazônicos — verdes profundos, ocre, terroso, com detalhes em verde-água e amarelo
- **MVP:** sprites gerados por IA (DALL-E / Midjourney) com base no estilo Stardew, substituíveis por arte original em v2
- **UI:** simples, legível para 8–11 anos, ícones grandes, texto curto

---

## 10. Fora do Escopo (MVP)

| Feature                                          | Versão |
| ------------------------------------------------ | ------ |
| Atravessador como NPC separado (loop negociação) | v2     |
| Expansão do grid além de 6×6                     | v2     |
| Múltiplas save slots                             | v2     |
| Android build                                    | v2     |
| Sistema de NPCs com diálogo                      | v2     |
| Deterioração de estoque                          | v2     |
| Clima (chuva bônus de crescimento)               | v2     |

---

## 11. Cronograma de Desenvolvimento (12 dias)

| Dias | Track A                                    | Track B                             | Track C                              |
| ---- | ------------------------------------------ | ----------------------------------- | ------------------------------------ |
| 1–2  | GameManager, DayNightCycle                 | InventorySystem, estrutura de dados | Tilemap base, casa, ponte            |
| 3–4  | CropSystem (estados + timer)               | PricingSystem + HUD saldo/stamina   | Sprites das 3 culturas (4 estágios)  |
| 5–6  | StaminaSystem + interação de tile          | BuyerSystem + MarketUIController    | Sprites compradores + UI mercado     |
| 7–8  | Integração Farm↔Market (transição de cena) | Resumo noturno + save JSON          | Sky gradient + notificação noturna   |
| 9–10 | End Screen + condição vitória/derrota      | Balanceamento economia (playtests)  | Polish: sons, partículas colheita    |
| 11   | Bug fixing integração                      | Ajuste de números (preços, stamina) | Arte final substituindo placeholders |
| 12   | Build final + testes completos             | —                                   | —                                    |
