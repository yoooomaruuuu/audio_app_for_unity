using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class GoalPost : MonoBehaviour
{
    bool isPlayerTouch = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) isPlayerTouch = true;
    }

    public bool getIsPlayerTouch()
    {
        return isPlayerTouch;
    }
}
