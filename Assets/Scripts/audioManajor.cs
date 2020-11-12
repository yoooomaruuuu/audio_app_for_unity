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
using MagicOnion;
using MagicOnion.Server;
using MagicOnion.Client;
using Grpc.Core;
using naudio_udp_server;
namespace Assets
{
    class audioManajor : MonoBehaviour
    {
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

        int LOCAL_PORT = 2222;
        static UdpClient udp;
        Thread rcv_wave_thread;
        ComplexData fftInput;
        ComplexData fftOutput;
        List<float> waveBuffer;
        int bufferSize;
        float typeMax;
        float[] dataSamples;
        const int timeLength = 1;
        FFTFuncs fftClass;
        float[] powerSpectre;
        Channel channel;
        IRecordingOrderService client;
        
        public audioManajor()
        {
            bufferSize = 1024;
            waveBuffer = new List<float>();
            dataSamples = new float[bufferSize];
            fftInput.real = new float[bufferSize];
            fftInput.imaginary = new float[bufferSize];
            fftOutput.real = new float[bufferSize];
            fftOutput.imaginary = new float[bufferSize];
            powerSpectre = new float[bufferSize];
            fftClass = new FFTFuncs(bufferSize, bufferSize);
            fftClass.setFFTMode(FFTFuncs.fftMode.FFT);

            channel = new Channel("localhost", 80, ChannelCredentials.Insecure);
            client = MagicOnionClient.Create<IRecordingOrderService>(channel);
        }

        void Start()
        {
            //udpスレッドスタート
            string command = @"D:\ProductionRelated\Unity\NAudioInput\naudio_udp_server.exe";
            VaNilla.InternalServerProcess.StartProcess(command);
            var result = client.StartRecording();
            Debug.Log(result);

            udp = new UdpClient(LOCAL_PORT);
            udp.Client.ReceiveTimeout = udpTimeout;
            rcv_wave_thread = new Thread(new ThreadStart(waveUdpRcv));
            rcv_wave_thread.Start();
        }

        void Update()
        {
            //データ変換
            if (inputBitRate == BitRate.Short) typeMax = short.MaxValue;
            else if (inputBitRate == BitRate.Int) typeMax = int.MaxValue;
            else if (inputBitRate == BitRate.Long) typeMax = long.MaxValue;
            else if (inputBitRate == BitRate.Float) typeMax = float.MaxValue;
            //waveBufferに十分量のデータが溜まったら、fft処理
            if(waveBuffer.Count > bufferSize)
            {
                float[] tmp = waveBuffer.GetRange(0, bufferSize).ToArray();
                dataSamples = Array.ConvertAll(tmp, (x) => x / (float)typeMax);
                fftInput.real = Array.ConvertAll(dataSamples, FFTFuncs.hann_window);
                fftClass.fftRun(fftInput, fftOutput);
                waveBuffer.RemoveRange(0, bufferSize);
            }
        }

        private void OnApplicationQuit()
        {
            client.StopRecording();
            channel.ShutdownAsync();
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
                    udpDataConvert(data);
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

        private void udpDataConvert(byte[] data)
        {
            int dataIncremation = (int)inputBitRate / 8 * inputChannels;
            if(inputBitRate == BitRate.Short)
            {
                for(int i=0; i<data.Length; i+=dataIncremation)
                {
                    byte[] tmp = new byte[] { data[i], data[i + 1] };
                    waveBuffer.Add((float)BitConverter.ToInt16(tmp, 0));
                }
            }
            else if(inputBitRate == BitRate.Int)
            {
                for(int i=0; i<data.Length; i+=dataIncremation)
                {
                    byte[] tmp = new byte[] { data[i], data[i + 1], data[i + 2], data[i + 3] };
                    waveBuffer.Add((float)BitConverter.ToInt32(tmp, 0));
                }
            }
            else if(inputBitRate == BitRate.Long)
            {
                for(int i=0; i<data.Length; i+=dataIncremation)
                {
                    byte[] tmp = new byte[] { data[i], data[i + 1], data[i + 2], data[i + 3], data[i + 4], data[i + 5], data[i + 6], data[i + 7] };
                    waveBuffer.Add((float)BitConverter.ToInt64(tmp, 0));
                }
            }
            else if(inputBitRate == BitRate.Float)
            {
                for(int i=0; i<data.Length; i+=dataIncremation)
                {
                    byte[] tmp = new byte[] { data[i], data[i + 1], data[i + 2], data[i + 3], data[i + 4], data[i + 5], data[i + 6], data[i + 7] };
                    waveBuffer.Add(BitConverter.ToSingle(tmp, 0));
                }
            }

        }
    }
}
