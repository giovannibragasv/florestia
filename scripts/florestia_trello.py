#!/usr/bin/env python3
"""Generate Florestia Trello assets:
- CSV for one-time import (one card per leaf task)
- Markdown live tracker with checkboxes (source of truth going forward)

After initial generation, prefer editing the markdown directly to mark
tasks done ([x]) or add new ones; this script is a seed tool.
"""
import csv
from dataclasses import dataclass, field
from pathlib import Path

CSV_OUT = Path("/Users/giovannivasconcelos/Documents/uni/CC7NA/florestia/Docs/florestia-trello-board.csv")
MD_OUT = Path("/Users/giovannivasconcelos/Documents/uni/CC7NA/florestia/Docs/florestia-tasks.md")

MVP = "🎯 Sprint MVP (11-19/05)"
FEIRA = "🌳 Sprint Feira (20/05-07/06)"
BACKLOG = "📋 Backlog"
DONE = "✅ Concluído"


@dataclass
class Cluster:
    id: str
    title: str
    list_name: str
    labels: str
    members: str
    due: str
    depends: str = ""
    tasks: list = field(default_factory=list)  # list of leaf-task strings
    single_desc: str = ""  # if no subtasks, this is the description
    completed: bool = False  # for items in DONE list, mark all as [x]


CLUSTERS: list[Cluster] = []


def add(cluster):
    CLUSTERS.append(cluster)
    return cluster


# ─── Bloco A · Refactor kid-friendly ──────────────────────────────────
add(Cluster("A01", "Audit + reescrita de todos os textos do jogo", MVP,
    "mentor,kid-friendly,conteúdo,educação", "biel,kadu", "2026-05-14",
    tasks=[
        "Listar todos os strings da UI (HUD, botões, modais, EndScreen)",
        "Reescrever conceitos adultos: 'margem', 'estoque', 'saldo', 'custo fixo'",
        "Reescrever mensagens educacionais da EndScreen",
        "Reescrever feedback do mercado (acceptLine / rejectLine atuais)",
        "Validar com o teste de 10 segundos da auditoria de UX por idade",
    ]))

add(Cluster("A02", "Refactor de textos no código", MVP,
    "mentor,kid-friendly,código", "vanni", "2026-05-15", depends="A01",
    tasks=[
        "Aplicar strings novas em HUDController, MarketUIController, EndScreenController",
        "Trocar 'ADMIN' → 'intermediadora' em todo o código e docs",
        "Remover notação % do HUD do mercado (manter sinal por cor)",
        "Renomear SerializedFields para nomes mais legíveis se ajudar leitura do código",
    ]))

add(Cluster("A03", "Tutorial inicial in-game", MVP,
    "mentor,kid-friendly,código,educação", "vanni", "2026-05-17", depends="A01, C01",
    tasks=[
        "Tela de boas-vindas no primeiro launch",
        "Tooltip contextual primeira ação de plantar",
        "Tooltip contextual primeira ação de regar",
        "Tooltip contextual primeira ação de colher",
        "Tooltip contextual primeira ida ao mercado",
        "Botão 'pular tutorial' sempre disponível",
        "Flag de 'tutorial completo' no save",
    ]))

add(Cluster("A04", "MarketScene kid-first", MVP,
    "mentor,kid-friendly,código", "vanni", "2026-05-16", depends="A01, C02",
    tasks=[
        "Substituir slider de quantidade por stepper ◀ [N] ▶",
        "Remover percentual da margem (manter R$ + cor)",
        "Botão vender com label dinâmico ('Vender 4 mandiocas')",
        "Retrato do comprador dominando 1/3 esquerdo com balão de fala",
        "Animação thumbs-up/down no resultado da venda",
        "Pilha de moedas visual para o total",
        "Eliminar runtime fallback EnsureQuantitySlider",
    ]))

add(Cluster("A05", "EndScreen kid-first", MVP,
    "mentor,kid-friendly,código", "vanni", "2026-05-17", depends="A01, C03, C09",
    tasks=[
        "Hero zone: 3 estados visuais (Falência / Sobreviveu / Lucrou) com ilustração",
        "Frase única encorajadora por desfecho (de C03)",
        "Gráfico de barras com rótulos de dia (1-15)",
        "Linha horizontal de zero no gráfico",
        "Marker visual no dia da virada (saldo cruzou para negativo)",
        "Barras coloridas por sinal (vermelho < 0, verde ≥ 0)",
        "Tip contextual baseado em comportamento do aluno",
        "Botão 'JOGAR DE NOVO' grande e dominante",
        "Remover 'Falência! O saldo chegou a zero' em vermelho-sobre-preto",
    ]))

