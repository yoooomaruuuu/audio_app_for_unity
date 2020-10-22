using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class line : MonoBehaviour
{
    GameObject audioSource;
    ExampleClass audioData;
    public Material lineMaterial;
    int xMax = 20;
    int yMax = 10;
    // Start is called before the first frame update
    float[] x;
    float[] y;
    float[] samples;
    int frameNum;
    void Start()
    {
        audioSource = GameObject.Find("Audio Source");
        audioData = audioSource.GetComponent<ExampleClass>();
        frameNum = audioData.getSamplingRate() * audioData.getTimeLength();
        x = Enumerable.Repeat<float>(0.0f, frameNum).ToArray() ;
        y = Enumerable.Repeat<float>(0.0f, frameNum).ToArray() ;
    }

    // Update is called once per frame
    void Update()
    {
        samples = audioData.getSamples();
        for(int i = 0; i < frameNum; i++)
        {
            x[i] = xMax * i / (float)frameNum - (xMax / 2.0f);
            y[i] = yMax * samples[i] / 2.0f;
        }
        
    }
    void OnRenderObject()
    {
        lineMaterial.SetPass(0);
        GL.PushMatrix ();
        GL.MultMatrix (transform.localToWorldMatrix);
        GL.Begin (GL.LINES);
        for(int i = 0; i<frameNum; i++)
        {
            GL.Vertex3 (x[i], y[i], 0f);
        }
        GL.End ();
        GL.PopMatrix ();
    }

}
