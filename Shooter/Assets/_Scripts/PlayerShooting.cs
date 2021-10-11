using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerShooting : MonoBehaviour
{
    public ParticleSystem fireEffect;
    public AudioSource shotSound;
    public GameObject shootingPoint;
    private Animator animator;
    private int bulletsAmount;
    private float lastShootTime;
    public float fireRate;
    public ObjectPool bulletPool;
    public ObjectPool shotVFXPool;
    public int BulletsAmount
    {
        get => bulletsAmount;
        set
        {
            bulletsAmount = value;
            if (bulletsAmount < 0)
            {
                bulletsAmount = 0;
            }
        }
    }

    public int maxBulletsAmount;
    public UnityEvent onBulletChanged;
    private void Start()
    {
        animator = GetComponent<Animator>();
        bulletsAmount = maxBulletsAmount;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && bulletsAmount > 0 && Time.timeScale > 0)
        {
            var timeSinceLastShoot = Time.time - lastShootTime;
            if (timeSinceLastShoot < fireRate)
            {
                return;
            }

            lastShootTime = Time.time;
            animator.SetTrigger("Shoot");
            Invoke("FireBullet", 0.1f);
        }
    }

    void FireBullet()
    {
        GameObject bullet = bulletPool.GetFirstPooledObject(); 
        bullet.layer = LayerMask.NameToLayer("Player Bullet");
        bullet.transform.position = shootingPoint.transform.position; 
        bullet.transform.rotation = shootingPoint.transform.rotation; 
        bullet.SetActive(true);
        bulletsAmount--;
        fireEffect.Play();
        GameObject shotVFX = shotVFXPool.GetFirstPooledObject();
        shotVFX.transform.position = shootingPoint.transform.position;
        shotVFX.transform.rotation = shootingPoint.transform.rotation;
        shotVFX.SetActive(true);
        shotVFX.GetComponent<AudioSource>().Play();
        //shotSound.Play();
        if (bulletsAmount < 0)
        {
            bulletsAmount = 0;
        }
        onBulletChanged.Invoke();
    }
}
