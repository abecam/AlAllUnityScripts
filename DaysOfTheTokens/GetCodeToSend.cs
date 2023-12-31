using Common.Cryptography;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetCodeToSend : MonoBehaviour
{
    public ManageBox ourBox;

    public InputField ourInputField = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        CreateCodeToSend ourCreateCodeToSend = new CreateCodeToSend(ourBox);

        string compressTxt = ourCreateCodeToSend.getFullTokens();

        Debug.Log("Would send (after compression) " + compressTxt + "(size " + compressTxt.Length + ")");
        ourInputField.SetTextWithoutNotify("****" + compressTxt + "****");
    }

    public void copyToClipboard()
    {
        TextEditor _textEditor = new TextEditor
        { text = ourInputField.text };
        _textEditor.SelectAll();

        _textEditor.Copy();
    }
}
