using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotCell : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("Visuals")]
    public Image cardBg;
    public Text letterText;

    public bool HasLetter { get; private set; }
    public char Letter { get; private set; }

    private SlotPanel _panel;
    private int _index;

    // Drag visuals
    private Canvas _rootCanvas;
    private CanvasGroup _canvasGroup;
    private RectTransform _rect;
    private Transform _originalParent;
    private Vector2 _originalPos;

    public void Init(SlotPanel panel, int index)
    {
        _panel = panel;
        _index = index;
        _rect = GetComponent<RectTransform>();
        _canvasGroup = gameObject.GetComponent<CanvasGroup>();
        if (_canvasGroup == null) _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        _rootCanvas = GetComponentInParent<Canvas>();
        RefreshVisual();
    }

    public void SetLetter(char c)
    {
        HasLetter = true;
        Letter = char.ToUpperInvariant(c);
        RefreshVisual();
    }

    public void Clear()
    {
        HasLetter = false;
        Letter = '\0';
        RefreshVisual();
    }

    private void RefreshVisual()
    {
        if (letterText != null)
            letterText.text = HasLetter ? Letter.ToString() : "";
        if (cardBg != null)
            cardBg.color = HasLetter ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0.45f);
    }

    // --- Drag & Drop to reorder ---

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!HasLetter) return;

        _originalParent = _rect.parent;
        _originalPos = _rect.anchoredPosition;
        _canvasGroup.blocksRaycasts = false;

        // Move to top canvas layer so it follows the cursor
        _rect.SetParent(_rootCanvas.transform, worldPositionStays: true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!HasLetter) return;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rootCanvas.transform as RectTransform, eventData.position, eventData.pressEventCamera, out var localPoint);
        _rect.anchoredPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!HasLetter) return;
        _canvasGroup.blocksRaycasts = true;

        // If not dropped on a valid target, snap back
        _rect.SetParent(_originalParent, worldPositionStays: true);
        _rect.anchoredPosition = _originalPos;

        // If dropped onto JarDropZone, that handler will call RemoveToJar().
    }

    public void OnDrop(PointerEventData eventData)
    {
        // If another SlotCell was dragged onto this, swap contents
        var dragged = eventData.pointerDrag ? eventData.pointerDrag.GetComponent<SlotCell>() : null;
        if (dragged != null && dragged != this && _panel != null)
        {
            _panel.SwapSlots(dragged._index, _index);
        }
    }

    // Called by JarDropZone when the user releases this slot over the jar area
    public void RemoveToJar(JarController jar)
    {
        
        if (!HasLetter) return;
        char c = Letter;
        Clear();
        _panel.UpdateWordLabel();
        if (jar != null) jar.RespawnBall(c);
    }
}
