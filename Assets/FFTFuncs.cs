using System;
using System.Runtime.InteropServices;

namespace lib_audio_analysis
{
    public struct complex_data
    {
        public float[] real;
        public float[] imaginary;
    }
    public class fft_funcs
    {
        private int m_frame_size;
        private IntPtr m_fft_object;

        public enum fft_mode
        {
            FFT = 0,
            IFFT = 1,
            ERROR = -1
        }

        public enum fft_exception
        {
            SUCCESS = 0,
            SETTING_ERROR = 1,
            DATA_OUT_OF_RANGE = 2,
            MODE_ERROR = 3
        }

        [DllImport("lib_audio_analysis.dll", EntryPoint = "init_fft_component", CallingConvention = CallingConvention.StdCall)]
        static extern void init_fft_component(int fft_size, ref IntPtr func_object);

        [DllImport("lib_audio_analysis.dll", EntryPoint = "mylib_fft", CallingConvention = CallingConvention.StdCall)]
        static extern fft_exception mylib_fft(float[] input_re, float[] input_im, float[] output_re, float[] output_im, IntPtr func_object);

        [DllImport("lib_audio_analysis.dll", EntryPoint = "mylib_ifft", CallingConvention = CallingConvention.StdCall)]
        static extern fft_exception mylib_ifft(float[] input_re, float[] input_im, float[] output_re, float[] output_im, IntPtr func_object);

        [DllImport("lib_audio_analysis.dll", EntryPoint = "fft_mode_setting", CallingConvention = CallingConvention.StdCall)]
        static extern fft_exception fft_mode_setting(fft_mode mode, IntPtr func_object);

        [DllImport("lib_audio_analysis.dll", EntryPoint = "get_fft_size", CallingConvention = CallingConvention.StdCall)]
        static extern int get_fft_size(IntPtr func_object);

        [DllImport("lib_audio_analysis.dll", EntryPoint = "delete_fft_component", CallingConvention = CallingConvention.StdCall)]
        static extern void delete_fft_component(ref IntPtr func_object);

        [DllImport("lib_audio_analysis.dll", EntryPoint = "hann_window", CallingConvention = CallingConvention.StdCall)]
        public static extern float hann_window(float x);


        public fft_funcs(int init_fft_size, int init_frame_size)
        {
            m_frame_size = init_frame_size;
            m_fft_object = new IntPtr();
            init_fft_component(init_fft_size, ref m_fft_object);
        } 

        ~fft_funcs()
        {
            delete_fft_component(ref m_fft_object);
        }

        //後でexceptionを返すのではなく返ってきたexceptionで例外処理する
        public fft_exception fft_run(complex_data input, complex_data output)
        {
            return mylib_fft(input.real, input.imaginary, output.real, output.imaginary, m_fft_object);
        }

        public fft_exception ifft_run(complex_data input, complex_data output)
        {
            return mylib_ifft(input.real, input.imaginary, output.real, output.imaginary, m_fft_object);
        }

        public fft_exception set_fft_mode(fft_mode mode)
        {
            return fft_mode_setting(mode, m_fft_object);
        }

        public int get_f_size()
        {
            return get_fft_size(m_fft_object);
        }
    }
}
