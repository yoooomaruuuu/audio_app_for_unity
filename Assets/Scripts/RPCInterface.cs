using MagicOnion;

namespace naudio_udp_server
{

    public interface IRecordingOrderService : IService<IRecordingOrderService>
    {
        //UnaryResult<int> init();//(int deviceNumber, int samplingRate, int bitRate, int channel, int bufferMilliSec = 16);
        UnaryResult<int> SettingInputDevice(int deviceNumber, int samplingRate, int bitRate, int channel, int bufferMilliSec = 16);
        UnaryResult<int> StartRecording();
        UnaryResult<int> StopRecording();
        UnaryResult<string[]> GetListInputDevices();
    }
}

