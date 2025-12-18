using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 120f;
    private float currentHealth;

    [Header("Combat Settings")]
    public float damageDealt = 5f;
    public float timeBetweenShots = 1.5f;
    private bool canShoot = true;

    [Header("Detection Settings")]
    public float visionRadius = 20f;
    public float shootingRadius = 10f;
    public LayerMask playerLayer;

    [Header("References")]
    public NavMeshAgent agent;
    public Transform playerTransform;
    public Transform shootingPoint;
    public Transform spawnPoint;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;

        // Auto-find player if not assigned via Tag
        if (playerTransform == null)
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        // 1. Check distances
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

        // 2. State Logic
        if (distanceToPlayer <= shootingRadius)
        {
            AttackState();
        }
        else if (distanceToPlayer <= visionRadius)
        {
            PursuePlayer();
        }
        else
        {
            StopMovement();
        }
    }

    private void PursuePlayer()
    {
        // Update destination to player's current position
        if (playerTransform != null)
        {
            agent.isStopped = false;
            agent.SetDestination(playerTransform.position);
            // Trigger Run Animation here
        }
    }

    private void AttackState()
    {
        // Stop moving to shoot accurately
        agent.isStopped = true;

        // Always face the player
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);

        if (canShoot)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        canShoot = false;

        RaycastHit hit;
        if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out hit, shootingRadius))
        {
            if (hit.transform.TryGetComponent(out PlayerMovement player))
            {
                player.playerHitDamage(damageDealt);
                Debug.Log("Hit Player!");
            }
        }

        Invoke(nameof(ResetShot), timeBetweenShots);
    }

    private void ResetShot() => canShoot = true;

    private void StopMovement()
    {
        agent.isStopped = true;
        // Trigger Idle Animation here
    }

    public void EnemyHitDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) StartCoroutine(HandleDeath());
    }

    IEnumerator HandleDeath()
    {
        // Disable AI
        agent.enabled = false;
        this.enabled = false; // Stops Update()

        Debug.Log("Enemy Down");
        yield return new WaitForSeconds(5f);

        // Respawn Logic
        transform.position = spawnPoint.position;
        currentHealth = maxHealth;
        agent.enabled = true;
        this.enabled = true;
    }
}
