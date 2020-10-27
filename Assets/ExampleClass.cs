using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using lib_audio_analysis;

public class ExampleClass : MonoBehaviour {
    AudioSource aud;
    complex_data fft_input;
    complex_data fft_output;
    float[] data_samples;
    const int timeLength = 1;
    const int samplingRate = 48000;
    fft_funcs fft_class;
    int frame_size;
    void Start() {
        aud = GetComponent<AudioSource>();
        // マイク名、ループするかどうか、AudioClipの秒数、サンプリングレート を指定する
        aud.clip = Microphone.Start(null, true, timeLength, samplingRate);
        frame_size = aud.clip.samples * aud.clip.channels;
        data_samples = new float[frame_size];
        fft_input.real = new float[frame_size];
        fft_input.imaginary = new float[frame_size];
        fft_output.real = new float[frame_size];
        fft_output.imaginary = new float[frame_size];
        for(int i =0; i<frame_size; i++)
        {
            fft_input.real[i] = 0f;
            fft_input.imaginary[i] = 0f;
            fft_output.real[i] = 0f;
            fft_output.imaginary[i] = 0f;
        }
        fft_class = new fft_funcs(frame_size, frame_size);
        fft_class.set_fft_mode(fft_funcs.fft_mode.FFT);
    }
    // Update is called once per frame
    void Update()
    {
        aud.clip.GetData(data_samples, 0);
        fft_input.real = Array.ConvertAll(data_samples, fft_funcs.hann_window);
        fft_funcs.fft_exception exc = fft_class.fft_run(fft_input, fft_output);
    }

    public float[] getSamples()
    {
        return data_samples;
    }

    public float[] getFFTOutReal()
    {
        return fft_output.real;
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
}
