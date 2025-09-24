using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class DictionaryTrieLoader : MonoBehaviour
{
    [Tooltip("Dictionary text file (one word per line) located in StreamingAssets.")]
    public string dictionaryFileName = "words_en_50k.txt"; // or your test file

    public static Trie TrieInstance { get; private set; }
    public static bool IsLoaded { get; private set; }
    public static event Action OnLoaded;

    private void Awake() => DontDestroyOnLoad(gameObject);

    private void Start()
    {
        if (!IsLoaded) StartCoroutine(LoadDictionaryCoroutine());
    }

    private IEnumerator LoadDictionaryCoroutine()
    {
        TrieInstance = new Trie();
        string path = Path.Combine(Application.streamingAssetsPath, dictionaryFileName);

        if (path.Contains("://") || path.Contains("jar:"))
        {
            using var www = UnityWebRequest.Get(path);
            yield return www.SendWebRequest();
#if UNITY_2020_2_OR_NEWER
            if (www.result != UnityWebRequest.Result.Success)
#else
            if (www.isNetworkError || www.isHttpError)
#endif
            {
                Debug.LogError("Failed to load dictionary: " + www.error);
                yield break;
            }
            AddLinesToTrie(www.downloadHandler.text);
        }
        else
        {
            if (!File.Exists(path))
            {
                Debug.LogError("Dictionary file not found at: " + path);
                yield break;
            }
            AddLinesToTrie(File.ReadAllText(path));
        }

        IsLoaded = true;
        OnLoaded?.Invoke();
        Debug.Log("Dictionary loaded. Trie ready.");
    }

    private void AddLinesToTrie(string fileText)
    {
        using var reader = new StringReader(fileText);
        string line;
        int count = 0;
        while ((line = reader.ReadLine()) != null)
        {
            var w = line.Trim().ToLowerInvariant();
            if (w.Length > 0) { TrieInstance.Insert(w); count++; }
        }
        Debug.Log($"Loaded {count} words.");
    }
}
