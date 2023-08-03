using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class mSave : MonoBehaviour
{
    static mSave inst;
    public static mSave Inst => inst;
    string sceneName = "sceneName";
    public string ppSceneName
    {
        get
        {
            return PlayerPrefs.GetString(sceneName);
        }
    }
    private void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(this);
        DontDestroyOnLoad(this);
    }
    void Start()
    {

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O)) SavePlayerData();
        if (Input.GetKeyDown(KeyCode.P)) LoadPlayerData();
        if (Input.GetKeyDown(KeyCode.I)) cScene.Inst.TransitionToMain();
    }
    public void SavePlayerData()
    {
        Save(mGame.Inst.playerStats.universalData, "小狗骑士");
    }
    public void LoadPlayerData()
    {
        Load(mGame.Inst.playerStats.universalData, "小狗骑士");
    }
    public void Save(Object data, string key)
    {
        var jsonData = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(key, jsonData);
        PlayerPrefs.SetString(sceneName, SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
    }
    public void Load(Object data, string key)
    {
        if (PlayerPrefs.HasKey(key))
        {
            JsonUtility.FromJsonOverwrite(PlayerPrefs.GetString(key), data);
        }
    }
}
