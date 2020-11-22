using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using lib_audio_analysis;
using audio_app.common;

namespace audio_app
{
    public class ApplicationManajor : MonoBehaviour
    {
        public struct Config
        {
            public Config(UInt32 s, UInt16 c, BitRate b, int f, int d)
            {
                SamplingRate = s;
                Channels = c;
                BitsPerSample = b;
                BitsPerSampleValue = (UInt16)b;
                FrameMs = f;
                DeviceIndex = d;
            }
            [field : SerializeField]
            public UInt32 SamplingRate { get; }
            public UInt16 Channels { get; }
            public BitRate BitsPerSample { get; }
            public UInt16 BitsPerSampleValue { get; }
            public int FrameMs { get; }
            public int DeviceIndex { get; }
        }

        public InputCaptureFuncs InputCap { get; private set; }
        [field : SerializeField]
        public Config AppConfig { get; set; }
        private void Start()
        {
            DontDestroyOnLoad(this);
            //初期値
            AppConfig = new Config(48000, 2, BitRate.Integer16, 8, 0);
            InputCap = new InputCaptureFuncs();
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
    }
}
