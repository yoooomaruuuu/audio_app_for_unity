using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using lib_audio_analysis;
using System.Threading;
using System.Threading.Tasks;
using audio_app.common;
using lib_world;

namespace audio_app
{
    class audioManajor : MonoBehaviour
    {
        ApplicationManajor appManage;
        InputCaptureFuncs inputCap;
        FFTFuncs fftClass;
        F0EstimateFuncs f0Class;

        //入力デバイスの設定はここで入力 
        ushort inputChannels;
        public uint SamplingRate { get; private set; } 
        BitRate inputBitRate;
        public float[] DataSamples { get; private set; }
        float[] powerSpectre;
        public float[] PowerSpectre { get { calcPowerSpectre(); return powerSpectre; } }
        public int FFTSize { get { return fftClass.getFFTSize(); } }
        ComplexData fftInput;
        ComplexData fftOutput;
        List<float> waveBuffer;
        int frameBufferSize = 512;
        double typeMax;

        float inputDb;
        public float InputDb { get { calcDb(); return inputDb; } }

        int captureBufferSize;
        byte[] captureData;
        IntPtr captureDataPtr;

        double[] f0Array;
        public double F0 { get; private set; }
        int f0Size;
        double[] f0TemporalPosition;

        bool cap_stop;

        Thread rcv_wave_thread;

        void Start()
        {
            inputDb = -60.0f;
            appManage = GameObject.Find("SceneManajor").GetComponent<ApplicationManajor>();
            inputCap = appManage.InputCap;

            cap_stop = false;
            long hr = inputCap.initInputCapture(appManage.AppConfig.SamplingRate, appManage.AppConfig.Channels, appManage.AppConfig.BitsPerSampleValue, appManage.AppConfig.FrameMs, appManage.AppConfig.DeviceIndex);
            if (hr != 0) appManage.error();
            Debug.Log("initInputCapture");
            SamplingRate = appManage.AppConfig.SamplingRate;
            inputChannels = appManage.AppConfig.Channels;
            inputBitRate = appManage.AppConfig.BitsPerSample;

            waveBuffer = new List<float>();
            DataSamples = new float[frameBufferSize];
            fftInput.Real = new float[frameBufferSize];
            fftInput.Imaginary = new float[frameBufferSize]; 
            fftOutput.Real = new float[frameBufferSize];
            fftOutput.Imaginary = new float[frameBufferSize];
            powerSpectre = new float[frameBufferSize];
            fftClass = new FFTFuncs(frameBufferSize, frameBufferSize);
            fftClass.setFFTMode(FFTFuncs.fftMode.FFT);
            Debug.Log("fftfuncsinit");

            double f0FramePeriod = Math.Ceiling(frameBufferSize / (double)SamplingRate) * 1000.0;
            f0Class = new F0EstimateFuncs(f0FramePeriod, 40, 2000);

            int f0Size = f0Class.GetF0Size((int)SamplingRate, frameBufferSize, f0FramePeriod);
            f0Array = new double[f0Size];
            f0TemporalPosition = new double[f0Size];

            //typeMaxの初期化
            if (inputBitRate == BitRate.Integer16) typeMax = Int16.MaxValue;
            else if (inputBitRate == BitRate.Integer24) typeMax = Int24.max();
            else if (inputBitRate == BitRate.Integer32) typeMax = Int32.MaxValue;
            else if (inputBitRate == BitRate.Integer64) typeMax = Int64.MaxValue;
            else if (inputBitRate == BitRate.Floating32) typeMax = Single.MaxValue;
            else typeMax = Int16.MaxValue;

            captureBufferSize = inputCap.getDataBufferSize();
            captureData = new byte[captureBufferSize];
            captureDataPtr = new IntPtr();
            captureDataPtr = Marshal.AllocCoTaskMem(captureBufferSize);
            if(inputCap.startCapture() != 0)
            {
                appManage.error();
            }
            Debug.Log("capture start");
            rcv_wave_thread = new Thread(new ThreadStart(capture));
            rcv_wave_thread.Start();
            cap_stop = true;
        }
        private async void capture()
        {
            while(true)
            {
                long hr = inputCap.getCaptureData(ref captureDataPtr);
                //if(test != 0) Debug.Log(test);
                if (hr == 0)
                {
                    Marshal.Copy(captureDataPtr, captureData, 0, captureBufferSize);
                    inputSoundDataConvert(captureData);
                    if (waveBuffer.Count > frameBufferSize)
                    {
                        if (inputBitRate != BitRate.Floating32)
                        {
                            float[] tmp = waveBuffer.GetRange(0, frameBufferSize).ToArray();
                            DataSamples = Array.ConvertAll(tmp, (x) => (float)(x / (double)typeMax));
                        }
                        else
                        {
                            DataSamples = waveBuffer.GetRange(0, frameBufferSize).ToArray();
                        }
                        // fft
                        fftInput.Real = Array.ConvertAll(DataSamples, FFTFuncs.hann_window);
                        fftClass.fftRun(fftInput, fftOutput);
                        //calcPowerSpectre();
                        //f0estimate
                        double[] f0tmp = Array.ConvertAll(DataSamples, (x) => (double)x);
                        f0Class.HarvestExecute(f0tmp, f0tmp.Length, (int)SamplingRate, f0TemporalPosition, f0Array);
                        F0 = f0Array.Min();

                        waveBuffer.RemoveRange(0, frameBufferSize);
                    }
                }
                else
                {
                    if(!cap_stop) 
                        break;
                }
                await Task.Delay(1);
            }
        }

