using UnityEngine;

namespace audio_app
{
    public class GoalPost : MonoBehaviour
    {
        bool isPlayerTouch = false;
        public bool IsPlayerTouch
        {
            get { return isPlayerTouch; }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player")) isPlayerTouch = true;
        }
    }
}
