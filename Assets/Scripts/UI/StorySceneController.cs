using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StorySceneController : MonoBehaviour
{
    private Button arrowButton;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        arrowButton = root.Q<Button>("NextButton");
        if (arrowButton != null)
        {
            arrowButton.clicked += LoadMainMenuScene;
        }
    }

    void OnDisable()
    {
        if (arrowButton != null)
        {
            arrowButton.clicked -= LoadMainMenuScene;
        }
    }

    void LoadMainMenuScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
