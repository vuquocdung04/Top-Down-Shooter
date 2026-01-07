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

    [Header("Audio")] [SerializeField] private AudioSource pointerEnterSFX;
    [SerializeField] private AudioSource pointerDownSFX;

    protected virtual void Start()
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

            transform.localScale = Vector3.one * scaleValue;
        }
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if(pointerEnterSFX != null)
            pointerEnterSFX.Play();
        
        targetScale = defaultScale * scaleRate;
        if (buttonImage != null)
            buttonImage.color = Color.yellow;
        if (buttonText != null)
            buttonText.color = Color.yellow;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        ReturnDefaultLook();

        if (pointerDownSFX != null)
        {
            pointerDownSFX.Play();
        }
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        ReturnDefaultLook();
    }

    private void ReturnDefaultLook()
    {
        targetScale = defaultScale;

        if (buttonImage != null)
            buttonImage.color = Color.white;

        if (buttonText != null)
            buttonText.color = Color.white;
    }

    // we are set up in editor
    public void AssignAudioSource()
    {
        pointerDownSFX = GameObject.Find("UI_PointerDown").GetComponent<AudioSource>();
        pointerEnterSFX = GameObject.Find("UI_PointerEnter").GetComponent<AudioSource>();
    }
}