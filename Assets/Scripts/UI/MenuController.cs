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
    private bool isShaking = false;

    private void OnEnable()
    {
        var root = uiDocument.rootVisualElement;

        tutorialButton = root.Q<Button>("TutorialButton");
        stageButton = root.Q<Button>("StageButton");
        rankingButton = root.Q<Button>("RankingButton");

        tutorialButton.RegisterCallback<MouseEnterEvent>(evt => StartCoroutine(ShakeButton(tutorialButton)));
        tutorialButton.RegisterCallback<MouseLeaveEvent>(evt => StopShake());
        tutorialButton.RegisterCallback<ClickEvent>(evt => OnButtonClick(tutorialButton));

        stageButton.RegisterCallback<MouseEnterEvent>(evt => StartCoroutine(ShakeButton(stageButton)));
        stageButton.RegisterCallback<MouseLeaveEvent>(evt => StopShake());
        stageButton.RegisterCallback<ClickEvent>(evt => OnButtonClick(stageButton));

        rankingButton.RegisterCallback<MouseEnterEvent>(evt => StartCoroutine(ShakeButton(rankingButton)));
        rankingButton.RegisterCallback<MouseLeaveEvent>(evt => StopShake());
        rankingButton.RegisterCallback<ClickEvent>(evt => OnButtonClick(rankingButton));
    }

    private IEnumerator ShakeButton(Button button)
    {
        isShaking = true;
        Vector3 originalPosition = button.transform.position;
        float shakeAmount = 5f;
        float shakeDuration = 0.1f;

        while (isShaking)
        {
            float offsetX = Random.Range(-shakeAmount, shakeAmount);
            button.transform.position = originalPosition + new Vector3(offsetX, 0, 0);
            yield return new WaitForSeconds(shakeDuration);
        }

        button.transform.position = originalPosition;
    }

    private void StopShake()
    {
        isShaking = false;
    }

    private void OnButtonClick(Button button)
    {
        StartCoroutine(ButtonClickAnimation(button));
    }

    private IEnumerator ButtonClickAnimation(Button button)
    {
        button.transform.scale = new Vector3(1.2f, 1.2f, 1.2f);
        yield return new WaitForSeconds(0.1f);
        button.transform.scale = new Vector3(1f, 1f, 1f);
        // 여기에 화면 전환 로직을 추가하세요.
        SceneManager.LoadScene(button.name.Replace("Button", ""));
    }
}
