using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : MonoBehaviour
{
    [Header("Rifle")]
    public Camera cam;
    public float giveDamage = 10f;
    public float shootingRange = 100f;
    public float fireCharge = 15f;
    public PlayerMovement player;
    public Animator animator;

    [Header("Rifle Animation and shooting")]
    private float nextTimeToShoot = 0f;
    private int maximunAmunition = 20;
    private int mag = 15;
    private int presentAmunition;
    public float reloadingTime = 1.3f;
    private bool setRealoding = false;


    private void Awake()
    {
        presentAmunition = maximunAmunition;
    }

    void Update()
    {
        if (setRealoding)
            return;

        if(presentAmunition <=0)
        {
            StartCoroutine(Reload());
            return;
        }

        if(Input.GetButton("Fire1") && Time.time >=  nextTimeToShoot)
        {
            animator.SetBool("Fire", true);
            animator.SetBool("Idle",false);
            nextTimeToShoot = Time.time + 1f / fireCharge;
            Shoot();
        }
        else if(Input.GetButton("Fire1") && Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            animator.SetBool("Idle", false);
            animator.SetBool("FireWalk",true);
            
        }

        else if(Input.GetButton("Fire1") && Input.GetButton("Fire2"))
        {
            animator.SetBool("Idle", false);
            animator.SetBool("IdleAim", true);
            animator.SetBool("FireWalk", true);
            animator.SetBool("Walk", true);
            animator.SetBool("Reloading", false);
        }
        else
        {
            animator.SetBool("Fire", false);
            animator.SetBool("Idle", true);
            animator.SetBool("FireWalk", false);
        }
    }

    void Shoot()
    {
        if (mag==0)
        {
            // show text

        }

        presentAmunition--;

        if(presentAmunition == 0)
        {
            mag--;
        }

        RaycastHit hitInfo;
        if(Physics.Raycast(cam.transform.position,cam.transform.forward, out hitInfo, shootingRange))
        {
            Debug.Log(hitInfo.transform.name);
            Objects objects = hitInfo.transform.GetComponent<Objects>();

            if (objects != null) 
            { 
                objects.objectHitDamage(giveDamage);
            }
        }
    }

    IEnumerator Reload()
    {
        player.playerSpeed = 0f;
        player.playerSprint = 0f;
        setRealoding = true;
        Debug.Log("Reloading..");
        animator.SetBool("Reloading",true);
        yield return new WaitForSeconds(reloadingTime);
        // animations
        animator.SetBool("Reloading", false);
        presentAmunition = maximunAmunition;
        player.playerSpeed = 1.9f;
        player.playerSprint = 3f;
        setRealoding = false;
    }
}
