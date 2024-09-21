using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("Enemy Health and Damage")]
    public float giveDamage = 5f;
    public float enemySpeed;

    [Header("Enemy Things")]
    public NavMeshAgent enemyAgent;
    public Transform Lookpoint;
    public GameObject shootingRaycastArea;
    public Transform playerBody;
    public LayerMask playerLayer;


    [Header("Enemy Shooting Var")]
    public float timebtwshoot;
    bool previouslyshoot;

    [Header("Enemy States")]
    public float visionRadius;
    public float shootingRadius;
    public bool playerInvisionRadius;
    public bool playerInshootingRadius;
    public bool iaPlayer = false;

    private void Awake()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
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
            }
        }

        previouslyshoot = true;
        Invoke(nameof(ActiveShooting), timebtwshoot);
    }

    private void ActiveShooting()
    {
        previouslyshoot = false;
    }

}
