using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalkPanel : MonoBehaviour
{
    public Text panelText;
    public GameObject panel;
    public GameObject nextButton;
    Animator anim;

    bool next;
    void Start()
    {
        anim = GetComponent<Animator>();
    }
    
    void UsePanel(string[] content)
    {
        StartCoroutine(UsePanelC(content));
    }
    
    IEnumerator UsePanelC(string[] content)
    {
        anim.SetBool("isShow", true);
        for (int i = 0; i < content.Length; i++)
        {
            panelText.text = content[i];
            nextButton.SetActive(true); // 이하 3줄은 세트임
            yield return new WaitUntil(() => next);
            next = false;
        }
        anim.SetBool("isShow", false);
    }

    public void NextButton() // 다음버튼 , 누르는 즉시 버튼 사라짐
    {
        next = true;
        nextButton.SetActive(false);
    }
}
