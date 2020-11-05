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
    class myNAudioClass : MonoBehaviour
    {
        int LOCAL_PORT = 2222;
        static UdpClient udp;
        Thread rcv_wave_thread;
        ComplexData fftInput;
        ComplexData fftOutput;
        List<short> waveBuffer1ch;
        //List<short> waveBuffer2ch;
        int bufferSize;
        float[] dataSamples;
        const int timeLength = 1;
        const int samplingRate = 48000;
        FFTFuncs fftClass;
        float[] powerSpectre;
        
        public myNAudioClass()
        {
            bufferSize = 1024;//(int)((frameMsec / 1000.0) * samplingRate);
            waveBuffer1ch = new List<short>();
            waveBuffer2ch = new List<short>();
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
            udp = new UdpClient(LOCAL_PORT);
            udp.Client.ReceiveTimeout = 1000;
            rcv_wave_thread = new Thread(new ThreadStart(waveUdpRcv));
            rcv_wave_thread.Start();
        }

        void Update()
        {
            if(waveBuffer1ch.Count > bufferSize)
            {
                short[] tmp = waveBuffer1ch.GetRange(0, bufferSize).ToArray();
                dataSamples = Array.ConvertAll(tmp, (x) => x / 32767.0f);
                fftInput.real = Array.ConvertAll(dataSamples, FFTFuncs.hann_window);
                fftClass.fftRun(fftInput, fftOutput);
                waveBuffer1ch.RemoveRange(0, bufferSize);
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
                    for(int i=0; i<data.Length; i+=2)
                    {
                        byte[] tmp = new byte[] { data[i], data[i + 1] };
                        waveBuffer1ch.Add(BitConverter.ToInt16(tmp, 0));
                    }
                }
            }
        }

        public float[] getSamples()
        {
            return dataSamples;
        }
        public float[] getFFTOutReal()
        {
            //fftInput.real = Array.ConvertAll(getSamples(), FFTFuncs.hann_window);
            //FFTFuncs.fftException exc = fftClass.fftRun(fftInput, fftOutput);
            return fftOutput.real;
        }

        public float[] getPowerSpectre()
        {
            calcPowerSpectre();
            return  powerSpectre;
        }

        //public int getRecordPosition()
        //{
        //    return Microphone.GetPosition(null);
        //}

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
        //return dbv
        private void calcPowerSpectre()
        {
            for(int i=0; i<powerSpectre.Length; i++)
            {
                powerSpectre[i] = (float)(Math.Pow(fftOutput.real[i], 2.0) + Math.Pow(fftOutput.imaginary[i], 2.0)) / (float)powerSpectre.Length;
            }
        }
    }
}
