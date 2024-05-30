using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject[] Blocks;

    public float MinX = -5.5f;
    public float MaxX = 5.5f;
    public float StartY = 20.5f;
    public float MoveDelay = 0.05f;
    private float _remainMoveDelay = 0.0f;
    public float RotateDelay = 0.1f;

    readonly private float MoveX = 1.0f;

    public GameObject _currentBlock;

    public uint CalcFloorPerFrame = 30;
    private uint _currentFrame = 0;

    private int _currentFloor = 0;
    public int CurrentFloor { get { return _currentFloor; } }
    public int MarginDistance = 17;
    private float _destPositionY = 0.0f;

    bool _cheatBlock = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        HandleKeyboardInput();

        CalcFloor();
    }

    void HandleMove()
    {
        if(_currentBlock == null)
        {
            return;
        }

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

    void HandleCheat()
    {
        if(Input.GetKeyDown(KeyCode.F1))
        {
            _cheatBlock = !_cheatBlock;
        }
    }

    void HandleFunctionKey()
    {
        HandleCheat();

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void HandleKeyboardInput()
    {
        HandleFunctionKey();

        HandleMove();

        HandleRotate();
    }

    GameObject PickBlock()
    {
        if(_cheatBlock)
        {
            return Blocks[3];
        }
        else
        {
            return Blocks[Random.Range(0, Blocks.Length)];
        }
    }

    public void PickNextBlock()
    {
        _currentBlock = Instantiate<GameObject>(PickBlock(), transform.position, Quaternion.identity);
        _currentBlock.tag = "ActiveBlock";
    }

    public void CalcFloor()
    {
        if(++_currentFrame % CalcFloorPerFrame == 0)
        {
            int layerMask = LayerMask.GetMask("StackBlock");  // Player 레이어만 충돌 체크함
            var hit = Physics2D.BoxCast(transform.position, new Vector2(6, 1), 0, Vector2.down, 100.0f, layerMask);
            if(hit.collider != null)
            {
                _currentFloor = (int)(hit.point.y - 3);

                _destPositionY = hit.point.y + MarginDistance;
            }
        }

        if(Mathf.Abs(_destPositionY - transform.position.y) > 1.0f)
        {
            float moveY = 10 * Time.deltaTime;
            if(_destPositionY < transform.position.y)
            {
                moveY = -moveY;
            }
            transform.Translate(new Vector3(0, moveY));
            if(transform.position.y < StartY)
            {
                transform.position = new Vector3(transform.position.x, StartY);
            }
        }
    }
}