add(Cluster("A06", "Insights e dicas in-game", MVP,
    "mentor,kid-friendly,código,educação", "vanni", "2026-05-18", depends="C05",
    tasks=[
        "Pop-up curto após primeira venda lucrativa",
        "Pop-up curto após primeira venda no prejuízo",
        "Pop-up no início de cada dia com dica do dia anterior",
        "Tip contextual na EndScreen baseado em padrão de comportamento",
    ]))

add(Cluster("A07", "Visualização da progressão do dia", MVP,
    "mentor,kid-friendly,código", "vanni", "2026-05-18",
    tasks=[
        "Indicador visual de fase do dia além do sky gradient",
        "Texto curto no HUD: 'Manhã' / 'Tarde' / 'Anoitecer'",
    ]))

# ─── Bloco B · Mecânica nova ───────────────────────────────────────────
add(Cluster("B01", "Save persistente por aluno", MVP,
    "mentor,kid-friendly,código,educação", "vanni", "2026-05-14",
    tasks=[
        "Identificação simples de aluno (nome digitado no boot, sem login)",
        "Estrutura de dados por aluno: plantios, vendas, perguntas, decisões, dia atual",
        "Salvar a cada ação relevante (extensão do SaveSystem)",
        "Carregar ao iniciar; menu pra trocar de aluno",
        "Reset functionality",
    ]))

add(Cluster("B02", "Pergunta diária no fim do dia (versão hardcoded)", MVP,
    "mentor,kid-friendly,código,educação", "vanni", "2026-05-16", depends="B01, C07, C08",
    tasks=[
        "Hook no fim do dia (modal antes do AdvanceDay)",
        "Carregar banco de perguntas hardcoded por cultura (de C08)",
        "UI da pergunta: enunciado + 3 alternativas, ou input numérico",
        "Feedback visual: certo (verde + parabéns) / errado (mostra a certa, sem punição)",
        "Salvar resposta no histórico do aluno",
        "Escolha simples: pergunta baseada na última cultura plantada/vendida",
    ]))

add(Cluster("B03", "Fallback de histórico se não plantou", MVP,
    "mentor,kid-friendly,código,educação", "vanni", "2026-05-17", depends="B01",
    tasks=[
        "Detectar 'não plantou hoje'",
        "Mostrar revisão histórica: mini-gráfico, melhor venda da semana",
    ]))

add(Cluster("B04", "Dia como fase com encerramento educacional", MVP,
    "kid-friendly,código,educação", "vanni", "2026-05-17", depends="B02",
    tasks=[
        "Encadeamento: Resumo Noturno → Pergunta → dica → fechar fase",
        "Tela 'Dia X concluído!' antes do resumo, com mood encorajador",
        "Transição visual de fim de dia → início do próximo dia",
    ]))

# ─── Bloco C · Conteúdo ─────────────────────────────────────────────────
add(Cluster("C01", "Roteiro do tutorial inicial", MVP,
    "conteúdo,educação", "kadu,biel", "2026-05-13",
    tasks=[
        "Roteiro passo 1: chegar à roça e plantar",
        "Roteiro passo 2: regar a plantação",
        "Roteiro passo 3: colher",
        "Roteiro passo 4: ir ao mercado pela ponte",
        "Roteiro passo 5: vender com slider de preço",
        "Roteiro passo 6: encerrar o dia + pergunta",
    ]))

add(Cluster("C02", "Diálogos dos compradores (v1 MVP)", MVP,
    "mentor,conteúdo,educação", "kadu", "2026-05-14",
    tasks=[
        "Escrever Atravessador: tom rápido, ofertas baixas mas certas",
        "Escrever Feirante Local: tom amigável, preços medianos",
        "Escrever Comprador Direto: tom respeitoso, paga bem mas raro",
        "Validar vocabulário 8-11, sem jargão financeiro",
    ]))

add(Cluster("C03", "Mensagens educacionais da EndScreen", MVP,
    "mentor,conteúdo,educação,kid-friendly", "biel", "2026-05-14",
    tasks=[
        "Escrever variante Falência: encorajadora, sugere estratégia",
        "Escrever variante Sobreviveu: validador, sugere upgrade",
        "Escrever variante Lucrou: parabéns + próximo desafio implícito",
    ]))

