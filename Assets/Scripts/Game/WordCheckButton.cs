using UnityEngine;
using UnityEngine.UI;

public class WordCheckButton : MonoBehaviour
{
    public SlotPanel slotPanel;
    public SlotPanelController slotPanelController;
    public Text feedbackText; // optional

    private void Reset()
    {
        if (slotPanelController == null) slotPanelController = FindObjectOfType<SlotPanelController>();
    }

    public void OnCheckPressed()
    {
        print("Checking");

        if (slotPanelController == null)
        {
            Debug.LogWarning("SlotPanel not set.");
            return;
        }

        string word = slotPanelController.GetCurrentWord();
        if (string.IsNullOrEmpty(word))
        {
            Show("Type/select letters first.", Color.gray);
            return;
        }

        bool valid = WordChecker.IsWord(word);
        if (valid)
        {
            Show($"{word} is valid!", new Color(0.15f, 0.6f, 0.2f));
            slotPanelController.ClearAll();
        }
        else
        {
            Show($"{word} not found.", new Color(0.8f, 0.2f, 0.2f));
        }
    }

    private void Show(string msg, Color c)
    {
        if (feedbackText != null)
        {
            feedbackText.text = msg;
            feedbackText.color = c;
        }
        else
        {
            Debug.Log(msg);
        }
    }
}
