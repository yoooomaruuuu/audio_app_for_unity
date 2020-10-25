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
        IntPtr m_input_re;
        IntPtr m_input_im;
        IntPtr m_output_re;
        IntPtr m_output_im;
        int m_frame_size;
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

        [DllImport("lib_audio_analysis.dll", EntryPoint = "fft_component")]
        static extern void fft_component(int fft_size);

        [DllImport("lib_audio_analysis.dll", EntryPoint = "fft")]
        static extern fft_exception fft(IntPtr input_re, IntPtr input_im, IntPtr output_re, IntPtr output_im);

        [DllImport("lib_audio_analysis.dll", EntryPoint = "ifft")]
        static extern fft_exception ifft(IntPtr input_re, IntPtr input_im, IntPtr output_re, IntPtr output_im);

        [DllImport("lib_audio_analysis.dll", EntryPoint = "fft_mode_setting")]
        static extern fft_exception fft_mode_setting(fft_mode mode);

        [DllImport("lib_audio_analysis.dll", EntryPoint = "get_fft_size")]
        static extern int get_fft_size();

        public fft_funcs(int init_fft_size, int init_frame_size)
        {
            m_input_re = Marshal.AllocCoTaskMem(m_frame_size*sizeof(float));
            m_input_im = Marshal.AllocCoTaskMem(m_frame_size*sizeof(float));
            m_output_re = Marshal.AllocCoTaskMem(m_frame_size*sizeof(float));
            m_output_im = Marshal.AllocCoTaskMem(m_frame_size*sizeof(float));
            fft_component(init_fft_size);
        } 

        ~fft_funcs()
        {
            Marshal.FreeCoTaskMem(m_input_re); 
            Marshal.FreeCoTaskMem(m_input_im); 
            Marshal.FreeCoTaskMem(m_output_re); 
            Marshal.FreeCoTaskMem(m_output_im); 
        }

        //後でexceptionを返すのではなく返ってきたexceptionで例外処理する
        public fft_exception fft_run(complex_data input, complex_data output)
        {
            Marshal.Copy(input.real, 0, m_input_re, input.real.Length);
            Marshal.Copy(input.imaginary, 0, m_input_im, input.imaginary.Length);
            Marshal.Copy(output.real, 0, m_output_re, output.real.Length);
            Marshal.Copy(output.imaginary, 0, m_output_re, output.imaginary.Length);
            return fft(m_input_re, m_input_im, m_output_re, m_output_im);
        }

        public fft_exception ifft_run(complex_data input, complex_data output)
        {
            Marshal.Copy(input.real, 0, m_input_re, input.real.Length);
            Marshal.Copy(input.imaginary, 0, m_input_im, input.imaginary.Length);
            Marshal.Copy(output.real, 0, m_output_re, output.real.Length);
            Marshal.Copy(output.imaginary, 0, m_output_re, output.imaginary.Length);
            return ifft(m_input_re, m_input_im, m_output_re, m_output_im);
        }

        public fft_exception set_fft_mode(fft_mode mode)
        {
            return fft_mode_setting(mode);
        }

        public int get_f_size()
        {
            return get_fft_size();
        }
    }
}
