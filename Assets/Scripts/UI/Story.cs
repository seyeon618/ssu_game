using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Story : MonoBehaviour
{
    public UIDocument uiDocument;
    private VisualElement storyText;
    private Button nextButton;

    private void OnEnable()
    {
        var root = uiDocument.rootVisualElement;

        storyText = root.Q<VisualElement>("StoryText");
        nextButton = root.Q<Button>("NextButton");

        storyText.style.opacity = 0;
        nextButton.style.opacity = 0;

        StartCoroutine(FadeInElement(storyText, 2f, () =>
        {
            // 스토리 텍스트가 모두 표시된 후 버튼을 표시합니다.
            nextButton.style.opacity = 1f;
        }));
        if (nextButton != null)
        {
            // NextButton 클릭 이벤트 연결
            nextButton.clicked += OnNextButtonClick;
            nextButton.RegisterCallback<MouseEnterEvent>(OnButtonHover);
            nextButton.RegisterCallback<MouseLeaveEvent>(OnButtonLeave);
        }
        else
        {
            Debug.LogError("NextButton not found in the UXML file.");
        }

    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            storyText.style.opacity = 1f;
            nextButton.style.opacity = 1f;
            StopAllCoroutines(); // 모든 코루틴을 멈춤
        }
    }

    void OnDisable()
    {
        if (nextButton != null)
        {
            // NextButton 클릭 이벤트 해제
            nextButton.clicked -= OnNextButtonClick;
        }
    }

    private IEnumerator FadeInElement(VisualElement element, float duration, System.Action onComplete = null)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsedTime / duration);
            element.style.opacity = alpha;
            yield return null;
        }

        element.style.opacity = 1f;
        onComplete?.Invoke(); // 애니메이션이 끝난 후 콜백 호출
    }
    private void OnNextButtonClick()
    {
        // NextButton 클릭 시 실행할 로직
        UISoundManager.Instance.PlayButtonClick();
        // 예: 다음 씬으로 전환
        SceneManager.LoadScene("Name");
    }
    private void OnButtonHover(MouseEnterEvent evt)
    {
        UISoundManager.Instance.PlayButtonHover();
        nextButton.style.scale = new Scale(new Vector2(1.2f, 1.2f));
    }

    private void OnButtonLeave(MouseLeaveEvent evt)
    {
        nextButton.style.scale = new Scale(new Vector2(1,1));
    }
}
