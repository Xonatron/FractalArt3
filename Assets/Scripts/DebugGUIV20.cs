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
    // gui enable/disable toggle
    bool GUIenabled; // default: debug = true, release = false

    // render texture
    public RenderTexture renderTexture;

    // gui font
    Font guiFont;
    const float margin = 10.0f; // in pixels
    const float fontHeightBuffer = 5.0f; // in pixels; add to "guiFont.lineHeight"; value depends on font size (why is this needed at all???)

    // gui font rectangles
    Rect upperLeft = new Rect(); // upper left
    Rect upperRight = new Rect(); // upper right
    // /-------------------   -------------------\
    // | [text goes here]       [text goes here] |
    // | [text goes here]       [text goes here] |
    // | [text goes here]       [text goes here] |
    // |                                         |

    // fps
    [Header("FPS Settings")]
    [Tooltip("FPS sample size (60 = sample FPS for 60 frames).")]
    public int fpsBufferSize = 60; // number of samples in buffer; if 60, then 60 samples across 60 frames
    struct FPS
    {
        public float[] buffer; // fps sample buffer (used to calculate various fps stats)
        public int index; // index pointer that cycles through fps buffer; 0..59 if 60 samples
        public float curr; // current fps
        public float currFroze; // snapshot of current fps (until sample buffer updates; every 60 frames if 60 samples)
        public float min; // minimum fps in buffer samples
        public float max; // maximum fps in buffer samples
        public float avg; // average fps in buffer samples
    }
    FPS fps;

    // aspect ratio
    struct AspectRatio
    {
        public int width;
        public int height;
        public float ratio;
    }
    AspectRatio aspectRatio;

    // Start is called before the first frame update
    void Start()
    {
        // gui enable/disable toggle
        GUIenabled = Debug.isDebugBuild; // default: debug = true, release = false

        // fps
        fps.index = 0; // start fps sample buffer at 0 (0..59 if 60 samples in buffer)
        fps.buffer = new float[fpsBufferSize]; // fps buffer; used for min/max/average

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
            fps.buffer[i] = 60.0f; // initialize to 60 fps
        }
    }

    // Update is called once per frame
    void Update()
    {
        // process user input
        if (Input.GetKeyDown(KeyCode.F3))
        {
            GUIenabled = !GUIenabled; // toggle
        }

        CalculateFPSStats(); // calculate fps stats
        aspectRatio = CalculateScreenAspectRatio(); // calculate aspect ratio (every frame as it can update in real-time in Unity editor)
    }

    void CalculateFPSStats()
    {
        fps.buffer[fps.index] = GetFPS();
        fps.curr = fps.buffer[fps.index];
        fps.currFroze = fps.buffer[0]; // can freeze on any sample buffer, might as well freeze on first
        fps.avg = fps.buffer.Average(); // https://msdn.microsoft.com/en-us/library/bb354760(v=vs.110).aspx
        fps.min = fps.buffer.Min(); // https://msdn.microsoft.com/en-us/library/system.linq.enumerable.min(v=vs.110).aspx
        fps.max = fps.buffer.Max(); // https://msdn.microsoft.com/en-us/library/system.linq.enumerable.max(v=vs.110).aspx
        IncFPSIndex(); // increment buffer pointer for next frame
    }

    private AspectRatio CalculateScreenAspectRatio()
    {
        // calculate aspect ratio of screen resolution:
        // 1) calculate greatest commmon divsor (GCD) of screen width and height
        // 2) divide screen width and height by (GCD)
        // 3) adjust to common aspect ratios: e.g. 8x5 --> 16:10
        // 4) calculate ratio (width/height)

        AspectRatio aspectRatio;

        // 1) calculate greatest commmon divsor (GCD) of screen width and height
        int gcd = GCD(Screen.width, Screen.height);
        // TODO - only call this when screen resolution updates (or cache results)

        // 2) divide screen width and height by (GCD)
        aspectRatio.width = Screen.width / gcd;
        aspectRatio.height = Screen.height / gcd;
        Debug.Assert(Screen.width % gcd == 0);
        Debug.Assert(Screen.height % gcd == 0);

        // 3) adjust to common aspect ratios:
        // 8:5 --> 16:10
        if ((aspectRatio.width == 8) && (aspectRatio.height == 5))
        {
            aspectRatio.width = 16; // 16:10 (accurate)
            aspectRatio.height = 10; // 16:10 (accurate)
        }
        // 1366x768 --> 16:9 (inaccurate)
        if ((aspectRatio.width == 683) && (aspectRatio.height == 384))
        {
            aspectRatio.width = 16; // 16:9 (inaccurate) -- is there a way to catch these in calculations?!
            aspectRatio.height = 9; // 16:9 (inaccurate) -- is there a way to catch these in calculations?!
        }

        // 4) calculate ratio
        aspectRatio.ratio = ((float)Screen.width / (float)Screen.height);

        return aspectRatio;
    }

    private int GCD(int a, int b) // greatest common divisor
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

    void OnGUI()
    {
        // enabled/disabled
        if (!GUIenabled) return;

        // gui font
        GUI.skin.font = guiFont;

        // gui color
        GUI.contentColor = Color.white;
        //GUI.contentColor = Color.green;
        //GUI.contentColor = Color.black;

        // gui rectangles
        upperLeft.x = margin;
        upperLeft.y = margin;
        upperLeft.width = Screen.width - margin * 2.0f; // width of screen minus margin? too short will cut off text
        upperLeft.height = GUI.skin.font.lineHeight + fontHeightBuffer; // add buffer else will cut off text
        upperRight = upperLeft;

        // gui output
        GUI.skin.label.alignment = TextAnchor.UpperLeft; // default
        // - app/credit stats
        GUI.Label(upperLeft, "\"" + Application.productName + "\" v" + Application.version); CRLeft();
        GUI.Label(upperLeft, "Matthew Doucette, " + Application.companyName); CRLeft();
        GUI.Label(upperLeft, "xona.com/fractal"); CRLeft();
        GUI.Label(upperLeft, ""); CRLeft();
        GUI.Label(upperLeft, "Unity " + Application.unityVersion); CRLeft();
        GUI.Label(upperLeft, "Build: " + (Debug.isDebugBuild ? "Debug" : "Release")); CRLeft();
        GUI.Label(upperLeft, "Genuine: " + (Application.genuine ? "Yes (Unaltered)" : "No (Altered)")); CRLeft();
        GUI.Label(upperLeft, System.DateTime.Now.ToString("dddd, MMMM d, yyyy, h:mm:ss.fff tt")); CRLeft();
        GUI.Label(upperLeft, ""); CRLeft();
        GUI.Label(upperLeft, "Resolution: " + Screen.width + "x" + Screen.height); CRLeft();
        GUI.Label(upperLeft, "Aspect Ratio: " + aspectRatio.width + ":" + aspectRatio.height + ": " + aspectRatio.ratio.ToString("0.000") + ":1"); CRLeft();
        // - app specific stats
        GUI.Label(upperLeft, "Render Texture Size: " + renderTexture.width + "x" + renderTexture.height); CRLeft();
        GUI.Label(upperLeft, ""); CRLeft();
        // - user controls
        GUI.Label(upperLeft, "Press ESC/ALT-F4 to quit..."); CRLeft();
        // - fps stats
        GUI.skin.label.alignment = TextAnchor.UpperRight;
        GUI.Label(upperRight, "FPS: " + fps.curr.ToString("0.0")); CRRight();
        GUI.Label(upperRight, "(froze) " + fps.currFroze.ToString("0.0")); CRRight();
        GUI.Label(upperRight, "(avg) " + fps.avg.ToString("0.0")); CRRight();
        GUI.Label(upperRight, "(min) " + fps.min.ToString("0.0")); CRRight();
        GUI.Label(upperRight, "(max) " + fps.max.ToString("0.0")); CRRight();
        GUI.Label(upperRight, "(from last " + fpsBufferSize + " frames)"); CRRight();
        GUI.Label(upperRight, "Time Scale: " + Time.timeScale.ToString("0.0") + "x"); CRRight();
    }

    private void CRLeft()
    {
        // gui carriage return
        upperLeft.y += CRHeight();
    }

    private void CRRight()
    {
        // gui carriage return
        upperRight.y += CRHeight();
    }

    private float CRHeight()
    {
        return GUI.skin.font.lineHeight;
    }

    private void IncFPSIndex()
    {
        // 0..59, if buffer size = 60
        fps.index++; // increment index
        if (fps.index >= fpsBufferSize) fps.index = 0; // wrap index to 0
    }

    // frames per second notes:
    // ------------------------
    // Time.deltaTime is AFFECTED by Time.timeScale
    // Time.unscaledDeltaTime is NOT affected by Time.timeScale
    // Time.fixedDeltaTime is AFFECTED by Time.timeScale

    // rendering frames/second
    float GetFPS() // NOT affected by Time.timeScale
    {
        // catch divide by zero
        if (Time.unscaledDeltaTime != 0.0f)
            return (1.0f / Time.unscaledDeltaTime);
        else
            return (0.0f);
    }
    
    // physics frames/second
    float GetFPSPhysics() // AFFECTED by Time.timeScale
    {
        // catch divide by zero
        if (Time.fixedDeltaTime != 0.0f)
            return (1.0f / Time.deltaTime);
        else
            return (0.0f);
    }
}
