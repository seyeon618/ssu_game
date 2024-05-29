using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameArea : MonoBehaviour
{
    private Player _player;
    private void Awake()
    {
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        switch(collision.tag)
        {
            case "FreeBlock":
            case "ActiveBlock":
                {
                    collision.gameObject.GetComponent<Block>().OnEscapeGameArea();
                }
                break;
        }
    }
}
