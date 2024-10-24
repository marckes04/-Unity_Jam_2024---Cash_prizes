using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Health and Damage")]
    private float enemyHealth = 120f;
    private float presentHealth;
    public float giveDamage = 5f;
    public float enemySpeed;

    [Header("Enemy Things")]
    public NavMeshAgent enemyAgent;
    public Transform Lookpoint;
    public GameObject shootingRaycastArea;
    public Transform playerBody;
    public LayerMask playerLayer;
    public Transform spawn;
    public Transform enemyCharacter;


    [Header("Enemy Shooting Var")]
    public float timebtwshoot;
    bool previouslyshoot;

    [Header("Enemy States")]
    public float visionRadius;
    public float shootingRadius;
    public bool playerInvisionRadius;
    public bool playerInshootingRadius;
    public bool isPlayer = false;

    private void Awake()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
        presentHealth = enemyHealth;
    }

    private void Update()
    {
        playerInvisionRadius = Physics.CheckSphere(transform.position, visionRadius, playerLayer);
        playerInshootingRadius = Physics.CheckSphere(transform.position, shootingRadius, playerLayer);

        if(playerInvisionRadius && !playerInshootingRadius)
        PursuePlayer();
        if (playerInshootingRadius && playerInshootingRadius)
            ShootPlayer();
    }

    private void PursuePlayer()
    {
        if(enemyAgent.SetDestination(playerBody.position))
        {
            // animations
        }
    }

    private void ShootPlayer()
    {
        enemyAgent.SetDestination(transform.position);

        transform.LookAt(Lookpoint);

        if (!previouslyshoot)
        {
            RaycastHit hit;

            if(Physics.Raycast(shootingRaycastArea.transform.position, shootingRaycastArea.transform.forward, out hit,shootingRadius))
            {
                Debug.Log("Shooting" + hit.transform.name);
                
                PlayerMovement playerBody = hit.transform.GetComponent<PlayerMovement>();

                if (playerBody != null) 
                {
                    playerBody.playerHitDamage(giveDamage);
                }
            }
        }

        previouslyshoot = true;
        Invoke(nameof(ActiveShooting), timebtwshoot);
    }

    private void ActiveShooting()
    {
        previouslyshoot = false;
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
        playerInshootingRadius = false;

        // animations

        Debug.Log("Dead");

        yield return new WaitForSeconds(5f);

        Debug.Log("Spawn");

        presentHealth = 120f;
        enemySpeed = 3f;
        shootingRadius = 10f;
        visionRadius = 100f;
        playerInvisionRadius = true;
        playerInshootingRadius = false;

        // animations

        //spawnpoints
        enemyCharacter.transform.position = spawn.transform.position;
        PursuePlayer();
    }

}
