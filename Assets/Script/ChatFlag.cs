using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChatFlag : MonoBehaviour
{
    public enum ChatType
    {
        Thesaurus = 0,
        Vagary,
        Era
    }

    public static ChatType type = 0;
    public ChatType displayType = 0;
    public GameObject chatRoot;

    void Update(){
        if(Input.GetMouseButtonDown(0)){
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)){
                if(hit.collider.gameObject.name == "Chatbot_Thesaurus"){
                    type = ChatType.Thesaurus;
                }
                else if(hit.collider.gameObject.name == "Chatbot_Vagary"){
                    type = ChatType.Vagary;
                }
                else if(hit.collider.gameObject.name == "Chatbot_Era"){
                    type = ChatType.Era;
                }
                chatRoot.SetActive(true);
            }
        }
        if(Input.GetKeyDown(KeyCode.Escape)){
            chatRoot.SetActive(false);
        }
        displayType = type;
    }
}
