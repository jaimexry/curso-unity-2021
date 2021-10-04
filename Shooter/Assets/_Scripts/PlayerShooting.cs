using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerShooting : MonoBehaviour
{
   
    public GameObject shootingPoint;
    private Animator animator;
    private int bulletsAmount;
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
            animator.SetTrigger("Shoot");
            Invoke("FireBullet", 0.1f);
        }
    }

    void FireBullet()
    {
        GameObject bullet = ObjectPool.SharedInstance.GetFirstPooledObject(); 
        bullet.layer = LayerMask.NameToLayer("Player Bullet");
        bullet.transform.position = shootingPoint.transform.position; 
        bullet.transform.rotation = shootingPoint.transform.rotation; 
        bullet.SetActive(true);
        bulletsAmount--;
        onBulletChanged.Invoke();
    }
}
