using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    // Start is called before the first frame update
    public int RotateAngleCount = 0;
    private int _currentRotation = 0;
    public int[] WidthByRotate = new int[2];
    public float[] WidthByRotateOffsetX = new float[2];

    private int CurrentWidth { get { return WidthByRotate[_currentRotation % WidthByRotate.Length]; } }
    private float CurrentOffset { get { return WidthByRotateOffsetX[_currentRotation % WidthByRotateOffsetX.Length]; } }

    private bool _isControlByPlayer = true;
    private Rigidbody2D _rigidbody = null;
    private Player _player = null;

    public Texture2D PreviewImage = null;

    [Header("Sounds")]
    public AudioSource HitBlockSound;
    public AudioSource RotationSound;

    public bool IsVineBlock { get; set; }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    void Start()
    {
        _rigidbody.mass = 0;
        _player.SetIndicatorWidth(CurrentWidth, transform.position.x + CurrentOffset);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        if (_isControlByPlayer)
        {
            _rigidbody.velocity = new Vector2(0.0f, -_player.CurrentVelocity);
        }
    }

    public void FinishControl()
    {
        HitBlockSound.Play();

        _isControlByPlayer = false;

        transform.tag = "FreeBlock";
        transform.gameObject.layer = LayerMask.NameToLayer("StackBlock");
        _rigidbody.mass = 5;

        _player.PickNextBlock();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Ground":
            case "FreeBlock":
                {
                    if(_isControlByPlayer)
                    {
                        FinishControl();
                        if(IsVineBlock)
                        {
                            if(collision.gameObject.tag == "FreeBlock")
                            {
                                gameObject.AddComponent<FixedJoint2D>().connectedBody = collision.rigidbody;
                                collision.gameObject.GetComponent<Block>().AddBlockEffect(_player.VineBlockSprite, 1.2f);
                                _player.VineSound.Play();
                            }
                        }
                    }
                }
                break;
        }
    }

    public void RotateNext()
    {
        if(RotateAngleCount == 0)
        {
            return;
        }
        _currentRotation = (_currentRotation + 1) % RotateAngleCount;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90 * _currentRotation));

        _player.SetIndicatorWidth(CurrentWidth, transform.position.x + CurrentOffset);

        RotationSound.Play();
    }

    public void OnEscapeGameArea()
    {
        if(_isControlByPlayer)
        {
            FinishControl();
        }
        Destroy(gameObject);

        _player.DecreaseHP();
    }

    public void AddBlockEffect(Sprite sprite, float scale)
    {
        for(int i = 0; i < transform.childCount; ++i)
        {
            var childTransform = transform.GetChild(i);
            if(childTransform.name.Contains("Square_") == false)
            {
                continue;
            }
            var childObject = new GameObject("BlockEffect", typeof(SpriteRenderer));
            childObject.transform.parent = childTransform;
            childObject.GetComponent<SpriteRenderer>().sprite = sprite;
            childObject.transform.localPosition = new Vector3(0, 0, -1);
            childObject.transform.localScale = new Vector3(scale, scale, 1.0f);
        }
    }
}
