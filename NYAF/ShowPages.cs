using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShowPages : MonoBehaviour
{
    private int currentPageShown = 0;
    private Vector2 touchPos;
    private Vector3 touchPosWP;

    // Start is called before the first frame update
    void Start()
    {
        showCurrentPage();
    }

    private void Update()
    {
        touchPos = Mouse.current.position.ReadValue();
        touchPosWP = Camera.main.ScreenToWorldPoint(touchPos);

        if ((Mouse.current.leftButton.wasReleasedThisFrame || Keyboard.current.spaceKey.wasReleasedThisFrame) && touchPosWP.x < 2f)
        {
            goToNextPage();
        }
        if ((Mouse.current.rightButton.wasReleasedThisFrame || Keyboard.current.backspaceKey.wasReleasedThisFrame) && touchPosWP.x < 2f)
        {
            goToPreviousPage();
        }
    }

    private void showCurrentPage()
    {
        int nbOfPage = 0;

        foreach (Transform pages in transform)
        {
            if (nbOfPage == currentPageShown)
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

            showCurrentPage();
        }
        else
        {
            currentPageShown = 0;

            showCurrentPage();
        }
    }

    public void goToPreviousPage()
    {
        if (currentPageShown > 0)
        {
            currentPageShown--;

            showCurrentPage();
        }
        else
        {
            currentPageShown = transform.childCount - 1;

            showCurrentPage();
        }
    }

    public void goToFirstPage()
    {
        currentPageShown = 0;

        showCurrentPage();
    }
}
