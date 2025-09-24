using UnityEngine;

public class JarDropZone : MonoBehaviour
{
    // Called when a slot returns a letter to the jar by dragging onto this zone
    private JarController jarController;


    private void Awake()
    {
        jarController = FindObjectOfType<JarController>();
    }
    public void ReceiveLetterFromSlot(char letter)
    {
        Debug.Log($"JarDropZone: received letter '{letter}' back from slot.");
        jarController.RespawnBall(letter);
        // TODO: integrate with your jar controller:
        // e.g., jarController.SpawnBall(letter);
    }
}
