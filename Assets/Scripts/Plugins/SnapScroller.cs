using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class SnapScroller : MonoBehaviour
{
    private int maxPage;
    private int currentPage;

    private Vector3 pageTargetPosition;
    private Vector3 pageMoveDistance;

    [SerializeField] RectTransform pagesRect;

    [SerializeField] float moveDuration;
    [SerializeField] Ease tweenType;

    [SerializeField] FMButton nextButton;
    [SerializeField] FMButton prevButton;

    public void SetScroller(int inMaxPage)
    {
        currentPage = 1;
        maxPage = inMaxPage;
        pageTargetPosition = pagesRect.localPosition;

        nextButton.AddListener(NextPage);
        prevButton.AddListener(PreviousPage);

        OnChangePage();
    }

    public void SetPages(List<GameObject> pages)
    {
        if (pages.Count <= 1)
        {
            return;
        }

        pageMoveDistance = pages[1].transform.localPosition - pages[0].transform.localPosition;
    }

    private void OnChangePage()
    {
        if (maxPage == 1)
        {
            nextButton.gameObject.SetActive(false);
            prevButton.gameObject.SetActive(false);
            return;
        }

        if (currentPage == 1)
        {
            nextButton.gameObject.SetActive(true);
            prevButton.gameObject.SetActive(false);
        }
        else if (currentPage == maxPage)
        {
            nextButton.gameObject.SetActive(false);
            prevButton.gameObject.SetActive(true);
        }
        else
        {
            nextButton.gameObject.SetActive(true);
            prevButton.gameObject.SetActive(true);
        }
    }

    private void NextPage()
    {
        if (currentPage < maxPage)
        {
            currentPage++;
            pageTargetPosition -= pageMoveDistance;
            MovePage();
        }

        OnChangePage();
    }

    private void PreviousPage()
    {
        if (currentPage > 1)
        {
            currentPage--;
            pageTargetPosition += pageMoveDistance;
            MovePage();
        }

        OnChangePage();
    }

    private void MovePage()
    {
        pagesRect.DOLocalMove(pageTargetPosition, moveDuration).SetEase(tweenType);
    }
}
