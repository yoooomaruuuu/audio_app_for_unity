using UnityEngine;

namespace audio_app
{
    public class PlayerIsWallTouch : MonoBehaviour
    {
        PlayerController playerController;
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

        private void OnDestroy()
        {
            playerController = null;
        }
    }
}