add(Cluster("C04", "Audit + glossário de termos do HUD", MVP,
    "mentor,conteúdo,kid-friendly", "biel", "2026-05-13",
    tasks=[
        "Listar todos os strings do HUDController",
        "Listar strings do MarketUIController",
        "Listar strings da EndScreenController",
        "Para cada string, propor substituto kid-friendly 8-11",
    ]))

add(Cluster("C05", "Roteiro de insights pop-up in-game", MVP,
    "mentor,conteúdo,educação", "biel", "2026-05-15",
    tasks=[
        "Pop-up primeira venda lucrativa: parabéns + nome do conceito (lucro)",
        "Pop-up primeira venda no prejuízo: sem culpa + explicação curta",
        "Pop-up início do dia 2+: dica do dia anterior baseada em comportamento",
    ]))

add(Cluster("C06", "Spec + wireframe do dashboard ADMIN", MVP,
    "mentor,conteúdo,educação", "pm", "2026-05-14",
    tasks=[
        "Listar métricas relevantes para a professora",
        "Wireframe em papel ou Figma simples",
        "Definir hierarquia de informação",
        "Entregar doc + imagem do wireframe",
    ]))

add(Cluster("C07", "Spec do mecanismo de pergunta diária", MVP,
    "mentor,conteúdo,educação", "biel,vanni", "2026-05-13",
    tasks=[
        "Decidir formato: múltipla escolha (3 alternativas) ou input numérico",
        "Definir política de escolha da pergunta (random vs adaptativo)",
        "Definir política de feedback: certo / errado",
        "Definir estrutura do banco de perguntas (campos)",
    ]))

add(Cluster("C08", "Banco inicial de perguntas hardcoded", MVP,
    "mentor,conteúdo,educação", "biel,luiz", "2026-05-15",
    tasks=[
        "5 perguntas Mandioca (custo, margem, multiplicação)",
        "5 perguntas Cacau",
        "5 perguntas Açaí",
        "Estruturar cada pergunta: enunciado + 3 alternativas + correta + explicação",
    ]))

add(Cluster("C09", "Ilustrações da Hero zone da EndScreen", MVP,
    "kid-friendly,polimento", "kadu", "2026-05-16",
    tasks=[
        "Ilustração Falência: agricultor pensativo, sem desespero",
        "Ilustração Sobreviveu: agricultor neutro com colheita modesta",
        "Ilustração Lucrou: agricultor com pilha de moedas e colheita farta",
    ]))

# ─── Bloco D · Modelo C v2 ──────────────────────────────────────────────
add(Cluster("D01", "Modelo C v2 · Cliente", MVP,
    "modelo-c", "pm", "2026-05-14",
    tasks=[
        "Revisar texto atual do campo Cliente",
        "Articular dois clientes técnicos (criança jogador + professora dashboard)",
        "Atualizar Modelo C v2 no formato visual do canvas",
    ]))

add(Cluster("D02", "Modelo C v2 · Mecanismos de Receita", MVP,
    "modelo-c", "pm", "2026-05-15",
    tasks=[
        "Analisar viabilidade Steam (monetização direta)",
        "Analisar viabilidade Governo (Sec. Municipal, Prefeitura, MEC)",
        "Analisar viabilidade empresas de jogos (parceria)",
        "Escrever recomendação argumentada",
    ]))

add(Cluster("D03", "Modelo C v2 · Acompanhamento do Impacto", MVP,
    "modelo-c,educação", "pm", "2026-05-16",
    tasks=[
        "Listar indicadores automáticos do jogo (tempo, margem, padrão de comprador, % acerto)",
        "Listar indicadores pedagógicos (provas antes/depois, participação, feedback)",
        "Definir critérios de sucesso",
        "Atualizar canvas Modelo C",
    ]))

add(Cluster("D04", "Modelo C v2 · Foco do Impacto / Ações", MVP,
    "modelo-c,educação", "pm", "2026-05-17",
    tasks=[
        "Revisar texto atual de Foco do Impacto",
        "Adicionar representatividade brasileira/jutaí",
        "Revisar Ações de Impacto Positivo correspondentes",
        "Atualizar canvas Modelo C",
    ]))

