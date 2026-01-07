using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ToolTipOnHover : MonoBehaviour, IPointerExitHandler, IPointerEnterHandler, IPointerDownHandler
{
    [SerializeField] private GameObject toolTip;
    [Header("Audio")] [SerializeField] private AudioSource pointerEnterSFX;
    [SerializeField] private AudioSource pointerDownSFX;
    public void OnPointerExit(PointerEventData eventData)
    {
        toolTip?.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(pointerEnterSFX != null)
            pointerEnterSFX.Play();
        toolTip?.SetActive(true);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(pointerDownSFX != null)
            pointerDownSFX.Play();
    }
}