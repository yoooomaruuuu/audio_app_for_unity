using UnityEngine;

namespace audio_app
{
    public class PlayerIsWallTouch : MonoBehaviour
    {
        PlayerController playerController;
        // Start is called before the first frame update
        void Start()
        {
            playerController = this.gameObject.transform.parent.gameObject.GetComponent<PlayerController>();
        }


        void OnTriggerEnter2D(Collider2D col)
        {
            playerController.IsWallTouch = true;
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            playerController.IsWallTouch = false;
        }
    }
}
