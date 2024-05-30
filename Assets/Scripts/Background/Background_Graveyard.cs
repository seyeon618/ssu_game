using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background_Graveyard : MonoBehaviour
{
    public float MoveSpeed = 1.0f;

    public GameObject StarLayer;
    private float _starLayerSize = 40.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveStarLayer();
    }

    void MoveStarLayer()
    {
        foreach (Transform child in StarLayer.transform)
        {
            child.Translate(new Vector3(-MoveSpeed * Time.deltaTime, 0));
            if (child.localPosition.x < -_starLayerSize)
            {
                child.localPosition = new Vector3(_starLayerSize, child.localPosition.y);
            }
        }
    }
}
