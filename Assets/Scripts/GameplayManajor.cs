using UnityEngine;
using UnityEngine.SceneManagement;

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
        if (player.transform.position.y < -10.0f) appmanage.death();
        if (goal.getIsPlayerTouch()) appmanage.gameEnd();
    }

}
