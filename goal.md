You are autonomously completing Florestia, a Stardew Valley-style educational farming game built in Unity 6 (6000.4.5f1) with C#. The
project is at /Users/giovannivasconcelos/Documents/uni/CC7NA/florestia.

THE VISUAL TARGET IS STARDEW VALLEY. Every artistic decision — tile layout, sprite style, color palette, UI proportions — must aim to
look as close to Stardew Valley as possible. Warm earth tones, pixel art 2D top-down, lush greens, chunky readable UI. When in doubt,
look at a Stardew Valley screenshot in your mind and match it.

You have access to image generation. Use it to create any missing pixel art sprites and save them to the correct paths under
Assets/Sprites/. All sprites must be pixel art style, top-down perspective, Stardew Valley aesthetic, warm Amazonian palette (deep
greens, ochre, terracotta, gold). Generate sprites for: player character (4-direction walk, 32x48px each), crop growth stages if
placeholders exist, buyer portraits, house, bridge. Save as PNG files at the exact paths listed below.

COMMIT RULES: Commit every completed task using conventional commits (feat/fix/refactor). NEVER push. Stage only specific changed
files,
trigger for market transition.

---

## WHAT ALREADY EXISTS — READ EVERY FILE BEFORE TOUCHING IT

### Assets/Scripts/Core/GameManager.cs

Singleton, DontDestroyOnLoad. StartingBalance=50, DailyCost=2, TotalDays=15.
AdvanceDay(): increments day, calls ApplyDailyFixedCost(), saves, loads FarmScene or EndScreen.
GoToMarket(): loads MarketScene.
IsGameOver: BUGGY — only checks day > TotalDays, does not check balance < 0.
BuildSaveData(): BUGGY — calls EndScreenController.Instance.GetBalanceHistory() which will be null because EndScreenController is
DontDestroyOnLoad from EndScreen scene and may not exist yet.
AddRevenue(float), SpendBalance(float), CanAfford(float) all exist.

### Assets/Scripts/Core/DayNightCycle.cs

Singleton. [SerializeField] Image skyOverlay, GameObject nightWarningPanel. float warningThreshold=60.
5-minute timer. At TimeRemaining<=60: shows nightWarningPanel. At elapsed>=300: pauses, calls GoToMarket().
UpdateSkyColor(): simple alpha lerp on skyOverlay from 0 to 0.6 — ONE COLOR ONLY. Needs 3-stop gradient.
Public: Pause(), Resume(), ResetDay(), TimeRemaining, NormalizedTime.
MISSING: public bool IsNight property.

### Assets/Scripts/Core/SaveSystem.cs

Static. Save(SaveData), Load(), Delete(). SaveData: day, balance, inventory, cropSlots, dailyBalanceHistory float[].
InventorySaveData: mandiocaCount, cacauCount, acaiCount. CropSlotSaveData: slotIndex, cropType, daysPlanted, isWatered.

### Assets/Scripts/Farm/CropSlot.cs

Two SpriteRenderers (soil layer + crop layer child "CropSprite"). BoxCollider2D isTrigger=true.
SerializedFields: soilSprite, soilWateredSprite. TryPlant(), TryWater(), TryHarvest(), OnDayEnd(), Interact().
Interact() dispatches on ToolbarController.Selected → calls CropSystem.GetCropData(ToolType).

### Assets/Scripts/Farm/CropSystem.cs

Singleton. 36 slots array, cropCatalog[3]. GetCropData(ToolType tool) with name fallback.
OnDayEnd() broadcasts to all slots. GetSaveData(), LoadSaveData().

### Assets/Scripts/Farm/StaminaSystem.cs

Singleton. TrySpend(int), ResetForNewDay(), Current, MaxStamina=20.

### Assets/Scripts/Farm/ToolType.cs

enum: Mandioca=0, Cacau=1, Acai=2, Water=3, Harvest=4.

### Assets/Scripts/Economy/InventorySystem.cs

Singleton. AddCrop(string, int), TryRemoveCrop(string, int), GetCount(string), GetAll(), GetSaveData(), LoadSaveData().

### Assets/Scripts/Economy/PricingSystem.cs

Singleton. Default asking prices: Mandioca=7, Cacau=16, Acai=28. Seed costs: Mandioca=3, Cacau=6, Acai=10.
GetAskingPrice(), SetAskingPrice(), GetSeedCost(), GetMarginValue(), GetMarginPercent().

### Assets/Scripts/Market/BuyerData.cs

