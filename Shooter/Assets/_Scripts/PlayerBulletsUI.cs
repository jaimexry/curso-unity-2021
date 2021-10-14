using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class PlayerBulletsUI : MonoBehaviour
{
   private TextMeshProUGUI _text;
   public PlayerShooting targetShooting;
   private void Awake()
   {
      _text = GetComponent<TextMeshProUGUI>();
   }

   private void Start()
   {
      targetShooting.onBulletChanged.AddListener(ReloadBulletText);
      _text.text = string.Format("{0}/{1}", targetShooting.maxBulletsAmount, targetShooting.maxBulletsAmount); 
   }

   private void ReloadBulletText()
   {
      _text.text = string.Format("{0}/{1}", targetShooting.BulletsAmount, targetShooting.maxBulletsAmount); 
   }
}
