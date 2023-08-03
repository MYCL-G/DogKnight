using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Golem : cEnemy
{
    [Header("Skill")]
    public float kickForce = 20;
    public GameObject rockPrefab;
    public Transform hanPos;
    public void KickOff()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<UniversalStats>();
            Vector3 direction = (attackTarget.transform.position - transform.position).normalized;
            NavMeshAgent agent = targetStats.GetComponent<NavMeshAgent>();
            agent.isStopped = true;
            agent.ResetPath();
            agent.velocity = direction * kickForce;
            targetStats.GetComponent<Animator>().SetTrigger("dizzy");
            targetStats.TakeDamage(universalStats, targetStats);
        }
    }
    public void ThrowRock()
    {
        if (attackTarget != null)
        {
            var rock = Instantiate(rockPrefab, hanPos.position, Quaternion.identity);
            rock.GetComponent<Rock>().target = attackTarget;
            rock.GetComponent<Rock>().FlyToTarget();
        }
    }
}
