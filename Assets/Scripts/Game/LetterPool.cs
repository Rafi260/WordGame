using Unity.VisualScripting;
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
    // You can tune these sets however you like.
    // Common: very frequent letters in English (incl. vowels, A, S, T, R, N, L, etc.)
    private static readonly char[] COMMON_LETTERS = "AEIOUASRTNLECD".ToCharArray();
    // Medium: still appear reasonably often
    private static readonly char[] MEDIUM_LETTERS = "PMHGBFYKVW".ToCharArray();
    // Rare: hardest to find
    private static readonly char[] RARE_LETTERS = "JXQZ".ToCharArray();

    // --- PER-TIER WEIGHTS (within the tier) ---
    // If you want *uniform inside each tier*, leave these null/empty and we’ll pick uniformly.
    // If you want to bias inside the tier (e.g., E more than U), add arrays of same length as the letter arrays.
    // Example: E and A heavier within COMMON
    private static readonly float[] COMMON_WEIGHTS = null; // e.g., new float[] {2,2,1.5f,1.3f,...}; // must match length
    private static readonly float[] MEDIUM_WEIGHTS = null; // uniform by default
    private static readonly float[] RARE_WEIGHTS = null; // uniform by default

    // --- TIER PROBABILITIES (global) ---
    // These define how often each tier is chosen overall (normalized at runtime).
    private static float pCommon = 0.68f; // ~68% of balls from Common
    private static float pMedium = 0.26f; // ~26%
    private static float pRare = 0.06f; // ~6%

    /// <summary>
    /// Globally adjust tier probabilities (optional). Values will be normalized internally.
    /// </summary>
    /// 
    
    public static void SetTierProbabilities(float common, float medium, float rare)
    {
        float sum = Mathf.Max(0.0001f, common + medium + rare);
        pCommon = common / sum;
        pMedium = medium / sum;
        pRare = rare / sum;
    }

    /// <summary>
    /// Pick a tier according to global tier probabilities, then pick a letter within that tier (weighted or uniform).
    /// </summary>
    public static LetterPick GetWeightedPick()
    {
        // Choose tier
        float r = Random.value;
        LetterTier tier;
        if (r < pCommon) tier = LetterTier.Common;
        else if (r < pCommon + pMedium) tier = LetterTier.Medium;
        else tier = LetterTier.Rare;

        // Choose letter within tier
        char letter = PickLetterInTier(tier);
        return new LetterPick(tier, letter);
    }

    /// <summary>
    /// Directly pick a letter from a specific tier.
    /// </summary>
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
            // Uniform
            int idx = Random.Range(0, letters.Length);
            return letters[idx];
        }

        // Weighted
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
}
