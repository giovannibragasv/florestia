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
            slotBackgrounds[i].color = (i == (int)Selected)
                ? selectedColor
                : normalColor;
        }
    }

    void StyleToolbarFrame()
    {
        if (slotBackgrounds == null || slotBackgrounds.Length == 0 || slotBackgrounds[0] == null) return;
        var frame = slotBackgrounds[0].transform.parent;
        if (frame != null && frame.TryGetComponent(out Image image))
            image.color = new Color(0.60f, 0.30f, 0.08f, 0.96f);
    }
}