# ─── Bloco E · Mock Dashboard ───────────────────────────────────────────
add(Cluster("E01", "Mock do dashboard ADMIN", MVP,
    "mentor,kid-friendly,código,educação", "vanni", "2026-05-18", depends="C06, D01",
    tasks=[
        "Tela acessível por flag ou senha simples",
        "Layout estático com dados dummy: aluno, dia, saldo, melhor cultura, % acerto",
        "Reaproveitar componente de gráfico de barras da EndScreen",
        "Header 'visão da intermediadora'",
        "Sem persistência real (funcional vem na Feira)",
    ]))

# ─── Bloco F · Pesquisa ─────────────────────────────────────────────────
add(Cluster("F01", "Estudo histórico/cultural Vila Jutaí · preliminar", MVP,
    "pesquisa", "tex,kadu", "2026-05-17",
    tasks=[
        "Perfil quilombola/ribeirinho da Vila Jutaí",
        "Cultivos locais e práticas agrícolas",
        "Dinâmicas econômicas (atravessadores, feiras)",
        "Entregar doc curto (1-2 páginas)",
    ]))

add(Cluster("F02", "Estudo do formato do resumo do dia", MVP,
    "pesquisa,educação", "vanni,biel", "2026-05-14",
    tasks=[
        "Pesquisar pelo menos 3 jogos com feedback diário",
        "Anotar o que funciona e o que não funciona",
        "Entregar doc curto com recomendações",
    ]))

add(Cluster("F03", "Pesquisa em educação financeira infantil · preliminar", MVP,
    "pesquisa,educação", "luiz,pm", "2026-05-17",
    tasks=[
        "Ler pelo menos 3 artigos sobre educação financeira infantil",
        "Listar conceitos adequados 8-11",
        "Listar conceitos a evitar",
        "Entregar doc curto",
    ]))

# ─── Bloco G · Easy wins ────────────────────────────────────────────────
add(Cluster("G01", "Barra 'life' de tempo de plantação", MVP,
    "kid-friendly,código", "vanni", "2026-05-13",
    tasks=[
        "Barra fina sobre cada CropSlot ocupado",
        "Preenchimento conforme daysPlanted / growthDays",
        "Esconde quando crop está ready",
    ]))

add(Cluster("G02", "Painel de vendas com vendedor (refinamento)", MVP,
    "kid-friendly,código", "vanni", "2026-05-14",
    tasks=[
        "Garantir update do painel após SelectCropByIndex",
        "Sell button label dinâmico (relacionado com A04)",
        "Bloquear vender 0 unidades",
    ]))

# ─── Bloco H · Apresentação MVP ─────────────────────────────────────────
add(Cluster("H01", "Slide para apresentação do MVP (mentor-facing)", MVP,
    "mentor,apresentação", "kadu,tex", "2026-05-18",
    tasks=[
        "Gravar gifs das mecânicas novas (plantio, mercado, pergunta, EndScreen)",
        "Coletar sprites e ilustrações da Hero zone",
        "Slide de fundamentação BNCC (referenciar EDD compliance PDF)",
        "Slide do Modelo C v2 (resumo dos 4 campos)",
        "Slide do preview da Feira",
    ]))

# ─── Sprint Feira ──────────────────────────────────────────────────────
add(Cluster("FA01", "Diálogos dos compradores v2 — representatividade plena", FEIRA,
    "mentor,conteúdo,educação,feira", "kadu,vanni", "2026-06-01", depends="FC01",
    single_desc="Versão profunda com representatividade brasileira/jutaí."))

add(Cluster("FA02", "Pergunta diária v2 — adaptativa", FEIRA,
    "código,educação,feira", "vanni", "2026-06-03",
    tasks=[
        "Banco expandido (10+ perguntas por cultura)",
        "Lógica adaptativa: dificuldade sobe com acertos",
        "Insights culturais alternando com matemáticos",
        "Persistir desempenho por aluno",
    ]))

add(Cluster("FA03", "Dashboard ADMIN funcional", FEIRA,
    "mentor,código,educação,feira", "vanni", "2026-06-05",
    tasks=[
        "Conectar com save persistente B01",
        "Métricas reais por aluno",
        "Visualizações: gráfico saldo, % acerto, padrão de compradores",
        "Exportação simples (CSV ou imprimir)",
    ]))

