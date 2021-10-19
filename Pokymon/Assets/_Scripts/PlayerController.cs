using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    private bool isMoving;
    public float speed;
    private Vector2 input;

    private Animator _animator;
    public LayerMask solidObjectsLayer;
    public LayerMask pokymonLayer;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0)
            {
                input.y = 0;
            }
            if (input != Vector2.zero)
            {
                _animator.SetFloat("MoveX", input.x);
                _animator.SetFloat("MoveY", input.y);
                
                var targetPosition = transform.position;
                targetPosition.x += input.x;
                targetPosition.y += input.y;
                
                if (IsAvailable(targetPosition))
                {
                    StartCoroutine(MoveTowards(targetPosition));
                }
            }
        }
    }

    private void LateUpdate()
    {
            _animator.SetBool("IsMoving", isMoving);
    }

    private IEnumerator MoveTowards(Vector3 destination)
    {
        isMoving = true;
        while (Vector3.Distance(transform.position, destination) > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            yield return null;
        }

        transform.position = destination;
        isMoving = false;
        
        CheckForPokymon();
    }
    
    /// <summary>
    /// Comprueba que la zona a la que queremos acceder esté disponible
    /// </summary>
    /// <param name="target">Zona a la que queremos acceder</param>
    /// <returns>True si el target está disponible y False en caso contrario</returns>
    private bool IsAvailable(Vector3 target)
    {
        if (Physics2D.OverlapCircle(target, 0.25f, solidObjectsLayer) != null)
        {
            return false;
        }

        return true;
    }

    private void CheckForPokymon()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.25f, pokymonLayer) != null)
        {
            if (Random.Range(0, 100) < 10)
            {
                Debug.Log("Empezar Batalla Pokemon");
            }
        }
    }
}
