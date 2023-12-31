using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectShop : MonoBehaviour
{
    public GameObject theShop;

    // Start is called before the first frame update
    void Start()
    {

    }

    public bool inHeroShop = false;
    bool changing = false;

    Quaternion normalShopRotation = Quaternion.Euler(0, 0, 0);
    Quaternion heroShopRotation = Quaternion.Euler(0, 90, 0);

    Quaternion fromRotation;
    Quaternion toRotation;
    float speed = 0.4f;

    float initTime = 0;

    void Update()
    {
        if (changing)
        {
            initTime += Time.deltaTime;

            theShop.transform.rotation = Quaternion.Lerp(fromRotation, toRotation, initTime * speed);
        }
        if (Mathf.Abs(theShop.transform.rotation.eulerAngles.y - toRotation.eulerAngles.y) < 0.1f)
        {
            changing = false;

            initTime = 0;
        }
    }

    void OnMouseDown()
    {
        changing = true;

        if (inHeroShop)
        {
            Debug.Log("In Hero shop!");
            fromRotation = heroShopRotation;
            toRotation = normalShopRotation;
        }
        else
        {
            Debug.Log("In regular shop!");
            fromRotation = normalShopRotation;
            toRotation = heroShopRotation;
        }
    }
}
