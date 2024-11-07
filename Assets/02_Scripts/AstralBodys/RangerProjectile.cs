using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class RangerProjectile : MonoBehaviour
{
    public GameObject Target;
    public GameObject Attacker;
    public int Damage;
    public float Speed; // 프로젝타일 속도
    public bool IsPenet = false;
    float ShootDistance;
    Vector3 Direction;
    Vector3 startPosition;

    void Update()
    {
        if (!IsPenet)
        {
            if (Target != null && Attacker != null) // 게임 끝나고 죽는걸 방지하기 위해 Attacker가 널이 아닐떄 추가. 나중에 GameManager에서 Battle 종료 즉시 무적 되도록 설정하자.
            {
                // 목표 위치까지의 방향 계산
                Vector3 direction = (Target.transform.position - transform.position).normalized;
                GetComponent<Collider>().enabled = false;   
                // 타겟을 향해 이동
                transform.position = Vector3.MoveTowards(transform.position, Target.transform.position, Speed * Time.deltaTime);

                if (Vector3.Distance(Target.transform.position, transform.position) < 0.01f)
                {
                    Target.GetComponent<AstralBody>().Damaged(Damage, Attacker);
                    Destroy(gameObject);
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
        else
        {
            transform.position += Direction * Speed * Time.deltaTime;
            if (Vector3.Distance(startPosition, transform.position) > 15f)
            {
                Destroy(gameObject);
            }
        }
    }

    public void RangeAttack(GameObject attacker, GameObject target, int damage, float speed = 5f)
    {
        Target = target;
        Damage = damage;
        Attacker = attacker;
        Speed = speed;
    }
    
    public void PenetratingAttack(GameObject attacker, GameObject target, int damage, bool isPenet, float shootDistance = 15f, float speed = 5f)
    {
        Target = target;
        Damage = damage;
        Attacker = attacker;
        Speed = speed;
        IsPenet = isPenet;
        Direction = (Target.transform.position - transform.position).normalized;
        transform.LookAt(Target.transform.position); // 소환 후 목표 바라보게 만들기
        startPosition = transform.position; // 시작위치 저장
        ShootDistance = shootDistance;
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

}
