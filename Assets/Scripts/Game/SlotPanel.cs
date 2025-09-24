using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class SlotPanel : MonoBehaviour
{
    [Tooltip("Exactly 8 SlotCell objects in order.")]
    public SlotCell[] slots = new SlotCell[8];

    [Header("UI (optional)")]
    public Text currentWordText;

    private void Start()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
            {
                slots[i].Init(this, i);
            }
        }
        UpdateWordLabel();
    }

    public bool TryAddLetter(char letter)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].HasLetter)
            {
                slots[i].SetLetter(letter);
                UpdateWordLabel();
                return true;
            }
        }
        return false; // no empty slot
    }

    public string GetCurrentWord()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].HasLetter) sb.Append(slots[i].Letter);
        }
        return sb.ToString().ToLowerInvariant();
    }

    public void ClearFilledIfValid(bool isValid)
    {
        if (!isValid) return;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].HasLetter) slots[i].Clear();
        }
        UpdateWordLabel();
    }

    public void UpdateWordLabel()
    {
        if (currentWordText != null)
        {
            var w = GetCurrentWord();
            currentWordText.text = string.IsNullOrEmpty(w) ? "(empty)" : w;
        }
    }

    // Swap letters between two slots (for drag reorder)
    public void SwapSlots(int indexA, int indexB)
    {
        if (indexA == indexB) return;
        var a = slots[indexA];
        var b = slots[indexB];

        bool aHas = a.HasLetter;
        bool bHas = b.HasLetter;
        char aCh = aHas ? a.Letter : '\0';
        char bCh = bHas ? b.Letter : '\0';

        if (aHas) a.Clear();
        if (bHas) b.Clear();

        if (bHas) a.SetLetter(bCh);
        if (aHas) b.SetLetter(aCh);

        UpdateWordLabel();
    }
}
