# Florestia — Educational Design Document (EDD)

**Versão:** 0.1  
**Data:** 2026-05-06  
**Referência curricular:** Base Nacional Comum Curricular (BNCC, MEC 2018)  
**Documento relacionado:** GDD.md (v0.1)

---

## 1. Fundamento Pedagógico

Florestia parte de um princípio extraído de Freire (1996): o conteúdo escolar ganha legitimidade cognitiva quando operado sobre a realidade concreta do educando. O jogo não ensina matemática sobre problemas abstratos — ensina matemática *dentro* das decisões econômicas reais da agricultura familiar amazônica.

A abordagem de game-based learning (GEE, 2007; PRENSKY, 2001) sustenta que o jogo oferece um ambiente de aprendizagem legítimo por três razões: feedback imediato (o saldo cai se o cálculo falhar), situação de escolha real (qual cultura plantar? a qual preço vender?) e risco controlado (perder no jogo não tem consequência, repetir o ciclo é seguro).

---

## 2. Público-Alvo e Faixa Curricular

| Atributo | Valor |
|---|---|
| Faixa etária | 8–11 anos |
| Segmento BNCC | Ensino Fundamental — Anos Iniciais |
| Anos de referência | 4º ano (EF04) e 5º ano (EF05) |
| Disciplina principal | Matemática |
| Tema Transversal | Educação Financeira e para o Consumo (BNCC, p. 266) |

O 4º e 5º anos são o recorte correto porque é nessa faixa que a BNCC introduz explicitamente situações de compra e venda (EF04MA25) e porcentagens aplicadas (EF05MA06) — os dois pilares matemáticos do jogo.

---

## 3. Competências Gerais da BNCC Desenvolvidas

| Código | Competência Geral | Como o jogo a desenvolve |
|---|---|---|
| CG2 | Pensamento científico, crítico e criativo | O jogador formula hipóteses de precificação ("se cobrar R$8, o feirante aceita?"), testa e refina com base no resultado — ciclo completo de pensamento investigativo |
| CG7 | Argumentação | Ao fim de cada dia, o resumo noturno exige que o jogador interprete dados (receita, custo, margem) e justifique suas escolhas de plantio e preço para si mesmo |
| CG10 | Responsabilidade e cidadania | A mecânica do atravessador ensina que aceitar qualquer preço perpetua dependência econômica — a vitória exige autonomia de precificação |

---

## 4. Habilidades Específicas da BNCC — Mapeamento por Mecânica

### 4.1 Gestão do capital diário (saldo, custo fixo, compra de sementes)

**Mecânica:** O jogador começa com R$50, paga R$2/dia de custo fixo e gasta capital comprando sementes. O saldo é exibido em tempo real no HUD.

| Habilidade | Descrição (BNCC) | Como é desenvolvida |
|---|---|---|
| **EF04MA03** | Resolver e elaborar problemas com números naturais envolvendo adição e subtração | Cada compra de semente e cada desconto do custo fixo é uma subtração real com consequência — o saldo cai e o jogador vê imediatamente |
| **EF04MA05** | Utilizar as propriedades das operações para desenvolver estratégias de cálculo | Para planejar o plantio, o jogador precisa calcular mentalmente: "se comprar 3 sementes de cacau (R$6 cada = R$18), quanto me sobra?" |
| **EF05MA07** | Resolver e elaborar problemas de adição e subtração com números naturais e racionais | O resumo noturno apresenta receita − custos = lucro/prejuízo; o jogador lê e interpreta a operação completa |

---

### 4.2 Precificação no mercado (slider de preço de venda)

**Mecânica:** Para cada cultura no estoque, o jogador define seu preço de venda. O HUD exibe em tempo real: `Custo: R$X | Seu preço: R$Y | Margem: R$Z (+W%)`.

| Habilidade | Descrição (BNCC) | Como é desenvolvida |
|---|---|---|
| **EF04MA25** | Resolver e elaborar problemas que envolvam situações de compra e venda e formas de pagamento, utilizando termos como troco e desconto, enfatizando consumo ético e responsável | A mecânica inteira do mercado é uma situação de venda: o jogador opera os conceitos de custo, preço e troco de forma ativa, não passiva |
| **EF05MA06** | Associar as representações 10%, 25%, 50%, 75% e 100% à fração correspondente, para calcular porcentagens | O HUD exibe a margem como percentual (+166%, −20%, etc.). O jogador aprende intuitivamente o que significa uma margem de 100%, 50% ou negativa antes de a escola formalizar o conceito |
| **EF05MA12** | Resolver problemas que envolvam variação de proporcionalidade direta entre duas quantidades | Dobrar a quantidade de açaí plantada dobra o custo e o potencial de receita — relação de proporcionalidade direta vivenciada sem enunciado |

---

### 4.3 Decisão de plantio (qual cultura, quanto e quando)

