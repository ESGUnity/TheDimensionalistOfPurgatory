using System.Collections;
using UnityEngine;

public class ScopeAttack : MonoBehaviour
{
    GameObject Target;
    GameObject Attacker;
    int Damage;
    float Duration;
    float ShotTiming;
    float Scope;
    bool IsOneShot = false;
    bool IsContinuous = false;
    public void Awake()
    {
        GetComponent<Collider>().enabled = false;
    }
    void Update()
    {
        if (IsOneShot)
        {
            StartCoroutine(OneShotAttack());
        }
        if (IsContinuous)
        {
            StartCoroutine(ContinuousAttack());
        }
    }
    public void OneShotAttack(GameObject attacker, GameObject target, int damage, float duration, float scope, bool isOneShot = true)
    {
        Attacker = attacker;
        Target = target;
        Damage = damage;
        IsOneShot = isOneShot;
        Duration = duration;
        Scope = scope;
    }

    public void ContinuousAttack(GameObject attacker, GameObject target, int damage, float duration, bool isContinuous = true)
    {
        Attacker = attacker;
        Target = target;
        Damage = damage;
        Duration = duration;
        IsContinuous = isContinuous;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Attacker.gameObject.tag == "Ally")
        {
            if (other.gameObject.tag == "Enemy")
            {
                other.gameObject.GetComponent<AstralBody>().Damaged(Damage, Attacker);
            }
        }
        else if (Attacker.gameObject.tag == "Enemy")
        {
            if (other.gameObject.tag == "Ally")
            {
                other.gameObject.GetComponent<AstralBody>().Damaged(Damage, Attacker);
            }
        }
    }

    IEnumerator OneShotAttack()
    {
        transform.localScale = Vector3.zero;

        SphereCollider col = GetComponent<SphereCollider>();
        col.enabled = true;
        col.radius = Scope;
        float remainTime = Duration;

        while (remainTime > 0)
        {
            remainTime -= Time.deltaTime;
            transform.localScale = Vector3.one * (1 - (remainTime / Duration));

            yield return null;
        }
        yield return new WaitForSeconds(0.3f); // 애님 지속 시간 기다려주기
        Destroy(gameObject);
    }

    IEnumerator ContinuousAttack()
    {
        for (int i = 0; i < Duration; i++)
        {
            GetComponent<Collider>().enabled = true;
            yield return null;
            GetComponent<Collider>().enabled = false;
            yield return new WaitForSeconds(1f);
        }
        Destroy(gameObject);
    }
}
