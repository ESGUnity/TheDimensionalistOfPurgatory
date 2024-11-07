using UnityEngine;

public class StigmaEffect : MonoBehaviour
{
    void Update()
    {
        if (GameManager.Instance.phase == GameManager.Phase.WaitingBeforePreparation)
        {
            Destroy(gameObject);
        }
    }
}
