using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Sight))]
public class EnemyFSM : MonoBehaviour
{
    public enum EnemyState { GoToBase, AttackBase, ChasePlayer, AttackPlayer }

    public EnemyState currentState;
    private Sight _sight;

    private Transform baseTransform;
    public float baseAttackDistance;
    public float playerAttackDistance;

    private NavMeshAgent _agent;
    private ObjectPool bulletPool;
    private ObjectPool shotFSXPool;
    private float lastShotTime;
    public float shootRate;
    
    private void Awake()
    {
        _sight = GetComponent<Sight>();
        _agent = GetComponentInParent<NavMeshAgent>();
    }

    private void Start()
    {
        baseTransform = GameObject.FindWithTag("Base").transform;
        bulletPool = GameObject.Find("BulletPooling").GetComponent<ObjectPool>();
        shotFSXPool = GameObject.Find("ShotVFXPooling").GetComponent<ObjectPool>();
    }

    private void Update()
    {
        switch (currentState)
        {
            case EnemyState.GoToBase:
                GoToBase();
                break;
                
            case EnemyState.AttackBase:
                AttackBase();
                break;
            
            case EnemyState.ChasePlayer:
                ChasePlayer();
                break;
            
            case  EnemyState.AttackPlayer:
                AttackPlayer();
                break;
            
            default:
                //TODO: caso por defecto
                break;
        }
    }

    void GoToBase()
    {
        print("Ir a base");
        _agent.isStopped = false;
        _agent.SetDestination(baseTransform.position);
        
        if (_sight.detectedTarget != null)
        {
            currentState = EnemyState.ChasePlayer;
            return;
        }

        float distanceToBase = Vector3.Distance(transform.position, baseTransform.position);
        if (distanceToBase < baseAttackDistance)
        {
            currentState = EnemyState.AttackBase;
        }
    }

    void AttackBase()
    {
        print("Atacar la base enemiga");
        _agent.isStopped = true;
        LookAt(baseTransform.position);
        ShootTarget();
    }

    void ChasePlayer()
    {
        print("Perseguir al jugador");

        if (_sight.detectedTarget == null)
        {
            currentState = EnemyState.GoToBase;
            return;
        }

        _agent.isStopped = false;
        _agent.SetDestination(_sight.detectedTarget.transform.position);

        float distanceToPlayer = Vector3.Distance(transform.position, _sight.detectedTarget.transform.position);
        if (distanceToPlayer < playerAttackDistance)
        {
            currentState = EnemyState.AttackPlayer;
        }
    }

    void AttackPlayer()
    {
        print("Atacar al jugador");
        _agent.isStopped = true;

        if (_sight.detectedTarget == null)
        {
            currentState = EnemyState.GoToBase;
            return;
        }
        LookAt(_sight.detectedTarget.transform.position);
        ShootTarget();

        float distanceToPlayer = Vector3.Distance(transform.position, _sight.detectedTarget.transform.position);
        if (distanceToPlayer >= playerAttackDistance * 1.1f)
        {
            currentState = EnemyState.ChasePlayer;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, playerAttackDistance);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, baseAttackDistance);
    }

    void ShootTarget()
    {
        if (Time.timeScale > 0)
        {
            var timeSinceLastShot = Time.time - lastShotTime;
            if (timeSinceLastShot < shootRate)
            {
                return;
            }

            lastShotTime = Time.time;
            var bullet = bulletPool.GetFirstPooledObject();
            
            bullet.layer = LayerMask.NameToLayer("Enemy Bullet");
            bullet.transform.position = transform.position;
            bullet.transform.rotation = transform.rotation;
            bullet.SetActive(true);

            var shotSFX = shotFSXPool.GetFirstPooledObject();
            shotSFX.transform.position = transform.position;
            shotSFX.transform.rotation = transform.rotation;
            shotSFX.SetActive(true);
            shotSFX.GetComponent<AudioSource>().Play();
        }
    }

    void LookAt(Vector3 targetPos)
    {
        var directionToLook = Vector3.Normalize(targetPos - transform.position);
        directionToLook.y = 0;
        transform.parent.forward = directionToLook;
    }
}
