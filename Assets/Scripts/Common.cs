using System;
namespace audio_app
{
    namespace common
    {
        public struct Int24
        {
            public Int24(Int32 data) { Value = data; }
            static public Int32 max() { return (Int32)Math.Pow(2.0, 24.0) / 2 - 1; }
            static public Int32 min() { return -1 * (Int32)Math.Pow(2.0, 24.0) / 2; }
            public Int32 Value { get; set; }
        }
        public enum BitRate 
        {
            Integer16 = 16,
            Integer24 = 24,
            Integer32 = 32,
        } ;
    }
}

