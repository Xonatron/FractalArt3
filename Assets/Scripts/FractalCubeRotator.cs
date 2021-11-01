using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractalCubeRotator : MonoBehaviour
{
    [Header("Fractal Cube Settings")]
    [Tooltip("Units = degrees / second")]
    public float rotationSpeed; // in degrees/second

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
}
