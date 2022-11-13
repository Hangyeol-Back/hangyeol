using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ChatSystemCell : MonoBehaviour 
{
    public RectTransform thisTransform;
    public Text lblText;
    public RectTransform textTransform;
    public Image profile;

    public void Init(){
        thisTransform = GetComponent<RectTransform>();
        lblText = transform.Find("lblText").GetComponent<Text>();
        textTransform = lblText.GetComponent<RectTransform>();
        profile = transform.Find("profile").GetComponent<Image>();
    }

    public void SetImage(Image img){

    }

    readonly int strLimit = 15;
    readonly int fontSize = 20;
    public void SetText(string str){

        for(int i = 1; i < str.Length; i++){
            if(i % strLimit == 0){
                str = str.Insert(i, "\n");
            }
        }
        int lineCount = 0;
        for(int i = 0; i < str.Length; i++)
        {
            if (str[i] == '\n')
            {
                lineCount++;
            }
        }
        
        lblText.text = str; 
        textTransform.sizeDelta = new Vector2(textTransform.sizeDelta.x, fontSize * (lineCount + 1));
        thisTransform.sizeDelta = new Vector2(thisTransform.sizeDelta.x, textTransform.sizeDelta.y + 20);
        textTransform.localPosition = Vector2.zero;
    }
}

public class ChatSystem : MonoBehaviour
{
    [SerializeField]
    private NetSystem netSystem;

    [SerializeField] 
    private ScrollRect scrollView;
    
    [SerializeField]
    private InputField inputField;
    
    [SerializeField]
    private Button btnSend;
    
    [SerializeField]
    private GameObject originCell_right;
    
    [SerializeField]
    private GameObject originCell_left;

    [SerializeField]
    private GameObject originCell_wait;

    [SerializeField]
    private GameObject originCell_btns;

    [SerializeField]
    private List<Image> waitDecoList = new List<Image>();
 
    protected void SetChild()
    {
        btnSend.onClick.RemoveAllListeners();
        btnSend.onClick.AddListener(delegate { OnClickSend(inputField.text); });
    }

    private void Start(){
        SetChild();
    }

    public static bool isWait = false;
    public static string receivedText = "";
    float opacityValue = 360f;

    void Update()
    {
        btnSend.interactable = !isWait;
        originCell_wait.SetActive(isWait);
        if(isWait)
        {
            originCell_wait.transform.SetSiblingIndex(0);
            for(int i = 0; i < waitDecoList.Count; i++)
                waitDecoList[i].color = new Color(1f, 1f, 1f, Mathf.Sin(opacityValue + i * 120f));
            opacityValue -= 0.1f;
            if(opacityValue < 0f)
                opacityValue = 360f;
        }
        else if(receivedText != "")
        {
            recevedMessage(receivedText);
            receivedText = "";
        }
    }

    void OnClickSend(string text)
    {
        GameObject newCell = Instantiate(originCell_right, scrollView.content);
        newCell.transform.SetSiblingIndex(0);
        newCell.SetActive(true);

        var systemCell = newCell.AddComponent<ChatSystemCell>();
        systemCell.Init();
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollView.content);
        Send(text);
        systemCell.SetText(inputField.text);
        //netSystem.Send(text);
        inputField.text = "";
        isWait = true;
    }

    void Send(string msg)
    {
        char flag = ' ';
        if(msg.Length > 0)
        {
            flag = msg[0];

            inputField.text = inputField.text.Substring(1, inputField.text.Length - 1);
        }
            
        switch(flag)
        {
            case '0':
            {
                NLP_TextGenA_Controller nlp = GameObject.FindObjectOfType<NLP_TextGenA_Controller>();
                nlp.OnClick_SendtextGenARequest();
                break;
            }   
            case '1':
            {
                NLP_Doc2Vec_Controller nlp = GameObject.FindObjectOfType<NLP_Doc2Vec_Controller>();
                nlp.OnClick_SendDoc2VecRequest();
                break;
            }
            case '2':
            {
                NLP_Word2Vec_Controller nlp = GameObject.FindObjectOfType<NLP_Word2Vec_Controller>();
                nlp.OnClick_SendWord2VecRequest();
                break;
            }
            case ' ':
            {
                netSystem.Send(msg);
                break;
            }
        }
    }

    public void recevedMessage(string message)
    {
        if(message == string.Empty || message.Length <= 0)
        {
            isWait = false;
            return;
        }

        char c = message[0];
        if(c != '@')
        {
            GameObject newCell = Instantiate(originCell_left, scrollView.content);
            newCell.transform.SetSiblingIndex(0);
            newCell.SetActive(true);

            var systemCell = newCell.AddComponent<ChatSystemCell>();
            systemCell.Init();
            systemCell.SetText(message);
            LayoutRebuilder.ForceRebuildLayoutImmediate(scrollView.content);
            isWait = false;
        }
        else
            recevedSelectAbleMessage(message);
        receivedText = "";
    }

    public void recevedSelectAbleMessage(string message)
    {
        string[] split = message.Split(',');
        GameObject newCell = Instantiate(originCell_btns, scrollView.content);
        newCell.transform.SetSiblingIndex(0);
        newCell.SetActive(true);

        for(int i = 0; i < split.Length; i++)
        {
            string str = split[i];
            GameObject eachCell = newCell.transform.Find("contents").GetChild(i).gameObject;
            eachCell.SetActive(true);
            eachCell.GetComponentInChildren<Text>().text = str;
            eachCell.GetComponent<Button>().onClick.RemoveAllListeners();
            eachCell.GetComponent<Button>().onClick.AddListener(delegate { OnClickSend(str); });
        }
        isWait =false;

    }
}