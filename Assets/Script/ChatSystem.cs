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
    private List<Image> waitDecoList = new List<Image>();
 
    protected void SetChild()
    {
        btnSend.onClick.RemoveAllListeners();
        btnSend.onClick.AddListener(delegate { OnClickSend(); });
    }

    private void Start(){
        SetChild();
    }

    bool isWait = false;
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
    }

    void OnClickSend()
    {
        GameObject newCell = Instantiate(originCell_right, scrollView.content);
        newCell.transform.SetSiblingIndex(0);
        newCell.SetActive(true);

        var systemCell = newCell.AddComponent<ChatSystemCell>();
        systemCell.Init();
        systemCell.SetText(inputField.text);
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollView.content);
        inputField.text = "";
    }

    void recevedMessage(string message)
    {
        GameObject newCell = Instantiate(originCell_left, scrollView.content);
        newCell.transform.SetSiblingIndex(0);
        newCell.SetActive(true);

        var systemCell = newCell.AddComponent<ChatSystemCell>();
        systemCell.Init();
        systemCell.SetText(message);
        LayoutRebuilder.ForceRebuildLayoutImmediate(scrollView.content);
    }
}