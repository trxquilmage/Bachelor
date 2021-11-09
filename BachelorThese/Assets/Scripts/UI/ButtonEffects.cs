using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ButtonEffects : MonoBehaviour, IPointerClickHandler, IPointerDownHandler
{
    public UnityEvent onStart;
    public UnityEvent onClick;
    public UnityEvent onEnable;

    // Start is called before the first frame update
    void Start()
    {
        onStart.Invoke();
    }
 public void OnPointerClick(PointerEventData eventData)
    {
        //this need to be here, bc i dont know why
    }

    void OnEnable()
    {
        onEnable.Invoke();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        onClick.Invoke();
    }
}
