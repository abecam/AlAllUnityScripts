using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * Manage all lines in the shop: select the ones visible, and push the new ones in place.
 */
public class ManageShopLines : MonoBehaviour
{
    /**
     * The items are ordered by price, so give a price even to the infinity items
     */
    const int nbOfItemShown = 8;
    const float bottomPos = -4;

    const float abovePos = 7;

    SortedDictionary<double, ManageOneShopLine> shopLines = new SortedDictionary<double, ManageOneShopLine>();

    private int nbBought = 0;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform childShop in transform)
        {
            ManageOneShopLine oneLineScript = childShop.gameObject.GetComponent<ManageOneShopLine>();

            if (oneLineScript.checkIfBought(nbBought))
            {
                nbBought++;
            }

            if (oneLineScript == null)
            {
                Debug.LogError("This line is not correctly done: " + childShop.name);
            }

            if (!oneLineScript.WasBought)
            {
                // Add it to our pool
                shopLines.Add(oneLineScript.price, oneLineScript);
            }
        }
        float yLines = bottomPos;

        // And take the nbOfItemShown cheapest to show, the others much above
        for (int iVisibleItem = 0; iVisibleItem < nbOfItemShown && iVisibleItem < shopLines.Count; iVisibleItem++)
        {
            KeyValuePair<double, ManageOneShopLine> pair = shopLines.ElementAt(iVisibleItem);

            pair.Value.transform.localPosition = new Vector3(pair.Value.transform.localPosition.x, yLines, -pair.Value.transform.localPosition.z);

            yLines += 1;
        }

        yLines = abovePos;

        // Place everything above, so we can eventually show them
        for (int iVisibleItem = nbOfItemShown; iVisibleItem < shopLines.Count; iVisibleItem++)
        {
            KeyValuePair<double, ManageOneShopLine> pair = shopLines.ElementAt(iVisibleItem);

            pair.Value.transform.localPosition = new Vector3(pair.Value.transform.localPosition.x, yLines, pair.Value.transform.localPosition.z);

            yLines += 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /**
     * When a line is removed, it call this method and gives its position, so the rest can be shifted and a new item added.
     */
    public void oneLineRemoved()
    {
        shopLines = new SortedDictionary<double, ManageOneShopLine>();

        foreach (Transform childShop in transform)
        {
            ManageOneShopLine oneLineScript = childShop.gameObject.GetComponent<ManageOneShopLine>();

            if (oneLineScript == null)
            {
                Debug.LogError("This line is not correctly done: " + childShop.name);
            }

            if (!oneLineScript.WasBought)
            {
                // Add it to our pool
                shopLines.Add(oneLineScript.price, oneLineScript);
            }
        }
        float yLines = bottomPos;

        // And take the nbOfItemShown cheapest to show, the others much above
        for (int iVisibleItem = 0; iVisibleItem < nbOfItemShown && iVisibleItem < shopLines.Count; iVisibleItem++)
        {
            KeyValuePair<double, ManageOneShopLine> pair = shopLines.ElementAt(iVisibleItem);

            pair.Value.moveTo(yLines);

            yLines += 1;
        }
    }
}
