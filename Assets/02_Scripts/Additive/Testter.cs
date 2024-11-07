using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Testter : MonoBehaviour
{
    public GameObject Tester;
    NavMeshAgent agent;
    Vector3 Direction;
    float stopDistance = 1.5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = (Tester.transform.position - transform.position).normalized;
        Vector3 offsetPosition = Tester.transform.position - (direction * 1f); // ��������� �ٸ� ��ġ ��ǥ
        agent.SetDestination(offsetPosition);
    }
}
