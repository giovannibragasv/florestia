using UnityEngine;
using UnityEngine.UI;

public class ToolbarController : MonoBehaviour
{
    public static ToolbarController Instance { get; private set; }

    [SerializeField] Image[] slotBackgrounds; // 5 slots
    [SerializeField] Color normalColor  = new Color(0.96f, 0.68f, 0.34f, 0.96f);
    [SerializeField] Color selectedColor = new Color(0.98f, 0.84f, 0.42f, 1f);

    public ToolType Selected { get; private set; } = ToolType.Mandioca;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        StyleToolbarFrame();
        RefreshHighlight();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) Select(ToolType.Mandioca);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) Select(ToolType.Cacau);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) Select(ToolType.Acai);
        else if (Input.GetKeyDown(KeyCode.Alpha4)) Select(ToolType.Water);
        else if (Input.GetKeyDown(KeyCode.Alpha5)) Select(ToolType.Harvest);
    }

    public void Select(ToolType tool)
    {
        Selected = tool;
        RefreshHighlight();
        HUDController.Instance?.RefreshCropPreview(tool);
    }

    public void SelectMandioca() => Select(ToolType.Mandioca);
    public void SelectCacau() => Select(ToolType.Cacau);
    public void SelectAcai() => Select(ToolType.Acai);
    public void SelectWater() => Select(ToolType.Water);
    public void SelectHarvest() => Select(ToolType.Harvest);

    void RefreshHighlight()
    {
        if (slotBackgrounds == null) return;

        for (int i = 0; i < slotBackgrounds.Length; i++)
        {
            if (slotBackgrounds[i] == null) continue;
            bool selected = i == (int)Selected;
            if (slotBackgrounds[i].sprite != null)
            {
                slotBackgrounds[i].color = selected
                    ? new Color(1f, 0.92f, 0.46f, 1f)
                    : Color.white;
            }
            else
            {
                slotBackgrounds[i].color = selected ? selectedColor : normalColor;
            }
        }
    }

    void StyleToolbarFrame()
    {
        if (slotBackgrounds == null || slotBackgrounds.Length == 0 || slotBackgrounds[0] == null) return;
        var frame = slotBackgrounds[0].transform.parent;
        if (frame != null && frame.TryGetComponent(out Image image))
            image.color = new Color(0.60f, 0.30f, 0.08f, 0.96f);
        if (frame is RectTransform frameRT)
        {
            frameRT.sizeDelta = new Vector2(400f, 88f);
            frameRT.anchoredPosition = new Vector2(0f, 18f);
        }

        float startX = -2f * (72f + 6f);
        for (int i = 0; i < slotBackgrounds.Length; i++)
        {
            var slot = slotBackgrounds[i];
            if (slot == null) continue;
            slot.color = slot.sprite != null ? Color.white : normalColor;
            var rt = slot.rectTransform;
            rt.sizeDelta = new Vector2(72f, 72f);
            rt.anchoredPosition = new Vector2(startX + i * (72f + 6f), 0f);

            var label = slot.transform.Find("Label");
            if (label != null) label.gameObject.SetActive(false);

            var icon = slot.transform.Find("Icon") as RectTransform;
            if (icon != null)
            {
                icon.sizeDelta = new Vector2(48f, 48f);
                icon.anchoredPosition = new Vector2(0f, -2f);
            }
        }
    }
}