ScriptableObject [CreateAssetMenu]. Fields: buyerName (string), portrait (Sprite), maxPriceMandioca, maxPriceCacau, maxPriceAcai,
rejectLine, acceptLine (all public). GetMaxPrice(string cropName).

### Assets/Scripts/Market/BuyerSystem.cs

Singleton. [SerializeField] BuyerData[] buyers — CURRENTLY EMPTY, no assets exist yet.
TrySell(buyer, cropName, qty, askingPrice): checks max price, removes from inventory, calls AddRevenue.
BUG: does NOT call EndScreenController.RecordSale().

### Assets/Scripts/Market/BuyerSelector.cs

[SerializeField] MarketUIController marketUI, BuyerData[] buyers, Button[] buyerButtons.
Wires each button to marketUI.OnBuyerSelected(buyers[i]). buyers[] EMPTY until assets created.

### Assets/Scripts/Market/MarketUIController.cs

[SerializeField] TMP_Dropdown cropDropdown, TMP_Text stockLabel, Slider priceSlider, TMP_Text costLabel/priceLabel/marginLabel,
TMP_Text buyerDialogueLine, Image buyerPortrait, Button sellButton/endDayButton, BuyerSelector buyerSelector.
Start(): returns early if cropDropdown==null (FarmScene compat). Wires all listeners.
OnCropChanged(): sets \_selectedCrop, refreshes stock/price display.
RefreshPriceDisplay(): updates costLabel/priceLabel/marginLabel in real time.
OnBuyerSelected(BuyerData): sets \_selectedBuyer, portrait, clears dialogue.
OnSellClicked(): calls BuyerSystem.TrySell qty=1. Shows accept/reject line.
OnEndDayClicked(): calls CropSystem.OnDayEnd, StaminaSystem.ResetForNewDay, EndScreenController.RecordDayBalance (STALE — this method
is being removed), GameManager.AdvanceDay().

### Assets/Scripts/UI/EndScreenController.cs

Singleton DontDestroyOnLoad. \_balanceHistory List<float>, \_revenuePerCrop Dict<string,float>.
RecordDayBalance(float) — BEING REMOVED. RecordSale(string, float) — exists but NEVER CALLED.
BuildEndScreen(): populates labels, calls BuildBalanceChart(). GetBalanceHistory() returns float[].
BuildBalanceChart(): instantiates barPrefab under chartContainer, sizes bars proportionally.
[SerializeField]: finalBalanceLabel, outcomeLabel, bestCropLabel, chartContainer (RectTransform), barPrefab (GameObject),
playAgainButton.

### Assets/Scripts/UI/HUDController.cs

Singleton. RefreshBalance(float), RefreshDay(int), RefreshStamina(int, int). timerLabel updated in Update().

### Assets/Scripts/UI/ToolbarController.cs

Singleton. [SerializeField] Image[] slotBackgrounds (5). Keys 1-5 map to ToolType. RefreshHighlight().

### Assets/Scripts/Player/PlayerController.cs

Singleton. Rigidbody2D gravityScale=0. WASD movement, camera follow LateUpdate (lerp factor 6).
FacingDirection 0=up/1=right/2=down/3=left. FacingOffset[] = {up, right, down, left}.
E key → OverlapCircleAll at facing pos (range 1.0, radius 0.45) → FindSlotNear → slot.Interact().
[SerializeField] SpriteRenderer tileHighlight (yellow semi-transparent child).
UpdateFacing(): prioritises horizontal axis, flips \_sr.flipX for left movement.

### Assets/Editor/ (existing editor scripts — do not rewrite these, add new ones)

FarmGridGenerator.cs — "Florestia/Generate 6x6 Farm Grid"
TilemapBuilder.cs — "Florestia/Build Grass Tilemap" (WorldGrid with Grid cellSize=1.1, Ground tilemap sortOrder=-1, grass from -4,-4 to
9,9)
ToolbarBuilder.cs — "Florestia/Build Farm Toolbar"
HUDBuilder.cs — "Florestia/Build Farm HUD" (balance+day+stamina top-left, timer top-right)
PlayerBuilder.cs — "Florestia/Build Player" (Player at 2.75,-0.8 with TileHighlight child)
MarketSceneBuilder.cs — "Florestia/Build Market Scene UI"
EndScreenBuilder.cs — "Florestia/Build End Screen UI"

---

## GDD VALUES — EXACT, DO NOT CHANGE

Crops:
Mandioca: growthDays=2, seedCost=3, baseMarketValue=7
Cacau: growthDays=4, seedCost=6, baseMarketValue=16
Açaí: growthDays=6, seedCost=10, baseMarketValue=28

