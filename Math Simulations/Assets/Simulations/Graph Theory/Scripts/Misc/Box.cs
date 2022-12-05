using UnityEngine;
using UnityEngine.EventSystems;

public class Box : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool inArea = false;

    //Properties
    public bool InArea
    {
        get { return inArea; }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        inArea = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inArea = false;
    }
}
