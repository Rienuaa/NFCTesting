using System;
using System.Collections;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;
public class NFCTest : MonoBehaviour
{
    public string tagID;
    public TMP_Text tag_output_text;
    public TMP_Text log;
    public bool tagFound = false;

    private AndroidJavaObject mActivity;
    private AndroidJavaObject mIntent;
    private string sAction;


    void Start()
    {
        tag_output_text.text = "Scan an RFID tag!";
    }

    void MakeDiscoverable()
    {
        tagFound = false;
    }

    void Update()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            if (!tagFound)
            {
                try
                {
                    // Create new NFC Android object
                    mActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity"); // Activities open apps
                    mIntent = mActivity.Call<AndroidJavaObject>("getIntent");
                    sAction = mIntent.Call<String>("getAction"); // result are returned in the Intent object
                    if (sAction == "android.nfc.action.NDEF_DISCOVERED")
                    {
                        Debug.Log("Tag of type NDEF");
                        log.text += "\nTag of type NDEF";
                    }
                    else if (sAction == "android.nfc.action.TECH_DISCOVERED")
                    {
                        Debug.Log("TECH DISCOVERED");
                        log.text += "\nTECH DISCOVERED";

                        // Get ID of tag
                        AndroidJavaObject mNdefMessage = mIntent.Call<AndroidJavaObject>("getParcelableExtra", "android.nfc.extra.TAG");
                        if (mNdefMessage != null)
                        {
                            byte[] payLoad = mNdefMessage.Call<byte[]>("getId");
                            string text = System.Convert.ToBase64String(payLoad);
                            tag_output_text.text += "This is your tag ID: " + text;
                            tagID = text;
                        }
                        else
                        {
                            tag_output_text.text = "No ID found !";
                        }

                        tagFound = true;
                        Invoke("MakeDiscoverable", 5);
                        return;
                    }
                    else if (sAction == "android.nfc.action.TAG_DISCOVERED")
                    {
                        Debug.Log("TAG DISCOVERED");
                        log.text += "\nTAG DISCOVERED!";

                        // get id of tag
                        AndroidJavaObject mNdefMessage = mIntent.Call<AndroidJavaObject>("getParcelableExtra", "android.nfc.extra.TAG");
                        if (mNdefMessage != null)
                        {
                            byte[] payLoad = mNdefMessage.Call<byte[]>("getId");
                            string text = System.Convert.ToBase64String(payLoad);
                            tag_output_text.text += "\nThis is your tag ID: " + text;
                            tagID = text;
                        }
                        else
                        {
                            tag_output_text.text = "No ID found !";
                        }

                        tagFound = true;
                        Invoke("MakeDiscoverable", 5);
                        return;
                    }
                    else
                    {
                        return;
                    }
                }
                catch (Exception ex)
                {
                    string text = ex.Message;
                    tag_output_text.text = text;
                }
            }
        }
    }
}
