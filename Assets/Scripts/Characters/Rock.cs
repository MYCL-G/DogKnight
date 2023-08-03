using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum RockStates
{
    HitPlayer,
    HitEnemy,
    HitNothing
}
public class Rock : MonoBehaviour
{
    Rigidbody rb;
    [Header("Basic Setting")]
    public float force;
    public GameObject target;
    Vector3 direction;
    public RockStates rockStates;
    public int damage;
    public GameObject breakEffect;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rockStates = RockStates.HitPlayer;
    }
    void Start()
    {
        rb.velocity = Vector3.one;
    }
    void Update()
    {

    }
    private void FixedUpdate()
    {
        if (rb.velocity.sqrMagnitude < 1)
            rockStates = RockStates.HitNothing;
    }
    public void FlyToTarget()
    {
        direction = (target.transform.position - transform.position + Vector3.up).normalized;
        rb.AddForce(direction * force, ForceMode.Impulse);
    }
    private void OnCollisionEnter(Collision other)
    {
        switch (rockStates)
        {
            case RockStates.HitPlayer:
                if (other.gameObject.CompareTag("Player"))
                {
                    NavMeshAgent agent = other.gameObject.GetComponent<NavMeshAgent>();
                    agent.isStopped = true;
                    agent.velocity = direction * force;
                    other.gameObject.GetComponent<Animator>().SetTrigger("dizzy");
                    other.gameObject.GetComponent<UniversalStats>().TakeDamage(damage, other.gameObject.GetComponent<UniversalStats>());

                    rockStates = RockStates.HitNothing;
                }
                if (other.gameObject.CompareTag("Ground"))
                {
                    rockStates = RockStates.HitNothing;
                }
                break;
            case RockStates.HitEnemy:
                if (other.gameObject.GetComponent<Golem>())
                {
                    var otherStats = other.gameObject.GetComponent<UniversalStats>();
                    otherStats.TakeDamage(damage, otherStats);
                    Instantiate(breakEffect, transform.position, Quaternion.identity);

                    Destroy(gameObject);
                }
                break;
            case RockStates.HitNothing:
                break;
        }
    }
}
