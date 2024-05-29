using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject[] Blocks;

    public float MinX = -5.5f;
    public float MaxX = 5.5f;
    public float MoveDelay = 0.05f;
    private float _remainMoveDelay = 0.0f;
    public float RotateDelay = 0.1f;

    readonly private float MoveX = 1.0f;

    public GameObject _currentBlock;

    // Start is called before the first frame update
    void Start()
    {
        PickNextBlock();
    }

    // Update is called once per frame
    void Update()
    {
        HandleKeyboardInput();
    }

    void HandleMove()
    {
        _remainMoveDelay = Mathf.Max(0.0f, _remainMoveDelay - Time.deltaTime);
        if (_remainMoveDelay > 0.0f)
        {
            return;
        }

        bool isKeyProcessed = false;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            if (_currentBlock.transform.position.x - MoveX >= MinX)
            {
                isKeyProcessed = true;
                _currentBlock.transform.Translate(new Vector3(-MoveX, 0), Space.World);
            }
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            if (_currentBlock.transform.position.x + MoveX <= MaxX)
            {
                isKeyProcessed = true;
                _currentBlock.transform.Translate(new Vector3(MoveX, 0), Space.World);
            }
        }

        if (isKeyProcessed)
        {
            _remainMoveDelay = MoveDelay;
        }
    }

    void HandleRotate()
    {
        // 회전은 키 누르고 있을 때 반응하지 않는 것이 의도.
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            _currentBlock.GetComponent<Block>().RotateNext();
        }
    }

    void HandleKeyboardInput()
    {
        HandleMove();

        HandleRotate();
    }

    GameObject PickBlock()
    {
        return Blocks[Random.Range(0, Blocks.Length)];
    }

    public void PickNextBlock()
    {
        _currentBlock = Instantiate<GameObject>(PickBlock(), transform.position, Quaternion.identity);
        _currentBlock.tag = "ActiveBlock";
    }

}
