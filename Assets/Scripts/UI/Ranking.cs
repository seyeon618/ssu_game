using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class Ranking : MonoBehaviour
{
    private VisualElement rankingScreen;
    private VisualElement tabContainer;
    private ScrollView rankingList;
    private Button tab1;
    private Button tab2;
    private Button tab3;
    private Button prevButton;

    private void OnEnable()
    {
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
        rankingScreen = rootVisualElement.Q<VisualElement>("MainContainer");
        tabContainer = rootVisualElement.Q<VisualElement>("TabContainer");
        rankingList = rootVisualElement.Q<ScrollView>("RankingList");
        tab1 = rootVisualElement.Q<Button>("Tab1");
        tab2 = rootVisualElement.Q<Button>("Tab2");
        tab3 = rootVisualElement.Q<Button>("Tab3");
        prevButton = rootVisualElement.Q<Button>("PrevButton");

        tab1.clicked += () => SwitchTab(1);
        tab2.clicked += () => SwitchTab(2);
        tab3.clicked += () => SwitchTab(3);

        // Default to Stage 1
        SwitchTab(1);

        if (prevButton != null)
        {
            prevButton.clicked += OnPrevButtonClick;
            prevButton.RegisterCallback<MouseEnterEvent>(OnButtonHover);
            prevButton.RegisterCallback<MouseLeaveEvent>(OnButtonLeave);
        }
        else
        {
            Debug.LogError("PrevButton not found in UXML");
        }
    }

    private void SwitchTab(int stage)
    {
        UISoundManager.Instance.PlayButtonClick();
        // Change background image based on the stage
        switch (stage)
        {
            case 1:
                tabContainer.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("UI/Ranking/stage1"));
                break;
            case 2:
                tabContainer.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("UI/Ranking/stage2"));
                break;
            case 3:
                tabContainer.style.backgroundImage = new StyleBackground(Resources.Load<Texture2D>("UI/Ranking/stage3"));
                break;
        }

        // Clear previous rankings
        rankingList.Clear();

        StageType eStage = (StageType)stage;

        int rankIndex = 1;
        // Add new rankings based on the stage
        foreach(var rank in RankingManager.Instance.Rankings[eStage])
        {
            var rankingItem = new VisualElement();
            rankingItem.AddToClassList("RankingItem");

            var rankLabel = new Label(rankIndex++.ToString());
            rankLabel.AddToClassList("RankNumber");

            var playerName = new Label(rank.Value);
            playerName.AddToClassList("PlayerName");
            playerName.style.unityFontDefinition = new StyleFontDefinition();
            playerName.style.unityFontStyleAndWeight = new StyleEnum<FontStyle>();
            var timeContainer = new VisualElement();
            timeContainer.AddToClassList("TimeContainer");

            var timeLabel = new Label($"{rank.Key} sec");
            timeLabel.AddToClassList("TimeLabel");

            timeContainer.Add(timeLabel);
            rankingItem.Add(rankLabel);
            rankingItem.Add(playerName);
            rankingItem.Add(timeContainer);

            rankingList.Add(rankingItem);
        }
    }
    private void OnPrevButtonClick()
    {
        UISoundManager.Instance.PlayButtonClick();
        // 이전 씬으로 이동
        SceneManager.LoadScene("MainMenu");
    }
    private void OnButtonHover(MouseEnterEvent evt)
    {
        UISoundManager.Instance.PlayButtonHover();
        prevButton.style.scale = new Scale(new Vector2(1.2f, 1.2f));
    }

    private void OnButtonLeave(MouseLeaveEvent evt)
    {
        prevButton.style.scale = new Scale(new Vector2(1, 1));
    }
}
