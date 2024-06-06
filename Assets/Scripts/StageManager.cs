using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    public int WaitCount = 4;
    public int GoalFloor = 30;

    public Player GamePlayer;
    public UIDocument UI;

    private bool _isStarted = false;
    private bool _isGameStarted = false;
    private float _elapsedTime = 0.0f;
    private Label _centerLabel;
    private Label _elapsedTimeLabel;
    private Label _goalLeftFloorLabel;
    private Label _currentFloorLabel;
    // Start is called before the first frame update
    void Start()
    {
        _centerLabel = UI.rootVisualElement.Q<Label>("StartLabel");
        _elapsedTimeLabel = UI.rootVisualElement.Q<Label>("ElapsedTime");
        _goalLeftFloorLabel = UI.rootVisualElement.Q<Label>("GoalLeftFloor");
        _currentFloorLabel = UI.rootVisualElement.Q<Label>("CurrentFloor");

        _goalLeftFloorLabel.text = GoalFloor.ToString();
        _elapsedTimeLabel.text = "0";
        _currentFloorLabel.text = "0";
    }

    // Update is called once per frame
    void Update()
    {
        if (_isStarted)
        {
            _elapsedTime += Time.deltaTime;
        }

        OnHandleInput();
        UpdateUI();

        if (GamePlayer.CurrentFloor >= GoalFloor)
        {
            OnSucceed();
        }
    }

    void OnGameStart()
    {
        _isGameStarted = true;

        GamePlayer.PickNextBlock();
    }
    void OnSucceed()
    {
        SceneManager.LoadScene("Tutorial");
    }

    void UpdateUI()
    {
        if (_isStarted == false)
        {
            return;
        }

        if (_isGameStarted == false)
        {
            int remainTime = (int)(WaitCount - _elapsedTime);
            if (remainTime <= 0)
            {
                OnGameStart();
                _centerLabel.visible = false;
            }
            else
            {
                _centerLabel.text = remainTime.ToString();
            }

            return;
        }

        int elapsedTime = (int)(_elapsedTime - WaitCount);
        _elapsedTimeLabel.text = elapsedTime.ToString();
        _currentFloorLabel.text = GamePlayer.CurrentFloor.ToString();
        _goalLeftFloorLabel.text = (GoalFloor - GamePlayer.CurrentFloor).ToString();
    }

    void OnHandleInput()
    {
        if (_isStarted == false)
        {
            if (Input.anyKeyDown)
            {
                _isStarted = true;
            }

            return;
        }
    }
}
