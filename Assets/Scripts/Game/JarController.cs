using UnityEngine;

public class JarController : MonoBehaviour
{
    public BallSpawner2D spawner;

    private void Awake()
    {
        if (spawner == null) spawner = FindObjectOfType<BallSpawner2D>();
    }

    public void RespawnBall(char letter)
    {
        if (spawner != null) spawner.SpawnWithLetter(letter);
    }
}
