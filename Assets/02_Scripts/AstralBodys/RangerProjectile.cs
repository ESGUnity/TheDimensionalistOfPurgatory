using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class RangerProjectile : MonoBehaviour
{
    public GameObject Target;
    public GameObject Attacker;
    public int Damage;
    public float Speed; // ������Ÿ�� �ӵ�
    public bool IsPenet = false;
    float ShootDistance;
    Vector3 Direction;
    Vector3 startPosition;

    void Update()
    {
        if (!IsPenet)
        {
            if (Target != null && Attacker != null) // ���� ������ �״°� �����ϱ� ���� Attacker�� ���� �ƴҋ� �߰�. ���߿� GameManager���� Battle ���� ��� ���� �ǵ��� ��������.
            {
                // ��ǥ ��ġ������ ���� ���
                Vector3 direction = (Target.transform.position - transform.position).normalized;
                GetComponent<Collider>().enabled = false;   
                // Ÿ���� ���� �̵�
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
        transform.LookAt(Target.transform.position); // ��ȯ �� ��ǥ �ٶ󺸰� �����
        startPosition = transform.position; // ������ġ ����
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
