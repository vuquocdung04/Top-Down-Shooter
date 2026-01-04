using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ToolTipOnHover : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler
{
    [SerializeField] private GameObject toolTip;
    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip?.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        toolTip?.SetActive(true);
    }
}