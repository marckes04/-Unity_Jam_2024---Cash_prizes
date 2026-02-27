using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class CharacterAI : MonoBehaviour
{
    public enum Team { PlayerSide, EnemySide }

    [Header("Identidad y Bando")]
    public Team characterTeam; // Elige PlayerSide para aliados y EnemySide para enemigos

    [Header("Salud y Daño")]
    [SerializeField] private float maxHealth = 120f;
    private float currentHealth;
    public float giveDamage = 5f;
    public float movementSpeed = 3.5f;

    [Header("IA de Patrulla")]
    public float patrolRadius = 25f;    // Radio de movimiento aleatorio
    public float waitTimeAtPoint = 3f; // Tiempo de espera en cada punto
    private bool isWaiting;

    [Header("Detección y Combate")]
    public LayerMask targetLayer;      // Layer del equipo contrario
    public float visionRadius = 30f;   // Distancia para ver enemigos
    public float shootingRadius = 15f; // Distancia para empezar a disparar
    public float timeBetweenShoots = 1f;
    private bool alreadyAttacked;

    [Header("Referencias")]
    public NavMeshAgent agent;
    public Transform lookPoint;
    public GameObject shootingRaycastArea;
    public Transform spawnPoint;
    public Animator anim;

    private Transform currentTarget;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;
        agent.speed = movementSpeed;

        // Ajuste inicial al NavMesh para evitar el error de "SetDestination"
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 3.0f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
    }

    private void Update()
    {
        if (!agent.isOnNavMesh) return;

        // 1. Buscar enemigos cercanos constantemente
        FindVisibleTarget();

        if (currentTarget != null)
        {
            // Lógica de Combate
            float distance = Vector3.Distance(transform.position, currentTarget.position);

            if (distance > shootingRadius)
                PursueTarget();
            else
                AttackTarget();
        }
        else
        {
            // Lógica de Patrulla Aleatoria
            PatrolLogic();
        }
    }

    private void FindVisibleTarget()
    {
        // Escanea el área buscando objetos en la capa enemiga
        Collider[] targetsInView = Physics.OverlapSphere(transform.position, visionRadius, targetLayer);

        if (targetsInView.Length > 0)
        {
            currentTarget = targetsInView[0].transform;
        }
        else
        {
            currentTarget = null;
        }
    }

    private void PatrolLogic()
    {
        // Si llegamos al punto de patrulla y no estamos esperando, buscar otro
        if (agent.remainingDistance <= agent.stoppingDistance && !isWaiting)
        {
            StartCoroutine(GetNextPatrolPoint());
        }
    }

    IEnumerator GetNextPatrolPoint()
    {
        isWaiting = true;
        anim.SetBool("Running", false);
        anim.SetBool("Shooting", false);

        yield return new WaitForSeconds(waitTimeAtPoint);

        // Buscar un punto aleatorio en el NavMesh
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, 1))
        {
            agent.SetDestination(hit.position);
            anim.SetBool("Running", true);
        }

        isWaiting = false;
    }

    private void PursueTarget()
    {
        isWaiting = false; // Interrumpir espera si ve a alguien
        agent.SetDestination(currentTarget.position);
        anim.SetBool("Running", true);
        anim.SetBool("Shooting", false);
    }

    private void AttackTarget()
    {
        agent.SetDestination(transform.position); // Detenerse para disparar

        if (lookPoint != null)
            transform.LookAt(new Vector3(currentTarget.position.x, transform.position.y, currentTarget.position.z));

        if (!alreadyAttacked)
        {
            if (shootingRaycastArea != null)
            {
                RaycastHit hit;
                if (Physics.Raycast(shootingRaycastArea.transform.position, shootingRaycastArea.transform.forward, out hit, shootingRadius))
                {
                    // Intentar dañar al objetivo si tiene este mismo script
                    CharacterAI victim = hit.transform.GetComponent<CharacterAI>();
                    if (victim != null && victim.characterTeam != this.characterTeam)
                    {
                        victim.TakeDamage(giveDamage);
                    }
                }
            }

            anim.SetBool("Running", false);
            anim.SetBool("Shooting", true);

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenShoots);
        }
    }

    private void ResetAttack() => alreadyAttacked = false;

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0) StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        agent.enabled = false;
        anim.SetBool("Die", true);
        yield return new WaitForSeconds(8f);

        currentHealth = maxHealth;
        anim.SetBool("Die", false);

        if (spawnPoint != null) agent.Warp(spawnPoint.position);
        agent.enabled = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootingRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
    }
}