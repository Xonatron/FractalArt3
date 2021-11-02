using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRainbowBackground : MonoBehaviour
{
    // hsv color format
    [Header("HSV Colors:")]
    [Tooltip("Period, in seconds.")]
    public float period; // in seconds; 1/frequency
    public float h; // 0.0..1.0 hue -- start at red
    public float s; // 0.0..1.0 saturation
    public float v; // 0.0..1.0 value
    float frequency; // 1/period

    // Start is called before the first frame update
    void Start()
    {
        // init
        frequency = 1.0f / period;
    }

    // Update is called once per frame
    void Update()
    {
        // calculate next rainbow color
        h += frequency * Time.deltaTime; // hue increment
        if (h > 1.0) h -= 1.0f; // clamp to 0.0..1.0

        // convert HSV color to RGB
        Color newRainbowColor = Color.HSVToRGB(h, s, v);

        // apply to color
        //gameObject.transform.GetComponent<Renderer>().material.SetColor("_Color", newRainbowColor);
        gameObject.transform.GetComponent<Camera>().backgroundColor = newRainbowColor;
    }
}
