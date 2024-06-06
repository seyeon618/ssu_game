using UnityEngine;
using UnityEngine.UIElements;

public class Ranking : MonoBehaviour
{
    private VisualElement rankingScreen;
    private VisualElement tabContainer;
    private ScrollView rankingList;
    private Button tab1;
    private Button tab2;
    private Button tab3;

    private void OnEnable()
    {
        var rootVisualElement = GetComponent<UIDocument>().rootVisualElement;
        rankingScreen = rootVisualElement.Q<VisualElement>("MainContainer");
        tabContainer = rootVisualElement.Q<VisualElement>("TabContainer");
        rankingList = rootVisualElement.Q<ScrollView>("RankingList");
        tab1 = rootVisualElement.Q<Button>("Tab1");
        tab2 = rootVisualElement.Q<Button>("Tab2");
        tab3 = rootVisualElement.Q<Button>("Tab3");

        tab1.clicked += () => SwitchTab(1);
        tab2.clicked += () => SwitchTab(2);
        tab3.clicked += () => SwitchTab(3);

        // Default to Stage 1
        SwitchTab(1);
    }

    private void SwitchTab(int stage)
    {
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

        // Add new rankings based on the stage
        for (int i = 1; i <= 10; i++)
        {
            var rankingItem = new VisualElement();
            rankingItem.AddToClassList("RankingItem");

            var rankLabel = new Label(i.ToString());
            rankLabel.AddToClassList("RankNumber");

            var playerName = new Label("NAME" + i);
            playerName.AddToClassList("PlayerName");

            var timeContainer = new VisualElement();
            timeContainer.AddToClassList("TimeContainer");

            var timeLabel = new Label(Random.Range(10, 30).ToString() + "sec");
            timeLabel.AddToClassList("TimeLabel");

            timeContainer.Add(timeLabel);
            rankingItem.Add(rankLabel);
            rankingItem.Add(playerName);
            rankingItem.Add(timeContainer);

            rankingList.Add(rankingItem);
        }
    }
}