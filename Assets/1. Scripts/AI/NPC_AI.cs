using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPC_AI : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;

    [Header("배회 설정")]
    public float wanderRadius = 5f;
    public float wanderDelayMin = 2f;
    public float wanderDelayMax = 4f;

    private float nextWanderTime;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        ScheduleNextWander();
    }

    private void Update()
    {
        animator.SetBool("isRun", agent.velocity.magnitude > 0.1f);

        if (!agent.pathPending && agent.remainingDistance <= 0.5f && Time.time >= nextWanderTime)
        {
            ScheduleNextWander();
        }
    }

    void ScheduleNextWander()
    {
        Vector3 newPos = RandomNavSphere(transform.position, wanderRadius);
        agent.SetDestination(newPos);
        nextWanderTime = Time.time + Random.Range(wanderDelayMin, wanderDelayMax);
    }

    public static Vector3 RandomNavSphere(Vector3 origin, float dist)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomPos = origin + Random.insideUnitSphere * dist;
            if (NavMesh.SamplePosition(randomPos, out NavMeshHit navHit, dist, NavMesh.AllAreas))
                return navHit.position;
        }
        return origin;
    }

    public void StopMoving()
    {
        agent.isStopped = true;
        animator.SetBool("isRun", false);
    }
}
