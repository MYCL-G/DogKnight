using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Grunt : cEnemy
{
    [Header("Skill")]
    public float kickForce = 10;
    public void KickOff()
    {
        if (attackTarget != null)
        {
            transform.LookAt(attackTarget.transform);
            Vector3 direction = attackTarget.transform.position - transform.position;
            direction.Normalize();
            NavMeshAgent agent = attackTarget.GetComponent<NavMeshAgent>();
            agent.isStopped = true;
            agent.velocity = direction * kickForce;
            attackTarget.GetComponent<Animator>().SetTrigger("dizzy");
        }
    }
}
