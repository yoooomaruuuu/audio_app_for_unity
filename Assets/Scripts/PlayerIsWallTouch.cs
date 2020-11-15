using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIsWallTouch : MonoBehaviour
{
    PlayerController parent;
    // Start is called before the first frame update
    void Start()
    {
        parent = this.gameObject.transform.parent.gameObject.GetComponent<PlayerController>();
        Debug.Log(parent);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        parent.setIsWallTouch(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        parent.setIsWallTouch(false);
    }


}