add(Cluster("FA04", "Histórico longitudinal por aluno", FEIRA,
    "código,educação,feira", "vanni", "2026-06-04",
    tasks=[
        "Estrutura de dados para múltiplas runs por aluno",
        "Agregação de métricas ao longo do tempo",
        "Visualização no dashboard ADMIN",
    ]))

add(Cluster("FA05", "Hero zone EndScreen v2 — ilustrações finais", FEIRA,
    "polimento,feira", "kadu,vanni", "2026-06-02",
    single_desc="Substituir ilustrações MVP (C09) por versão final polida."))

add(Cluster("FB01", "Build WebGL para Chromebook", FEIRA,
    "código,feira", "vanni", "2026-05-30",
    tasks=[
        "Configurar Build Profile WebGL",
        "Resolver issues de compatibility",
        "Testar em Chromebook real",
        "Upload em URL acessível",
    ]))

add(Cluster("FB02", "Integração de audio", FEIRA,
    "código,polimento,feira", "vanni,luiz", "2026-06-01",
    tasks=[
        "Ambient da roça (loop curto)",
        "SFX plant",
        "SFX water",
        "SFX harvest",
        "SFX sell + reject",
        "Música mercado noturno",
        "Volume controls básicos",
    ]))

add(Cluster("FB03", "Frames adicionais walkUp e walkSide", FEIRA,
    "polimento,feira", "kadu,vanni", "2026-05-28",
    tasks=[
        "Sprites walkUp 2-frame (kadu)",
        "Sprites walkSide 2-frame (kadu)",
        "Integrar no PlayerController (vanni)",
    ]))

add(Cluster("FB04", "Decisão e implementação de balanceamento de stamina", FEIRA,
    "código,educação,feira", "vanni,pm", "2026-05-25",
    tasks=[
        "Decisão pm + vanni sobre balanço final (3/2/3 vs 1/1/1)",
        "Atualizar CropData assets se mudar",
        "Atualizar GDD para refletir",
    ]))

add(Cluster("FB05", "Smoke test de 1 partida completa", FEIRA,
    "código,feira", "vanni", "2026-06-05",
    tasks=[
        "Iniciar run nova",
        "Jogar até o fim (15 dias)",
        "Anotar todos os bugs e issues",
        "Priorizar fixes pré-Feira",
    ]))

add(Cluster("FB06", "Suporte completo a mouse para interação", FEIRA,
    "kid-friendly,código,feira", "vanni", "2026-06-02",
    tasks=[
        "Mouse hover destaca CropSlot apontado (substitui tile highlight do facing)",
        "Click do mouse executa interação no tile (substitui tecla E)",
        "Cursor muda de ícone conforme ferramenta selecionada no toolbar",
        "Teste de touchpad em Chromebook real (público-alvo)",
    ]))

add(Cluster("FC01", "Estudo histórico/cultural v2 profundo", FEIRA,
    "pesquisa,feira", "tex,kadu", "2026-05-28",
    tasks=[
        "Contatar lideranças da Vila Jutaí",
        "Pesquisa em fontes primárias se viável",
        "Entregar doc estendido",
    ]))

add(Cluster("FC02", "Pesquisa educação financeira infantil v2 profundo", FEIRA,
    "pesquisa,educação,feira", "luiz,pm", "2026-05-30",
    tasks=[
        "Ler pelo menos 8 artigos",
        "Análise crítica dos achados",
        "Entregar artigo curto (~2-3 páginas)",
    ]))

add(Cluster("FC03", "Reunião com profissional pedagogo", FEIRA,
    "pesquisa,mentor,feira", "grupo todo", "2026-05-25",
    tasks=[
        "Identificar pedagogo (CESUPA?)",
        "Marcar reunião",
        "Apresentar o jogo e Modelo C",
        "Capturar feedback",
        "Iterar com base no feedback",
    ]))

add(Cluster("FC04", "Aplicação piloto curta na escola Vila Jutaí", FEIRA,
    "pesquisa,educação,feira", "grupo todo", "2026-06-04",
    tasks=[
        "Contatar a escola",
        "Marcar visita",
        "Aplicar com pelo menos 1 turma",
        "Capturar dados",
    ]))

add(Cluster("FD01", "Abordagem a empresas de jogos", FEIRA,
    "modelo-c,mentor,feira", "pm", "2026-06-02",
    tasks=[
        "Listar pelo menos 5 empresas potencialmente alinhadas",
        "Draft de proposta (1 página)",
        "Pesquisar canais de contato",
    ]))

