using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DraggableLetter : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Optional visuals")]
    public float dragScale = 1.05f;

    private SlotPanelController _panel;
    private Slot _homeSlot;
    private RectTransform _dragLayer;
    private JarDropZone _jarZone;
    private GraphicRaycaster _raycaster;
    private EventSystem _eventSystem;

    private GameObject _ghost;           // runtime clone for the visual drag
    private RectTransform _ghostRt;
    private CanvasGroup _homeCg;         // CanvasGroup of the card (this)
    private Vector3 _startScale;

    public void Init(SlotPanelController panel, Slot homeSlot, RectTransform dragLayer, JarDropZone jarZone)
    {
        _panel = panel;
        _homeSlot = homeSlot;
        _dragLayer = dragLayer;
        _jarZone = jarZone;
    }

    private void Awake()
    {
        _homeCg = GetComponent<CanvasGroup>();
        _raycaster = GetComponentInParent<Canvas>().GetComponent<GraphicRaycaster>();
        _eventSystem = EventSystem.current;
        _startScale = transform.localScale;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {

        if (_homeSlot.IsEmpty) return; // nothing to drag

        // make ghost
        _ghost = Instantiate(gameObject, _dragLayer);
        _ghost.name = name + "_Ghost";
        _ghostRt = _ghost.GetComponent<RectTransform>();
        var ghostCg = _ghost.GetComponent<CanvasGroup>();
        if (ghostCg != null)
        {
            ghostCg.blocksRaycasts = false;
            ghostCg.interactable = false;
        }
        _ghostRt.position = eventData.position;
        _ghostRt.localScale = _startScale * dragScale;

        // fade original a bit to indicate drag
        if (_homeCg != null) _homeCg.alpha = 0.6f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_ghostRt == null) return;
        _ghostRt.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_ghost != null) Destroy(_ghost);
        if (_homeCg != null) _homeCg.alpha = 1f;

        if (_homeSlot.IsEmpty) return;

        // Raycast UI under pointer
        var results = new List<RaycastResult>();
        _raycaster.Raycast(eventData, results);

        Slot targetSlot = null;
        JarDropZone targetJar = null;

        foreach (var hit in results)
        {
            if (targetSlot == null) targetSlot = hit.gameObject.GetComponentInParent<Slot>();
            if (targetJar == null) targetJar = hit.gameObject.GetComponentInParent<JarDropZone>();
            if (targetSlot != null || targetJar != null) break;
        }

        if (targetSlot != null && targetSlot != _homeSlot)
        {
            // Swap letters
            _panel.SwapSlots(_homeSlot, targetSlot);
            return;
        }

        if (targetJar != null && _jarZone != null)
        {
            // Return letter to jar and clear slot
            if (_homeSlot.Letter.HasValue)
            {
                char c = _homeSlot.Letter.Value;
                _jarZone.ReceiveLetterFromSlot(c);
                _homeSlot.Clear();
            }
            return;
        }

        // Else: drop nowhere valid → do nothing (letter stays where it was)
    }
}
