using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private bool rotate;

    private void Update()
    {
       if(rotate) transform.Rotate(Vector3.up * speed * Time.deltaTime); 
    }
}
