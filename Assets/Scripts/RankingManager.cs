using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

// ����Ƽ�� JsonUtility �� ������..
[System.Serializable]
public class SaveData
{
    public List<UserData> Stage1Ranks = new List<UserData>();
    public List<UserData> Stage2Ranks = new List<UserData>();
    public List<UserData> Stage3Ranks = new List<UserData>();
}

[System.Serializable]
public class UserData
{
    public string Name;
    public int Time;
}


public class RankingManager : MonoBehaviour
{
    public static RankingManager Instance { get; private set; }

    public Dictionary<StageType, SortedDictionary<int, string>> Rankings = null;

    private string _filePath = string.Empty;

    public string NowUserName;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private Dictionary<StageType, SortedDictionary<int, string>> MakeDummyData()
    {
        var result = new Dictionary<StageType, SortedDictionary<int, string>>();
        var stage1Rank = new SortedDictionary<int, string>();
        stage1Rank.Add(47, "���ӱ�ȹ");
        stage1Rank.Add(53, "�ÿ�");
        stage1Rank.Add(62, "AAA");
        stage1Rank.Add(65, "�÷��̾�");
        stage1Rank.Add(78, "����");
        stage1Rank.Add(89, "���");
        stage1Rank.Add(105, "��������");
        stage1Rank.Add(132, "����");
        stage1Rank.Add(151, "���");
        stage1Rank.Add(181, "�ʺ�");
        result.Add(StageType.Stage1, stage1Rank);
        var stage2Rank = new SortedDictionary<int, string>();
        stage2Rank.Add(67, "���ӱ�ȹ");
        stage2Rank.Add(83, "�ÿ�");
        stage2Rank.Add(92, "AAA");
        stage2Rank.Add(105, "�÷��̾�");
        stage2Rank.Add(112, "����");
        stage2Rank.Add(115, "���");
        stage2Rank.Add(117, "��������");
        stage2Rank.Add(132, "����");
        stage2Rank.Add(141, "���");
        stage2Rank.Add(181, "�ʺ�");
        result.Add(StageType.Stage2, stage2Rank);
        var stage3Rank = new SortedDictionary<int, string>();
        stage3Rank.Add(50, "���ӱ�ȹ");
        stage3Rank.Add(59, "�ÿ�");
        stage3Rank.Add(65, "AAA");
        stage3Rank.Add(67, "�÷��̾�");
        stage3Rank.Add(73, "����");
        stage3Rank.Add(83, "���");
        stage3Rank.Add(102, "��������");
        stage3Rank.Add(132, "����");
        stage3Rank.Add(151, "���");
        stage3Rank.Add(181, "�ʺ�");
        result.Add(StageType.Stage3, stage3Rank);
        return result;
    }

    // Start is called before the first frame update
    void Start()
    {
        _filePath = Application.persistentDataPath + "/data.txt";
        if (File.Exists(_filePath))
        {
            Rankings = ReadRankFromFile(_filePath);
        }
        else
        {
            Rankings = MakeDummyData();
        }
    }

    Dictionary<StageType, SortedDictionary<int, string>> ReadRankFromFile(string filePath)
    {
        string json = File.ReadAllText(filePath);
        SaveData savedData = JsonUtility.FromJson<SaveData>(json);
        Dictionary<StageType, SortedDictionary<int, string>> result = new Dictionary<StageType, SortedDictionary<int, string>>();

        result.Add(StageType.Stage1, new SortedDictionary<int, string>(savedData.Stage1Ranks.Select(x => new KeyValuePair<int, string>(x.Time, x.Name)).ToDictionary( x => x.Key, x => x.Value)));
        result.Add(StageType.Stage2, new SortedDictionary<int, string>(savedData.Stage2Ranks.Select(x => new KeyValuePair<int, string>(x.Time, x.Name)).ToDictionary(x => x.Key, x => x.Value)));
        result.Add(StageType.Stage3, new SortedDictionary<int, string>(savedData.Stage3Ranks.Select(x => new KeyValuePair<int, string>(x.Time, x.Name)).ToDictionary(x => x.Key, x => x.Value)));

        return result;
    }

    void WriteRankToFile(Dictionary<StageType, SortedDictionary<int, string>> rankings)
    {
        SaveData saveData = new SaveData();
        saveData.Stage1Ranks = rankings[StageType.Stage1].Select(x => new UserData() { Name = x.Value, Time = x.Key }).ToList();
        saveData.Stage2Ranks = rankings[StageType.Stage2].Select(x => new UserData() { Name = x.Value, Time = x.Key }).ToList();
        saveData.Stage3Ranks = rankings[StageType.Stage3].Select(x => new UserData() { Name = x.Value, Time = x.Key }).ToList();

        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(_filePath, json);
    }

    public void UpdateRanking(StageType stage, int seconds)
    {
        Rankings[stage].Add(seconds, NowUserName);

        if (Rankings[stage].Count > 10)
        {
            var lastKey = Rankings[stage].Keys.Last();
            Rankings[stage].Remove(lastKey);
        }

        WriteRankToFile(Rankings);
    }

    public void SetUserName(string userName)
    {
        NowUserName = userName;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
