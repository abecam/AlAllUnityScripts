using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenNyafOnPlayStore : MonoBehaviour
{
    public void openShopLink()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            Application.OpenURL("https://play.google.com/store/apps/details?id=com.tgb.nyaf");
        }
        else
        {
            Application.OpenURL("http://www.thegiantball.com/2020/04/nyaf.html");
        }
    }
}
