// DebugGUI v2.0
// -------------
// October 26th, 2021
// Matthew Doucette, Xona Games
// http://xona.com/
//
// Versions Explained:
// -------------------
// v0.1: GUI events are handled in GameManager.cs (had not grown enough to gradate to its own class)
// v1.0: GUI events are handled in DebugGUI.cs (notably without a version number)
//       NOTE: There are variations in all versions we now call v1.0.
//       NOTE: One version of v1.0 had shadow text, which was ignored in v2.0 as aesthetic text output is not its purpose.
// v2.0: GUI events are handled in DebugGUIv2.0.cs
//       NOTE: Plan is to expand this with v2.01, v2.1, v2.11, v2.2, etc. as it expands.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; // Math.*
using System.Linq; // Enumerable Class: .Min(), .Max(), .Average(), etc.

public class DebugGUIV20 : MonoBehaviour
{
    // render texture
    public RenderTexture renderTexture;

    // gui font
    Font guiFont;
    const float margin = 10.0f; // in pixels
    const float fontHeightBuffer = 5.0f; // in pixels; add to "guiFont.lineHeight"; value depends on font size (why is this needed at all???)

    // gui font rectangle
    Rect rectLeft = new Rect(); // upper left
    Rect rectRight = new Rect(); // upper right

    // gui fps
    const int fpsBufferSize = 60; // 60 frame fps buffer
    float[] fpsBuffer = new float[fpsBufferSize]; // fps buffer; used for min/max/average
    int fpsIndex = 0; // 0..9; index pointer that cycles through fps buffer
    float fpsCurr;
    float fpsCurrFroze; // snapshot a fps read and freeze it on the screen (until buffer updates; currently every 60 frames)
    float fpsMin; // relating to fps buffer
    float fpsMax; // relating to fps buffer
    float fpsAvg; // relating to fps buffer

    // aspect ratio
    int aspectRatioWidth = 0; // 0 = temp value
    int aspectRatioHeight = 0; // 0 = temp value
    // TODO - CREATE AN ASPECT RATIO STRUCT to store width, height, and ratio

    // Start is called before the first frame update
    void Start()
    {
        // gui depth
        GUI.depth = 0; // larger = further away

        // initialize gui font
        guiFont = Font.CreateDynamicFontFromOSFont("Arial", 14);
        //guiFont = Font.CreateDynamicFontFromOSFont("Arial", 28);
        //guiFont = Font.CreateDynamicFontFromOSFont("Courier New", 16);
        //guiFont = Font.CreateDynamicFontFromOSFont("Lucida Sans Typewriter", 10);
        //guiFont = Font.CreateDynamicFontFromOSFont("Lucida Sans Typewriter", 12);
        // https://support.microsoft.com/en-us/help/90057/microsoft-supplied-monospaced-truetype-fonts

        // initialize fps buffer
        for (int i = 0; i < fpsBufferSize; i++)
        {
            fpsBuffer[i] = 60.0f; // initialize to 60 fps
        }
    }

    // Update is called once per frame
    void Update()
    {
        // calculate fps stats
        fpsBuffer[fpsIndex] = FPS();
        fpsCurr = fpsBuffer[fpsIndex];
        fpsCurrFroze = fpsBuffer[0];
        fpsAvg = fpsBuffer.Average(); // https://msdn.microsoft.com/en-us/library/bb354760(v=vs.110).aspx
        fpsMin = fpsBuffer.Min(); // https://msdn.microsoft.com/en-us/library/system.linq.enumerable.min(v=vs.110).aspx
        fpsMax = fpsBuffer.Max(); // https://msdn.microsoft.com/en-us/library/system.linq.enumerable.max(v=vs.110).aspx
        IncFPSIndex(); // increment buffer pointer for next frame

        // aspect ratio (why in update? aspect ratio can change in Unity editor / build)
        CalculateScreenAspectRatio(ref aspectRatioWidth, ref aspectRatioHeight);
    }

