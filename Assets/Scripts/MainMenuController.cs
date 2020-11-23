using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using audio_app;

public class MainMenuController : MonoBehaviour
{
    ApplicationManager appmanage;
    // Start is called before the first frame update
    void Start()
    {
        appmanage = GameObject.Find("SceneManajor").GetComponent<ApplicationManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            appmanage.gameQuit();
        }
        else if(Input.GetMouseButtonDown(0))
        {
            appmanage.gameDescription();
        }
        
    }
}
