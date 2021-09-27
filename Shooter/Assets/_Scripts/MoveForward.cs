using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveForward : MonoBehaviour
{
    [Tooltip("Velocidad de Movimiento del Objeto en m/s")]
    public float speed;
    void Update()
    {
        transform.Translate(0, 0, speed * Time.deltaTime);
    }
}
