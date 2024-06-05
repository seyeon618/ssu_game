using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Story : MonoBehaviour
{
    public UIDocument uiDocument;
    private VisualElement storyText;
    private VisualElement nextButton;

    private void OnEnable()
    {
        var root = uiDocument.rootVisualElement;

        storyText = root.Q<VisualElement>("StoryText");
        nextButton = root.Q<VisualElement>("NextButton");

        storyText.style.opacity = 0;
        nextButton.style.opacity = 0;

        StartCoroutine(FadeInElement(storyText, 2f, () =>
        {
            // 스토리 텍스트가 모두 표시된 후 버튼을 표시합니다.
            nextButton.style.opacity = 1f;
        }));

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
}
