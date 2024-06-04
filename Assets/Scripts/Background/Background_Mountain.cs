using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background_Mountain : MonoBehaviour
{
    public float MoveSpeed = 1.0f;

    public GameObject CloudLayer;
    private float _cloudLayerSize = 40.0f;
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
        foreach (Transform child in CloudLayer.transform)
        {
            child.Translate(new Vector3(-MoveSpeed * Time.deltaTime, 0));
            if (child.localPosition.x < -_cloudLayerSize)
            {
                child.localPosition = new Vector3(_cloudLayerSize, child.localPosition.y);
            }
        }
    }
}
