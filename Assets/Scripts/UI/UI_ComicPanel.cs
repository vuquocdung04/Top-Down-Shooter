using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ComicPanel : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Image[] comicPanel;
    [SerializeField] private int imageIndex;
    [SerializeField] private GameObject buttonToEnable;

    private Image myImage;
    [SerializeField] private bool comicShowOver;

    private void Start()
    {
        myImage = GetComponent<Image>();
        ShowNextImage();
    }

    private void ShowNextImage()
    {
        if(comicShowOver) return;
        StartCoroutine(ChangeImageAlpha(1, 1.5f, ShowNextImage));
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        ShowNextImageOnClick();
    }

    private void ShowNextImageOnClick()
    {
        comicPanel[imageIndex].color = Color.white;
        imageIndex++;
        if(imageIndex >= comicPanel.Length)
            FinishComicShow();
        
        if(comicShowOver) return;
        ShowNextImage();
    }

    private IEnumerator ChangeImageAlpha(float targetAlpha, float duration, System.Action callback)
    {
        float time = 0;
        Color currentColor = comicPanel[imageIndex].color;
        float startAlpha = currentColor.a;

        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);

            comicPanel[imageIndex].color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
            yield return null;
        }

        comicPanel[imageIndex].color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);

        imageIndex++;
        if (imageIndex >= comicPanel.Length)
        {
            FinishComicShow();
        }
        
        // call the completion method if it exists
        callback?.Invoke();
    }

    private void FinishComicShow()
    {
        StopAllCoroutines();
        comicShowOver = true;
        buttonToEnable.SetActive(true);
        myImage.raycastTarget = false;
    }
}