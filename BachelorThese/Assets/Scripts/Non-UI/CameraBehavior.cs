using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    TransformValues defaultPosition;
    [SerializeField] TransformValues pointToNPC;

    [SerializeField] GameObject player;
    void Awake()
    {
        defaultPosition = new TransformValues(transform);
    }

    public void BackToStandart()
    {
        StartCoroutine(TurnCamera(new TransformValues(transform), defaultPosition));
    }

    public void TurnToNPC(Transform npc)
    {
        TransformValues temp = new TransformValues(pointToNPC);
        temp.AddToPosition(Vector3.Scale(npc.position - player.transform.position, new Vector3(1, 0, 1)));
        StartCoroutine(TurnCamera(new TransformValues(transform), temp));
    }

    public IEnumerator TurnCamera(TransformValues current, TransformValues target)
    {
        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        while (!current.IsAppoximatelyEqualTo(target))
        {
            current = TransformValues.Lerp(current, target, 3f * Time.deltaTime);
            current.FillTransform(transform);
            yield return delay;
        }
    }
}

[System.Serializable]
public struct TransformValues
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public TransformValues(Vector3 pos, Quaternion rot, Vector3 sca)
    {
        position = pos;
        rotation = rot;
        scale = sca;
    }

    public TransformValues(Transform transform)
    {
        position = transform.localPosition;
        rotation = transform.rotation;
        scale = transform.localScale;
    }

    public TransformValues(TransformValues transform)
    {
        position = transform.position;
        rotation = transform.rotation;
        scale = transform.scale;
    }

    public void UpdateValues(Transform transform)
    {
        position = transform.localPosition;
        rotation = transform.rotation;
        scale = transform.localScale;
    }

    public void AddToPosition(Vector3 pos)
    {
        position = pos + position;
    }

    public static TransformValues Lerp(Transform currentTransform, TransformValues toValue, float t)
    {
        t = (t > 1) ? 1 : t;
        TransformValues transform = new TransformValues();
        transform.position = Vector3.Lerp(currentTransform.localPosition, toValue.position, t);
        transform.rotation = Quaternion.Lerp(currentTransform.rotation, toValue.rotation, t);
        transform.scale = Vector3.Lerp(currentTransform.localScale, toValue.scale, t);
        return transform;
    }

    public static TransformValues Lerp(TransformValues currentTransform, TransformValues toValue, float t)
    {
        TransformValues transform = new TransformValues();
        transform.position = Vector3.Lerp(currentTransform.position, toValue.position, t);
        transform.rotation = Quaternion.Lerp(currentTransform.rotation, toValue.rotation, t);
        transform.scale = Vector3.Lerp(currentTransform.scale, toValue.scale, t);
        return transform;
    }

    public void SetTransform(Transform transform)
    {
        transform.position = position;
        transform.rotation = rotation;
        transform.localScale = scale;
    }

    public void FillTransform(Transform transformToFill)
    {
        transformToFill.localPosition = position;
        transformToFill.rotation = rotation;
        transformToFill.localScale = scale;
    }

    public bool IsAppoximatelyEqualTo(TransformValues target)
    {
        if (PositionsApproximatelyEqual(target.position, position, 0.1f) &&
            Quaternion.Dot(target.rotation, rotation) > 0.99f &&
            Vector3.Dot(target.scale.normalized, scale.normalized) > 0.99f)
            return true;
        return false;
    }

    public static bool PositionsApproximatelyEqual(Vector3 position, Vector3 compareTo, float allowedBuffer)
    {
        if (position.x + allowedBuffer > compareTo.x && position.x - allowedBuffer < compareTo.x &&
            position.y + allowedBuffer > compareTo.y && position.y - allowedBuffer < compareTo.y &&
            position.z + allowedBuffer > compareTo.z && position.z - allowedBuffer < compareTo.z
            )
            return true;
        return false;
    }
}
