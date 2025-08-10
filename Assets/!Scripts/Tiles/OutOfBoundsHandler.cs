using UnityEngine;
using UnityEngine.EventSystems;

public class OutOfBoundsHandler : MonoBehaviour, IPointerEnterHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        EventManager.OnSelectionEnded.Invoke();
    }
}
