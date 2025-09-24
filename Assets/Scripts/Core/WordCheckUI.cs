using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WordCheckUI : MonoBehaviour
{
    [Header("UI References")]
    public TMP_InputField wordInput;
    public Button checkButton;
    public TMP_Text resultText;

    private void OnEnable()
    {
        if (checkButton != null)
            checkButton.onClick.AddListener(OnCheckClicked);
    }

    private void OnDisable()
    {
        if (checkButton != null)
            checkButton.onClick.RemoveListener(OnCheckClicked);
    }

    private void Start()
    {
        if (!DictionaryTrieLoader.IsLoaded)
        {
            // Subscribe so we can enable UI when ready (optional for small file).
            DictionaryTrieLoader.OnLoaded += HandleLoaded;
            SetResult("Loading dictionary...", dim: true);
            checkButton.interactable = false;
        }
        else
        {
            SetResult("Dictionary ready. Try a word.");
        }
    }

    private void HandleLoaded()
    {
        SetResult("Dictionary ready. Try a word.");
        checkButton.interactable = true;
        DictionaryTrieLoader.OnLoaded -= HandleLoaded;
    }

    public void OnCheckClicked()
    {
        var input = (wordInput != null ? wordInput.text : "").Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(input))
        {
            SetResult("Type a word first.", warn: true);
            return;
        }

        if (!DictionaryTrieLoader.IsLoaded || DictionaryTrieLoader.TrieInstance == null)
        {
            SetResult("Dictionary not loaded yet.", warn: true);
            return;
        }

        bool ok = DictionaryTrieLoader.TrieInstance.IsWord(input);
        if (ok)
        {
            SetResult($" \"{input}\" exists!", success: true);
        }
        else
        {
            SetResult($" \"{input}\" not found.", warn: true);
        }
    }

    private void SetResult(string msg, bool success = false, bool warn = false, bool dim = false)
    {
        if (resultText == null) return;
        resultText.text = msg;

        // Nice little color cues
        if (success) resultText.color = new Color(0.15f, 0.6f, 0.2f);   // greenish
        else if (warn) resultText.color = new Color(0.8f, 0.2f, 0.2f);    // reddish
        else if (dim) resultText.color = new Color(0.6f, 0.6f, 0.6f);    // gray
        else resultText.color = Color.white;
    }
}
