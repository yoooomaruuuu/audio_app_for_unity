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
            STRONG = 0,
            MEDIUM = 1,
            WEAK = 2
        }

        private Sensitivity _sensi;
        public Sensitivity Sensi { get { return _sensi; } }
        // Start is called before the first frame update
        void Start()
        {
            sensiList = canvas.transform.Find("Sensitivity").GetComponent<Dropdown>();
            sensiList.onValueChanged.AddListener(delegate { sensiListValueChanged(sensiList); });
            _sensi = Sensitivity.STRONG;
        }

        void sensiListValueChanged(Dropdown change)
        {
            _sensi = (Sensitivity)change.value;
        }

    }
}