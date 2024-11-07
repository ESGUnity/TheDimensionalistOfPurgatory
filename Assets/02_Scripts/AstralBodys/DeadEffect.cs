using UnityEngine;

public class DeadEffect : MonoBehaviour
{
    private void Awake()
    {
        Destroy(gameObject, 0.5f);
    }
}
