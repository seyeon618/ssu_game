using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    void Update()
    {
        // 마우스 클릭 또는 아무 키 입력을 감지합니다.
        if (Input.anyKeyDown)
        {
            LoadStoryScene();
        }
    }

    void LoadStoryScene()
    {
        UISoundManager.Instance.PlayGameStart();
        // "Story" 씬을 로드합니다.
        SceneManager.LoadScene("Story");
    }
}
