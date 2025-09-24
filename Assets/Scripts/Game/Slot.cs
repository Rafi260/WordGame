using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Slot : MonoBehaviour
{
    [Header("Refs (auto/wire)")]
    public TMP_Text letterText;      // assign LetterText
    public Image cardImage;      // assign Card image for tinting
    public CanvasGroup cardCg;   // on Card

    public int Index { get; private set; }
    public char? Letter { get; private set; }
    public bool IsEmpty => Letter == null;

    private SlotPanelController _panel;

    public void Setup(SlotPanelController panel, int index)
    {
        _panel = panel;
        Index = index;

        if (letterText == null)
            letterText.text = transform.Find("Card/LetterText").GetComponent<Text>().ToString();
        if (cardImage == null)
            cardImage = transform.Find("Card").GetComponent<Image>();
        if (cardCg == null)
            cardCg = transform.Find("Card").GetComponent<CanvasGroup>();

        Clear();
    }

    public void SetLetter(char c)
    {
        Letter = char.ToLowerInvariant(c);
        Refresh();
    }

    public void Clear()
    {
        Letter = null;
        Refresh();
    }

    public void Refresh()
    {
        if (Letter == null)
        {
            letterText.text = "";
            cardImage.color = new Color(1f, 1f, 1f, 0.35f); // ghosted
        }
        else
        {
            letterText.text = Letter.Value.ToString().ToUpperInvariant();
            cardImage.color = Color.white;
        }
    }
}
