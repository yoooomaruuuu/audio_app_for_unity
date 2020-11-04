using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using lib_audio_analysis;
using NAudio.Wave;

public class AudioManajor : MonoBehaviour {
    AudioSource aud;
    ComplexData fftInput;
    ComplexData fftOutput;
    float[] tmp;
    float[] dataSamples;
    const int timeLength = 1;
    const int fftSize = 1024;
    const int samplingRate = 48000;
    FFTFuncs fftClass;
    int frameSize;
    float[] powerSpectre;
    void Start() 
    {
        aud = GetComponent<AudioSource>();
        // マイク名、ループするかどうか、AudioClipの秒数、サンプリングレート を指定する
        aud.clip = Microphone.Start(null, true, timeLength, samplingRate);
        frameSize = aud.clip.samples * aud.clip.channels;
        tmp = new float[frameSize];
        dataSamples = new float[fftSize];
        fftInput.real = new float[fftSize];
        fftInput.imaginary = new float[fftSize];
        fftOutput.real = new float[fftSize];
        fftOutput.imaginary = new float[fftSize];
        powerSpectre = new float[fftSize];
        for(int i =0; i<fftSize; i++)
        {
            fftInput.real[i] = 0f;
            fftInput.imaginary[i] = 0f;
            fftOutput.real[i] = 0f;
            fftOutput.imaginary[i] = 0f;
            powerSpectre[i] = 0f;
        }
        fftClass = new FFTFuncs(fftSize, frameSize);
        fftClass.setFFTMode(FFTFuncs.fftMode.FFT);
    }

    // Update is called once per frame
    void Update()
    {
        fftInput.real = Array.ConvertAll(dataSamples, FFTFuncs.hann_window);
        FFTFuncs.fftException exc = fftClass.fftRun(fftInput, fftOutput);
    }

    public float[] getSamples()
    {
        aud.clip.GetData(tmp, 0);
        int nowPos = getRecordPosition();
        for(int i = nowPos; i < fftSize + nowPos; i++)
        {
            if(i < frameSize)
            {
                dataSamples[i - nowPos] = tmp[i];
            }
            else
            {
                dataSamples[i - nowPos] = tmp[i - frameSize];
            }
        }
        return dataSamples;
    }

    public float[] getFFTOutReal()
    {
        fftInput.real = Array.ConvertAll(getSamples(), FFTFuncs.hann_window);
        FFTFuncs.fftException exc = fftClass.fftRun(fftInput, fftOutput);
        return fftOutput.real;
    }

    public float[] getPowerSpectre()
    {
        calcPowerSpectre();
        return  powerSpectre;
    }

    public int getRecordPosition()
    {
        return Microphone.GetPosition(null);
    }

    public int getFFTSize()
    {
        return fftClass.getFFTSize();
    }

    public int getTimeLength()
    {
        return timeLength;
    }

    public int getSamplingRate()
    {
        return samplingRate;
    }
    //return dbv
    private void calcPowerSpectre()
    {
        for(int i=0; i<powerSpectre.Length; i++)
        {
            powerSpectre[i] = (float)(Math.Pow(fftOutput.real[i], 2.0) + Math.Pow(fftOutput.imaginary[i], 2.0)) / (float)powerSpectre.Length;
        }
    }
}
