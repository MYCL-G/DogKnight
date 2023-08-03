using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mGame : MonoBehaviour
{
    static mGame inst;
    public static mGame Inst => inst;
    CinemachineFreeLook followCamera;
    public UniversalStats playerStats;
    List<iEndGameObserver> endGameObserverList = new List<iEndGameObserver>();
    private void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(this);
        DontDestroyOnLoad(gameObject);
    }
    public void RegisterPlayer(UniversalStats playerStats)
    {
        this.playerStats = playerStats;
        followCamera = FindAnyObjectByType<CinemachineFreeLook>();
        if (followCamera != null)
        {
            followCamera.Follow = playerStats.transform.GetChild(2);
            followCamera.LookAt = playerStats.transform.GetChild(2);
        }
    }
    public void AddObserver(iEndGameObserver observer)
    {
        endGameObserverList.Add(observer);
    }
    public void RemoveObserver(iEndGameObserver observer)
    {
        endGameObserverList.Remove(observer);
    }
    public void NotifyObserver()
    {
        foreach (var observer in endGameObserverList)
        {
            observer.EndNotify();
        }
    }
    public Transform GetEntrance()
    {
        foreach (TransitionDestination item in FindObjectsOfType<TransitionDestination>())
        {
            if (item.destinationTag == DestinationTag.C)
                return item.transform;
        }
        return null;
    }
}
