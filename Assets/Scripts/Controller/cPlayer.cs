using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class cPlayer : MonoBehaviour
{
    NavMeshAgent agent;
    Animator anim;
    GameObject attackTarget;
    float lastAttackTime;
    UniversalStats universalStats;
    float stopDistance;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        universalStats = GetComponent<UniversalStats>();
        stopDistance = agent.stoppingDistance;
    }
    private void OnEnable()
    {
        mMouse.Inst.OnMouseClick += MoveToTarget;
        mMouse.Inst.OnEnemyClick += MoveToEnemy;
        mGame.Inst.RegisterPlayer(universalStats);
    }
    void Start()
    {
        mSave.Inst.LoadPlayerData();
    }
    void Update()
    {
        SwitchAnimation();
        if (universalStats.isDead) mGame.Inst.NotifyObserver();
        lastAttackTime -= Time.deltaTime;
    }
    private void OnDisable()
    {
        mMouse.Inst.OnMouseClick -= MoveToTarget;
        mMouse.Inst.OnEnemyClick -= MoveToEnemy;
    }
    void SwitchAnimation()
    {
        anim.SetFloat("speed", agent.velocity.sqrMagnitude);
        anim.SetBool("death", universalStats.isDead);
    }
    void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();
        if (universalStats.isDead) return;
        agent.stoppingDistance = stopDistance;
        agent.isStopped = false;
        agent.destination = target;
    }
    private void MoveToEnemy(GameObject enemy)
    {
        if (universalStats.isDead) return;
        if (enemy != null)
        {
            attackTarget = enemy;
            universalStats.isCrit = UnityEngine.Random.value < universalStats.attackData.critChance;
            StartCoroutine(MoveToAttackTarget());
        }
    }
    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;
        agent.stoppingDistance = universalStats.attackData.attackRange;
        transform.LookAt(attackTarget.transform);
        while (Vector3.Distance(attackTarget.transform.position, transform.position) > universalStats.attackData.attackRange)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
        }
        agent.isStopped = true;
        if (lastAttackTime <= 0)
        {
            anim.SetBool("crit", universalStats.isCrit);
            anim.SetTrigger("attack");
            lastAttackTime = universalStats.attackData.coolDown;
        }
    }
    void Hit()
    {
        switch (attackTarget.tag)
        {
            case "Attackable":
                if (attackTarget.GetComponent<Rock>())
                {
                    attackTarget.GetComponent<Rock>().rockStates = RockStates.HitEnemy;
                    attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                    attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
                }
                break;
            default:
                var targetStats = attackTarget.GetComponent<UniversalStats>();
                targetStats.TakeDamage(universalStats, targetStats);
                break;
        }
    }
}
