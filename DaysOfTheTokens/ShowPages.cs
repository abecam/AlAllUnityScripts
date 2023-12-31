using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowPages : MonoBehaviour
{
    private int currentPageShown = 0;

    // Start is called before the first frame update
    void Start()
    {
        showCurrentPageAndPrevious();
    }

    private void showCurrentPageAndPrevious()
    {
        int nbOfPage = 0;

        foreach (Transform pages in transform)
        {
            if (nbOfPage == currentPageShown || nbOfPage == currentPageShown - 1)
            {
                pages.gameObject.SetActive(true);
            }
            else
            {
                pages.gameObject.SetActive(false);
            }

            nbOfPage++;
        }
    }

    public void goToNextPage()
    {
        if (currentPageShown < transform.childCount - 1)
        {
            currentPageShown++;

            showCurrentPageAndPrevious();
        }
        else
        {
            currentPageShown = 0;

            showCurrentPageAndPrevious();
        }
    }

    public void goToPreviousPage()
    {
        if (currentPageShown > 1)
        {
            currentPageShown--;

            showCurrentPageAndPrevious();
        }
        else
        {
            currentPageShown = transform.childCount - 1;

            showCurrentPageAndPrevious();
        }
    }
}
