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
