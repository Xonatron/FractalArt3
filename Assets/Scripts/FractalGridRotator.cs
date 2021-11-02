// FractalGridRotator.cs
// Rotates grid of fractal cubes

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractalGridRotator : MonoBehaviour
{
    [Header("Fractal Grid Settings")]
    [Tooltip("Units = degrees / second")]
    public float rotationSpeed; // degrees / second
    public float rotationSpeedRandomVariance; // vary rotation speed up to this percent (0.01 = 1%)

    // Start is called before the first frame update
    void Start()
    {
        rotationSpeed = MathV10.RandomVariance(rotationSpeed, rotationSpeedRandomVariance); // randomize rotation speed
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
}
