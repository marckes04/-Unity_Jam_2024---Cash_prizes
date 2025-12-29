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
    public Transform lookPoint;
    public GameObject shootingRaycastArea;
    public Transform playerBody;
    public LayerMask playerLayer;

    [Header("Enemy Shooting Var")]
    public float timebtwShoot;
    bool previouslyShoot;

    [Header("Enemy States")]
    public float visionRadius;
    public float shootingRadius;
    public bool playerInvisionRadius;
    public bool playerInShootingRadius;
    public bool IsPlayer = false;

    void Awake()
    {
        enemyAgent = GetComponent<NavMeshAgent>();
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
        if(enemyAgent.SetDestination(playerBody.position))
        {
            //Animations
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

            }
        }
        previouslyShoot = true;
        Invoke(nameof(ActiveShooting), timebtwShoot);
    }
    private void ActiveShooting()
    {
        previouslyShoot = false;
    }
}
