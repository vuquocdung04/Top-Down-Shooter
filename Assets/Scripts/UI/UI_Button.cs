using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{
    [Header("Mouse hover settings")] public float scaleSpeed = 1;
    public float scaleRate = 1.2f;
    private Vector3 defaultScale;
    private Vector3 targetScale;
    
    private Image buttonImage;
    private TextMeshProUGUI buttonText;
    private void Start()
    {
        defaultScale = transform.localScale;
        targetScale = transform.localScale;
        
        buttonImage = GetComponent<Image>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (Mathf.Abs(transform.lossyScale.x - targetScale.x) > 0.01f)
        {
            float scaleValue = Mathf.Lerp(transform.localScale.x, targetScale.x, Time.deltaTime * scaleSpeed);
            
            transform.localScale = Vector3.one *  scaleValue;
        }
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = defaultScale * scaleRate;
        buttonImage.color = Color.yellow;
        buttonText.color = Color.yellow;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        ReturnDefaultLook();
    }
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        ReturnDefaultLook();
    }

    private void ReturnDefaultLook()
    {
        targetScale = defaultScale;
        
        buttonImage.color = Color.white;
        buttonText.color = Color.white;
    }

}