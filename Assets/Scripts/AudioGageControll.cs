﻿using UnityEngine;

namespace audio_app
{
    public class AudioGageControll : MonoBehaviour
    {
        [SerializeField]
        int waveDisplayHz = 8000;
        [SerializeField]
        int gageNum = 15;
        [SerializeField]
        GameObject gagePrefab;
        [SerializeField]
        GameObject player;
        [SerializeField]
        bool DEBUG_FFT_WAVE = true;

        AudioManager audioData;
        Camera cam;

        GameObject[] gages;
        SpriteRenderer[] gagesRenderer;
        bool gagesActivation = false;
        float gageInterval = 0;

        int xLength = 0;
        int yLength = 0;
        float[] x;
        float[] y;
        float[] samples;
        int fftSize;
        int viewIndex = 0;

        AudioSensitivityController sensiController;
        float gageLevelSensi = 30.0f;
        void Start()
        {
            sensiController = GameObject.Find("UI").GetComponent<AudioSensitivityController>();
            audioData = GameObject.Find("NAudioData").GetComponent<AudioManager>();
            fftSize = audioData.FFTSize;
            x = new float[fftSize];
            y = new float[fftSize];

            cam = GameObject.Find("MainCamera").GetComponent<Camera>();
            xLength = (int)cam.ViewportToWorldPoint(new Vector3(1, 1, 0)).x * 2;
            yLength = (int)cam.ViewportToWorldPoint(new Vector3(1, 1, 0)).y * 2;

            gages = new GameObject[gageNum];
            gagesRenderer = new SpriteRenderer[gageNum];
            float stan = ((float)xLength - ((gageNum - 1) * gageInterval)) / gageNum;
            float sizeE = stan / xLength;
            for (int i = 0; i < gageNum; i++)
            {
                gages[i] = Instantiate(gagePrefab) as GameObject;
                gages[i].transform.position = new Vector3(stan * i + stan / 2.0f - xLength / 2.0f, 0, -1);
                gages[i].transform.localScale = new Vector3(sizeE, 1, 1);
                gages[i].transform.SetParent(this.gameObject.transform);
                gagesRenderer[i] = gages[i].GetComponent<SpriteRenderer>();
            }
        }

        void Update()
        {
            if (sensiController.Sensi == AudioSensitivityController.Sensitivity.STRONG) gageLevelSensi = 24.0f;
            else if (sensiController.Sensi == AudioSensitivityController.Sensitivity.MEDIUM) gageLevelSensi = 48.0f;
            else if (sensiController.Sensi == AudioSensitivityController.Sensitivity.WEAK) gageLevelSensi = 72.0f;

            if (player.GetComponent<Rigidbody2D>().velocity.y != 0.0) gagesActive(0.4f);
            else if (player.GetComponent<Rigidbody2D>().velocity.x != 0.0) gagesActive(0.3f);
            else gagesInActive(0.2f);
             

            if (DEBUG_FFT_WAVE)
            {
                samples = audioData.PowerSpectre;
                viewIndex = (int)System.Math.Floor(waveDisplayHz * fftSize / (float)audioData.SamplingRate);
                int sampleNum = viewIndex / gageNum;
                for (int i = 0; i < gageNum; i++)
                {
                    float value = 0.0f;
                    for (int j = 0; j < sampleNum; j++)
                    {
                        value = System.Math.Max(samples[sampleNum * i + j], value);
                    }
                    value /= gageLevelSensi;
                    gages[i].transform.Find("maskPivot").transform.localScale = new Vector3(1, value, 1);
                }
            }
            else
            {
                samples = audioData.DataSamples;
                for (int i = 0; i < fftSize; i++)
                {
                    x[i] = xLength * i / (float)fftSize - (xLength / 2.0f);
                    y[i] = yLength * samples[i] / 2.0f;
                }
            }
        }

        private void OnDestroy()
        {
            audioData = null;
        }

        public void gagesActive(float pow)
        {
            if (!gagesActivation)
            {
                for (int i = 0; i < gageNum; i++)
                {
                    gagesRenderer[i].color = new Color(1, 1, 1, pow);
                }
                gagesActivation = true;
            }
        }
        public void gagesInActive(float pow)
        {
            if(gagesActivation)
            {
                for(int i=0; i<gageNum; i++)
                {
                    gagesRenderer[i].color = new Color(1, 1, 1, pow);
                }
                gagesActivation = false;
            }
        }
    }
}