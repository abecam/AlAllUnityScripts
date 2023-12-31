using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShareToken : MonoBehaviour
{
    NativeShare ourNativeShare;

    public ManageBox ourBox;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void shareGoodTokens()
    {
        CreateCodeToSend ourCreateCodeToSend = new CreateCodeToSend(ourBox);

        string content = ourCreateCodeToSend.getFullTokens();

        Debug.Log("Tokens:\n" + content);
        shareCodes(content);
    }

    public void shareBadTokens()
    {
        CreateCodeToSend ourCreateCodeToSend = new CreateCodeToSend(ourBox);

        string content = ourCreateCodeToSend.getBadTokens(false);
        Debug.Log("Tokens:\n" + content);
        shareCodes(content);
    }

    public void shareBothTokens()
    {
        CreateCodeToSend ourCreateCodeToSend = new CreateCodeToSend(ourBox);

        string content = ourCreateCodeToSend.getBothTokens();
        Debug.Log("Tokens:\n" + content);
        shareCodes(content);
    }

    private void shareCodes(string content)
    {
        ourNativeShare = new NativeShare();

        ourNativeShare.SetTitle("Day of the Tokens: Tokens to share");
        ourNativeShare.SetSubject("Day of the Tokens: Tokens to share");
        ourNativeShare.SetText(content);
        ourNativeShare.Share();
    }
}
