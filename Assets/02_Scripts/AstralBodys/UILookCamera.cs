using UnityEngine;
using UnityEngine.AI;

public class UILookCamera : MonoBehaviour
{
    public NavMeshAgent agent;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + Camera.main.transform.forward);
    }
}
