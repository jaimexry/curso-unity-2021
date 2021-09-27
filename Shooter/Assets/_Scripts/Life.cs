using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Life : MonoBehaviour
{
    [SerializeField]
    private float amount;

    public float Amount
    {
        get => amount;
        set
        {
            amount = value;
            if (amount <= 0)
            {
                Animator anim = GetComponent<Animator>();
                anim.SetTrigger("Die");
                Invoke("PlayDestruction", 1f);
                Destroy(gameObject, 1.2f);
            }
        } 
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
