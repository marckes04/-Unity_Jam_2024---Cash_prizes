using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : MonoBehaviour
{
    [Header("Rifle Settings")]
    public Camera cam;
    public float giveDamage = 10f;
    public float shootingRange = 100f;
    public float fireCharge = 15f;
    public PlayerMovement player; // Asegúrate de que este script exista
    public Animator animator;

    [Header("Ammo Settings")]
    private float nextTimeToShoot = 0f;
    [SerializeField] private int maximumAmmunition = 20;
    [SerializeField] private int mag = 15; // Cantidad de cargadores
    private int presentAmmunition;
    public float reloadingTime = 1.3f;
    private bool isReloading = false;

    [Header("Rifle Effects")]
    public GameObject muzzleSpark;
    public GameObject woodEffect;
    public GameObject goreEffect;

    private void Awake()
    {
        presentAmmunition = maximumAmmunition;
    }

    void Update()
    {
        if (isReloading) return;

        // Recarga automática si se acaba la munición
        if (presentAmmunition <= 0 && mag > 0)
        {
            StartCoroutine(Reload());
            return;
        }

        // Lógica de Disparo
        if (Input.GetButton("Fire1") && Time.time >= nextTimeToShoot && presentAmmunition > 0)
        {
            nextTimeToShoot = Time.time + 1f / fireCharge;
            Shoot();
        }

        // Control de Animaciones
        UpdateAnimations();
    }

    private void UpdateAnimations()
    {
        bool isFiring = Input.GetButton("Fire1");
        bool isWalking = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow) ||
                         Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D);
        bool isAiming = Input.GetButton("Fire2");

        animator.SetBool("Fire", isFiring && presentAmmunition > 0);
        animator.SetBool("Idle", !isFiring && !isWalking && !isAiming);
        animator.SetBool("FireWalk", isFiring && isWalking);
        animator.SetBool("IdleAim", isAiming);
        animator.SetBool("Walk", isWalking && !isFiring);

        // Desactivar flash si no se está disparando
        if (Input.GetButtonUp("Fire1") || presentAmmunition <= 0)
        {
            muzzleSpark.SetActive(false);
        }
    }

    void Shoot()
    {
        presentAmmunition--;

        // Activar efecto de disparo
        if (muzzleSpark != null) muzzleSpark.SetActive(true);

        RaycastHit hitInfo;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, shootingRange))
        {
            Debug.Log("Impacto en: " + hitInfo.transform.name);

            // 1. Impacto en Objetos
            Objects objects = hitInfo.transform.GetComponent<Objects>();
            if (objects != null)
            {
                objects.objectHitDamage(giveDamage);
                SpawnEffect(woodEffect, hitInfo);
                return; // Salimos para no procesar el resto
            }

            // 2. Impacto en IA (Usando el script único CharacterAI)
            CharacterAI enemy = hitInfo.transform.GetComponent<CharacterAI>();
            if (enemy != null)
            {
                // CORRECCIÓN: Llamamos al método de la instancia 'enemy', no de la clase
                enemy.TakeDamage(giveDamage);
                SpawnEffect(goreEffect, hitInfo);
            }
        }
    }

    // Método auxiliar para instanciar efectos
    void SpawnEffect(GameObject effectPrefab, RaycastHit hit)
    {
        if (effectPrefab != null)
        {
            GameObject impactGo = Instantiate(effectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(impactGo, 1f);
        }
    }

    IEnumerator Reload()
    {
        isReloading = true;
        mag--; // Consumir un cargador

        // Detener al jugador si es necesario
        if (player != null)
        {
            player.playerSpeed = 0f;
            player.playerSprint = 0f;
        }

        animator.SetBool("Reloading", true);
        muzzleSpark.SetActive(false);

        yield return new WaitForSeconds(reloadingTime);

        animator.SetBool("Reloading", false);
        presentAmmunition = maximumAmmunition;

        // Restaurar velocidad
        if (player != null)
        {
            player.playerSpeed = 1.9f;
            player.playerSprint = 3f;
        }

        isReloading = false;
    }
}