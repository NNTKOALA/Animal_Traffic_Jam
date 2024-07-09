using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwipeLevelController : MonoBehaviour, IEndDragHandler
{
    [SerializeField] int maxPage;
    int currentPage;
    Vector3 targetPos;
    [SerializeField] Vector3 pageStep;
    [SerializeField] RectTransform levelPagesRect;
    [SerializeField] float tweenTime;
    [SerializeField] LeanTweenType tweenType;
    float dragThreshould;

    [SerializeField] Image[] barImg;
    [SerializeField] Sprite barClosed, barOpen;

    private void Awake()
    {
        currentPage = 1;
        targetPos = levelPagesRect.localPosition;
        dragThreshould = Screen.width / 15;
        UpdateBar();
    }

    public void Next()
    {
        if (currentPage < maxPage)
        {
            currentPage++;
            targetPos += pageStep;
            MovePage();
        }
    }

    public void Previous()
    {
        if (currentPage > 1)
        {
            currentPage--;
            targetPos -= pageStep;
            MovePage();
        }
    }

    void MovePage()
    {
        levelPagesRect.LeanMoveLocal(targetPos, tweenTime).setEase(tweenType);
        UpdateBar();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Mathf.Abs(eventData.position.x - eventData.position.y) > dragThreshould)
        {
            if (eventData.position.x > eventData.pressPosition.x) Previous();
            else Next();
        }
        else
        {
            MovePage();
        }
    }

    void UpdateBar()
    {
        foreach (var item in barImg)
        {
            item.sprite = barClosed;
        }
        barImg[currentPage - 1].sprite = barOpen;
    }
}