**Mecânica:** O jogador escolhe entre Mandioca (2d/R$3→R$7), Cacau (4d/R$6→R$16) e Açaí (6d/R$10→R$28) com stamina limitada (20 pts/dia) e 15 dias de ciclo.

| Habilidade | Descrição (BNCC) | Como é desenvolvida |
|---|---|---|
| **EF04MA05** | Utilizar as propriedades das operações para desenvolver estratégias de cálculo mental | O jogador precisa comparar retornos: "Mandioca rende R$4 em 2 dias (R$2/dia); Cacau rende R$10 em 4 dias (R$2,50/dia); Açaí rende R$18 em 6 dias (R$3/dia)" — divisão aplicada |
| **EF05MA08** | Resolver problemas de multiplicação e divisão com números naturais e decimais | Calcular a receita esperada de um tile (ex.: 4 tiles de mandioca × R$7 = R$28) é multiplicação direta com contexto real |
| **EF05MA12** | Proporcionalidade direta | Mais tiles plantados = mais sementes compradas = mais receita esperada — o jogador experimenta proporcionalidade antes de a nomear |

---

### 4.4 Interpretação do resumo financeiro e End Screen

**Mecânica:** Ao fim de cada dia, o resumo noturno exibe a tabela de vendas, receita e saldo. No dia 15, o End Screen exibe o gráfico de evolução do saldo e a cultura mais rentável do ciclo.

| Habilidade | Descrição (BNCC) | Como é desenvolvida |
|---|---|---|
| **EF04MA27** | Analisar dados apresentados em tabelas simples ou de dupla entrada e em gráficos de barras | O resumo noturno é uma tabela de vendas (cultura × quantidade × preço × total); o jogador lê e interpreta a cada sessão |
| **EF05MA24** | Interpretar dados estatísticos apresentados em textos, tabelas e gráficos sobre temas de interesse | O gráfico de linha do End Screen (saldo × dias) é uma série temporal que o jogador lê para entender sua trajetória financeira no ciclo |

---

## 5. Quadro Consolidado BNCC × Mecânica

| Habilidade BNCC | Unidade Temática | Mecânica do Jogo |
|---|---|---|
| EF04MA03 | Números | Gestão de capital diário (saldo, compra de sementes) |
| EF04MA05 | Números | Cálculo mental no planejamento de plantio |
| EF04MA25 | Números | Precificação no mercado noturno |
| EF04MA27 | Probabilidade e Estatística | Leitura do resumo noturno em tabela |
| EF05MA06 | Números | HUD de margem (%) ao definir preço de venda |
| EF05MA07 | Números | Receita − custos no resumo noturno |
| EF05MA08 | Números | Cálculo de receita esperada por tile (multiplicação) |
| EF05MA12 | Álgebra | Proporcionalidade na decisão de quantos tiles plantar |
| EF05MA24 | Probabilidade e Estatística | Leitura do gráfico de saldo no End Screen |

---

## 6. Tema Transversal: Educação Financeira e para o Consumo

A BNCC reconhece Educação Financeira e para o Consumo como tema transversal que deve permear todas as disciplinas (BNCC, 2018, p. 266). Florestia endereça esse tema de forma integral:

- **Planejamento financeiro:** o jogador gerencia receita, custo e saldo ao longo de 15 dias
- **Consumo consciente e responsável:** a mecânica do atravessador penaliza quem aceita o menor preço por conveniência — a vitória requer autonomia de precificação
- **Precificação justa:** o jogador aprende que preço = custo + margem, não apenas "quanto o comprador aceita"
- **Risco e retorno:** Açaí tem a maior margem mas imobiliza capital por 6 dias — decisão de risco vivenciada sem abstração

---

## 7. Progressão Pedagógica (Loops do GDD)

O GDD estrutura o jogo em três loops. A progressão pedagógica correspondente é:

| Loop (GDD) | Conteúdo matemático | Habilidades BNCC |
|---|---|---|
| **Gestão** (plantar, regar, colher) | Adição, subtração, multiplicação simples; contagem de dias | EF04MA03, EF04MA05, EF05MA08 |
| **Decisão** (qual cultura, quanto plantar, quando colher) | Comparação de margens, proporcionalidade, cálculo mental | EF04MA05, EF05MA12 |
| **Negociação** (definir preço, vender) | Compra e venda, porcentagem, margem bruta | EF04MA25, EF05MA06, EF05MA07 |

---

## 8. Referências

- BRASIL. **Base Nacional Comum Curricular.** Brasília: MEC, 2018. Disponível em: basenacionalcomum.mec.gov.br
- FREIRE, Paulo. **Pedagogia da Autonomia:** saberes necessários à prática educativa. São Paulo: Paz e Terra, 1996.
- GEE, James Paul. **What Video Games Have to Teach Us About Learning and Literacy.** New York: Palgrave Macmillan, 2007.
- PRENSKY, Marc. **Digital Game-Based Learning.** New York: McGraw-Hill, 2001.
