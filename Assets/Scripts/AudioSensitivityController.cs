using UnityEngine;
using UnityEngine.UI;

namespace audio_app
{
    public class AudioSensitivityController : MonoBehaviour
    {
        public Canvas canvas;
        Dropdown sensiList;
        public enum Sensitivity
        {
            WEAK = 0,
            MEDIUM = 1,
            STRONG = 2
        }

        private Sensitivity sensi;
        public Sensitivity Sensi { get { return sensi; } }
        // Start is called before the first frame update
        void Start()
        {
            sensiList = canvas.transform.Find("Sensitivity").GetComponent<Dropdown>();
            sensiList.onValueChanged.AddListener(delegate { sensiListValueChanged(sensiList); });
            sensi = (Sensitivity)PlayerPrefs.GetInt("sensitivity", 0);
            //sensi = Sensitivity.STRONG;
        }

        void sensiListValueChanged(Dropdown change)
        {
            sensi = (Sensitivity)change.value;
            PlayerPrefs.SetInt("sensitivity", (int)sensi);
        }

    }
}