Buyers (hidden max prices — player discovers by trial/error):
Atravessador: Mandioca=5, Cacau=12, Açaí=20 (high volume, high frequency)
Feirante Local: Mandioca=7, Cacau=15, Açaí=26 (medium volume, medium frequency)
Comprador Direto: Mandioca=9, Cacau=18, Açaí=30 (low volume=2 units, low frequency)

Economy: StartingBalance=50, DailyCost=2/day, TotalDays=15
Win: balance > 0 after day 15. Lose: balance < 0 at any point.

---

## SPRITE PATHS — GENERATE ALL MISSING ONES WITH IMAGEGEN

Generate pixel art sprites (Stardew Valley style, warm Amazonian palette, top-down 2D) for:

Assets/Sprites/Player/player_walk_down_0.png (32x48 — player facing down, frame 0)
Assets/Sprites/Player/player_walk_down_1.png (32x48 — player facing down, frame 1, mid-step)
Assets/Sprites/Player/player_walk_up_0.png (32x48 — player facing away)
Assets/Sprites/Player/player_walk_side_0.png (32x48 — player facing right)
Assets/Sprites/Buyers/buyer_atravessador.png (48x64 — shady market middleman, dark vest)
Assets/Sprites/Buyers/buyer_feirante.png (48x64 — friendly market vendor, apron)
Assets/Sprites/Buyers/buyer_comprador.png (48x64 — well-dressed direct buyer)
Assets/Sprites/UI/ui_house.png (80x96 — small Amazonian farmhouse, thatched roof)
Assets/Sprites/Terrain/terrain_bridge.png (32x32 — wooden bridge plank tile)

Crop sprites (if placeholders are missing or zero-byte):
Assets/Sprites/Crops/Mandioca/crop_mandioca_00.png through crop_mandioca_02.png (32x32 each, 3 growth stages)
Assets/Sprites/Crops/Cacau/crop_cacau_00.png through crop_cacau_04.png (32x32 each, 5 stages)
Assets/Sprites/Crops/Acai/crop_acai_00.png through crop_acai_06.png (32x32 each, 7 stages)

After generating, set each file's Unity import settings via .meta or SerializedObject to:
TextureType = Sprite (2D and UI), SpriteMode = Single, FilterMode = Point (no blur), PPU = 32.

---

## TASKS — IMPLEMENT ALL IN ORDER

### TASK 1 — Fix GameManager: defeat condition + balance history

Edit Assets/Scripts/Core/GameManager.cs:

1. Add property: `public bool IsLoss { get; private set; }`
2. Add property: `public List<float> DailyBalances { get; private set; } = new();`
3. Change IsGameOver: `public bool IsGameOver => CurrentDay > TotalDays || IsLoss;`
4. In AdvanceDay(), after ApplyDailyFixedCost():
   - DailyBalances.Add(Balance);
   - if (Balance < 0) { IsLoss = true; SceneManager.LoadScene("EndScreen"); return; }
5. Fix BuildSaveData() to use DailyBalances.ToArray() instead of calling EndScreenController.
6. In ApplySave(): restore DailyBalances from data.dailyBalanceHistory if not null.

Commit: fix(core): defeat on negative balance, move balance history to GameManager

### TASK 2 — Fix EndScreenController: use GameManager data, add educational message

Edit Assets/Scripts/UI/EndScreenController.cs:

1. Remove \_balanceHistory list and RecordDayBalance() method.
2. Add [SerializeField] TMP_Text educationalLabel.
3. In BuildEndScreen(), replace \_balanceHistory with GameManager.Instance.DailyBalances.
4. outcomeLabel logic:
   - IsLoss → "Falência! O saldo chegou a zero." (color red)
   - Balance > 50 → "Você lucrou! Ótima gestão." (color green)
   - else → "Você sobreviveu! Dá pra melhorar." (color yellow)
5. educationalLabel logic:
   - IsLoss → "Dica: venda sempre acima do custo da semente para não ter prejuízo."
   - Balance > 50 → "Você dominou a precificação! Margem = Preço − Custo. É assim na roça de verdade."
   - else → "Dica: Açaí tem margem de R$18 por unidade. Vale imobilizar capital por 6 dias?"

Commit: fix(ui): EndScreen reads GameManager balance history, adds educational outcome message

### TASK 3 — Wire RecordSale in BuyerSystem

Edit Assets/Scripts/Market/BuyerSystem.cs:

After `GameManager.Instance.AddRevenue(revenue);` in TrySell(), add:
`EndScreenController.Instance?.RecordSale(cropName, revenue);`

