using UnityEngine;

public class EffectDestroyer : MonoBehaviour
{
    public float DestroyTiming;
    private void Start()
    {
        Destroy(gameObject, DestroyTiming);
    }
}
