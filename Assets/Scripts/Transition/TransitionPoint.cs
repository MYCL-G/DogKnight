using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TransitionType
{
    sameScene,
    differentScene
}
public class TransitionPoint : MonoBehaviour
{
    [Header("Transition Info")]
    public string sceneName;
    public TransitionType transitionType;
    public DestinationTag destinationTag;
    bool canTrans;
    void Start()
    {

    }
    void Update()
    {
        if (canTrans && Input.GetKeyDown(KeyCode.E))
        {
            cScene.Inst.TransitionToDestination(this);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
            canTrans = true;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            canTrans = false;
    }
}
