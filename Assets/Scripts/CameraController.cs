using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // camera z-position range:
    [Header("Camera z-position:")]
    public float minZ;
    public float maxZ;

    // sine wave
    [Header("Sine Wave:")]
    [Tooltip("Sine wave period, in seconds.")]
    public float period; // in seconds; sine wave period (1/frequency)
    public float periodRandomVariance; // vary period up to this percent (0.01 = 1%)
    float frequency; // sine wave frequency (1/period)
    float amplitude; // sine wave amplitude (half distance between min & max)
    float avgZ; // average (middle) z-value
    
    // Start is called before the first frame update
    void Start()
    {
        // init 
        period = MathV10.RandomVariance(period, periodRandomVariance); // randomize period
        frequency = 1.0f / period;
        amplitude = (maxZ - minZ) / 2.0f;
        avgZ = (maxZ + minZ) / 2.0f;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 position;
        position.x = 0.0f;
        position.y = 0.0f;
        position.z = avgZ + Mathf.Sin((Time.time * Mathf.PI * 2.0f) * frequency) * amplitude;
        gameObject.transform.position = position;
    }
}