add(Cluster("FD02", "Plano de continuidade pós-Amazon Hacking", FEIRA,
    "modelo-c,feira", "pm", "2026-06-04",
    tasks=[
        "Definir 3 cenários de continuidade",
        "Para cada cenário, listar próximos passos",
        "Entregar doc para júri da Feira",
    ]))

add(Cluster("FE01", "Plano de demonstração presencial", FEIRA,
    "apresentação,feira", "tex,vanni", "2026-05-30",
    tasks=[
        "Roteiro demo curta (2-5 min) para visitante casual",
        "Roteiro demo júri (10-15 min)",
        "Ensaio interno",
    ]))

add(Cluster("FE02", "Materiais visuais do estande", FEIRA,
    "apresentação,feira", "kadu,biel", "2026-06-02",
    tasks=[
        "Banner principal",
        "Folder explicativo",
        "Identidade visual coesa (usar paleta Florestia)",
    ]))

add(Cluster("FE03", "Setup técnico do estande", FEIRA,
    "feira", "tex,vanni", "2026-05-31",
    tasks=[
        "Laptops com build WebGL/standalone",
        "Energia e cabeamento",
        "Conectividade redundante",
        "Save anônimo por visitante (resetável)",
    ]))

add(Cluster("FE04", "Q&A prep para o júri", FEIRA,
    "apresentação,feira", "grupo todo", "2026-06-04",
    tasks=[
        "Listar 15 perguntas prováveis",
        "Respostas escritas",
        "Ensaio interno tipo arguição",
    ]))

add(Cluster("FE05", "Iteração com feedback do mentor pós-19/05", FEIRA,
    "feira,mentor", "grupo todo", "2026-05-21",
    tasks=[
        "Capturar feedback completo na sala 19/05",
        "Priorizar 3 ajustes de maior retorno",
        "Atribuir donos e due dates",
    ]))

add(Cluster("FE06", "Slide para o júri Amazon Hacking", FEIRA,
    "mentor,apresentação,feira", "kadu,tex,vanni", "2026-06-03",
    tasks=[
        "Slide de abertura: tensão observada na Vila Jutaí",
        "Slide do produto: demo de mecânicas",
        "Slide de fundamentação: BNCC + EDD + Modelo C",
        "Slide de impacto: indicadores + ODS 1 e 4",
        "Slide de continuidade: plano pós-Amazon",
    ]))

# ─── Backlog ────────────────────────────────────────────────────────────
add(Cluster("BL01", "Sistema de NPCs com diálogo (GDD §10)", BACKLOG,
    "código,polimento", "", "", single_desc="Fora de escopo MVP e Feira. Pós-Feira."))
add(Cluster("BL02", "Expansão do grid além de 6x6", BACKLOG,
    "código,polimento", "", "", single_desc="Pós-Feira."))
add(Cluster("BL03", "Múltiplas save slots por dispositivo", BACKLOG,
    "código", "", "", single_desc="Pós-Feira. O save-por-aluno do MVP cobre o caso principal."))
add(Cluster("BL04", "Deterioração de estoque", BACKLOG,
    "código", "", "", single_desc="Pós-Feira."))
add(Cluster("BL05", "Sistema de clima", BACKLOG,
    "código", "", "", single_desc="Pós-Feira."))
add(Cluster("BL06", "Implementação em escolas além da Vila Jutaí", BACKLOG,
    "educação", "", "", single_desc="Depende do resultado da Feira."))

# ─── Concluído ──────────────────────────────────────────────────────────
add(Cluster("DONE01", "TMP_Dropdown → 3 botões de cultura", DONE,
    "código,kid-friendly,mentor", "vanni", "", completed=True,
    single_desc="Código + builder atualizados. Eliminou o erro de SetupTemplate."))
add(Cluster("DONE02", "Resumo Noturno modal versão básica", DONE,
    "código,educação", "vanni", "", completed=True,
    single_desc="Modal entre mercado e AdvanceDay. Perguntas serão adicionadas em B02."))
add(Cluster("DONE03", "Multi-quantity sell (slider)", DONE,
    "código,educação", "vanni", "", completed=True,
    single_desc="Slider de quantidade funciona. Vira stepper em A04."))
add(Cluster("DONE04", "HUD math coaching labels", DONE,
    "código,educação", "vanni", "", completed=True,
    single_desc="Preview + proporcionalidade. Vai ser revisto em A02 para remover %."))
