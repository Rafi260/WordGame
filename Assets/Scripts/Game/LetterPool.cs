using UnityEngine;

public enum LetterTier { Common = 0, Medium = 1, Rare = 2 }

public struct LetterPick
{
    public LetterTier Tier;
    public char Letter;
    public LetterPick(LetterTier tier, char letter) { Tier = tier; Letter = letter; }
}

public static class LetterPool
{
    // --- TIER DEFINITIONS ---
    private static readonly char[] COMMON_LETTERS = "AEIOUASRTNLECD".ToCharArray();
    private static readonly char[] MEDIUM_LETTERS = "PMHGBFYKVW".ToCharArray();
    private static readonly char[] RARE_LETTERS = "JXQZ".ToCharArray();

    private static readonly float[] COMMON_WEIGHTS = null;
    private static readonly float[] MEDIUM_WEIGHTS = null;
    private static readonly float[] RARE_WEIGHTS = null;

    private static float pCommon = 0.68f;
    private static float pMedium = 0.26f;
    private static float pRare = 0.06f;

    public static void SetTierProbabilities(float common, float medium, float rare)
    {
        float sum = Mathf.Max(0.0001f, common + medium + rare);
        pCommon = common / sum;
        pMedium = medium / sum;
        pRare = rare / sum;
    }

    public static LetterPick GetWeightedPick()
    {
        float r = Random.value;
        LetterTier tier;
        if (r < pCommon) tier = LetterTier.Common;
        else if (r < pCommon + pMedium) tier = LetterTier.Medium;
        else tier = LetterTier.Rare;

        char letter = PickLetterInTier(tier);
        return new LetterPick(tier, letter);
    }

    public static char GetLetter(LetterTier tier) => PickLetterInTier(tier);

    private static char PickLetterInTier(LetterTier tier)
    {
        switch (tier)
        {
            case LetterTier.Common: return WeightedPick(COMMON_LETTERS, COMMON_WEIGHTS);
            case LetterTier.Medium: return WeightedPick(MEDIUM_LETTERS, MEDIUM_WEIGHTS);
            case LetterTier.Rare: return WeightedPick(RARE_LETTERS, RARE_WEIGHTS);
            default: return 'A';
        }
    }

    private static char WeightedPick(char[] letters, float[] weightsOrNull)
    {
        if (letters == null || letters.Length == 0) return 'A';

        if (weightsOrNull == null || weightsOrNull.Length != letters.Length)
        {
            int idx = Random.Range(0, letters.Length);
            return letters[idx];
        }

        float sum = 0f;
        for (int i = 0; i < weightsOrNull.Length; i++) sum += Mathf.Max(0f, weightsOrNull[i]);
        if (sum <= 0f) return letters[Random.Range(0, letters.Length)];

        float r = Random.value * sum;
        float acc = 0f;
        for (int i = 0; i < weightsOrNull.Length; i++)
        {
            acc += Mathf.Max(0f, weightsOrNull[i]);
            if (r <= acc) return letters[i];
        }
        return letters[letters.Length - 1];
    }

    /// <summary>
    /// Returns which tier the given letter belongs to (Common, Medium, Rare).
    /// If not found, defaults to Rare.
    /// </summary>
    public static LetterTier GetTierForLetter(char c)
    {
        c = char.ToUpperInvariant(c);

        if (System.Array.IndexOf(COMMON_LETTERS, c) >= 0)
            return LetterTier.Common;
        if (System.Array.IndexOf(MEDIUM_LETTERS, c) >= 0)
            return LetterTier.Medium;
        if (System.Array.IndexOf(RARE_LETTERS, c) >= 0)
            return LetterTier.Rare;

        // fallback
        return LetterTier.Rare;
    }
}
