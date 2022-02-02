using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class NPC : MonoBehaviour
{
    public string characterName = "";
    [HideInInspector] public string talkToNode = "";
    [HideInInspector] public string askNode = "";

    public float interactionRadius = 3;
    public GameObject pivotForE;

    protected Vector3 normalizedDefaultForward;
    protected GameObject targetPlayer;
    protected ReferenceManager refM;
    [SerializeField] protected GameObject npcMesh;

    protected virtual void Start()
    {
        SetValues();
    }
    protected void SetValues()
    {
        refM = ReferenceManager.instance;
        normalizedDefaultForward = npcMesh.transform.forward;
        targetPlayer = refM.player;
        talkToNode = characterName + ".Start";
        askNode = characterName + ".Ask";

        pivotForE = WordUtilities.GetChildWithTag(this.gameObject, "PivotForE");
    }



    public bool IsInRangeToPlayer()
    {
        return GetDistanceToPlayer() <= interactionRadius;
    }
    public float GetDistanceToPlayer()
    {
        if (targetPlayer != null)
            return (transform.position - targetPlayer.transform.position).magnitude;
        return interactionRadius + 1;
    }

    #region Turning
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
        turnTowards.Scale(new Vector3(1, 0, 1));

        while (Vector3.Dot(turnTowards, npcMesh.transform.forward) < 0.99f)
        {
            npcMesh.transform.forward = Vector3.Lerp(npcMesh.transform.forward, turnTowards, 0.4f);
            yield return delay;
        }
    }
    #endregion
}

