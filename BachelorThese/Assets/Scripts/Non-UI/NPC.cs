using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class NPC : MonoBehaviour
{
    public string characterName = "";
    public string talkToNode = "";
    public string askNode = "";

    protected Vector3 normalizedDefaultForward;

    [SerializeField] protected GameObject npcMesh;

    private void Awake()
    {
        normalizedDefaultForward = npcMesh.transform.forward;
    }
    public virtual void TurnTowardsPlayer(Vector3 directionToPlayer)
    {
        StartCoroutine(Turn(directionToPlayer));
    }
    public virtual void TurnAwayFromPlayer()
    {
        StartCoroutine(Turn(normalizedDefaultForward));
    }
    IEnumerator Turn(Vector3 turnTowards)
    {
        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        while (Vector3.Dot(turnTowards, npcMesh.transform.forward) < 0.99f)
        {
            npcMesh.transform.forward = Vector3.Lerp(npcMesh.transform.forward, turnTowards, 0.4f);
            yield return delay;
        }
    }
}

