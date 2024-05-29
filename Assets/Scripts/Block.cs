using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    // Start is called before the first frame update
    public float FixedVelocity = 10.0f;
    public int RotateAngleCount = 0;
    private int _currentRotation = 0;

    private bool _isControlByPlayer = true;
    private Rigidbody2D _rigidbody = null;
    private Player _player = null;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();

        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        if (_isControlByPlayer)
        {
            _rigidbody.velocity = new Vector2(0.0f, -FixedVelocity);
        }
    }

    public void FinishControl()
    {
        _isControlByPlayer = false;

        transform.tag = "FreeBlock";

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
    }

    public void OnEscapeGameArea()
    {
        if(_isControlByPlayer)
        {
            FinishControl();
        }
        Destroy(gameObject);
    }
}
