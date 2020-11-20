using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using lib_audio_analysis;
using System.Threading;
using audio_app.common;

namespace audio_app
{
    class audioManajor : MonoBehaviour
    {
        ApplicationManajor appManage;
        InputCaptureFuncs inputCap;
        FFTFuncs fftClass;

        //入力デバイスの設定はここで入力 
        ushort inputChannels;
        public uint SamplingRate { get; private set; } 
        BitRate inputBitRate;
        public float[] DataSamples { get; private set; }
        float[] powerSpectre;
        public float[] PowerSpectre
        {
            get { calcPowerSpectre(); return powerSpectre; }
        }
        public int FFTSize { get { return fftClass.getFFTSize(); } }
        ComplexData fftInput;
        ComplexData fftOutput;
        List<float> waveBuffer;
        int bufferSize;
        double typeMax;

        int buf_size;
        byte[] data;
        IntPtr data_ptr;

        bool cap_stop;

        Thread rcv_wave_thread;

        void Start()
        {
            appManage = GameObject.Find("SceneManajor").GetComponent<ApplicationManajor>();
            inputCap = appManage.InputCap;

            bufferSize = 2048;
            waveBuffer = new List<float>();
            DataSamples = new float[bufferSize];
            fftInput.Real = new float[bufferSize];
            fftInput.Imaginary = new float[bufferSize]; 
            fftOutput.Real = new float[bufferSize];
            fftOutput.Imaginary = new float[bufferSize];
            powerSpectre = new float[bufferSize];
            fftClass = new FFTFuncs(bufferSize, bufferSize);
            fftClass.setFFTMode(FFTFuncs.fftMode.FFT);

            cap_stop = false;
            inputCap.initInputCapture(appManage.AppConfig.SamplingRate, appManage.AppConfig.Channels, appManage.AppConfig.BitsPerSampleValue, appManage.AppConfig.FrameMs, appManage.AppConfig.DeviceIndex);
            SamplingRate = appManage.AppConfig.SamplingRate;
            inputChannels = appManage.AppConfig.Channels;
            inputBitRate = appManage.AppConfig.BitsPerSample;

            //typeMaxの初期化
            if (inputBitRate == BitRate.Integer16) typeMax = Int16.MaxValue;
            else if (inputBitRate == BitRate.Integer24) typeMax = Int24.max();
            else if (inputBitRate == BitRate.Integer32) typeMax = Int32.MaxValue;
            else if (inputBitRate == BitRate.Integer64) typeMax = Int64.MaxValue;
            else if (inputBitRate == BitRate.Floating32) typeMax = Single.MaxValue;
            else typeMax = Int16.MaxValue;

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
        private async void capture()
        {
            while(cap_stop)
            {
                long test = inputCap.getCaptureData(ref data_ptr);
                //if(test != 0) Debug.Log(test);
                if(test == 0)
                {
                    Marshal.Copy(data_ptr, data, 0, buf_size);
                    inputSoundDataConvert(data);
                    //await Task.Delay(16);
                    if(waveBuffer.Count > bufferSize)
                    {
                        float[] tmp = waveBuffer.GetRange(0, bufferSize).ToArray();
                        DataSamples = Array.ConvertAll(tmp, (x) => (float)(x / (double)typeMax));
                        fftInput.Real = Array.ConvertAll(DataSamples, FFTFuncs.hann_window);
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


        private void calcPowerSpectre()
        {
            for(int i=0; i<powerSpectre.Length; i++)
            {
                powerSpectre[i] = (float)(Math.Pow(fftOutput.Real[i], 2.0) + Math.Pow(fftOutput.Imaginary[i], 2.0)) / FFTSize;
            }
            Debug.Log("powerSpectre:" + powerSpectre[1]);
        }

        private void inputSoundDataConvert(byte[] data)
        {
            int dataIncremation = (int)inputBitRate / 8 * inputChannels;
            if (inputBitRate == BitRate.Integer16)
            {
                for (int i = 0; i < data.Length; i += dataIncremation)
                {
                    byte[] tmp = new byte[] { data[i], data[i + 1] };
                    waveBuffer.Add((float)BitConverter.ToInt16(tmp, 0));
                }
            }
            else if (inputBitRate == BitRate.Integer24)
            {
                for (int i = 0; i < data.Length; i += dataIncremation)
                {
                    byte[] tmp = new byte[] { data[i], data[i + 1], data[i + 2] };
                    waveBuffer.Add((float)BitConverter.ToInt32(tmp, 0));
                }
            }
            else if (inputBitRate == BitRate.Integer32)
            {
                for (int i = 0; i < data.Length; i += dataIncremation)
                {
                    byte[] tmp = new byte[] { data[i], data[i + 1], data[i + 2], data[i + 3] };
                    waveBuffer.Add((float)BitConverter.ToInt32(tmp, 0));
                }
            }
            else if (inputBitRate == BitRate.Integer64)
            {
                for (int i = 0; i < data.Length; i += dataIncremation)
                {
                    byte[] tmp = new byte[] { data[i], data[i + 1], data[i + 2], data[i + 3], data[i + 4], data[i + 5], data[i + 6], data[i + 7] };
                    waveBuffer.Add((float)BitConverter.ToInt64(tmp, 0));
                }
            }
            else if (inputBitRate == BitRate.Floating32)
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
