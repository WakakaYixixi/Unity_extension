using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
public class EventTriggerListener : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler,
IPointerExitHandler, IPointerUpHandler, ISelectHandler, IUpdateSelectedHandler
{
    public delegate void VoidDelegate1(GameObject go, PointerEventData eventData);
    public delegate void VoidDelegate2(GameObject go, BaseEventData eventData);
    public VoidDelegate1 onClick;
    public VoidDelegate1 onDown;
    public VoidDelegate1 onEnter;
    public VoidDelegate1 onExit;
    public VoidDelegate1 onUp;
    public VoidDelegate2 onSelect;
    public VoidDelegate2 onUpdateSelect;

    static public EventTriggerListener Get(GameObject go)
    {
        EventTriggerListener listener = go.GetComponent<EventTriggerListener>();
        if (listener == null) listener = go.AddComponent<EventTriggerListener>();
        return listener;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (onClick != null) onClick(gameObject,eventData);
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (onDown != null) onDown(gameObject, eventData);
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (onEnter != null) onEnter(gameObject, eventData);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if (onExit != null) onExit(gameObject, eventData);
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (onUp != null) onUp(gameObject, eventData);
    }
    public void OnSelect(BaseEventData eventData)
    {
        if (onSelect != null) onSelect(gameObject,eventData);
    }
    public void OnUpdateSelected(BaseEventData eventData)
    {
        if (onUpdateSelect != null) onUpdateSelect(gameObject, eventData);
    }
}