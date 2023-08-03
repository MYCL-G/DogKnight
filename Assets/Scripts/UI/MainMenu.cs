using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public Button btnStart;
    public Button btnContinue;
    public Button btnExit;
    public PlayableDirector director;
    private void Awake()
    {
    }
    void Start()
    {
        btnStart.onClick.AddListener(PlayTimeline);
        btnContinue.onClick.AddListener(ContinueGame);
        btnExit.onClick.AddListener(QuitGame);
        director.stopped += NewGame;
    }
    void Update()
    {
    }
    void PlayTimeline()
    {
        director.Play();
    }
    void NewGame(PlayableDirector director)
    {
        PlayerPrefs.DeleteAll();
        cScene.Inst.TransitionToFirstLevel();
    }
    void ContinueGame()
    {
        cScene.Inst.TransitionToLoadGame();
    }
    void QuitGame()
    {
        Application.Quit();
    }
}
