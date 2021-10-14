using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerShooting : MonoBehaviour
{
    private Animator animator;
    private int bulletsAmount;

    public Weapon weapon;
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
            FireBullet();
        }
    }

    void FireBullet()
    {
        if (weapon.ShootBullet("Player Bullet"))
        {
            animator.SetTrigger("Shoot");
            bulletsAmount--;
        }
        
        if (bulletsAmount < 0)
        {
            bulletsAmount = 0;
        }
        onBulletChanged.Invoke();
    }
}