add(Cluster("DONE05", "EDD compliance PDF", DONE,
    "modelo-c,educação", "vanni", "", completed=True,
    single_desc="12 páginas via Kami. Auditoria das 9 habilidades BNCC."))


# ────────────────────────────────────────────────────────────────────────
# WRITE CSV
# ────────────────────────────────────────────────────────────────────────
def write_csv():
    CSV_OUT.parent.mkdir(parents=True, exist_ok=True)
    rows = []
    for c in CLUSTERS:
        context = f"Cluster {c.id} · {c.title}"
        dep_line = f"\n**Depende de:** {c.depends}" if c.depends else ""
        if c.tasks:
            for idx, sub in enumerate(c.tasks, 1):
                rows.append((c.list_name, f"{c.id}.{idx} · {sub}",
                             f"{context}.{dep_line}", c.labels, c.members, c.due))
        else:
            title_prefix = "✓" if c.completed else c.id
            rows.append((c.list_name, f"{title_prefix} · {c.title}" if not c.completed else f"✓ {c.title}",
                         f"{c.single_desc}{dep_line}", c.labels, c.members, c.due))

    with CSV_OUT.open("w", encoding="utf-8", newline="") as f:
        w = csv.writer(f, quoting=csv.QUOTE_ALL)
        w.writerow(["List", "Card Name", "Description", "Labels", "Members", "Due Date"])
        for row in rows:
            w.writerow(row)
    return len(rows)


# ────────────────────────────────────────────────────────────────────────
# WRITE MARKDOWN
# ────────────────────────────────────────────────────────────────────────
def write_markdown():
    lines = [
        "# Florestia · Live Task Tracker",
        "",
        "Fonte única de tudo que está pendente, em curso ou concluído no projeto.",
        "",
        "**Como usar:**",
        "- `- [ ]` = pendente · `- [x]` = concluído",
        "- Marcar uma task como feita: editar `[ ]` para `[x]` neste arquivo",
        "- Adicionar nova task: criar nova linha no cluster certo, ou novo cluster se for tema novo",
        "- Esse arquivo é a fonte; o Trello é o espelho operacional",
        "",
        "**Última geração:** este markdown foi gerado a partir do script `scripts/florestia_trello.py`. Edições manuais NÃO são reaplicadas pelo script — depois desta geração inicial, edite o markdown direto.",
        "",
        "---",
        "",
    ]

    # Group by list
    by_list: dict[str, list[Cluster]] = {}
    for c in CLUSTERS:
        by_list.setdefault(c.list_name, []).append(c)

    list_order = [MVP, FEIRA, BACKLOG, DONE]
    for lst in list_order:
        if lst not in by_list:
            continue
        lines.append(f"## {lst}")
        lines.append("")

        for c in by_list[lst]:
            # Cluster header
            meta_parts = []
            if c.labels:
                labels_pretty = " · ".join(c.labels.split(","))
                meta_parts.append(f"**Labels:** {labels_pretty}")
            if c.members:
                meta_parts.append(f"**Dono:** {c.members}")
            if c.due:
                meta_parts.append(f"**Due:** {c.due}")
            if c.depends:
                meta_parts.append(f"**Depende:** {c.depends}")
            meta_line = " | ".join(meta_parts)

            lines.append(f"### {c.id} · {c.title}")
            if meta_line:
                lines.append(f"> {meta_line}")
            lines.append("")

            check = "x" if c.completed else " "
            if c.tasks:
                for idx, sub in enumerate(c.tasks, 1):
                    lines.append(f"- [{check}] **{c.id}.{idx}** {sub}")
            else:
                if c.single_desc:
                    lines.append(f"- [{check}] **{c.id}** {c.title}")
                    lines.append(f"  - {c.single_desc}")
                else:
                    lines.append(f"- [{check}] **{c.id}** {c.title}")
            lines.append("")

        lines.append("---")
        lines.append("")

    MD_OUT.parent.mkdir(parents=True, exist_ok=True)
    MD_OUT.write_text("\n".join(lines), encoding="utf-8")
    return sum(len(c.tasks) if c.tasks else 1 for c in CLUSTERS)


if __name__ == "__main__":
    n_csv = write_csv()
    n_md = write_markdown()
    print(f"CSV: {n_csv} cards → {CSV_OUT}")
    print(f"MD:  {n_md} tasks → {MD_OUT}")
