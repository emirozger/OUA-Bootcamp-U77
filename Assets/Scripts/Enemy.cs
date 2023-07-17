using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public NavMeshAgent agent;
    public Animator animator;
    public event Action<Enemy> OnDeath;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    public float health;


    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;


    public float timeBetweenAttacks;
    public bool alreadyAttacked;
    public GameObject projectilePrefab;



    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("PlayerObj").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

    }

    private void Update()
    {

        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange && health != 0) Patroling();
        if (playerInSightRange && !playerInAttackRange && health != 0) ChasePlayer();
        if (playerInAttackRange && playerInSightRange && health != 0) AttackPlayer();




    }

    private void Patroling()
    {
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet)
            agent.SetDestination(walkPoint);
        animator.SetBool("Patroling", true);
        animator.SetBool("Shooting", false);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 1f)
            walkPointSet = false;
    }
    private void SearchWalkPoint()
    {
        float randomZ = UnityEngine.Random.Range(-walkPointRange, walkPointRange);
        float randomX = UnityEngine.Random.Range(-walkPointRange, walkPointRange);

        walkPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        if (Physics.Raycast(walkPoint, -transform.up, 2f, whatIsGround))
            walkPointSet = true;
    }

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
        animator.SetBool("Patroling", true);
        animator.SetBool("Shooting", false);
    }

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            animator.SetBool("Patroling", false);
            animator.SetBool("Shooting", true);
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.LookRotation(transform.forward));
            Rigidbody rb = projectile.GetComponent<Rigidbody>();

            float forwardForce = 40f;
            float upwardForce = 6f;

            Vector3 forwardDirection = transform.forward;
            forwardDirection.y = 0f;
            forwardDirection.Normalize();
            rb.AddForce(forwardDirection * forwardForce, ForceMode.Impulse);

            Vector3 upwardDirection = transform.up;
            rb.AddForce(upwardDirection * upwardForce, ForceMode.Impulse);


            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }


    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            health = 0;
            Die();
        }

    }
    private void Die()
    {
        OnDeath?.Invoke(this); // Ölüm olayını tetikle

        playerInAttackRange = false;
        this.gameObject.GetComponent<CapsuleCollider>().enabled = false;
        alreadyAttacked = false;
        animator.SetTrigger("Die");
        Destroy(gameObject, 3.5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
