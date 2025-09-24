using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class BallLetter : MonoBehaviour
{
    public char Letter { get; private set; } = 'A';
    [Header("Visual")]
    public TMP_Text letterText;
    public SpriteRenderer ballImage;

    [Header("Refs")]
    public SlotPanel slotPanel; // assign at runtime if you like
    public SlotPanelController _slots;
    public JarController jar;   // optional reference to the jar root

    public void SetLetter(char c)
    {
        Letter = char.ToUpperInvariant(c);
        if (letterText) letterText.text = Letter.ToString();
        name = $"Ball_{Letter}";
    }




    private void Start()
    {
        if (slotPanel == null)
        {
            slotPanel = FindObjectOfType<SlotPanel>();
            _slots = FindObjectOfType<SlotPanelController>();
        }
        /*if (jar == null)
        {
            jar = FindObjectOfType<JarController>();
        }*/
    }

    private void OnMouseDown()
    {
        // Pick from jar: try to place into slots; if success, destroy this ball.
        //if (slotPanel != null && slotPanel.TryAddLetter(Letter))
        if (_slots != null && _slots.TryPlaceLetter(Letter))
        {
            Destroy(gameObject);
        }
        else
        {
            // optional feedback
            // Debug.Log("No empty slot.");
        }
    }

    public void SetTierVisual(LetterTier tier)
    {
        if (ballImage == null) return;

        float scale = 1f;

        switch (tier)
        {
            case LetterTier.Common:
                ballImage.color = new Color(0.80f, 1.00f, 0.80f); // light green
                scale = 0.5f;
                break;

            case LetterTier.Medium:
                ballImage.color = new Color(1.00f, 0.95f, 0.75f); // light yellow
                scale = 0.75f;
                break;

            case LetterTier.Rare:
                ballImage.color = new Color(1.00f, 0.80f, 0.80f); // light red
                scale = 1.0f;
                break;
        }

        // Apply scale to the entire ball object
        transform.localScale = new Vector3(scale, scale, 1f);
    }

}
