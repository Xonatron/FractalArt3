using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // disable cursor (in build only)
        if (!Debug.isDebugBuild) Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        // quit...
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit(); // quit build
        }
    }
}
