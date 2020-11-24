using System.Runtime.InteropServices;

namespace lib_world
{
    class F0EstimateFuncs
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct HarvestOption {
            public double f0Floor;
            public double f0Ceil;
            public double framePeriod;
        }

        [DllImport("win", EntryPoint = "Harvest", CallingConvention = CallingConvention.StdCall)]
        static extern void Harvest(double[] x, int xLength, int fs, ref HarvestOption option, double[] temporalPositions, double[] f0);

        [DllImport("win", EntryPoint = "InitializeHarvestOption", CallingConvention = CallingConvention.StdCall)]
        static extern void InitializeHarvestOption(ref HarvestOption option);

        [DllImport("win", EntryPoint = "GetSamplesForHarvest", CallingConvention = CallingConvention.StdCall)]
        static extern int GetSamplesForHarvest(int fs, int xLength, double framePeriod);

        HarvestOption mOption;
        public F0EstimateFuncs(double framePeriod, double f0Floor=40, double f0Ceil=8000)
        {
            mOption = new HarvestOption();
            InitializeHarvestOption(ref mOption);
            mOption.framePeriod = framePeriod;
            mOption.f0Floor = f0Floor;
            mOption.f0Ceil = f0Ceil;
        }

        public void HarvestExecute(double[] x, int xLength, int fs, double[] temporalPostions, double[] f0)
        {
            Harvest(x, xLength, fs, ref mOption, temporalPostions, f0);
        }

        public int GetF0Size(int fs, int xLength, double framePeriod)
        {
            return GetSamplesForHarvest(fs, xLength, framePeriod);
        }
    }
}
