using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Health and Damage")]
    [SerializeField]
    private float enemyHealth = 120f;
    private float presentHealth;
    public float giveDamage = 5f;
    public float enemySpeed;

    [Header("Enemy Things")]
    public NavMeshAgent enemyAgent;
    public Transform lookPoint;
    public GameObject shootingRaycastArea;
    public Transform playerBody;
    public LayerMask playerLayer;
    public Transform spawn;
    public Transform enemyCharacter;

    [Header("Enemy Shooting Var")]
    public float timebtwShoot;
    bool previouslyShoot;

    [Header("Enemy Animation and Spark Effect")]
    public Animator anim;

    [Header("Enemy States")]
    public float visionRadius;
    public float shootingRadius;
    public bool playerInvisionRadius;
    public bool playerInShootingRadius;
    public bool IsPlayer = false;

    void Awake()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        presentHealth = enemyHealth;
    }

    private void Update()
    {
        playerInvisionRadius = Physics.CheckSphere(transform.position,visionRadius, playerLayer);
        playerInShootingRadius = Physics.CheckSphere(transform.position, shootingRadius, playerLayer);

        if(playerInvisionRadius && !playerInShootingRadius) PursuePlayer();
        if (playerInvisionRadius && playerInShootingRadius) ShootPlayer();  
    }

    private void  PursuePlayer()
    {
        if (enemyAgent.SetDestination(playerBody.position))
        {
            //Animations
            anim.SetBool("Running", true);
            anim.SetBool("Shooting", false);
        }

        else
        {
            anim.SetBool("Running", false);
            anim.SetBool("Shooting", false);
        }

    }

    private void ShootPlayer()
    {
        enemyAgent.SetDestination(transform.position);

        transform.LookAt(lookPoint);

        if(!previouslyShoot)
        {
            RaycastHit hit;
            
            if(Physics.Raycast(shootingRaycastArea.transform.position, shootingRaycastArea.transform.forward, out hit, shootingRadius))
            {
                PlayerMovement playerBody = hit.transform.GetComponent<PlayerMovement>();
                
                if(playerBody != null)
                {
                    playerBody.playerHitDamage(giveDamage);
                }

            }

            anim.SetBool("Running", false);
            anim.SetBool("Shooting", true);

        }
        previouslyShoot = true;
        Invoke(nameof(ActiveShooting), timebtwShoot);
    }
    private void ActiveShooting()
    {
        previouslyShoot = false;
    }

    public void enemyHitDamage(float takeDamage)
    {
        presentHealth -= takeDamage;

        if (presentHealth <= 0)
        {
           StartCoroutine(Respawn());
        }
    }

    IEnumerator Respawn()
    {
        enemyAgent.SetDestination(transform.position);
        enemySpeed = 0f;
        shootingRadius = 0f;
        visionRadius = 0f;
        playerInvisionRadius = false;
        playerInShootingRadius = false;
        anim.SetBool("Die", true);
        anim.SetBool("Running", false);
        anim.SetBool("Shooting", false);

        yield return new WaitForSeconds(8f);
        presentHealth = 120f;
        enemySpeed = 3f;
        shootingRadius = 10f;
        visionRadius = 100f;
        playerInvisionRadius = true;
        playerInShootingRadius = false;

        anim.SetBool("Die", false);
        anim.SetBool("Running", true);

        enemyCharacter.transform.position = spawn.transform.position;
        PursuePlayer();
    }
}
