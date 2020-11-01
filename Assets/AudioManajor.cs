using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using lib_audio_analysis;

public class AudioManajor : MonoBehaviour {
    AudioSource aud;
    ComplexData fftInput;
    ComplexData fftOutput;
    float[] dataSamples;
    const int timeLength = 1;
    const int samplingRate = 48000;
    FFTFuncs fftClass;
    int frameSize;
    float[] powerSpectre;
    //const int fftSize = 024;
    void Start() 
    {
        aud = GetComponent<AudioSource>();
        // マイク名、ループするかどうか、AudioClipの秒数、サンプリングレート を指定する
        aud.clip = Microphone.Start(null, true, timeLength, samplingRate);
        frameSize = aud.clip.samples * aud.clip.channels;
        dataSamples = new float[frameSize];
        fftInput.real = new float[frameSize];
        fftInput.imaginary = new float[frameSize];
        fftOutput.real = new float[frameSize];
        fftOutput.imaginary = new float[frameSize];
        powerSpectre = new float[frameSize];
        for(int i =0; i<frameSize; i++)
        {
            fftInput.real[i] = 0f;
            fftInput.imaginary[i] = 0f;
            fftOutput.real[i] = 0f;
            fftOutput.imaginary[i] = 0f;
            powerSpectre[i] = 0f;
        }
        fftClass = new FFTFuncs(frameSize, frameSize);
        fftClass.setFFTMode(FFTFuncs.fftMode.FFT);
    }

    // Update is called once per frame
    void Update()
    {
        aud.clip.GetData(dataSamples, 0);
        fftInput.real = Array.ConvertAll(dataSamples, FFTFuncs.hann_window);
        FFTFuncs.fftException exc = fftClass.fftRun(fftInput, fftOutput);
    }

    public float[] getSamples()
    {
        return dataSamples;
    }

    public float[] getFFTOutReal()
    {
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
            //powerSpectre[i] = (float)(10 * Math.Log10(powerSpectre[i]));
        }
    }
}
