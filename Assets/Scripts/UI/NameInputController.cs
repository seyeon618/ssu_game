using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class NameInputController : MonoBehaviour
{
    public UIDocument uiDocument;

    private Button nextButton;
    private TextField nameInput;
    void Start()
    {
        var root = uiDocument.rootVisualElement;
        nameInput = root.Q<TextField>("NameInput");
        nextButton = root.Q<Button>("NextButton");

        nextButton.clicked += OnNextButtonClick;
        nextButton.RegisterCallback<MouseEnterEvent>(OnButtonHover);
        nextButton.RegisterCallback<MouseLeaveEvent>(OnButtonLeave);
        nameInput.RegisterCallback<KeyDownEvent>(OnEnterPress);
    }

    private void OnNextButtonClick()
    {
        UISoundManager.Instance.PlayButtonClick();
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
    private void OnButtonHover(MouseEnterEvent evt)
    {
        UISoundManager.Instance.PlayButtonHover();
        nextButton.style.scale = new Scale(new Vector2(1.2f, 1.2f));
    }

    private void OnButtonLeave(MouseLeaveEvent evt)
    {
        nextButton.style.scale = new Scale(new Vector2(1, 1));
    }
}
