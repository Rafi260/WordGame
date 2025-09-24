using System.Collections.Generic;

public class TrieNode
{
    public bool IsWordEnd;
    public readonly Dictionary<char, TrieNode> Next = new Dictionary<char, TrieNode>();
}

public class Trie
{
    private readonly TrieNode _root = new TrieNode();

    public void Insert(string word)
    {
        if (string.IsNullOrWhiteSpace(word)) return;
        var node = _root;
        foreach (char raw in word)
        {
            char ch = char.ToLowerInvariant(raw);
            if (!node.Next.TryGetValue(ch, out var child))
            {
                child = new TrieNode();
                node.Next[ch] = child;
            }
            node = child;
        }
        node.IsWordEnd = true;
    }

    public bool IsWord(string s)
    {
        var node = FindNode(s);
        return node != null && node.IsWordEnd;
    }

    public bool IsPrefix(string s) => FindNode(s) != null;

    private TrieNode FindNode(string s)
    {
        if (string.IsNullOrEmpty(s)) return null;
        var node = _root;
        foreach (char raw in s)
        {
            char ch = char.ToLowerInvariant(raw);
            if (!node.Next.TryGetValue(ch, out node))
                return null;
        }
        return node;
    }
}
