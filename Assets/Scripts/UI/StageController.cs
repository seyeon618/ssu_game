using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StageController : MonoBehaviour
{
    private VisualElement root;
    private VisualElement stage1;
    private VisualElement stage2;
    private VisualElement stage3;
    private Button prevButton;

    private void OnEnable()
    {
        var uiDocument = GetComponent<UIDocument>();
        if (uiDocument != null)
        {
            root = uiDocument.rootVisualElement;
        }

        if (root == null)
        {
            Debug.LogError("Root visual element is null. Please ensure the UXML is correctly assigned.");
            return;
        }

        stage1 = root.Q<VisualElement>("Stage1");
        stage2 = root.Q<VisualElement>("Stage2");
        stage3 = root.Q<VisualElement>("Stage3");
        prevButton = root.Q<Button>("PrevButton");

        if (stage1 == null || stage2 == null || stage3 == null)
        {
            Debug.LogError("One or more stage elements are null. Please check the UXML file for correct naming.");
            return;
        }


        if (prevButton != null)
        {
            prevButton.clicked += OnPrevButtonClick;
        }
        else
        {
            Debug.LogError("PrevButton not found in UXML");
        }
        UpdateStageStatus();
    }

    private void UpdateStageStatus()
    {
        // 예시 데이터로 Stage1만 오늘 클리어한 것으로 설정
        string todayDate = System.DateTime.Now.ToString("yyyy. MM. dd");
        PlayerPrefs.SetInt("Stage1Completed", 1);
        PlayerPrefs.SetString("Stage1CompletionDate", todayDate);

        UpdateStage(stage1, "Stage1Completed", "Stage1CompletionDate");
        UpdateStage(stage2, "Stage2Completed", "Stage2CompletionDate");
        UpdateStage(stage3, "Stage3Completed", "Stage3CompletionDate");
    }

    private void UpdateStage(VisualElement stage, string completionKey, string completionDateKey)
    {
        bool isCompleted = PlayerPrefs.GetInt(completionKey, 0) == 1;
        if (isCompleted)
        {
            stage.RemoveFromClassList("bg_inclear");
            stage.AddToClassList("bg_clear");

            var stamp = stage.Q<VisualElement>("Stamp");
            if (stamp != null)
            {
                stamp.style.display = DisplayStyle.Flex;
            }

            var completionDate = PlayerPrefs.GetString(completionDateKey, "");
            var completionDateLabel = stage.Q<Label>("CompletionDate");
            if (completionDateLabel != null)
            {
                completionDateLabel.text = completionDate;
                completionDateLabel.style.display = DisplayStyle.Flex;
            }
        }
    }
    private void OnPrevButtonClick()
    {
        // 이전 씬으로 이동
        SceneManager.LoadScene("MainMenu");
    }
}
