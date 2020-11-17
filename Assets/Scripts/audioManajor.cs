using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using lib_audio_analysis;
using System.Threading;
namespace Assets
{
    class audioManajor : MonoBehaviour
    {
        ApplicationManajor appManage;
        InputCaptureFuncs inputCap;

        //入力デバイスの設定はここで入力 
        public int inputChannels = 2;
        public int samplingRate = 48000;
        public int udpTimeout = 1000;
        public enum BitRate 
        {
            Short = 16,
            Int = 32,
            Long = 64,
            Float = 32,
        } ;

        public BitRate inputBitRate = BitRate.Short;

        ComplexData fftInput;
        ComplexData fftOutput;
        List<float> waveBuffer;
        int bufferSize;
        float typeMax;
        float[] dataSamples;
        const int timeLength = 1;
        FFTFuncs fftClass;
        float[] powerSpectre;

        int buf_size;
        byte[] data;
        IntPtr data_ptr;

        bool cap_stop;

        Thread rcv_wave_thread;

        void Start()
        {
            appManage = GameObject.Find("SceneManajor").GetComponent<ApplicationManajor>();
            inputCap = appManage.GetInputCap();

            bufferSize = 2048;
            waveBuffer = new List<float>();
            dataSamples = new float[bufferSize];
            fftInput.real = new float[bufferSize];
            fftInput.imaginary = new float[bufferSize];
            fftOutput.real = new float[bufferSize];
            fftOutput.imaginary = new float[bufferSize];
            powerSpectre = new float[bufferSize];
            fftClass = new FFTFuncs(bufferSize, bufferSize);
            fftClass.setFFTMode(FFTFuncs.fftMode.FFT);

            cap_stop = false;
            inputCap.initInputCapture(appManage.config.samplingRate, appManage.config.channels, appManage.config.bitsPerSample, appManage.config.frameMs, appManage.config.deviceIndex);
            buf_size = inputCap.getDataBufferSize();
            data = new byte[buf_size];
            data_ptr = new IntPtr();
            data_ptr = Marshal.AllocCoTaskMem(buf_size);
            if(inputCap.startCapture() != 0)
            {
                appManage.error();
            }
            rcv_wave_thread = new Thread(new ThreadStart(capture));
            rcv_wave_thread.Start();
            cap_stop = true;
        }

        void Update()
        {
            //データ変換
            if (inputBitRate == BitRate.Short) typeMax = short.MaxValue;
            else if (inputBitRate == BitRate.Int) typeMax = int.MaxValue;
            else if (inputBitRate == BitRate.Long) typeMax = long.MaxValue;
            else if (inputBitRate == BitRate.Float) typeMax = float.MaxValue;
            //waveBufferに十分量のデータが溜まったら、fft処理
        }

        private async void capture()
        {
            while(cap_stop)
            {
                long test = inputCap.getCaptureData(ref data_ptr);
                //if(test != 0) Debug.Log(test);
                if(test == 0)
                {
                    Marshal.Copy(data_ptr, data, 0, buf_size);
                    udpDataConvert(data);
                    //await Task.Delay(16);
                    if(waveBuffer.Count > bufferSize)
                    {
                        float[] tmp = waveBuffer.GetRange(0, bufferSize).ToArray();
                        dataSamples = Array.ConvertAll(tmp, (x) => x / (float)typeMax);
                        fftInput.real = Array.ConvertAll(dataSamples, FFTFuncs.hann_window);
                        fftClass.fftRun(fftInput, fftOutput);
                        calcPowerSpectre();
                        waveBuffer.RemoveRange(0, bufferSize);
                    }
                }
            }
        }

        private void OnDestroy()
        {
            cap_stop = false;
            rcv_wave_thread.Join();
            inputCap.stopCapture();
        }


        public float[] getSamples()
        {
            return dataSamples;
        }
        public float[] getPowerSpectre()
        {
            return  powerSpectre;
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
        private void calcPowerSpectre()
        {
            for(int i=0; i<powerSpectre.Length; i++)
            {
                powerSpectre[i] = (float)(Math.Pow(fftOutput.real[i], 2.0) + Math.Pow(fftOutput.imaginary[i], 2.0)) / getFFTSize();
            }
        }

        private void udpDataConvert(byte[] data)
        {
            int dataIncremation = (int)inputBitRate / 8 * inputChannels;
            if (inputBitRate == BitRate.Short)
            {
                for (int i = 0; i < data.Length; i += dataIncremation)
                {
                    byte[] tmp = new byte[] { data[i], data[i + 1] };
                    waveBuffer.Add((float)BitConverter.ToInt16(tmp, 0));
                }
            }
            else if (inputBitRate == BitRate.Int)
            {
                for (int i = 0; i < data.Length; i += dataIncremation)
                {
                    byte[] tmp = new byte[] { data[i], data[i + 1], data[i + 2], data[i + 3] };
                    waveBuffer.Add((float)BitConverter.ToInt32(tmp, 0));
                }
            }
            else if (inputBitRate == BitRate.Long)
            {
                for (int i = 0; i < data.Length; i += dataIncremation)
                {
                    byte[] tmp = new byte[] { data[i], data[i + 1], data[i + 2], data[i + 3], data[i + 4], data[i + 5], data[i + 6], data[i + 7] };
                    waveBuffer.Add((float)BitConverter.ToInt64(tmp, 0));
                }
            }
            else if (inputBitRate == BitRate.Float)
            {
                for (int i = 0; i < data.Length; i += dataIncremation)
                {
                    byte[] tmp = new byte[] { data[i], data[i + 1], data[i + 2], data[i + 3], data[i + 4], data[i + 5], data[i + 6], data[i + 7] };
                    waveBuffer.Add(BitConverter.ToSingle(tmp, 0));
                }
            }
        }
    }
}
