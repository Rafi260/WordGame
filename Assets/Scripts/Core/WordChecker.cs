using UnityEngine;

public static class WordChecker
{
    public static bool IsWord(string candidate)
    {
        if (!DictionaryTrieLoader.IsLoaded || DictionaryTrieLoader.TrieInstance == null)
        {
            Debug.LogWarning("Dictionary not loaded yet.");
            return false;
        }
        return DictionaryTrieLoader.TrieInstance.IsWord(candidate.Trim().ToLowerInvariant());
    }
}
