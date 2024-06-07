using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public UIDocument uiDocument;
    private Button tutorialButton;
    private Button stageButton;
    private Button rankingButton;

    private bool isTutorialButtonShaking = false;
    private bool isStageButtonShaking = false;
    private bool isRankingButtonShaking = false;

    private void Awake()
    {
        if (!UISoundManager.Instance.bgmSource.isPlaying)
        {
            UISoundManager.Instance.PlayBGM();
        }
    }

    private void OnEnable()
    {
        var root = uiDocument.rootVisualElement;

        tutorialButton = root.Q<Button>("TutorialButton");
        stageButton = root.Q<Button>("StageButton");
        rankingButton = root.Q<Button>("RankingButton");

        tutorialButton.RegisterCallback<MouseEnterEvent>(evt => OnMouseEnter(tutorialButton, "tutorial"));
        tutorialButton.RegisterCallback<MouseLeaveEvent>(evt => OnMouseLeave("tutorial"));
        tutorialButton.RegisterCallback<ClickEvent>(evt => OnButtonClick(tutorialButton));

        stageButton.RegisterCallback<MouseEnterEvent>(evt => OnMouseEnter(stageButton, "stage"));
        stageButton.RegisterCallback<MouseLeaveEvent>(evt => OnMouseLeave("stage"));
        stageButton.RegisterCallback<ClickEvent>(evt => OnButtonClick(stageButton));

        rankingButton.RegisterCallback<MouseEnterEvent>(evt => OnMouseEnter(rankingButton, "ranking"));
        rankingButton.RegisterCallback<MouseLeaveEvent>(evt => OnMouseLeave("ranking"));
        rankingButton.RegisterCallback<ClickEvent>(evt => OnButtonClick(rankingButton));
    }

    private void OnMouseEnter(Button button, string buttonType)
    {
        UISoundManager.Instance.PlayButtonHover();
        switch (buttonType)
        {
            case "tutorial":
                if (!isTutorialButtonShaking)
                {
                    StartCoroutine(ShakeButton(button, buttonType));
                }
                break;
            case "stage":
                if (!isStageButtonShaking)
                {
                    StartCoroutine(ShakeButton(button, buttonType));
                }
                break;
            case "ranking":
                if (!isRankingButtonShaking)
                {
                    StartCoroutine(ShakeButton(button, buttonType));
                }
                break;
        }
    }

    private void OnMouseLeave(string buttonType)
    {
        switch (buttonType)
        {
            case "tutorial":
                isTutorialButtonShaking = false;
                break;
            case "stage":
                isStageButtonShaking = false;
                break;
            case "ranking":
                isRankingButtonShaking = false;
                break;
        }
    }

    private void OnButtonClick(Button button)
    {
        StartCoroutine(ButtonClickAnimation(button));
    }

    private IEnumerator ShakeButton(Button button, string buttonType)
    {
        switch (buttonType)
        {
            case "tutorial":
                isTutorialButtonShaking = true;
                break;
            case "stage":
                isStageButtonShaking = true;
                break;
            case "ranking":
                isRankingButtonShaking = true;
                break;
        }

        Vector3 originalPosition = button.transform.position;
        float shakeAmount = 15f;
        float shakeDuration = 0.1f;

        while ((buttonType == "tutorial" && isTutorialButtonShaking) ||
               (buttonType == "stage" && isStageButtonShaking) ||
               (buttonType == "ranking" && isRankingButtonShaking))
        {
            float offsetX = Random.Range(-shakeAmount, shakeAmount);
            button.transform.position = originalPosition + new Vector3(offsetX, 0, 0);
            yield return new WaitForSeconds(shakeDuration);
        }

        button.transform.position = originalPosition;
    }

    private IEnumerator ButtonClickAnimation(Button button)
    {
        button.transform.scale = new Vector3(1.2f, 1.2f, 1.2f);
        if (button.name == "TutorialButton")
        {
            UISoundManager.Instance.PlayGameStart();
            UISoundManager.Instance.StopBGM();
        } else
        {
            UISoundManager.Instance.PlayButtonClick();
        }
        yield return new WaitForSeconds(0.1f);
        button.transform.scale = new Vector3(1f, 1f, 1f);       
        SceneManager.LoadScene(button.name.Replace("Button", ""));
    }
}
