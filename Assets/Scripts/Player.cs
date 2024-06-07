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

    public Transform BlockSpawnPosition;
    public float BlockFixedVelocity = 5.0f;
    public float BlockVelocityIncrease = 1.0f;
    private float _increasedVelocity = 0.0f;
    private int _blockCount = 0;
    private float _currentVelocity = 0.0f;
    public float CurrentVelocity { get { return _currentVelocity + _increasedVelocity; } set { _currentVelocity = value; } }

    private bool _isMovable = true;

    [Header("스테이지1_강제회전")]
    [Tooltip("확률은 백만분율")]
    public int ForceRotationProbability = 250000;
    public float ForceRotationDelay = 0.5f;
    public Sprite ForceRotationBlockSprite = null;
    private bool _isRotatable = true;
    private Coroutine _forceRotationCoroutine;
    public AudioSource ForceRotationSound;
    [Header("스테이지1_안개")]
    public int InvokeStartFogFloor = 27;
    public GameObject FogObject;
    public Transform FogPosition;
    private bool _isFogCreated = false;
    public AudioSource FogSound;

    [Header("스테이지2")]
    public int IceProbability = 250000;
    public Sprite IceBlockSprite = null;
    public PhysicsMaterial2D IceMaterial;
    public AudioSource IceSound;

    [Header("스테이지3")]
    public int VineProbability = 200000;
    public Sprite VineBlockSprite = null;
    public AudioSource VineSound;

    [Header("스테이지3_비누방울")]
    public int BubbleProbability = 200000;
    public GameObject BubbleObject = null;
    public Sprite BubblePopSprite = null;
    public AudioSource BubbleSound;
    public AudioSource BubblePopSound;
    public float BubbleMinDelay = 1.5f;
    public float BubbleMaxDelay = 4.0f;
    public float BubbleFixedVelocity = 3.0f;
    private float _bubbleMoveXPos = 0;
    private float _bubbleStartXPos = 0;
    private float _bubbleElapsed = 0;
    private float _bubbleMoveDelay = 0;
    private bool _isBubble = false;

    private bool _isGameOver = false;

    // Start is called before the first frame update
    void Start()
    {
        _blockPreview = UI.rootVisualElement.Q<VisualElement>("BlockPreview");
        _playerHUD = UI.rootVisualElement.Q<VisualElement>("PlayerHUD");
        //foreach(GameObject block in Blocks)
        //{
        //    SaveTexture(AssetPreview.GetAssetPreview(block));
        //}

        SetupHP(MaxHP);

        CurrentVelocity = BlockFixedVelocity;
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

        OnEffect();

        CalcFloor();
    }

    void HandleMove()
    {
        if(_currentBlock == null)
        {
            return;
        }
        if(_isMovable == false)
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
        var block = targetBlock.GetComponent<Block>();
        var collider = targetBlock.GetComponent<Collider2D>();
        int layerMask = LayerMask.GetMask("StackBlock");  // Player 레이어만 충돌 체크함

        ContactFilter2D contactFilter2D = new ContactFilter2D() { layerMask = layerMask, useLayerMask = true };
        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        if(collider.Cast(Vector2.down, contactFilter2D, hits, 100.0f) > 0)
        {
            var targetDistance = hits[0].distance - (-CurrentVelocity * (Time.fixedDeltaTime * 3)); // 3프레임 이전 위치
            targetBlock.transform.Translate(Vector3.down * targetDistance, Space.World);
        }
    }

    void HandleRotate()
    {
        if(_isRotatable == false) // 블록 효과에 의해 회전 불가
        {
            return;
        }

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
        if (Input.GetKeyDown(KeyCode.F2))
        {
            SetupHP(_currentHP + 1);
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
            SceneManager.LoadScene("MainMenu");
        }
    }

    void HandleKeyboardInput()
    {
        if(_isGameOver)
        {
            return;
        }

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
        if(_forceRotationCoroutine != null)
        {
            StopCoroutine(_forceRotationCoroutine);
            _forceRotationCoroutine = null;
        }
        _isRotatable = true; // 블럭을 바꾸는 순간 다시 활성화
        _isMovable = true;
        if(_isBubble)
        {
            // 기획 의도는 아닌데 일단 예외처리
            CurrentVelocity = BlockFixedVelocity;

            var bubbleObject = _currentBlock.transform.Find("Bubble(Clone)");
            if (bubbleObject != null)
            {
                bubbleObject.GetComponentInChildren<SpriteRenderer>().sprite = BubblePopSprite;
                BubblePopSound.Play();
                StartCoroutine(RemoveBubble(bubbleObject.gameObject));
            }

            _isBubble = false;
        }

        if(_isGameOver)
        {
            return;
        }

        if (NextBlock == null)
        {
            NextBlock = PickBlock();
            Indicator.SetActive(true);
        }

        _currentBlock = Instantiate(NextBlock, BlockSpawnPosition.position, Quaternion.identity);
        NextBlock = PickBlock();

        _currentBlock.tag = "ActiveBlock";
        _blockPreview.style.backgroundImage = NextBlock.GetComponent<Block>().PreviewImage;

        switch(Stage)
        {
            case StageType.Stage1:
                {
                    int prob = Random.Range(1, 1000000);
                    if(prob - ForceRotationProbability <= 0)
                    {
                        _currentBlock.GetComponent<Block>().AddBlockEffect(ForceRotationBlockSprite, 1.2f);
                        _isRotatable = false;
                        _forceRotationCoroutine = StartCoroutine(BlockForceRotation(ForceRotationDelay));
                    }
                }
                break;
            case StageType.Stage2:
                {
                    int prob = Random.Range(1, 1000000);
                    if (prob - IceProbability <= 0)
                    {
                        _currentBlock.GetComponent<Block>().AddBlockEffect(IceBlockSprite, 0.4f);
                        _currentBlock.GetComponent<Rigidbody2D>().sharedMaterial = IceMaterial;
                        IceSound.Play();
                    }
                }
                break;
            case StageType.Stage3:
                {
                    int prob = Random.Range(1, 1000000);
                    if(prob - VineProbability <= 0)
                    {
                        _currentBlock.GetComponent<Block>().AddBlockEffect(VineBlockSprite, 1.2f);
                        _currentBlock.GetComponent<Block>().IsVineBlock = true;
                    }
                    else
                    {
                        prob = Random.Range(1, 1000000);
                        if (prob - BubbleProbability <= 0)
                        {
                            var bubbleGameObject = Instantiate(BubbleObject, _currentBlock.transform);
                            bubbleGameObject.transform.localPosition = new Vector3(0, 0, -2);
                            var col = _currentBlock.GetComponent<Collider2D>();
                            var offset = col.offset;
                            if (col is PolygonCollider2D)
                            {
                                offset.y += 0.5f;
                            }
                            bubbleGameObject.transform.localPosition = new Vector3(offset.x, offset.y, -2.0f);
                            BubbleSound.Play();
                            _isRotatable = false;
                            _isMovable = false;

                            _bubbleMoveDelay = Random.Range(BubbleMinDelay, BubbleMaxDelay);
                            _bubbleElapsed = 0.0f;
                            _bubbleStartXPos = bubbleGameObject.transform.position.x;
                            _bubbleMoveXPos = _bubbleStartXPos + (-4.0f + (0.5f * Random.Range(0, 16))); // 0.5씩 움직이도록
                            CurrentVelocity = BubbleFixedVelocity;
                            _isBubble = true;
                        }
                    }
                }
                break;
        }

        if (++_blockCount % 10 == 0)
        {
            _increasedVelocity += BlockVelocityIncrease;
        }
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
                _currentFloor = (int)(hit.point.y - 3 + 0.1f); // 오차 무시하기위해 0.1 더함

                _destPositionY = hit.point.y + MarginDistance;
            }

            switch(Stage)
            {
                case StageType.Stage1:
                    {
                        if(_currentFloor >= InvokeStartFogFloor)
                        {
                            if(_isFogCreated == false)
                            {
                                StartCoroutine(FogAppear(Instantiate(FogObject, FogPosition.position, Quaternion.identity)));
                                FogSound.Play();
                                _isFogCreated = true;
                            }
                        }
                    }
                    break;
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

    private void SetupHP(int hp)
    {
        _currentHP = hp;

        _playerHUD.Clear();

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
        if(nowRemoveHeartIndex >= 1)
        {
            var removeElement = elements[nowRemoveHeartIndex - 1];
            removeElement.style.backgroundImage = EmptyHeartImage;
        }
        if (--_currentHP <= 0)
        {
            if(_isGameOver == false)
            {
                _isGameOver = true;

                Destroy(_currentBlock);
                

                GameObject.Find("StageManager").GetComponent<StageManager>().OnGameOver();
            }
        }
    }

    IEnumerator BlockForceRotation(float delay)
    {
        yield return new WaitForSeconds(delay);

        for(int i = 0; i < Random.Range(0, 3); ++i)
        {
            _currentBlock.GetComponent<Block>().RotateNext();
        }

        ForceRotationSound.Play();
    }

    IEnumerator FogAppear(GameObject fogObject)
    {
        for(int i = 0; i <= 10; ++i)
        {
            var renderers = fogObject.GetComponentsInChildren<SpriteRenderer>();
            foreach (var renderer in renderers)
            {
                renderer.color = new Color(1, 1, 1, Mathf.Lerp(0, 1, i / 10.0f));
            }

            yield return new WaitForSeconds(0.07f);
        }
    }

    IEnumerator RemoveBubble(GameObject bubbleObject)
    {
        yield return new WaitForSeconds(0.6f);

        Destroy(bubbleObject);
    }

    private void OnEffect()
    {
        if(_isBubble)
        {
            if(_bubbleElapsed >= _bubbleMoveDelay)
            {
                _isBubble = false;
                _isRotatable = true;
                _isMovable = true;
                CurrentVelocity = BlockFixedVelocity;

                var bubbleObject = _currentBlock.transform.Find("Bubble(Clone)");
                if (bubbleObject != null)
                {
                    bubbleObject.GetComponentInChildren<SpriteRenderer>().sprite = BubblePopSprite;
                    BubblePopSound.Play();
                    StartCoroutine(RemoveBubble(bubbleObject.gameObject));
                }

                return;
            }

            float t = _bubbleElapsed / _bubbleMoveDelay;
            float destX = Mathf.Lerp(_bubbleStartXPos, _bubbleMoveXPos, t);
            _currentBlock.transform.position = new Vector3(destX, _currentBlock.transform.position.y, _currentBlock.transform.position.z);
            var blockCollider = _currentBlock.GetComponent<Collider2D>();
            Indicator.transform.position = new Vector3(destX + blockCollider.offset.x, Indicator.transform.position.y, Indicator.transform.position.z);
            _bubbleElapsed += Time.deltaTime;
        }
    }
}
