using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCanvas : MonoBehaviour
{
    // Start is called before the first frame update
    private Canvas _canvas;
    private Camera camera;
    void Start()
    {
        camera = Camera.main;
        _canvas = GetComponent<Canvas>();
        _canvas.worldCamera = camera;
    }

    private void Update()
    {
        _canvas.transform.LookAt(camera.transform.position);
    }
}
