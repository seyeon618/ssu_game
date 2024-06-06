using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEditor;

public enum StageType
{
    Tutorial,
    Stage1,
    Stage2,
    Stage3
}

public class Player : MonoBehaviour
{
    public GameObject[] Blocks;

    public float MinX = -5.5f;
    public float MaxX = 5.5f;
    public float StartY = 20.5f;
    public float MoveDelay = 0.05f;
    private float _remainMoveDelay = 0.0f;
    public float RotateDelay = 0.1f;

    readonly private float MoveX = 0.5f;

    public GameObject _currentBlock;
    public GameObject NextBlock { get; set; }

    public uint CalcFloorPerFrame = 30;
    private uint _currentFrame = 0;

    private int _currentFloor = 0;
    public int CurrentFloor { get { return _currentFloor; } }
    public int MarginDistance = 17;
    private float _destPositionY = 0.0f;

    bool _cheatBlock = false;

    public int MaxHP = 3;
    private int _currentHP = 0;

    // For UI
    public UIDocument UI;
    private VisualElement _blockPreview;
    private VisualElement _playerHUD;

    public Texture2D HeartImage;
    public Texture2D EmptyHeartImage;

    public GameObject Indicator;

    public StageType Stage;

    // Start is called before the first frame update
    void Start()
    {
        _blockPreview = UI.rootVisualElement.Q<VisualElement>("BlockPreview");
        _playerHUD = UI.rootVisualElement.Q<VisualElement>("PlayerHUD");
        //foreach(GameObject block in Blocks)
        //{
        //    SaveTexture(AssetPreview.GetAssetPreview(block));
        //}

        SetupHP();
    }
    private void SaveTexture(Texture2D texture)
    {
        byte[] bytes = texture.EncodeToPNG();
        var dirPath = Application.dataPath + "/RenderOutput";
        if (!System.IO.Directory.Exists(dirPath))
        {
            System.IO.Directory.CreateDirectory(dirPath);
        }
        System.IO.File.WriteAllBytes(dirPath + "/R_" + Random.Range(0, 100000) + ".png", bytes);
        Debug.Log(bytes.Length / 1024 + "Kb was saved as: " + dirPath);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
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
                Indicator.transform.Translate(new Vector3(-MoveX, 0), Space.World);
            }
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            if (_currentBlock.transform.position.x + MoveX <= MaxX)
            {
                isKeyProcessed = true;
                _currentBlock.transform.Translate(new Vector3(MoveX, 0), Space.World);
                Indicator.transform.Translate(new Vector3(MoveX, 0), Space.World);
            }
        }
        else if(Input.GetKeyDown(KeyCode.Space))
        {
            MoveDownDirect(_currentBlock);
        }

        if (isKeyProcessed)
        {
            _remainMoveDelay = MoveDelay;
        }
    }

    void MoveDownDirect(GameObject targetBlock)
    {
        var collider = _currentBlock.GetComponent<Collider2D>();
        if(collider is BoxCollider2D)
        {
            var block = targetBlock.GetComponent<Block>();
            var boxCollider = collider as BoxCollider2D;
            int layerMask = LayerMask.GetMask("StackBlock");  // Player 레이어만 충돌 체크함
            //var cols = Physics2D.BoxCastAll(boxCollider.bounds.center, boxCollider.size, 0, Vector2.down, 100.0f);
            //var cols2 = Physics2D.RaycastAll(boxCollider.bounds.center, Vector2.down, 100.0f, layerMask);
            //Debug.DrawRay(boxCollider.bounds.center, Vector2.down * 100.0f, Color.red, 5.0f);
            var hit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.size, 0f, Vector2.down, 50.0f, layerMask);
            Debug.DrawRay(_currentBlock.transform.position, Vector3.down * 100.0f, Color.red, 5.0f);
            if (hit.collider != null)
            {
                var targetDistance = hit.distance - (block.FixedVelocity * (Time.fixedDeltaTime * 3)); // 3프레임 이전 위치
                targetBlock.transform.Translate(Vector3.down * targetDistance, Space.World);
            }
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
        if(Input.GetKeyDown(KeyCode.F5))
        {
            SceneManager.LoadScene("Tutorial");
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
        if(NextBlock == null)
        {
            NextBlock = PickBlock();
            Indicator.SetActive(true);
        }

        _currentBlock = Instantiate(NextBlock, transform.position, Quaternion.identity);
        NextBlock = PickBlock();

        _currentBlock.tag = "ActiveBlock";

        _blockPreview.style.backgroundImage = NextBlock.GetComponent<Block>().PreviewImage;
    }

    public void SetIndicatorWidth(int width, float x)
    {
        Indicator.transform.localScale = new Vector3(0.055f * width, Indicator.transform.localScale.y, Indicator.transform.localScale.z);
        Indicator.transform.position = new Vector3(x, Indicator.transform.position.y, Indicator.transform.position.z);
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

    private void SetupHP()
    {
        _currentHP = MaxHP;

        var test = _playerHUD.Children();
        foreach(var item in test)
        {
            Debug.Log(item);
        }

        for (int i = 0; i < _currentHP; ++i)
        {
            VisualElement visualElement = new VisualElement();
            visualElement.name = $"HP_{i + 1}";
            visualElement.style.backgroundImage = new StyleBackground(HeartImage);
            visualElement.style.width = 120;
            visualElement.style.height= 120;
            visualElement.style.marginLeft = 10;
            visualElement.style.marginRight = 10;
            _playerHUD.Add(visualElement);
        }
    }

    public void DecreaseHP()
    {
        int nowRemoveHeartIndex = _currentHP;
        var childs = _playerHUD.Children();
        List<VisualElement> elements = new List<VisualElement>(childs);
        var removeElement = elements[nowRemoveHeartIndex - 1];
        removeElement.style.backgroundImage = EmptyHeartImage;
        if (--_currentHP <= 0)
        {
            // OnGameOver
        }
    }
}
