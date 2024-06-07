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

    private bool _isRestartable = false;
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

        if(_isRestartable)
        {
            if(Input.anyKey)
            {
                SceneManager.LoadScene("MainMenu");
            }
        }
    }

    public void OnGameOver()
    {
        StartCoroutine(RunUIGameOver());
    }

    IEnumerator RunUIGameOver()
    {
        UI.rootVisualElement.Q<VisualElement>("GameUI").style.opacity = new StyleFloat(0.0f);

        for (int i = 0; i < 210; ++i)
        {
            //List<VisualElement> elements = new List<VisualElement>(UI.rootVisualElement.Children());
            //elements[0].style.backgroundColor = new StyleColor(new Color(0, 0, 0, i));
            UI.rootVisualElement.style.backgroundColor = new StyleColor(new Color(0, 0, 0, Mathf.Lerp(0, 0.7f, i / 210.0f)));
            yield return new WaitForFixedUpdate();
        }

        var gameoverVisualElement = UI.rootVisualElement.Q<VisualElement>("GameOver");
        gameoverVisualElement.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

        var maintextElement = gameoverVisualElement.Q<VisualElement>("MainText");

        for (int i = 0; i < 120; ++i)
        {
            maintextElement.style.opacity = new StyleFloat(Mathf.Lerp(0, 1, i / 120.0f));
            yield return new WaitForFixedUpdate();
        }

        _isRestartable = true;
    }
}
