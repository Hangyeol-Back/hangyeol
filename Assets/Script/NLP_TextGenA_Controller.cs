using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;

public class NLP_TextGenA_Controller : MonoBehaviour
{
    NLP_Server_Controller ServerController;

    /*** mask, nli, qa, sentiment, similarity, summarizationA, summarizationB, textgenB, textgenC ***/
    string ModelName = "textgenA";
    public InputField inputField;

    private void Start()
    {
        ServerController = GameObject.FindObjectOfType<NLP_Server_Controller>();
    }

    public void OnClick_SendtextGenARequest()
    {
        StartCoroutine(ISendRequest());
    }

    IEnumerator ISendRequest()
    {
        string Text = inputField.text;
        print("Text=" + Text);

        string Request = MakeRequest(Text);
        print("Request=" + Request);
        UnityWebRequest www = UnityWebRequest.Get(Request);

        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {

            ChatSystem.isWait = false;
            print("Error:" + www.error);
        }
        else
        {
            ChatSystem.isWait = false;
            ChatSystem.receivedText = www.downloadHandler.text;
            print("Success:" + www.downloadHandler.text);

            string GeneratedText = www.downloadHandler.text;
            //var jo = JObject.Parse(www.downloadHandler.text);
            //var jArray = JArray.Parse(jsonString);
            print("GeneratedText:" + GeneratedText);
        }
    }

    string MakeRequest(string _Text)
    {
        string ServerIP = GameObject.FindObjectOfType<NLP_Server_Controller>().GetServerIP(ModelName);
        string Request = "";
        Request += ServerIP;
        Request += "&text=" + _Text;

        return Request;
    }
}
