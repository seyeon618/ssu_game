using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using UnityEngine.UIElements.Experimental;

public class HomeController : MonoBehaviour
{
    public UIDocument uiDocument;
    private VisualElement titleLabel;
    private VisualElement anyKeyLabel;
    private VisualElement pinkShape;
    private VisualElement yellowShape;
    private VisualElement greenShape;
    private VisualElement orangeShape;
    private VisualElement purpleShape;

    private void OnEnable()
    {
        var root = uiDocument.rootVisualElement;

        titleLabel = root.Q<VisualElement>("Title");
        anyKeyLabel = root.Q<VisualElement>("Desc");
        pinkShape = root.Q<VisualElement>("PinkShape");
        yellowShape = root.Q<VisualElement>("YellowShape");
        greenShape = root.Q<VisualElement>("GreenShape");
        orangeShape = root.Q<VisualElement>("OrangeShape");
        purpleShape = root.Q<VisualElement>("PurpleShape");

        StartBlinking(anyKeyLabel);
        StartCoroutine(StartShakingWithDelay(pinkShape, 5f, 0.1f, 0.0f));
        StartCoroutine(StartShakingWithDelay(yellowShape, 10f, 0.1f, 0.05f));
        StartCoroutine(StartShakingWithDelay(greenShape, 12f, 0.1f, 0.1f));
        StartCoroutine(StartShakingWithDelay(orangeShape, 8f, 0.1f, 0.15f));
        StartCoroutine(StartShakingWithDelay(purpleShape, 15f, 0.1f, 0.2f));
    }

    private void StartBlinking(VisualElement element)
    {
        var anim = element.experimental.animation.Start(
            new StyleValues { opacity = 1 },
            new StyleValues { opacity = 0 },
            500 // duration in milliseconds
        );
        anim.easingCurve = Easing.InOutQuad;
        anim.OnCompleted(() =>
        {
            var animBack = element.experimental.animation.Start(
                new StyleValues { opacity = 0 },
                new StyleValues { opacity = 1 },
                500 // duration in milliseconds
            );
            animBack.easingCurve = Easing.InOutQuad;
            animBack.OnCompleted(() => StartBlinking(element));
        });
    }

    private IEnumerator StartShakingWithDelay(VisualElement element, float amplitude, float duration, float delay)
    {
        yield return new WaitForSeconds(delay);
        StartShaking(element, amplitude, duration);
    }

    private void StartShaking(VisualElement element, float amplitude, float duration)
    {
        float initialPosition = element.resolvedStyle.position == Position.Absolute ? element.resolvedStyle.left : 0;
        int durationMs = Mathf.RoundToInt(duration * 1000); // convert seconds to milliseconds

        var anim = element.experimental.animation.Start(
            new StyleValues { left = initialPosition },
            new StyleValues { left = initialPosition - amplitude },
            durationMs
        );
        anim.easingCurve = Easing.InOutQuad;
        anim.OnCompleted(() =>
        {
            var animBack = element.experimental.animation.Start(
                new StyleValues { left = initialPosition - amplitude },
                new StyleValues { left = initialPosition + amplitude },
                durationMs
            );
            animBack.easingCurve = Easing.InOutQuad;
            animBack.OnCompleted(() =>
            {
                var animReturn = element.experimental.animation.Start(
                    new StyleValues { left = initialPosition + amplitude },
                    new StyleValues { left = initialPosition },
                    durationMs
                );
                animReturn.easingCurve = Easing.InOutQuad;
                animReturn.OnCompleted(() => StartShaking(element, amplitude, duration));
            });
        });
    }
}
