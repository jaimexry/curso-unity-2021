using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
   [Tooltip("Cantidad de Puntos que se obtiene al derrotar al Enemigo")]
   public int pointsAmount = 10;

   private void Start()
   {
      EnemyManager.SharedInstance.enemies.Add(this);
   }

   private void OnDestroy()
   {
      EnemyManager.SharedInstance.enemies.Remove(this);
      ScoreManager.SharedInstance.Amount += pointsAmount;
   }
}