Commit: fix(market): wire sale revenue tracking to EndScreenController

### TASK 4 — Fix MarketUIController.OnEndDayClicked

Edit Assets/Scripts/Market/MarketUIController.cs:

Replace OnEndDayClicked with:
void OnEndDayClicked()
{
CropSystem.Instance.OnDayEnd();
StaminaSystem.Instance.ResetForNewDay();
DayNightCycle.Instance?.ResetDay();
GameManager.Instance.AdvanceDay();
}

(Remove stale RecordDayBalance call. Add DayNightCycle.ResetDay so timer restarts next morning.)

Commit: fix(market): remove stale RecordDayBalance, add DayNightCycle.ResetDay on end day

### TASK 5 — Create BuyerData ScriptableObject assets

Create Assets/Editor/BuyerDataCreator.cs:

[MenuItem("Florestia/Create Buyer Data Assets")]
Creates three BuyerData assets at Assets/Data/Buyers/ using AssetDatabase.CreateAsset.
After creating, wire them into BuyerSystem.buyers[] and BuyerSelector.buyers[] via SerializedObject on the found scene objects.

EXACT VALUES:
Atravessador.asset: buyerName="Atravessador", maxPriceMandioca=5f, maxPriceCacau=12f, maxPriceAcai=20f
acceptLine="Feito! Vendo logo.", rejectLine="Tá caro demais, amigo."

Feirante.asset: buyerName="Feirante Local", maxPriceMandioca=7f, maxPriceCacau=15f, maxPriceAcai=26f
acceptLine="Trato feito! Boa mercadoria.", rejectLine="Não tenho esse dinheiro não."

CompradorDireto.asset: buyerName="Comprador Direto", maxPriceMandioca=9f, maxPriceCacau=18f, maxPriceAcai=30f
acceptLine="Pode deixar, pago bem por qualidade.", rejectLine="Prefiro buscar em outro lugar."

Commit: feat(market): create 3 BuyerData ScriptableObject assets with GDD max prices

### TASK 6 — Bridge trigger

Create Assets/Scripts/Farm/BridgeTrigger.cs:
MonoBehaviour. OnTriggerEnter2D: if other.GetComponent<PlayerController>() != null → DayNightCycle.Instance?.Pause();
GameManager.Instance.GoToMarket();

Create Assets/Editor/BridgeBuilder.cs:
[MenuItem("Florestia/Build Bridge")]
Creates "Bridge" GameObject at position (2.75f, -1.65f, 0f).
Loads terrain_bridge.png sprite from Assets/Sprites/Terrain/terrain_bridge.png (or terrain_grass.png if missing).
SpriteRenderer: sortingOrder=2. Scale (2.2f, 1f, 1f).
BoxCollider2D: isTrigger=true, size (2.2f, 0.6f).
Adds BridgeTrigger. Undo.RegisterCreatedObjectUndo.

Commit: feat(farm): bridge trigger lets player walk to market at any time

### TASK 7 — IsNight on DayNightCycle + 3-stop sky gradient

Edit Assets/Scripts/Core/DayNightCycle.cs:

1. Add: `public bool IsNight => TimeRemaining <= warningThreshold;`
2. Replace UpdateSkyColor() with a 3-stop gradient (Stardew Valley sky feel):
   - 0% → 60%: morning blue to midday (alpha lerp overlay stays minimal, sky tint cool blue → warm)
   - 60% → 90%: golden hour (overlay tints orange-ish)
   - 90% → 100%: dusk to night (deep purple)
     Use a Color[] stops array and Mathf.InverseLerp to interpolate between stops:
     Color morning = new Color(0.53f, 0.81f, 0.98f, 0f); // clear sky, no overlay
     Color golden = new Color(0.98f, 0.72f, 0.3f, 0.25f); // golden hour tint
     Color dusk = new Color(0.15f, 0.05f, 0.3f, 0.6f); // deep purple night
     At NormalizedTime 0→0.6: lerp morning→golden. At 0.6→1.0: lerp golden→dusk.
     Apply to skyOverlay.color.

Commit: feat(core): 3-stop sky gradient (morning/golden/night) + IsNight property

### TASK 8 — Camera confiner

Create Assets/Scripts/Core/CameraConfiner.cs:
[RequireComponent(typeof(Camera))]
SerializedFields: Vector2 minBounds=(-1f,-2.5f), maxBounds=(7f,7.5f).
LateUpdate: clamp position using halfH=orthographicSize, halfW=halfH\*aspect.

