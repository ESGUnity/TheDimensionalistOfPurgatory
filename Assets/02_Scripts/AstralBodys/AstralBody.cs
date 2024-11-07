using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class AstralBody : MonoBehaviour
{
    [Header("CardStats")]
    public int Id; // 배치 후 프리팹의 정보를 보기 위한 변수
    public float DetectionRadius;
    public int Cost;
    public int MaxHealth;
    public int CurrentHealth;
    public int MaxMana;
    protected int CurrentMana;
    public int Damage;
    public float AttackSpeed; // 애니메이션 클립의 Speed가 2가 되면 속도가 2배 빨라진다. Speed는 애니메이션 클립에 곱하기로 적용이 된다. 그래서 Speed가 1이면 기존 애니메이션 클립이 걸리는 시간 초만큼 걸리게 된다.
    public float Range; // Grid로 전장을 만든 후 Range 정규화
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
    AnimationClip attackAnimation; // 애니메이터에 연결된 모든 애니메이션 클립 중 공격 애니메이션 클립
    float normalizedVelocity; // 공격속도를 1초로 만들어주는 정규화 속도
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
        agent.speed = 1f; // 에이전트 컴포넌트 값 모두 초기화
        agent.angularSpeed = 1080f;
        agent.acceleration = 100f;
        agent.stoppingDistance = 0;
        agent.radius = 0.4f;
        agent.height = 2f;
        agent.avoidancePriority = 1;
        CurrentHealth = MaxHealth;
        CurrentMana = 0;
        SetAttackSpeed(); // 공격속도를 AttackSpeed 변수 값으로 설정.
    }
    void Start()
    {
        state = State.Idle;
        isTargetInRange = false;

        if (gameObject.tag == "Enemy")
        {
            HealthUI.transform.GetChild(2).GetComponentInChildren<Image>().color = Color.red; // 적 영체 체력을 빨갛게 만들기
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

        if (GameManager.Instance.phase == GameManager.Phase.WaitingBeforePreparation && !isInStack) // 살아남았다면 다음 라운드로 넘기기 위한 코드
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

        if (GameManager.Instance.phase != GameManager.Phase.Battle) // 배틀 페이즈가 아니면 FSM이 작동하지 않도록 만드는 조건.
        {
            agent.enabled = false;
            return;
        }

        if (isStuned) // 기절 상태라면 또 리턴
        {
            StartIdle();
            return;
        }

        agent.enabled = true; // 에이전트 다시 켜기
        isInStack = false; // 다음 라운드로 넘길 때 여러번 넘기는걸 방지하는 변수

        if (target != null) // 꾸준하게 방향 정해주기
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
    public void FSM() // 상태가 전환될 조건을 작성한 메서드
    {
        if (target != null)
        {
            switch (state)
            {
                case State.Idle: // Idle일 때 사거리 내에 적이 있다면 공격, 아니면 이동
                    if (MaxMana != 0 && CurrentMana >= MaxMana)
                    {
                        StartFullness();
                    }
                    if (distance <= Range)
                    {
                        agent.enabled = false;
                        transform.LookAt(target.transform.position); // 방향전환을 도와주는 코드. 엉뚱한 곳을 가끔 공격한다. // 오류만 없으면 제거해도 되는 코드.
                        agent.enabled = true;
                        StartAttack();
                    }
                    else
                    {
                        StartMove();
                    }
                    break;
                case State.Move: // Move일 때 사거리 내에 target이 있다면 Idle
                    if (distance <= Range)
                    {
                        StartIdle();
                    }
                    break;
                case State.Attack:
                    if (distance > Range)
                    {
                        StartIdle(); // 공격 애니메이션의 마지막에 state를 Idle로 바꾸는 함수를 넣어서 1번 공격하면 무조건 Idle로 넘어갔다 오도록 만들어놨다. 공격 대상이 움직이면 머리를 안 돌리고 계속 공격해서 어쩔 수 없다.
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
    public void SetAttackSpeed() // 공격 애니메이션 클립의 길이에 상관없이 AttackSpeed에 의해 공격 속도가 정해지도록 만드는 함수
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips) // runtimeAnimatorController는 애니메이터에 연결된 모든 애니메이션 클립에 대한 정보를 가지고 있다.
        {
            if (clip.name == "Attack")
            {
                attackAnimation = clip;
                normalizedVelocity = attackAnimation.length; // 공격속도를 1초로 만들어주는 정규화 속도를 할당
                animator.SetFloat("SetAttackSpeed", normalizedVelocity * AttackSpeed); // 애니메이션 클립 실행시간을 결정하는 부분. AttackSpeed에 반비례.
            }
        }
    }
    public void FindClosestTarget() // 거리 상 가장 가까운 적 찾고 target에 할당 // isTargetInRange가 false거나 target이 없다면 target을 다시 찾기
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
    public void CheckTargetInRange() // 계속 가까운 적만 target으로 설정한다면 조금 움직일 때마다 계속 다른 적을 공격할 것이다. 그래서 공격 사거리에서 4f의 거리를 target이 벗어나면 그 때 새로운 target을 찾도록 만드는 함수
    {
        if (target != null && distance < Range + 0.2f) // 4f는 사거리를 벗어나도 여전히 target으로 유효한지를 확인하는 범위
        {
            isTargetInRange = true; // false가 되기 전까진 target은 그대로
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
    public void StartMove() // target에게 이동. 이동 애니메이션은 오브젝트의 속도가 0 이상 일 때와 같은 조건없이 Move 함수를 실행하면 애니메이션도 실행하게끔 함수를 작성. 다른 모든 FSM도 Move와 같이 작성
    {
        state = State.Move;
        agent.isStopped = false;
        animator.SetTrigger("DoMove");
    }
    public void StartAttack() // target을 공격
    {
        state = State.Attack;
        agent.isStopped = true;
        animator.SetTrigger("DoAttack");
    }
    public virtual void StartFullness() // 충만 시
    {
        state = State.Fullness;
        agent.isStopped = true;
        CurrentMana = 0;
        animator.SetTrigger("DoAbility");
    }
    public virtual void StartNecromancy() // 교전 단계 시작 시
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
    public void Stigmaed(int duration) // 낙인 상태이상. 받는 피해 2배
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
    } // 클립에서 실행될 함수
    public void RangeAttacking()
    {
        GameObject go = Instantiate(Projectile);
        go.transform.position = transform.position;
        go.GetComponent<RangerProjectile>().RangeAttack(gameObject, target, Damage);
        if (MaxMana != 0)
        {
            CurrentMana += 10;
        }
    } // 클립에서 실행될 함수
    public void TurnStateIdle()
    {
        state = State.Idle;
    } // 공격이나 특수공격 클립에서 반드시 실행되어야하는 함수



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
    } // 낙인 코루틴
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
    } // 기절 코루틴
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
    } // 기절 코루틴
}
