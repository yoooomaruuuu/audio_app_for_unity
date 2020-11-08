﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using Assets;

[RequireComponent(typeof(Camera))]
public class audioWaveLine : MonoBehaviour
{
    public int waveDisplayHz = 4000;

    bool DEBUG_FFT_WAVE = true;
    GameObject audioSource;
    audioManajor audioData;
    GameObject cameraObj;
    Camera cam;
    int xLength = 0;
    int yLength = 0;
    float[] x;
    float[] y;
    float[] samples;
    int fftSize;

    static Material lineMaterial;
    static void CreateLineMaterial()
    {
        if (!lineMaterial)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            lineMaterial = new Material(shader);
            lineMaterial.hideFlags = HideFlags.HideAndDontSave;
            lineMaterial.color = Color.red;
            // Turn on alpha blending
            lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            lineMaterial.SetInt("_ZWrite", 0);
        }
    }
    void Start()
    {
        audioSource = GameObject.Find("NAudioData");
        audioData = audioSource.GetComponent<audioManajor>();
        fftSize = audioData.getFFTSize();
        x = Enumerable.Repeat<float>(0.0f, fftSize).ToArray() ;
        y = Enumerable.Repeat<float>(0.0f, fftSize).ToArray() ;
        
        cameraObj = GameObject.Find("Main Camera");
        cam = cameraObj.GetComponent<Camera>();
        xLength = (int)cam.ViewportToWorldPoint(new Vector3(1, 1, 0)).x * 2;
        yLength = (int)cam.ViewportToWorldPoint(new Vector3(1, 1, 0)).y * 2;
    }

    // Update is called once per frame
    void Update()
    {
        if(DEBUG_FFT_WAVE)
        {
            samples = audioData.getPowerSpectre();
            for(int i = 0; i < fftSize/ 2.0f; i++)
            {
                x[i] = xLength * i / (float)(fftSize / 4.0f) - (xLength / 2.0f);
                //powerスペクトル
                y[i] = samples[i] - (yLength / 2.0f);
            }
        }
        else
        {
            samples = audioData.getSamples();
            for(int i = 0; i < fftSize; i++)
            {
                x[i] = xLength * i / (float)fftSize - (xLength / 2.0f);
                y[i] = yLength * samples[i] / 2.0f;
            }
        }
    }
    void OnRenderObject()
    {
        CreateLineMaterial();
        lineMaterial.SetPass(0);
        GL.PushMatrix ();
        GL.MultMatrix (transform.localToWorldMatrix);
        GL.Begin (GL.LINES);
        for(int i = 0; i<fftSize; i++)
        {
            GL.Vertex3 (x[i], y[i], 0f);
        }
        GL.End ();
        GL.PopMatrix ();
    }

}