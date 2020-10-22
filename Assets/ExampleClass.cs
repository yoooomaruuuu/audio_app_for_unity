using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleClass : MonoBehaviour {
    AudioSource aud;
    float[] samples;
    const int timeLength = 1;
    const int samplingRate = 44100;
    void Start() {
        aud = GetComponent<AudioSource>();
        // マイク名、ループするかどうか、AudioClipの秒数、サンプリングレート を指定する
        aud.clip = Microphone.Start(null, true, timeLength, samplingRate);
        samples = new float[aud.clip.samples * aud.clip.channels];
        // aud.Play();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    void FixedUpdate()
    {
        aud.clip.GetData(samples, 0);
        //aud.clip.SetData(samples, 1024);
        //Debug.Log("44099: " + samples[44099]);
        //Debug.Log("0    : " + samples[0]);
    }

    public float[] getSamples()
    {
        return samples;
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
