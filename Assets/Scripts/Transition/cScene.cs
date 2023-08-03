using System.Collections;

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class cScene : MonoBehaviour, iEndGameObserver
{
    static cScene inst;
    public static cScene Inst => inst;
    GameObject player;
    NavMeshAgent agent;
    public GameObject playerPrefab;
    public SceneFader sceneFaderPrefab;
    bool fadeFinished;
    private void Awake()
    {
        if (Inst == null) inst = this;
        else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        mGame.Inst.AddObserver(this);
        fadeFinished = true;
    }
    void Update()
    {

    }
    public void TransitionToDestination(TransitionPoint transitionPoint)
    {
        switch (transitionPoint.transitionType)
        {
            case TransitionType.sameScene:
                StartCoroutine(Transition(SceneManager.GetActiveScene().name, transitionPoint.destinationTag));
                break;
            case TransitionType.differentScene:
                StartCoroutine(Transition(transitionPoint.sceneName, transitionPoint.destinationTag));
                break;
        }
    }
    IEnumerator Transition(string sceneName, DestinationTag destinationTag)
    {
        mSave.Inst.SavePlayerData();
        if (sceneName != SceneManager.GetActiveScene().name)
        {
            yield return SceneManager.LoadSceneAsync(sceneName);
            yield return Instantiate(playerPrefab, GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            mSave.Inst.LoadPlayerData();
            yield break;
        }
        else
        {
            player = mGame.Inst.playerStats.gameObject;
            agent = player.GetComponent<NavMeshAgent>();
            player.transform.SetPositionAndRotation(GetDestination(destinationTag).transform.position, GetDestination(destinationTag).transform.rotation);
            agent.destination = GetDestination(destinationTag).transform.position;
        }
        yield return null;
    }
    TransitionDestination GetDestination(DestinationTag destinationTag)
    {
        var entrances = FindObjectsOfType<TransitionDestination>();
        foreach (var entrance in entrances)
            if (entrance.destinationTag == destinationTag)
                return entrance;
        return null;
    }
    public void TransitionToFirstLevel()
    {
        StartCoroutine(LoadLevel("Grassland"));
    }
    IEnumerator LoadLevel(string scene)
    {
        SceneFader fader = Instantiate(sceneFaderPrefab);
        if (scene != "")
        {
            yield return fader.StartCoroutine(fader.FadeIn(2));
            yield return SceneManager.LoadSceneAsync(scene);
            yield return player = Instantiate(playerPrefab, mGame.Inst.GetEntrance().position, mGame.Inst.GetEntrance().rotation);
            mSave.Inst.SavePlayerData();
            yield return fader.StartCoroutine(fader.FadeOut(2));
            yield break;
        }
    }
    public void TransitionToLoadGame()
    {
        StartCoroutine(LoadLevel(mSave.Inst.ppSceneName));
    }
    public void TransitionToMain()
    {
        StartCoroutine(LoadMain());
    }
    IEnumerator LoadMain()
    {
        SceneFader fader = Instantiate(sceneFaderPrefab);
        yield return fader.StartCoroutine(fader.FadeIn(2));
        yield return SceneManager.LoadSceneAsync("Start");
        yield return fader.StartCoroutine(fader.FadeOut(2));
        yield break; 
    }

    public void EndNotify()
    {
        if (fadeFinished)
        {
            fadeFinished = false;
            StartCoroutine(LoadMain());
        }
    }
}