    void OnGUI()
    {
        // gui font
        GUI.skin.font = guiFont;

        // gui color
        GUI.contentColor = Color.white;
        //GUI.contentColor = Color.green;
        //GUI.contentColor = Color.black;

        // gui rectangles
        rectLeft.x = margin;
        rectLeft.y = margin;
        rectLeft.width = Screen.width - margin * 2.0f; // width of screen minus margin? too short will cut off text
        rectLeft.height = GUI.skin.font.lineHeight + fontHeightBuffer; // add buffer else will cut off text
        rectRight = rectLeft;

        //Rect rect = new Rect();
        //rect.x = 10.0f;
        //rect.y = 10.0f;
        //rect.width = Screen.width; // too short will cut off long text
        //rect.height = Screen.height; // too short will cut off
        //float crHeight = 15.0f; // carriage return (height) -- CAN WE DESIGN THIS TO CALCULATED AUTOMATICALLY FROM FONT SIZE???

        // aspect ratio
        int aspectRatioWidth = Screen.width; // temp value
        int aspectRatioHeight = Screen.height; // temp value
        CalculateScreenAspectRatio(ref aspectRatioWidth, ref aspectRatioHeight);

        // fps
        //float fpsCurrent = FPS();
        //float fpsCurrentFixed = FPSPhysics();
        //if (fpsCurrent > fpsHighest) fpsHighest = fpsCurrent;
        //if (fpsCurrent < fpsLowest) fpsLowest = fpsCurrent;

        // gui output
        GUI.skin.label.alignment = TextAnchor.UpperLeft; // default
        // - app/credit stats
        GUI.Label(rectLeft, "\"" + Application.productName + "\" v" + Application.version); CRLeft();
        GUI.Label(rectLeft, "Matthew Doucette, " + Application.companyName); CRLeft();
        GUI.Label(rectLeft, "http://xona.com/fractal/"); CRLeft();
        GUI.Label(rectLeft, ""); CRLeft();
        GUI.Label(rectLeft, "Unity " + Application.unityVersion); CRLeft();
        GUI.Label(rectLeft, "Build: " + (Debug.isDebugBuild ? "Debug" : "Release")); CRLeft();
        GUI.Label(rectLeft, "Genuine: " + (Application.genuine ? "Yes (Unaltered)" : "No (Altered)")); CRLeft();
        GUI.Label(rectLeft, System.DateTime.Now.ToString("dddd, MMMM d, yyyy, h:mm:ss.fff tt")); CRLeft();
        GUI.Label(rectLeft, ""); CRLeft();
        GUI.Label(rectLeft, "Resolution: " + Screen.width + "x" + Screen.height); CRLeft();
        GUI.Label(rectLeft, "Aspect Ratio: " + aspectRatioWidth + ":" + aspectRatioHeight + ": " + ((float)Screen.width / (float)Screen.height).ToString("0.000") + ":1"); CRLeft();
        // - app specific stats
        GUI.Label(rectLeft, "Render Texture Size: " + renderTexture.width + "x" + renderTexture.height); CRLeft();
        GUI.Label(rectLeft, ""); CRLeft();
        // - user controls
        GUI.Label(rectLeft, "Press ESC/ALT-F4 to quit..."); CRLeft();
        // - fps stats
        GUI.skin.label.alignment = TextAnchor.UpperRight;
        GUI.Label(rectRight, "FPS: " + fpsCurr.ToString("0.0")); CRRight();
        GUI.Label(rectRight, "(froze) " + fpsCurrFroze.ToString("0.0")); CRRight();
        GUI.Label(rectRight, "(avg) " + fpsAvg.ToString("0.0")); CRRight();
        GUI.Label(rectRight, "(min) " + fpsMin.ToString("0.0")); CRRight();
        GUI.Label(rectRight, "(max) " + fpsMax.ToString("0.0")); CRRight();
        GUI.Label(rectRight, "(from last " + fpsBufferSize + " frames)"); CRRight();
        GUI.Label(rectRight, "Time Scale: " + Time.timeScale.ToString("0.0") + "x"); CRRight();
    }

    private void CRLeft()
    {
        // gui carriage return
        rectLeft.y += CRHeight();
    }

    private void CRRight()
    {
        // gui carriage return
        rectRight.y += CRHeight();
    }

    private float CRHeight()
    {
        return GUI.skin.font.lineHeight;
    }

    private void IncFPSIndex()
    {
        // 0..59, if buffer size = 60
        fpsIndex++; // increment index
        if (fpsIndex >= fpsBufferSize) fpsIndex = 0; // wrap index to 0
    }

    // frames per second notes:
    // ------------------------
    // Time.deltaTime is AFFECTED by Time.timeScale
    // Time.unscaledDeltaTime is NOT affected by Time.timeScale
    // Time.fixedDeltaTime is AFFECTED by Time.timeScale

    // rendering frames/second
    float FPS() // NOT affected by Time.timeScale
    {
        // catch divide by zero
        if (Time.unscaledDeltaTime != 0.0f)
            return (1.0f / Time.unscaledDeltaTime);
        else
            return (0.0f);
    }
    
    // physics frames/second
    float FPSPhysics() // AFFECTED by Time.timeScale
    {
        // catch divide by zero
        if (Time.fixedDeltaTime != 0.0f)
            return (1.0f / Time.deltaTime);
        else
            return (0.0f);
    }

    private void CalculateScreenAspectRatio(ref int aspectRatioWidth, ref int aspectRatioHeight)
    {
        // calculate aspect ratio of screen resolution:
        // 1) calculate greatest commmon divsor (GCD) of screen width and height
        // 2) divide screen width and height by (GCD)
        // 3) adjust to common aspect ratios: e.g. 8x5 --> 16:10

        // 1) calculate greatest commmon divsor (GCD) of screen width and height
        int gcd = GCD(Screen.width, Screen.height);

        // 2) divide screen width and height by (GCD)
        aspectRatioWidth = Screen.width / gcd;
        aspectRatioHeight = Screen.height / gcd;
        Debug.Assert(Screen.width % gcd == 0);
        Debug.Assert(Screen.height % gcd == 0);

        // 3) adjust to common aspect ratios:
        // 8:5 --> 16:10
        if ((aspectRatioWidth == 8) && (aspectRatioHeight == 5))
        {
            aspectRatioWidth = 16; // 16:10 (accurate)
            aspectRatioHeight = 10; // 16:10 (accurate)
        }
        // 1366x768 --> 16:9 
        if ((aspectRatioWidth == 683) && (aspectRatioHeight == 384))
        {
            aspectRatioWidth = 16; // 16:9 (inaccurate) -- is there a way to catch these in calculations?!
            aspectRatioHeight = 9; // 16:9 (inaccurate) -- is there a way to catch these in calculations?!
        }
    }

    private int GCD(int a, int b)
    {
        int gcd = 1; // set gcd at 1
        int minDistance = Math.Min(a, b); // only try to divide up to the smaller number
        for (int i = 2; i < minDistance; i++) // i = 2..smallest_number-1; e.g. 1920x1080 --> 2..1079
        {
            // if a (larger) common divisor is found...
            if ((Screen.width % i == 0) && (Screen.height % i == 0))
            {
                gcd = i; // ...update gcd
            }
        }
        return gcd;
    }
}
