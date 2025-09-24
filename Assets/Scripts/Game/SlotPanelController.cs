using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SlotPanelController : MonoBehaviour
{
    [Header("Setup")]
    public RectTransform slotsPanel;       // assign SlotsPanel
    public GameObject slotPrefab;          // assign Slot prefab
    public int slotCount = 8;
    public RectTransform dragLayer;        // assign DragLayer
    public JarDropZone jarZone;            // optional: assign if you have a jar area

    private readonly List<Slot> _slots = new List<Slot>();

    private void Awake()
    {
        if (slotsPanel == null) slotsPanel = (RectTransform)transform;
        BuildSlots();
    }

    private void BuildSlots()
    {
        // Clear existing children
        for (int i = slotsPanel.childCount - 1; i >= 0; i--)
            Destroy(slotsPanel.GetChild(i).gameObject);

        _slots.Clear();
        for (int i = 0; i < slotCount; i++)
        {
            var go = Instantiate(slotPrefab, slotsPanel);
            go.name = $"Slot_{i + 1}";
            var slot = go.GetComponent<Slot>();
            slot.Setup(this, i);
            // hook DraggableLetter with references it needs
            var drag = go.transform.Find("Card")?.GetComponent<DraggableLetter>();
            if (drag != null)
            {
                drag.Init(this, slot, dragLayer, jarZone);
            }
            _slots.Add(slot);
        }
    }

    public bool TryPlaceLetter(char letter)
    {
        foreach (var s in _slots)
        {
            if (s.IsEmpty)
            {
                s.SetLetter(letter);
                return true;
            }
        }
        return false; // no empty slot
    }

    public string GetCurrentWord()
    {
        var sb = new StringBuilder(_slots.Count);
        foreach (var s in _slots)
        {
            if (!s.IsEmpty) sb.Append(s.Letter);
        }
        return sb.ToString();
    }

    public void ClearAll()
    {
        foreach (var s in _slots) s.Clear();
    }

    public IReadOnlyList<Slot> Slots => _slots;

    // Called by drag logic to swap letters between two slots
    public void SwapSlots(Slot a, Slot b)
    {
        char? la = a.IsEmpty ? (char?)null : a.Letter.Value;
        char? lb = b.IsEmpty ? (char?)null : b.Letter.Value;

        if (la == null && lb == null) return;

        if (lb == null) a.Clear(); else a.SetLetter(lb.Value);
        if (la == null) b.Clear(); else b.SetLetter(la.Value);
    }
}
