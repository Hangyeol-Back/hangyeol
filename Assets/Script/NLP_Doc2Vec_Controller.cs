using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using Newtonsoft.Json;

public class NLP_Doc2Vec_Controller : MonoBehaviour
{
    NLP_Server_Controller ServerController;

    /*** mask, nli, qa, sentiment, similarity, summarizationA, summarizationB, textgenB, textgenC ***/
    string ModelName = "doc2vec";
    public InputField inputField;

    private void Start()
    {
        ServerController = GameObject.FindObjectOfType<NLP_Server_Controller>();
    }

    public void OnClick_SendDoc2VecRequest()
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

            string result = "@";
            string text = www.downloadHandler.text;
            while(text != "" && text != null)
            {
                string temp = GetMiddleString(text, "\'", "\'");
                if(temp != null)
                {
                    result += temp + ",";
                    text = text.Substring(text.IndexOf("\'") + 1);
                    text = text.Substring(text.IndexOf("\'") + 1);
                }
                else
                {
                    break;
                }
            }
            
            ChatSystem.receivedText = result;
            print(result);

            print("Success:" + www.downloadHandler.text);

            string GeneratedText = www.downloadHandler.text;
            //var jo = JObject.Parse(www.downloadHandler.text);
            //var jArray = JArray.Parse(jsonString);
            print("Recommendations:" + GeneratedText);
        }
    }

    public static string GetMiddleString(string str, string begin, string end)
    {
        if (string.IsNullOrEmpty(str))
        {
            return null;
        }

        string result = null;
        if (str.IndexOf(begin) > -1)
        {
            str = str.Substring(str.IndexOf(begin) + begin.Length);
            if (str.IndexOf(end) > -1) result = str.Substring(0, str.IndexOf(end));
            else result = str;
        }
        return result;
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
