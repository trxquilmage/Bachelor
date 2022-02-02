using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pizza : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(TurnPizza());
    }
    IEnumerator TurnPizza()
    {
        Vector3 rotationOffset = new Vector3(0,1,0);
        WaitForEndOfFrame delay = new WaitForEndOfFrame();
        while(true)
        {
            transform.eulerAngles += rotationOffset;
            if (transform.eulerAngles.y > 360)
                transform.eulerAngles += Vector3.zero;
            yield return delay;
        }
    }
}
