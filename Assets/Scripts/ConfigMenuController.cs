using System.Text;
using UnityEngine;
using UnityEngine.UI;
using lib_audio_analysis;
using audio_app.common;
using TMPro;

namespace audio_app
{
    public class ConfigMenuController : MonoBehaviour
    {
        public Canvas canvas;
        ApplicationManager appmanage;
        Dropdown samplingRateList;
        Dropdown channelList;
        Dropdown bitsPerSampleList;
        Dropdown devicesList;

        string descTextColor = "#1C2209";
        string errorTextColor = "#ed4747";

        [SerializeField]
        private GameObject textObject;

        TextMeshProUGUI descriptionText;

        InputCaptureFuncs inputCap;

        // Start is called before the first frame update
        void Start()
        {
            descriptionText = textObject.GetComponent<TextMeshProUGUI>();

            appmanage = GameObject.Find("SceneManajor").GetComponent<ApplicationManager>();
            samplingRateList = canvas.transform.Find("SamplingRate").GetComponent<Dropdown>();
            channelList = canvas.transform.Find("Channel").GetComponent<Dropdown>();
            bitsPerSampleList = canvas.transform.Find("BitsPerSample").GetComponent<Dropdown>();
            devicesList = canvas.transform.Find("Devices").GetComponent<Dropdown>();
            samplingRateList.value = PlayerPrefs.GetInt("s-rate", 0);
            channelList.value = PlayerPrefs.GetInt("ch", 0);
            bitsPerSampleList.value = PlayerPrefs.GetInt("bps", 0);
            devicesList.value = PlayerPrefs.GetInt("device", 0);


            inputCap = appmanage.InputCap;

            for (int i = 0; i < inputCap.getInputDevicesListSize(); i++)
            {
                var tmp = new StringBuilder(256, 256);
                inputCap.getInputDevicesList(i, tmp);
                devicesList.options.Add(new Dropdown.OptionData { text = tmp.ToString() });
            }
        }

        public void configApply()
        {
            if (devicesList.value != 0)
            {
                BitRate bitRate = BitRate.Integer16;
                if(bitsPerSampleList.value == 0)
                {
                    bitRate = BitRate.Integer16;
                }
                else if(bitsPerSampleList.value == 1)
                {
                    bitRate = BitRate.Integer24;
                }
                else if(bitsPerSampleList.value == 2)
                {
                    bitRate = BitRate.Integer32;
                }
                else if(bitsPerSampleList.value == 3)
                {
                    bitRate = BitRate.Integer64;
                }
                else if(bitsPerSampleList.value == 4)
                {
                    bitRate = BitRate.Floating32;
                }
                appmanage.AppConfig = new ApplicationManager.Config(
                    uint.Parse(samplingRateList.captionText.text),
                    ushort.Parse(channelList.captionText.text),
                    bitRate,
                    8,
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
                Color textColor = Color.red;
                ColorUtility.TryParseHtmlString(errorTextColor, out textColor);
                descriptionText.color = textColor;
                descriptionText.text = "入力音声デバイスが選択されていません。";
            }
        }

        public void enterMouseDeviceDrop()
        {
            Color textColor = Color.black;
            ColorUtility.TryParseHtmlString(descTextColor, out textColor);
            descriptionText.color = textColor;
            descriptionText.text = "入力音声デバイスの設定";
        }
        public void exitMouseDeviceDrop()
        {
            descriptionText.text = null;
        }

        public void enterMouseSrateDrop()
        {
            Color textColor = Color.black;
            ColorUtility.TryParseHtmlString(descTextColor, out textColor);
            descriptionText.color = textColor;
            descriptionText.text = "デバイスのサンプリングレートの設定";
        }
        public void exitMouseSrateDrop()
        {
            descriptionText.text = null;
        }
        public void enterMouseChDrop()
        {
            Color textColor = Color.black;
            ColorUtility.TryParseHtmlString(descTextColor, out textColor);
            descriptionText.color = textColor;
            descriptionText.text = "デバイスのチャンネルの設定";
        }
        public void exitMouseChDrop()
        {
            descriptionText.text = null;
        }
        public void enterMouseBpsDrop()
        {
            Color textColor = Color.black;
            ColorUtility.TryParseHtmlString(descTextColor, out textColor);
            descriptionText.color = textColor;
            descriptionText.text = "デバイスのサンプルごとのビットレートの設定";
        }
        public void exitMouseBpsDrop()
        {
            descriptionText.text = null;
        }

        private void OnDestroy()
        {
            inputCap = null;
            appmanage = null;
        }

    }
}