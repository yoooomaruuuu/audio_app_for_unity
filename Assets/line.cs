using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Camera))]
public class line : MonoBehaviour
{
    bool DEBUG_FFT_WAVE = true;
    GameObject audioSource;
    ExampleClass audioData;
    GameObject camera;
    Camera cam;
    int xLength = 0;
    int yLength = 0;
    // Start is called before the first frame update
    float[] x;
    float[] y;
    float[] samples;
    int frameNum;

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
            lineMaterial.color = Color.cyan;
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
        audioSource = GameObject.Find("Audio Source");
        audioData = audioSource.GetComponent<ExampleClass>();
        frameNum = audioData.getSamplingRate() * audioData.getTimeLength();
        x = Enumerable.Repeat<float>(0.0f, frameNum).ToArray() ;
        y = Enumerable.Repeat<float>(0.0f, frameNum).ToArray() ;

        camera = GameObject.Find("Main Camera");
        cam = camera.GetComponent<Camera>();
        xLength = (int)cam.ViewportToWorldPoint(new Vector3(1, 1, 0)).x * 2;
        yLength = (int)cam.ViewportToWorldPoint(new Vector3(1, 1, 0)).y * 2;
        lineMaterial.color = Color.cyan;
    }

    // Update is called once per frame
    void Update()
    {
        if(DEBUG_FFT_WAVE)
        {
            samples = audioData.getFFTOutReal();
            for(int i = 0; i < frameNum; i++)
            {
                x[i] = xLength * (i) / (float)frameNum - (xLength / 2.0f);
                y[i] = yLength * samples[i] / 2.0f;
            }
        }
        else
        {
            samples = audioData.getSamples();
            int nowPos = audioData.getRecordPosition();
            for(int i = nowPos; i < frameNum + nowPos; i++)
            {
                x[i - nowPos] = xLength * (i - nowPos) / (float)frameNum - (xLength / 2.0f);
                if(i < frameNum)
                {
                    y[i - nowPos] = yLength * samples[i] / 2.0f;
                }
                else
                {
                    y[i - nowPos] = yLength * samples[i - frameNum] / 2.0f;
                }
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
        for(int i = 0; i<frameNum; i++)
        {
            GL.Vertex3 (x[i], y[i], 0f);
        }
        GL.End ();
        GL.PopMatrix ();
    }

}
