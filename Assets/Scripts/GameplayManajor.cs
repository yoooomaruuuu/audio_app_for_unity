using UnityEngine;

namespace audio_app
{
    public class GameplayManajor : MonoBehaviour
    {
        ApplicationManajor appmanage;
        GameObject player;
        GoalPost goal;
        // Start is called before the first frame update
        void Start()
        {
            player = GameObject.Find("player");
            appmanage = GameObject.Find("SceneManajor").GetComponent<ApplicationManajor>();
            goal = GameObject.Find("goal").GetComponent<GoalPost>();
        }

        // Update is called once per frame
        void Update()
        {
            //エリア外判定
            if (player.transform.position.y < -10.0f) appmanage.death();
            //ゴール判定
            if (goal.IsPlayerTouch) appmanage.gameEnd();
        }

        private void OnDestroy()
        {
            appmanage = null;
            goal = null;
        }

    }
}
