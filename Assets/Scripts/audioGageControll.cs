using UnityEngine;
using Assets;

public class audioGageControll : MonoBehaviour
{
    public int waveDisplayHz = 4000;
    public int gageNum = 10;
    public GameObject gagePrefab;

    bool DEBUG_FFT_WAVE = true;
    GameObject audioSource;
    audioManajor audioData;
    GameObject cameraObj;
    Camera cam;

    GameObject[] gages;
    float gageInterval = 0;

    int xLength = 0;
    int yLength = 0;
    float[] x;
    float[] y;
    float[] samples;
    int fftSize;
    int viewIndex = 0;

    static Material lineMaterial;
    void Start()
    {
        audioSource = GameObject.Find("NAudioData");
        audioData = audioSource.GetComponent<audioManajor>();
        fftSize = audioData.getFFTSize();
        x = new float[fftSize];
        y = new float[fftSize];
        
        cameraObj = GameObject.Find("Main Camera");
        cam = cameraObj.GetComponent<Camera>();
        xLength = (int)cam.ViewportToWorldPoint(new Vector3(1, 1, 0)).x * 2;
        yLength = (int)cam.ViewportToWorldPoint(new Vector3(1, 1, 0)).y * 2;

        gages = new GameObject[gageNum];
        float stan = ((float)xLength - ((gageNum - 1) * gageInterval)) / gageNum;
        float sizeE = stan / xLength;
        for(int i=0; i<gageNum; i++)
        {
            gages[i] = Instantiate(gagePrefab) as GameObject;
            gages[i].transform.position = new Vector3(stan * i + stan / 2.0f - xLength / 2.0f, 0, 1) ;
            gages[i].transform.localScale = new Vector3(sizeE, 1, 1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(DEBUG_FFT_WAVE)
        {
            samples = audioData.getPowerSpectre();
            viewIndex = (int)System.Math.Floor(waveDisplayHz * fftSize / (float)audioData.getSamplingRate());
            int sampleNum = viewIndex / gageNum;
            for(int i = 0; i < gageNum; i++)
            {
                //x[i] = xLength * i / (float)viewIndex - (xLength / 2.0f);
                ////powerスペクトル
                //y[i] = samples[i] - (yLength / 2.0f) + 0.5f;
                float test = 0.0f;
                for(int j = 0; j<sampleNum; j++)
                {
                    test += samples[sampleNum*i + j];
                }
                //test = test / (float)sampleNum;
                if (double.IsNaN(test)) test = 0.0f;
                gages[i].transform.Find("maskPivot").transform.localScale = new Vector3(1, test * 4.0f, 1);
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
}
