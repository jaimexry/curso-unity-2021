using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private ObjectPool bulletPool;
    private ObjectPool shotFSXPool;
    public ParticleSystem fireEffect;
    public float shootRate;
    public GameObject shootingPoint;
    private float lastShotTime;

    private void Start()
    {
        bulletPool = GameObject.Find("BulletPooling").GetComponent<ObjectPool>();
        shotFSXPool = GameObject.Find("ShotVFXPooling").GetComponent<ObjectPool>();
    }

    public bool ShootBullet(string layerName)
    {
        if (Time.timeScale > 0)
        {
            var timeSinceLastShot = Time.time - lastShotTime;
            if (timeSinceLastShot < shootRate)
            {
                return false;
            }
            
            lastShotTime = Time.time;
            var bullet = bulletPool.GetFirstPooledObject();
            
            bullet.layer = LayerMask.NameToLayer(layerName);
            bullet.transform.position = shootingPoint.transform.position;
            bullet.transform.rotation = shootingPoint.transform.rotation;
            bullet.SetActive(true);

            var shotSFX = shotFSXPool.GetFirstPooledObject();
            shotSFX.transform.position = shootingPoint.transform.position;
            shotSFX.transform.rotation = shootingPoint.transform.rotation;
            shotSFX.SetActive(true);
            shotSFX.GetComponent<AudioSource>().Play();
            fireEffect.Play();

            return true;
        }
        return false;
    }
}
