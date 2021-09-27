using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
   
    public GameObject shootingPoint;
    
    void Update()
    {
        if (Input.GetButton("Fire1"))
        {
            GameObject bullet = ObjectPool.SharedInstance.GetFirstPooledObject();
            bullet.layer = LayerMask.NameToLayer("Player Bullet");
            bullet.transform.position = shootingPoint.transform.position;
            bullet.transform.rotation = shootingPoint.transform.rotation;
            bullet.SetActive(true);
        }
    }

   
}
