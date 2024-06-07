using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class NameInputController : MonoBehaviour
{
    public UIDocument uiDocument;

    void Start()
    {
        var root = uiDocument.rootVisualElement;
        var nameInput = root.Q<TextField>("NameInput");
        var nextButton = root.Q<Button>("NextButton");

        nextButton.clicked += OnNextButtonClick;
        nameInput.RegisterCallback<KeyDownEvent>(OnEnterPress);
    }

    private void OnNextButtonClick()
    {
        LoadMainMenuScene();
    }

    private void OnEnterPress(KeyDownEvent evt)
    {
        if (evt.keyCode == KeyCode.Return)
        {
            LoadMainMenuScene();
        }
    }

    private void LoadMainMenuScene()
    {
        var nameInput = uiDocument.rootVisualElement.Q<TextField>("NameInput");
        RankingManager.Instance.SetUserName(nameInput.value);

        SceneManager.LoadScene("MainMenu");
    }
}
