using System.Collections;
using UnityEngine;

public class BallSpawner2D : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject ballPrefabDefault;
    public GameObject ballPrefabCommon;
    public GameObject ballPrefabMedium;
    public GameObject ballPrefabRare;
    public bool useTierSpecificPrefabs = false;

    [Header("Tier Probabilities")]
    [Range(0f, 1f)] public float pCommon = 0.3f;
    [Range(0f, 1f)] public float pMedium = 0.35f;
    [Range(0f, 1f)] public float pRare = 0.35f;

    [Header("Timing")]
    public float spawnInterval = 1.0f;
    public int minBurst = 2;
    public int maxBurst = 3;
    public float intraBurstDelay = 0.05f;

    [Header("Spawn Area")]
    public float spawnXRange = 4f;
    public float spawnY = 6f;
    public float burstSpreadX = 1.25f;

    [Header("Physics")]
    public float initialDownVelocity = -1.0f;
    public float velocityJitterPercent = 0.1f;

    private float _timer;

    private void Awake()
    {
        ApplyTierProbabilities();
    }

    private void OnValidate()
    {
        // Normalize values when you tweak them in the Inspector
        ApplyTierProbabilities();
    }

    private void ApplyTierProbabilities()
    {
        // Ensure the three values always sum to 1.0
        float sum = Mathf.Max(0.0001f, pCommon + pMedium + pRare);
        LetterPool.SetTierProbabilities(pCommon / sum, pMedium / sum, pRare / sum);
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= spawnInterval)
        {
            _timer = 0f;
            StartCoroutine(SpawnBurst());
        }
    }

    private IEnumerator SpawnBurst()
    {
        int count = Mathf.Clamp(Random.Range(minBurst, maxBurst + 1), 1, 16);
        float baseX = Random.Range(-spawnXRange, spawnXRange);

        for (int i = 0; i < count; i++)
        {
            var pick = LetterPool.GetWeightedPick();
            float x = baseX + Random.Range(-burstSpreadX, burstSpreadX);
            x = Mathf.Clamp(x, -spawnXRange, spawnXRange);
            SpawnWithLetterAndTierAt(pick.Letter, pick.Tier, new Vector3(x, spawnY, 0f));
            if (intraBurstDelay > 0f) yield return new WaitForSeconds(intraBurstDelay);
        }
    }

    public void SpawnWithLetter(char letter)
    {
        // If you want to infer tier by letter, add a helper; default to Common
        SpawnWithLetterAndTierAt(letter, LetterTier.Common, new Vector3(Random.Range(-spawnXRange, spawnXRange), spawnY, 0f));
    }

    private void SpawnWithLetterAndTierAt(char letter, LetterTier tier, Vector3 pos)
    {
        GameObject prefab = useTierSpecificPrefabs ? GetPrefabForTier(tier) : (ballPrefabDefault != null ? ballPrefabDefault : GetPrefabForTier(tier));
        var go = Instantiate(prefab, pos, Quaternion.identity);
        go.name = $"Ball_{tier}_{char.ToUpper(letter)}";

        var ball = go.GetComponent<BallLetter>();
        if (ball != null)
        {
            ball.SetLetter(letter);
            ball.SetTierVisual(tier);
        }

        var rb = go.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            float jitter = 1f + Random.Range(-velocityJitterPercent, velocityJitterPercent);
            rb.linearVelocity = new Vector2(0f, initialDownVelocity * jitter);
        }
    }

    private GameObject GetPrefabForTier(LetterTier tier)
    {
        switch (tier)
        {
            case LetterTier.Common: return ballPrefabCommon ?? ballPrefabDefault ?? ballPrefabRare ?? ballPrefabMedium;
            case LetterTier.Medium: return ballPrefabMedium ?? ballPrefabDefault ?? ballPrefabCommon ?? ballPrefabRare;
            case LetterTier.Rare: return ballPrefabRare ?? ballPrefabDefault ?? ballPrefabCommon ?? ballPrefabMedium;
            default: return ballPrefabDefault ?? ballPrefabCommon ?? ballPrefabMedium ?? ballPrefabRare;
        }
    }
}
