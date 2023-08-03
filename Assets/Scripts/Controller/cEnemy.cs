using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

enum EnemyStatus
{
    Guard,
    Patrol,
    Chase,
    Dead
}

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(UniversalStats))]
public class cEnemy : MonoBehaviour, iEndGameObserver
{
    protected UniversalStats universalStats;
    EnemyStatus enemyStatus;
    NavMeshAgent agent;
    Animator anim;
    Collider coll;
    float speed;

    //bool动画
    bool isWalk;
    bool isChase;
    bool isFollow;
    bool isDead;
    bool playerDead;

    [Header("Basic Settings")]
    public float sightRadius;
    public bool isGuard;
    protected GameObject attackTarget;
    public float lookAtTime;
    float remainLookAtTime;
    float lastAttackTime;
    Quaternion guardRotation;
    [Header("Patrol State")]
    public float patrolRange;
    Vector3 wayPoint;
    Vector3 guardPos;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        universalStats = GetComponent<UniversalStats>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider>();
        speed = agent.speed;
        guardPos = transform.position;
        guardRotation = transform.rotation;
        remainLookAtTime = lookAtTime;
    }
    void Start()
    {
        if (isGuard)
        {
            enemyStatus = EnemyStatus.Guard;
        }
        else
        {
            enemyStatus = EnemyStatus.Patrol;
            GetWayPoint();
        }
        mGame.Inst.AddObserver(this);
    }
    void Update()
    {
        if (!playerDead)
        {
            SwitchStatus();
            SwitchAnim();
            lastAttackTime -= Time.deltaTime;
        }
    }
    void SwitchAnim()
    {
        anim.SetBool("walk", isWalk);
        anim.SetBool("chase", isChase);
        anim.SetBool("follow", isFollow);
        anim.SetBool("crit", universalStats.isCrit);
        anim.SetBool("death", universalStats.isDead);
    }
    void SwitchStatus()
    {
        if (universalStats.isDead)
            enemyStatus = EnemyStatus.Dead;
        else if (FoundPlayer())
        {
            enemyStatus = EnemyStatus.Chase;
        }
        switch (enemyStatus)
        {
            case EnemyStatus.Guard:
                isChase = false;
                if (transform.position != guardPos)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;
                    if (Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
                    {
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.03f);
                    }
                }
                break;
            case EnemyStatus.Patrol:
                isChase = false;
                agent.speed = speed * 0.5f;
                //是否到达随机巡逻点
                if (Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    if (remainLookAtTime > 0)
                    {
                        remainLookAtTime -= Time.deltaTime;
                        agent.destination = transform.position;
                    }
                    else
                        GetWayPoint();
                }
                else
                {
                    isWalk = true;
                    agent.destination = wayPoint;
                }
                break;
            case EnemyStatus.Chase:
                agent.speed = speed;
                isWalk = false;
                isChase = true;
                if (!FoundPlayer())
                {
                    isFollow = false;
                    if (remainLookAtTime > 0)
                    {
                        agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else if (isGuard)
                    {
                        enemyStatus = EnemyStatus.Guard;
                    }
                    else
                        enemyStatus = EnemyStatus.Patrol;
                }
                else
                {
                    isFollow = true;
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position;
                }
                if (TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;
                    agent.isStopped = true;
                    if (lastAttackTime <= 0)
                    {
                        lastAttackTime = universalStats.attackData.coolDown;
                        universalStats.isCrit = UnityEngine.Random.value < universalStats.attackData.critChance;
                        Attack();
                    }
                    //agent.isStopped = false;
                }
                break;
            case EnemyStatus.Dead:
                coll.enabled = false;
                agent.radius = 0;
                Destroy(gameObject, 2f);
                break;
        }
    }

    void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if (TargetInAttackRange())
        {
            anim.SetBool("crit", universalStats.isCrit);
            anim.SetTrigger("attack");
        }
        if (TargetInSkillRange())
        {
            anim.SetTrigger("skill");
        }
    }

    bool FoundPlayer()
    {
        var c = Physics.OverlapSphere(transform.position, sightRadius);
        foreach (var target in c)
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;
        return false;
    }
    bool TargetInAttackRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= universalStats.attackData.attackRange;
        return false;
    }
    bool TargetInSkillRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= universalStats.attackData.skillRange;
        return false;
    }
    void GetWayPoint()
    {
        remainLookAtTime = lookAtTime;
        float randomX = UnityEngine.Random.Range(-patrolRange, patrolRange);
        float randomZ = UnityEngine.Random.Range(-patrolRange, patrolRange);
        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);
        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
    }
    void Hit()
    {
        if (attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStats = attackTarget.GetComponent<UniversalStats>();
            targetStats.TakeDamage(universalStats, targetStats);
        }
    }
    //private void OnEnable()
    //{
    //    mGame.Inst.AddObserver(this);
    //}
    private void OnDisable()
    {
        mGame.Inst.RemoveObserver(this);
    }
    public void EndNotify()
    {
        isChase = false;
        isWalk = false;
        attackTarget = null;
        anim.SetBool("win", true);
        playerDead = true;
    }
}
