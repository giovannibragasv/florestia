# Florestia

Educational farming game set in the Amazon. Players manage a small agroforestry plot over 15 in-game days, pricing and selling crops to survive financially. Built for Amazon Hacking 2026 and CC7NA.

---

## Requirements

| Tool | Version |
|---|---|
| Unity | 6000.0 LTS (Unity 6) or 2022.3 LTS |
| C# | .NET Standard 2.1 (handled by Unity) |
| TextMeshPro | included via Unity Package Manager |
| Git | any recent version |

No external packages or NuGet dependencies.

---

## Getting Started

### 1. Clone the repo

```bash
git clone https://github.com/giovannibragasv/florestia.git
cd florestia
```

### 2. Open in Unity

1. Open **Unity Hub**
2. Click **Open → Add project from disk**
3. Select the `florestia/` folder (the one that contains `Assets/`, `Packages/`, `ProjectSettings/`)
4. Unity will import assets and generate solution files automatically — this takes a minute on first open

### 3. Install TextMeshPro essentials

On first open Unity may prompt: **"Import TMP Essentials"** — click it. The HUD and market UI scripts depend on `TMPro`.

---

## Project Structure

```
florestia/
├── Assets/
│   ├── Animations/
│   │   ├── Crops/          # animator controllers per crop stage
│   │   └── Buyers/         # buyer idle/react animations
│   ├── Audio/
│   │   ├── Music/          # background loops per scene
│   │   └── SFX/            # plant, harvest, sell, reject sounds
│   ├── Fonts/              # custom fonts (assign in Inspector)
│   ├── Materials/          # shared materials
│   ├── Prefabs/
│   │   ├── Crops/          # one prefab per crop (has CropSlot component)
│   │   ├── Buyers/         # one prefab per buyer NPC
│   │   └── UI/             # bar chart bar prefab, warning panel
│   ├── Scenes/
│   │   ├── FarmScene       # main gameplay (6×6 grid + HUD)
│   │   ├── MarketScene     # nightly market modal
│   │   └── EndScreen       # day-15 results + balance chart
│   ├── Scripts/
│   │   ├── Core/           # GameManager, DayNightCycle, SaveSystem
│   │   ├── Farm/           # CropData, CropSlot, CropSystem, StaminaSystem
│   │   ├── Economy/        # InventorySystem, PricingSystem
│   │   ├── Market/         # BuyerData, BuyerSystem, BuyerSelector, MarketUIController
│   │   └── UI/             # HUDController, EndScreenController
│   ├── Sprites/
│   │   ├── Crops/
│   │   │   ├── Acai/       # growth stage sprites (seed → ready)
│   │   │   ├── Cacau/
│   │   │   └── Mandioca/
│   │   ├── Buyers/         # portraits for Atravessador, Feirante, Comprador
│   │   ├── Terrain/        # tilemap tiles
│   │   └── UI/             # HUD icons, buttons
│   └── Tilemaps/           # tilemap assets
├── Docs/
│   ├── GDD.md / GDD.pdf    # Game Design Document
│   └── EDD.md / EDD.pdf    # Educational Design Document (BNCC)
├── Packages/               # Unity package manifest
└── ProjectSettings/        # Unity project settings
```

---

## Scene Setup Guide

Each scene needs GameObjects wired up in the Inspector. Do this once after opening the project.

### FarmScene

1. Create an empty GameObject named `_GameManager`, add `GameManager`
2. Create an empty GameObject named `_DayNightCycle`, add `DayNightCycle`
   - Assign a full-screen `Image` to **Sky Overlay**
   - Assign the night warning panel to **Night Warning Panel**
3. Create an empty GameObject named `_StaminaSystem`, add `StaminaSystem`
4. Create an empty GameObject named `_CropSystem`, add `CropSystem`
   - Assign all 36 `CropSlot` GameObjects to **Slots**
   - Assign the three `CropData` ScriptableObjects to **Crop Catalog**
5. Create an empty GameObject named `_InventorySystem`, add `InventorySystem`
6. Create an empty GameObject named `_PricingSystem`, add `PricingSystem`
7. Create an empty GameObject named `_HUD`, add `HUDController`, wire all labels/slider
8. Create an empty GameObject named `_EndScreen`, add `EndScreenController`, set `DontDestroyOnLoad`

### MarketScene

1. Create an empty GameObject named `_BuyerSystem`, add `BuyerSystem`
   - Assign the three `BuyerData` ScriptableObjects to **Buyers**
2. Create an empty GameObject named `_MarketUI`, add `MarketUIController`
   - Wire all `[SerializeField]` slots from the Canvas hierarchy
3. Create an empty GameObject named `_BuyerSelector`, add `BuyerSelector`
   - Assign buyer buttons and `MarketUIController`

### EndScreen

1. `EndScreenController` persists from FarmScene via `DontDestroyOnLoad` — do not add it again here
2. Create the results Canvas with `finalBalanceLabel`, `outcomeLabel`, `bestCropLabel`, `chartContainer`, `playAgainButton`
3. Assign a simple colored-rectangle prefab to **Bar Prefab** in `EndScreenController`

---

## ScriptableObject Setup

Create these assets via **Assets → Create → Florestia → …** in the Project window.

### CropData (×3)

| Field | Mandioca | Cacau | Açaí |
|---|---|---|---|
| Crop Name | `Mandioca` | `Cacau` | `Acai` |
| Growth Days | 2 | 4 | 6 |
| Seed Cost | 3 | 6 | 10 |
| Base Market Value | 7 | 16 | 28 |
| Stamina Cost To Plant | 3 | 3 | 3 |
| Stamina Cost To Water | 2 | 2 | 2 |
| Stamina Cost To Harvest | 3 | 3 | 3 |

### BuyerData (×3)

| Field | Atravessador | Feirante Local | Comprador Direto |
|---|---|---|---|
| Buyer Name | `Atravessador` | `Feirante Local` | `Comprador Direto` |
| Max Price Mandioca | 5 | 7 | 9 |
| Max Price Cacau | 12 | 15 | 18 |
| Max Price Acai | 20 | 26 | 30 |

---

## Build (PC)

1. **File → Build Settings**
2. Select **PC, Mac & Linux Standalone**, platform **Windows** or **macOS**
3. Add all three scenes in order: `FarmScene`, `MarketScene`, `EndScreen`
4. Click **Build** — output to `Builds/`

Save data is written to `Application.persistentDataPath/florestia_save.json`. On macOS that is `~/Library/Application Support/DefaultCompany/florestia/`.

---

## Game Rules (quick reference)

- **Start:** R$50, Day 1 of 15
- **Each day:** 5 minutes, 20 stamina points
- **Crops:** Mandioca (2d / R$3 seed), Cacau (4d / R$6), Açaí (6d / R$10)
- **Daily fixed cost:** R$2 deducted at day-end
- **Market:** set your asking price per unit; buyer accepts if price ≤ their hidden max
- **Win:** saldo ≥ R$0 on Day 15. Lose: saldo < R$0

---

## Team

| Member | Track |
|---|---|
| TBD | Core Systems (GameManager, DayNightCycle, SaveSystem) |
| TBD | Economy / UI (Pricing, Inventory, Market, HUD) |
| TBD | Art / Farm / World (Sprites, Tilemaps, CropSlot, Animations) |

---

## Docs

- [`Docs/GDD.pdf`](Docs/GDD.pdf) — full Game Design Document
- [`Docs/EDD.pdf`](Docs/EDD.pdf) — Educational Design Document (BNCC alignment)
