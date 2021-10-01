using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
   [Tooltip("Cantidad de Puntos que se obtiene al derrotar al Enemigo")]
   public int pointsAmount = 10;

   private void Awake()
   {
      var life = GetComponent<Life>();
      life.onDeath.AddListener(DestroyEnemy);
   }

   private void Start()
   {
      EnemyManager.SharedInstance.AddEnemy(this);
   }

   public void DestroyEnemy()
   {
      Animator anim = GetComponent<Animator>();
      anim.SetTrigger("Die");
      Invoke("PlayDestruction", 1f);
      Destroy(gameObject, 1.2f);
      
      EnemyManager.SharedInstance.RemoveEnemy(this);
      ScoreManager.SharedInstance.Amount += pointsAmount;
   }

   private void OnDestroy()
   {
      var life = GetComponent<Life>();
      life.onDeath.RemoveListener(DestroyEnemy);
   }

   void PlayDestruction()
   {
      ParticleSystem explosion = GetComponentInChildren<ParticleSystem>();
      if (explosion != null)
      {
         explosion.Play();
      }
   }
}
