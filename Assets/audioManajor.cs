using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using lib_audio_analysis;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Threading;

namespace Assets
{
    class audioManajor : MonoBehaviour
    {
        //入力デバイスの設定はここで入力 
        public int inputChannels = 2;
        public int bitRate = 16;
        public int samplingRate = 48000;
        public int udpTimeout = 1000;

        int LOCAL_PORT = 2222;
        static UdpClient udp;
        Thread rcv_wave_thread;
        ComplexData fftInput;
        ComplexData fftOutput;
        List<short> waveBuffer;
        int bufferSize;
        float[] dataSamples;
        const int timeLength = 1;
        FFTFuncs fftClass;
        float[] powerSpectre;
        
        public audioManajor()
        {
            bufferSize = 1024;
            waveBuffer = new List<short>();
            dataSamples = new float[bufferSize];
            fftInput.real = new float[bufferSize];
            fftInput.imaginary = new float[bufferSize];
            fftOutput.real = new float[bufferSize];
            fftOutput.imaginary = new float[bufferSize];
            powerSpectre = new float[bufferSize];
            fftClass = new FFTFuncs(bufferSize, bufferSize);
            fftClass.setFFTMode(FFTFuncs.fftMode.FFT);
        }

        void Start()
        {
            //udpスレッドスタート
            udp = new UdpClient(LOCAL_PORT);
            udp.Client.ReceiveTimeout = udpTimeout;
            rcv_wave_thread = new Thread(new ThreadStart(waveUdpRcv));
            rcv_wave_thread.Start();
        }

        void Update()
        {
            //waveBufferに十分量のデータが溜まったら、fft処理
            if(waveBuffer.Count > bufferSize)
            {
                short[] tmp = waveBuffer.GetRange(0, bufferSize).ToArray();
                dataSamples = Array.ConvertAll(tmp, (x) => x / 32767.0f);
                fftInput.real = Array.ConvertAll(dataSamples, FFTFuncs.hann_window);
                fftClass.fftRun(fftInput, fftOutput);
                waveBuffer.RemoveRange(0, bufferSize);
            }
        }

        private void OnApplicationQuit()
        {
            rcv_wave_thread.Abort();
        }

        private void waveUdpRcv()
        {
            while(true)
            {
                IPEndPoint remoteEP = null;
                if(udp.Available != 0)
                {
                    byte[] data = udp.Receive(ref remoteEP);
                    // udpデータは [ch1, ch2, ch3, ... , chN, ch1, ...]の順で流れてくることを前提としている
                    // bitRateを8で割ることでbyte数を出し、それが何チャネルあるかを確定させている
                    // 今回の実装では1chのデータのみを用いるため、1ch分のみを読み込むようにループ 
                    int dataIncremation = bitRate / 8 * inputChannels;
                    for(int i=0; i<data.Length; i+=dataIncremation)
                    {
                        byte[] tmp = new byte[] { data[i], data[i + 1] };
                        waveBuffer.Add(BitConverter.ToInt16(tmp, 0));
                    }
                }
            }
        }

        public float[] getSamples()
        {
            return dataSamples;
        }
        public float[] getPowerSpectre()
        {
            calcPowerSpectre();
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
                powerSpectre[i] = (float)(Math.Pow(fftOutput.real[i], 2.0) + Math.Pow(fftOutput.imaginary[i], 2.0));
            }
        }
    }
}
