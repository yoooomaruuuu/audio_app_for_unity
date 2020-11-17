using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using lib_audio_analysis;


public class ApplicationManajor : MonoBehaviour
{
    public struct Config
    {
        public Config(UInt32 s, UInt16 c, UInt16 b, int f, int d)
        {
            samplingRate = s;
            channels = c;
            bitsPerSample = b;
            frameMs = f;
            deviceIndex = d;
        }
        public UInt32 samplingRate { get; set; }
        public UInt16 channels { get; set; }
        public UInt16 bitsPerSample { get; set; }
        public int frameMs { get; set; }
        public int deviceIndex { get; set; }
    }

    InputCaptureFuncs inputCap;
    public Config config { get; set; }
    private void Start()
    {
        DontDestroyOnLoad(this);
        config = new Config(48000, 2, 16, 16, 0);
        inputCap = new InputCaptureFuncs();
        SceneManager.LoadScene("MainMenu");
    }
    public void configSetting()
    {
        SceneManager.LoadScene("ConfigMenu");
    }

    public void returnMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void gameStart()
    {
        SceneManager.LoadScene("GameplayStage1");
    }

    public void resetScene()
    {
        SceneManager.LoadScene(0);
    }

    public void gameEnd()
    {
        SceneManager.LoadScene("GameClear");
    }

    public void death()
    {
        SceneManager.LoadScene("Death");
    }
    
    public void error()
    {
        SceneManager.LoadScene("ErrorDialog");
    }

    public InputCaptureFuncs GetInputCap()
    {
        return inputCap;
    }
}
