using System.Text;
using UnityEngine;
using UnityEngine.UI;
using lib_audio_analysis;

public class configMenuController : MonoBehaviour
{
    ApplicationManajor appmanage;
    public Canvas canvas;
    Dropdown samplingRateList;
    Dropdown channelList;
    Dropdown bitsPerSampleList;
    Dropdown devicesList;

    InputCaptureFuncs inputCap;

    // Start is called before the first frame update
    void Start()
    {
        appmanage = GameObject.Find("SceneManajor").GetComponent<ApplicationManajor>();
        samplingRateList = canvas.transform.Find("SamplingRate").GetComponent<Dropdown>();
        channelList = canvas.transform.Find("Channel").GetComponent<Dropdown>();
        bitsPerSampleList = canvas.transform.Find("BitsPerSample").GetComponent<Dropdown>();
        devicesList = canvas.transform.Find("Devices").GetComponent<Dropdown>();
        samplingRateList.value = PlayerPrefs.GetInt("s-rate", 0);
        channelList.value = PlayerPrefs.GetInt("ch", 0);
        bitsPerSampleList.value = PlayerPrefs.GetInt("bps", 0);
        devicesList.value = PlayerPrefs.GetInt("device", 0);
        inputCap = appmanage.GetInputCap();

        for (int i = 0; i < inputCap.getInputDevicesListSize(); i++)
        {
            var tmp = new StringBuilder(256, 256);
            inputCap.getInputDevicesList(i, tmp);
            devicesList.options.Add(new Dropdown.OptionData { text = tmp.ToString() });
        }
    }

    public void configApply()
    {
        if(devicesList.value != 0)
        {
            appmanage.config = new ApplicationManajor.Config(
                uint.Parse(samplingRateList.captionText.text),
                ushort.Parse(channelList.captionText.text), 
                ushort.Parse(bitsPerSampleList.captionText.text), 
                16, 
                devicesList.value - 1
                );
            PlayerPrefs.SetInt("s-rate", samplingRateList.value);
            PlayerPrefs.SetInt("ch", channelList.value);
            PlayerPrefs.SetInt("bps", bitsPerSampleList.value);
            PlayerPrefs.SetInt("device", devicesList.value);
            appmanage.gameStart();
        }
        else
        {
            Debug.Log("non selected device");
        }
    }

}
