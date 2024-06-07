using UnityEngine;
using UnityEngine.UIElements;

public class MessageController : MonoBehaviour
{
    private VisualElement root;

    void OnEnable()
    {
        // Load the UXML and USS
        var visualTree = Resources.Load<VisualTreeAsset>("UI/Tutorial/MessageBox");
        var styleSheet = Resources.Load<StyleSheet>("UI/Tutorial/MessageBox");

        // Instantiate and apply the stylesheet
        root = visualTree.CloneTree();
        root.styleSheets.Add(styleSheet);

        // Add the UI to the document
        var uiDocument = GetComponent<UIDocument>();
        uiDocument.rootVisualElement.Add(root);
    }

    public void ShowMessage(int blockCount)
    {
        // 모든 tutoBox 숨기기
        var boxes = root.Query<VisualElement>(className: "tutoBox").ToList();
        foreach (var box in boxes)
        {
            box.AddToClassList("hidden");
        }

        // 해당 블록 수의 tutoBox 표시
        var targetBox = root.Q<VisualElement>(blockCount.ToString());
        if (targetBox != null)
        {
            targetBox.RemoveFromClassList("hidden");
        }
    }
}
