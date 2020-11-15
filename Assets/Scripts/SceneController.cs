using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    GoalPost goal;
    // Start is called before the first frame update
    void Start()
    {
        goal = GameObject.Find("goal").GetComponent<GoalPost>();
    }

    // Update is called once per frame
    void Update()
    {
        if (goal.getIsPlayerTouch()) SceneManager.LoadScene(0);
    }
}
