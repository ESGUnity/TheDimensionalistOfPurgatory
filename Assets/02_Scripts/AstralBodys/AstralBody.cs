using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AstralBody : MonoBehaviour
{
    [Header("CardStats")]
    public int Id; // ��ġ �� �������� ������ ���� ���� ����
    public float DetectionRadius;
    public int Cost;
    public int MaxHealth;
    public int CurrentHealth;
    public int MaxMana;
    protected int CurrentMana;
    public int Damage;
    public float AttackSpeed; // �ִϸ��̼� Ŭ���� Speed�� 2�� �Ǹ� �ӵ��� 2�� ��������. Speed�� �ִϸ��̼� Ŭ���� ���ϱ�� ������ �ȴ�. �׷��� Speed�� 1�̸� ���� �ִϸ��̼� Ŭ���� �ɸ��� �ð� �ʸ�ŭ �ɸ��� �ȴ�.
    public float Range; // Grid�� ������ ���� �� Range ����ȭ
    public int AbilityValue;
    public string TargetTag;
    public Slider HealthUI;
    public Slider ManaUI;
    public TextMeshProUGUI DamageText;
    public TextMeshProUGUI HealthText;
    public GameObject DeadEffect;
    public GameObject StigmaEffect;
    public GameObject StunEffect;

    enum State { Idle, Move, Attack, Fullness, Necromancy, Dead }
    State state;
    public GameObject target;
    public GameObject Projectile;
    bool isTargetInRange;
    AnimationClip attackAnimation; // �ִϸ����Ϳ� ����� ��� �ִϸ��̼� Ŭ�� �� ���� �ִϸ��̼� Ŭ��
    float normalizedVelocity; // ���ݼӵ��� 1�ʷ� ������ִ� ����ȭ �ӵ�
    float distance;
    bool isInStack = false;
    bool isStigmaed = false;
    bool isStuned = false;
    bool isInvincible = false;

    NavMeshAgent agent;
    Animator animator;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        agent.speed = 1f; // ������Ʈ ������Ʈ �� ��� �ʱ�ȭ
        agent.angularSpeed = 1080f;
        agent.acceleration = 100f;
        agent.stoppingDistance = 0;
        agent.radius = 0.4f;
        agent.height = 2f;
        agent.avoidancePriority = 1;
        CurrentHealth = MaxHealth;
        CurrentMana = 0;
        SetAttackSpeed(); // ���ݼӵ��� AttackSpeed ���� ������ ����.
    }
    void Start()
    {
        state = State.Idle;
        isTargetInRange = false;

        if (gameObject.tag == "Enemy")
        {
            HealthUI.transform.GetChild(2).GetComponentInChildren<Image>().color = Color.red; // �� ��ü ü���� ������ �����
        }
;
    }

    void Update()
    {
        if (state == State.Dead)
        {
            return;
        }
        HealthUI.value = (float)CurrentHealth / MaxHealth;
        if (MaxMana != 0)
        {
            ManaUI.value = (float)CurrentMana / MaxMana;
        }
        else
        {
            ManaUI.value = 0;
        }

        DamageText.text = Damage.ToString();
        HealthText.text = CurrentHealth.ToString();

        if (GameManager.Instance.phase == GameManager.Phase.WaitingBeforePreparation && !isInStack) // ��Ƴ��Ҵٸ� ���� ����� �ѱ�� ���� �ڵ�
        {
            if (gameObject.tag == "Ally")
            {
                PlacementManager.Instance.AliveAllyAstralBody.Push(transform.parent.gameObject);
                isInStack = true;
            }
            if (gameObject.tag == "Enemy")
            {
                PlacementManager.Instance.AliveEnemyAstralBody.Push(transform.parent.gameObject);
                isInStack = true;
            }
        }

        if (GameManager.Instance.phase != GameManager.Phase.Battle) // ��Ʋ ����� �ƴϸ� FSM�� �۵����� �ʵ��� ����� ����.
        {
            agent.enabled = false;
            return;
        }

        if (isStuned) // ���� ���¶�� �� ����
        {
            StartIdle();
            return;
        }

        agent.enabled = true; // ������Ʈ �ٽ� �ѱ�
        isInStack = false; // ���� ����� �ѱ� �� ������ �ѱ�°� �����ϴ� ����

        if (target != null) // �����ϰ� ���� �����ֱ�
        {
            agent.SetDestination(target.transform.position);
        }

        FindClosestTarget();
        UpdateTargetDistance();
        CheckTargetInRange();
        FSM();
    }

    public void UpdateTargetDistance()
    {
        if (target != null)
        {
            distance = Vector3.Distance(transform.position, target.transform.position);
        }
    }
    public void FSM() // ���°� ��ȯ�� ������ �ۼ��� �޼���
    {
        if (target != null)
        {
            switch (state)
            {
                case State.Idle: // Idle�� �� ��Ÿ� ���� ���� �ִٸ� ����, �ƴϸ� �̵�
                    if (MaxMana != 0 && CurrentMana >= MaxMana)
                    {
                        StartFullness();
                    }
                    if (distance <= Range)
                    {
                        agent.enabled = false;
                        transform.LookAt(target.transform.position); // ������ȯ�� �����ִ� �ڵ�. ������ ���� ���� �����Ѵ�. // ������ ������ �����ص� �Ǵ� �ڵ�.
                        agent.enabled = true;
                        StartAttack();
                    }
                    else
                    {
                        StartMove();
                    }
                    break;
                case State.Move: // Move�� �� ��Ÿ� ���� target�� �ִٸ� Idle
                    if (distance <= Range)
                    {
                        StartIdle();
                    }
                    break;
                case State.Attack:
                    if (distance > Range)
                    {
                        StartIdle(); // ���� �ִϸ��̼��� �������� state�� Idle�� �ٲٴ� �Լ��� �־ 1�� �����ϸ� ������ Idle�� �Ѿ�� ������ ��������. ���� ����� �����̸� �Ӹ��� �� ������ ��� �����ؼ� ��¿ �� ����.
                    }
                    break;
                //case State.Fullness:

                //    break;
            }
        }
        else
        {
            state = State.Idle;
        }
    }
    public void SetAttackSpeed() // ���� �ִϸ��̼� Ŭ���� ���̿� ������� AttackSpeed�� ���� ���� �ӵ��� ���������� ����� �Լ�
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips) // runtimeAnimatorController�� �ִϸ����Ϳ� ����� ��� �ִϸ��̼� Ŭ���� ���� ������ ������ �ִ�.
        {
            if (clip.name == "Attack")
            {
                attackAnimation = clip;
                normalizedVelocity = attackAnimation.length; // ���ݼӵ��� 1�ʷ� ������ִ� ����ȭ �ӵ��� �Ҵ�
                animator.SetFloat("SetAttackSpeed", normalizedVelocity * AttackSpeed); // �ִϸ��̼� Ŭ�� ����ð��� �����ϴ� �κ�. AttackSpeed�� �ݺ��.
            }
        }
    }
    public void FindClosestTarget() // �Ÿ� �� ���� ����� �� ã�� target�� �Ҵ� // isTargetInRange�� false�ų� target�� ���ٸ� target�� �ٽ� ã��
    {
        if (!isTargetInRange)
        {
            float minDistance = float.MaxValue;
            GameObject closestTarget = null;

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, DetectionRadius);
            if (hitColliders.Length > 0)
            {
                foreach (Collider col in hitColliders)
                {
                    if (col.gameObject.CompareTag(TargetTag))
                    {
                        float colDistance = Vector3.Distance(transform.position, col.transform.position);

                        if (colDistance < minDistance)
                        {
                            minDistance = colDistance;
                            closestTarget = col.gameObject;
                        }
                    }
                }

                target = closestTarget;
            }
            else
            {
                target = null;
            }
        }
    }
    public void CheckTargetInRange() // ��� ����� ���� target���� �����Ѵٸ� ���� ������ ������ ��� �ٸ� ���� ������ ���̴�. �׷��� ���� ��Ÿ����� 4f�� �Ÿ��� target�� ����� �� �� ���ο� target�� ã���� ����� �Լ�
    {
        if (target != null && distance < Range + 0.2f) // 4f�� ��Ÿ��� ����� ������ target���� ��ȿ������ Ȯ���ϴ� ����
        {
            isTargetInRange = true; // false�� �Ǳ� ������ target�� �״��
        }
        else if (target == null || distance >= Range + 0.2f)
        {
            isTargetInRange = false;
        }
    }
    public void StartIdle()
    {
        state = State.Idle;
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        animator.SetTrigger("DoIdle");
    }
    public void StartMove() // target���� �̵�. �̵� �ִϸ��̼��� ������Ʈ�� �ӵ��� 0 �̻� �� ���� ���� ���Ǿ��� Move �Լ��� �����ϸ� �ִϸ��̼ǵ� �����ϰԲ� �Լ��� �ۼ�. �ٸ� ��� FSM�� Move�� ���� �ۼ�
    {
        state = State.Move;
        agent.isStopped = false;
        animator.SetTrigger("DoMove");
    }
    public void StartAttack() // target�� ����
    {
        state = State.Attack;
        agent.isStopped = true;
        animator.SetTrigger("DoAttack");
    }
    public virtual void StartFullness() // �游 ��
    {
        state = State.Fullness;
        agent.isStopped = true;
        CurrentMana = 0;
        animator.SetTrigger("DoAbility");
    }
    public virtual void StartNecromancy() // ���� �ܰ� ���� ��
    {
        state = State.Necromancy;
    }


    public void Damaged(int damage, GameObject Attacker)
    {
        if (CurrentHealth > 0 && !isInvincible)
        {
            CurrentHealth -= damage;
            if (MaxMana != 0)
            {
                CurrentMana += 5;
            }
            if (isStigmaed)
            {
                CurrentHealth -= damage;
            }
        }
        if (CurrentHealth <= 0)
        {
            OnDie();
        }
    }
    Coroutine CurrentStigma = null;
    public void Stigmaed(int duration) // ���� �����̻�. �޴� ���� 2��
    {
        if (CurrentStigma != null)
        {
            StopCoroutine(CurrentStigma);
        }

        CurrentStigma = StartCoroutine(StigmaedDuration(duration));
    }
    Coroutine CurrentStun = null;
    public void Stuned(int duration)
    {
        if (CurrentStun != null)
        {
            StopCoroutine(CurrentStun);
        }

        CurrentStun = StartCoroutine(StunedDuration(duration));
    }
    Coroutine CurrentInvincible = null;
    public void Invincible(int duration)
    {
        if (CurrentInvincible != null)
        {
            StopCoroutine(CurrentInvincible);
        }

        CurrentInvincible = StartCoroutine(InvincibleDuration(duration));
    }
    public virtual void OnDie()
    {
        state = State.Dead;
        GameObject go = Instantiate(DeadEffect);
        go.transform.position = transform.position;
        Destroy(gameObject);
    }

    public void Attacking()
    {
        target.GetComponent<AstralBody>().Damaged(Damage, gameObject);
        if (MaxMana != 0)
        {
            CurrentMana += 10;
        }
    } // Ŭ������ ����� �Լ�
    public void RangeAttacking()
    {
        GameObject go = Instantiate(Projectile);
        go.transform.position = transform.position;
        go.GetComponent<RangerProjectile>().RangeAttack(gameObject, target, Damage);
        if (MaxMana != 0)
        {
            CurrentMana += 10;
        }
    } // Ŭ������ ����� �Լ�
    public void TurnStateIdle()
    {
        state = State.Idle;
    } // �����̳� Ư������ Ŭ������ �ݵ�� ����Ǿ���ϴ� �Լ�



    IEnumerator StigmaedDuration(int duration)
    {
        isStigmaed = true;
        GameObject go = Instantiate(StigmaEffect);
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;
        yield return new WaitForSeconds(duration);
        isStigmaed = false;
        Destroy(go);
        yield return null;
    } // ���� �ڷ�ƾ
    IEnumerator StunedDuration(int duration)
    {
        isStuned = true;
        GameObject go = Instantiate(StunEffect);
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;
        yield return new WaitForSeconds(duration);
        isStuned = false;
        Destroy(go);
        yield return null;
    } // ���� �ڷ�ƾ
    IEnumerator InvincibleDuration(int duration)
    {
        isInvincible = true;
        GameObject go = Instantiate(EffectManager.Instance.InvincibleEffect);
        go.transform.SetParent(transform);
        go.transform.localPosition = Vector3.zero;
        yield return new WaitForSeconds(duration);
        isInvincible = false;
        Destroy(go);
        yield return null;
    } // ���� �ڷ�ƾ
}
