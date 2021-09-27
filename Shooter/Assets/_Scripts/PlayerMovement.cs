using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Tooltip("Fuerza de Movimiento del Personaje en N/s")]
    [Range(0, 1000)]
    public float speed;

    [Tooltip("Fuerza de Rotación del Personaje en N/s")]
    [Range(0, 360)]
    public float rotationSpeed;

    private float horizontal, vertical;
    private Rigidbody rb;
    private Vector3 direction;
    private float mouseX;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        direction = new Vector3(horizontal, 0, vertical);
        //transform.Translate(direction.normalized * space);
        
        mouseX = Input.GetAxis("Mouse X");
        //transform.Rotate(0, mouseX * angle, 0);
        /*
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            transform.Translate(0, 0, space);
        }else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            transform.Translate(0, 0, -space);
        }else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Translate(-space, 0, 0);
        }else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.Translate(space, 0, 0);
        }
        */
    }

    private void FixedUpdate()
    {
        rb.AddRelativeForce(direction.normalized * speed * Time.fixedDeltaTime);
        rb.AddRelativeTorque(0, mouseX * rotationSpeed * Time.fixedDeltaTime, 0);
    }
}