Create Assets/Editor/CameraConfineryBuilder.cs:
[MenuItem("Florestia/Add Camera Confiner")]
Finds Camera.main in scene, adds CameraConfiner component if not present. Marks scene dirty.

Commit: feat(core): CameraConfiner keeps camera within world bounds

### TASK 9 — House decoration in FarmScene

Create Assets/Scripts/Farm/HouseObstacle.cs:
Empty MonoBehaviour marker (used to identify house in scene).

Create Assets/Editor/HouseBuilder.cs:
[MenuItem("Florestia/Build Farm House")]
Creates "House" at position (-0.55f, 6.6f, 0f) — NW corner above the 6x6 grid.
Loads ui_house.png from Assets/Sprites/UI/ui_house.png.
SpriteRenderer: sortingOrder=3, scale (2.2f, 2.2f, 1f).
BoxCollider2D: isTrigger=false, size (1.8f, 1.8f) — solid obstacle, player cannot walk through.
Adds HouseObstacle component.

Commit: feat(farm): house decoration with solid collider in FarmScene NW corner

### TASK 10 — Font assigner

Create Assets/Editor/FontAssigner.cs:
[MenuItem("Florestia/Assign TMP Fonts")]
Loads font asset from "Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset".
Finds all TextMeshProUGUI and TextMeshPro components in current open scene.
For each with null or missing font: assigns the loaded font. Marks objects dirty.
Debug.Log count updated.

Commit: feat(editor): batch TMP font assigner for all labels in scene

### TASK 11 — Player walk animation (sprite-swap based)

Edit Assets/Scripts/Player/PlayerController.cs:

Add [SerializeField] Sprite[] walkDown, walkUp, walkSide (2 frames each, set in Inspector).
Add float \_animTimer, int \_animFrame.
In Update() when \_input != Vector2.zero:
\_animTimer += Time.deltaTime;
if (\_animTimer >= 0.2f) { \_animTimer = 0; \_animFrame = (\_animFrame + 1) % 2; }
Sprite[] frames = FacingDirection == 0 ? walkUp : FacingDirection == 2 ? walkDown : walkSide;
if (frames != null && frames.Length > \_animFrame && frames[_animFrame] != null)
\_sr.sprite = frames[_animFrame];
When \_input == Vector2.zero: reset to frame 0.

Commit: feat(player): sprite-swap walk animation with 2-frame cycle per direction

### TASK 12 — HUD wires DayNightCycle timer and defeat detection

Edit Assets/Scripts/UI/HUDController.cs:

In Update(), after setting timerLabel: check GameManager.Instance.Balance. If balance < 20 (danger zone, less than 10 days of costs
remaining): set balanceLabel.color red, else white. This gives the player a visual warning before hitting zero.

Also add: if timerLabel != null and DayNightCycle.Instance.IsNight: set timerLabel.color = red (urgency signal that market time is
now).

Commit: feat(ui): HUD balance danger color warning and red timer on IsNight

### TASK 13 — Stardew-style tile highlight pulse

Edit Assets/Scripts/Player/PlayerController.cs:

    float alpha = 0.3f + 0.15f * Mathf.Sin(Time.time * 4f);
    Color c = tileHighlight.color; c.a = alpha; tileHighlight.color = c;

Commit: feat(player): pulse tile highlight for Stardew-style interaction feedback

---

## CONSTRAINTS

- Never rewrite a complete working script — surgical edits only
- Never push to remote
- Use Object.FindFirstObjectByType<T>() (Unity 6 API), not FindObjectOfType<T>()
- No comments unless the WHY is non-obvious
- SerializedFields: camelCase with [SerializeField]
- No new scenes — only C# scripts and Editor scripts
- After each task: commit with correct conventional commit message before starting next task
- If a file doesn't compile, fix it before committing

## DONE WHEN

- GameManager correctly detects balance < 0 as loss
- Three BuyerData assets exist at Assets/Data/Buyers/ with correct GDD prices
- BuyerSystem.TrySell calls EndScreenController.RecordSale on success
- BridgeTrigger.cs and BridgeBuilder.cs exist
- CameraConfiner.cs and its builder exist
- DayNightCycle has IsNight property and 3-stop gradient
- HouseBuilder.cs exists
- FontAssigner.cs exists
- PlayerController has sprite-swap animation
- MarketUIController.OnEndDayClicked is clean (no stale calls)
- Zero compile errors across all modified scripts
- Every task has its own conventional commit

YOU ARE ALLOWED TO EXECUTE ANY AND EVERY SINGLE COMMAND YOU WANT WITHOUT ASKING PERSMISSION FOR THIS PROJECT AND THISPROJECT ONLY
