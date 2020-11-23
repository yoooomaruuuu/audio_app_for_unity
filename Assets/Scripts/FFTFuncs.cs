using System;
using System.Runtime.InteropServices;

namespace lib_audio_analysis
{
    public struct ComplexData
    {
        private float[] real;
        private float[] imaginary;
        public float[] Real
        {
            set { this.real = value; }
            get { return this.real; }
        }
        public float[] Imaginary
        {
            set { this.imaginary = value; }
            get { return this.imaginary; }
        }
    }
    public class FFTFuncs
    {
        private int mFrameSize;
        private IntPtr mFFTObject;

        public enum fftMode
        {
            FFT = 0,
            IFFT = 1,
            ERROR = -1
        }

        public enum fftException
        {
            SUCCESS = 0,
            SETTING_ERROR = 1,
            DATA_OUT_OF_RANGE = 2,
            MODE_ERROR = 3
        }

        [DllImport("lib_audio_analysis", EntryPoint = "create_fft_component", CallingConvention = CallingConvention.StdCall)]
        static extern void create_fft_component(int fft_size, ref IntPtr func_object);

        [DllImport("lib_audio_analysis", EntryPoint = "mylib_fft", CallingConvention = CallingConvention.StdCall)]
        static extern fftException mylib_fft(float[] input_re, float[] input_im, float[] output_re, float[] output_im, IntPtr func_object);

        [DllImport("lib_audio_analysis", EntryPoint = "mylib_ifft", CallingConvention = CallingConvention.StdCall)]
        static extern fftException mylib_ifft(float[] input_re, float[] input_im, float[] output_re, float[] output_im, IntPtr func_object);

        [DllImport("lib_audio_analysis", EntryPoint = "fft_mode_setting", CallingConvention = CallingConvention.StdCall)]
        static extern fftException fft_mode_setting(fftMode mode, IntPtr func_object);

        [DllImport("lib_audio_analysis", EntryPoint = "get_fft_size", CallingConvention = CallingConvention.StdCall)]
        static extern int get_fft_size(IntPtr func_object);

        [DllImport("lib_audio_analysis", EntryPoint = "delete_fft_component", CallingConvention = CallingConvention.StdCall)]
        static extern void delete_fft_component(ref IntPtr func_object);

        [DllImport("lib_audio_analysis", EntryPoint = "hann_window", CallingConvention = CallingConvention.StdCall)]
        public static extern float hann_window(float x);

        public FFTFuncs(int initFFTSize, int initFrameSize)
        {
            mFrameSize = initFrameSize;
            mFFTObject = new IntPtr();
            create_fft_component(initFFTSize, ref mFFTObject);
        } 

        ~FFTFuncs()
        {
            delete_fft_component(ref mFFTObject);
        }

        //後でexceptionを返すのではなく返ってきたexceptionで例外処理する
        public fftException fftRun(ComplexData input, ComplexData output)
        {
            return mylib_fft(input.Real, input.Imaginary, output.Real, output.Imaginary, mFFTObject);
        }

        public fftException ifftRun(ComplexData input, ComplexData output)
        {
            return mylib_ifft(input.Real, input.Imaginary, output.Real, output.Imaginary, mFFTObject);
        }

        public fftException setFFTMode(fftMode mode)
        {
            return fft_mode_setting(mode, mFFTObject);
        }

        public int getFFTSize()
        {
            return get_fft_size(mFFTObject);
        }
    }
}