        private void OnDestroy()
        {
            cap_stop = false;
            rcv_wave_thread.Join();
            rcv_wave_thread = null;
            inputCap.stopCapture();
            appManage = null;
            inputCap = null;
            fftClass = null;
            f0Class = null;
        }

        private void calcDb()
        {
            inputDb = -60.0f;
            for (int i = 0; i < powerSpectre.Length; i++)
            {
                double t = Math.Pow(fftInput.Real[i], 2.0);
                inputDb = Math.Max((float)(10.0 * Math.Log10(t)), inputDb);
            }
            inputDb += 60.0f;
        }

        private void calcPowerSpectre()
        {
            for(int i=0; i<powerSpectre.Length; i++)
            {
                // 振幅のパワー計測 0以上にしたほうが扱いやすいため+60.0f
                // double t = Math.Pow(fftInput.Real[i], 2.0);
                // powerSpectre[i] = Math.Max((float)(10.0 * Math.Log10(t)), -60.0f) + 60.0f;
                double t = (Math.Pow(fftOutput.Real[i], 2.0) + Math.Pow(fftOutput.Imaginary[i], 2.0));
                powerSpectre[i] = Math.Max((float)(10.0 * Math.Log10(t)), -60.0f);// + 60.0f;
            }
        }

        private void inputSoundDataConvert(byte[] captureData)
        {
            int captureDataIncremation = (int)inputBitRate / 8 * inputChannels;
            if (inputBitRate == BitRate.Integer16)
            {
                for (int i = 0; i < captureData.Length; i += captureDataIncremation)
                {
                    byte[] tmp = new byte[] { captureData[i], captureData[i + 1] };
                    waveBuffer.Add((float)BitConverter.ToInt16(tmp, 0));
                }
            }
            else if (inputBitRate == BitRate.Integer24)
            {
                for (int i = 0; i < captureData.Length; i += captureDataIncremation)
                {
                    byte[] tmp = new byte[] { captureData[i], captureData[i + 1], captureData[i + 2] };
                    waveBuffer.Add((float)BitConverter.ToInt32(tmp, 0));
                }
            }
            else if (inputBitRate == BitRate.Integer32)
            {
                for (int i = 0; i < captureData.Length; i += captureDataIncremation)
                {
                    byte[] tmp = new byte[] { captureData[i], captureData[i + 1], captureData[i + 2], captureData[i + 3] };
                    waveBuffer.Add((float)BitConverter.ToInt32(tmp, 0));
                }
            }
            else if (inputBitRate == BitRate.Integer64)
            {
                for (int i = 0; i < captureData.Length; i += captureDataIncremation)
                {
                    byte[] tmp = new byte[] { captureData[i], captureData[i + 1], captureData[i + 2], captureData[i + 3], captureData[i + 4], captureData[i + 5], captureData[i + 6], captureData[i + 7] };
                    waveBuffer.Add((float)BitConverter.ToInt64(tmp, 0));
                }
            }
            else if (inputBitRate == BitRate.Floating32)
            {
                for (int i = 0; i < captureData.Length; i += captureDataIncremation)
                {
                    byte[] tmp = new byte[] { captureData[i], captureData[i + 1], captureData[i + 2], captureData[i + 3], captureData[i + 4], captureData[i + 5], captureData[i + 6], captureData[i + 7] };
                    waveBuffer.Add(BitConverter.ToSingle(tmp, 0));
                }
            }
        }
    }
}
