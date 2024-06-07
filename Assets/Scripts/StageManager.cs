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
    private bool _isStageClear = false;

    public List<Texture2D> StageClearEffectSprite = new List<Texture2D>();
    int _stageClearEffectSpriteIndex = 0;
    public float StageClearEffectChangeDelay = 0.2f;
    private float _stageClearEffectRemainDelay = 0.0f;

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
            if(_isStageClear == false)
            {
                OnStageClear();
                _isStageClear = true;
            }
        }
    }

    void OnGameStart()
    {
        _isGameStarted = true;

        GamePlayer.PickNextBlock();
    }

    void UpdateUI()
    {
        if(_isStageClear)
        {
            _stageClearEffectRemainDelay -= Time.deltaTime;
            if(_stageClearEffectRemainDelay <= 0)
            {
                _stageClearEffectSpriteIndex = (_stageClearEffectSpriteIndex + 1) % StageClearEffectSprite.Count;
                _stageClearEffectRemainDelay = StageClearEffectChangeDelay;

                var stageClearEffect = UI.rootVisualElement.Q<VisualElement>("StageClearEffect");
                stageClearEffect.style.backgroundImage = StageClearEffectSprite[_stageClearEffectSpriteIndex];
            }
        }

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

    public void OnStageClear()
    {
        GamePlayer.OnStageClear();
        StartCoroutine(RunUIStageClear());
    }

    IEnumerator RunUIStageClear()
    {
        UI.rootVisualElement.Q<VisualElement>("GameUI").style.opacity = new StyleFloat(0.0f);

        for (int i = 0; i < 210; ++i)
        {
            //List<VisualElement> elements = new List<VisualElement>(UI.rootVisualElement.Children());
            //elements[0].style.backgroundColor = new StyleColor(new Color(0, 0, 0, i));
            UI.rootVisualElement.style.backgroundColor = new StyleColor(new Color(0, 0, 0, Mathf.Lerp(0, 0.8f, i / 210.0f)));
            yield return new WaitForFixedUpdate();
        }

        var stageClearVisualElement = UI.rootVisualElement.Q<VisualElement>("StageClear");
        stageClearVisualElement.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);

        var elapsedText = UI.rootVisualElement.Q<Label>("TimeText");
        elapsedText.text = ((int)_elapsedTime).ToString();

        var maintextElement = stageClearVisualElement.Q<VisualElement>("StageClearText");
        for (int i = 0; i < 120; ++i)
        {
            maintextElement.style.opacity = new StyleFloat(Mathf.Lerp(0, 1, i / 120.0f));
            yield return new WaitForFixedUpdate();
        }

        _stageClearEffectRemainDelay = StageClearEffectChangeDelay;

        _isRestartable = true;
    }
}
