using UnityEngine;

public class EffectManager : MonoBehaviour
{
    private static EffectManager instance;
    public static EffectManager Instance
    {
        get
        {
            return instance;
        }
        private set
        {
            instance = value;
        }
    }
    public GameObject InvincibleEffect;
    public GameObject DeadEffect;
    public GameObject StigmaEffect;
    public GameObject StunEffect;

    void Awake()
    {
        instance = this;
    }

}
