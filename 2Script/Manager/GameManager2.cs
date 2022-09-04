using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager2 : MonoBehaviour
{
    public TypeEffect typeEffect; // 이건 프롤로그용
    public TypeEffect typeEffect2; // 이건 일반대화
    public Animator talkPanel;
    public TalkManager talkManager;
    public GameObject scanObject;

    public Image portrait;
    public Animator portraitAnim;
    public Sprite prevPortrait;

    public QuestManager questManager;
    public Text questTalk;
    public Prologue prologue;
    public PlayerAction player;
    public GameObject menuSet;

    public bool isAction; // 대화중인가?
    public int talkIndex;

    OrderManager orderManager;

    void Start()
    {
        GameLoad();
        orderManager = FindObjectOfType<OrderManager>();
        //questTalk.text = questManager.CheckQuest(); 
    }

    void Update()
    {
        /* 서브메뉴
        if (Input.GetButtonDown("Cancel"))
            SubMenuActive();
    }

    public void SubMenuActive()
    {
        if (menuSet.activeSelf)
            menuSet.SetActive(false);
        else
            menuSet.SetActive(true); */
    }
    

    public void Action(GameObject scanObj)
    {
    
        scanObject = scanObj;
        ObData obData = scanObject.GetComponent<ObData>(); // 스캔한 오브젝트 데이타접근
        Talk(obData.id, obData.isNpc);

        //대화창 유무
        talkPanel.SetBool("isShow", isAction);
        portraitAnim.SetBool("isShow", isAction);
    }

    void Talk(int id,bool isNpc)
    {
        int questTalkIndex = 0;
        string talkData = "";

        //대화 데이터 셋
        if (typeEffect2.isAnim)
        {
            typeEffect2.SetMsg("");
            return;
        }
        else
        {
            questTalkIndex = questManager.GetQuestTalkIndex(id);
            talkData = talkManager.GetTalk(id + questTalkIndex, talkIndex);//토크매니저에있던 대사 가져옴
        }

        //대화 끝내기
        if (talkData == null)
        {
            isAction = false;
            talkIndex = 0;
            //questTalk.text = questManager.CheckQuest(id);
            orderManager.Move();
            return;
        }

        orderManager.NotMove();
        if (isNpc)//대화 계속하기
        {
            typeEffect2.SetMsg(talkData.Split(':')[0]);

            portrait.sprite = talkManager.GetPortrait(id, int.Parse(talkData.Split(':')[1]));//톸매니저에 있던 이미지 가져옴
            portrait.color = new Color(1, 1, 1, 1);

            //일러가 다르면 일러위아래로 움직이는 애니실행
            if (prevPortrait != portrait.sprite)
            {
                portraitAnim.SetTrigger("doEffect");
                prevPortrait = portrait.sprite;
            }
        }
        else
        {
            typeEffect2.SetMsg(talkData);
            //초상화 숨기기
            portrait.color = new Color(1, 1, 1, 0);
        }
        //다음 대화
        isAction = true;
        talkIndex++;
    }

    public void TouchArea() // 다음버튼 말고 대화상자 눌렀을 때 대화 한번에 다나오기
    {
        StartCoroutine(TouchAreaC());
    }

    IEnumerator TouchAreaC()
    {
        typeEffect.touchArea.SetActive(false);
        yield return new WaitForSeconds(0.2f); // 중복 실행 방지용 코루틴
        Talk2("");
    }

    public void Talk2(string content)
    {
        //대화 데이터 셋
        if (typeEffect.isAnim && content == "") 
        {
            typeEffect.SetMsg("");
            return;
        }
        if (content == "")
            return;
        typeEffect.touchArea.SetActive(true);
        typeEffect.SetMsg(content.Split('$')[0]);

        portrait.sprite = (talkManager.portraitArr[int.Parse(content.Split('$')[1])]);//톸매니저에 있던 이미지 가져옴
        portrait.color = new Color(1, 1, 1, 1);

        //일러가 다르면 일러위아래로 움직이는 애니실행
        if (prevPortrait != portrait.sprite)
        {
            //portraitAnim.SetTrigger("doEffect");
            prevPortrait = portrait.sprite;
        }
        //다음 대화
    }

    //저장!!!!
    public void GameSave()
    {
        menuSet.SetActive(false);
        PlayerPrefs.SetFloat("PlayerX", player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerY", player.transform.position.y);
        PlayerPrefs.SetFloat("PlayerZ", player.transform.position.z);
        PlayerPrefs.SetInt("QuestId", questManager.questId);
        PlayerPrefs.SetInt("QuestActionIndex", questManager.questActionIndex);
        PlayerPrefs.Save();
    }

    public void GameLoad()
    {
        if (!PlayerPrefs.HasKey("PlayerX"))
        {
            return;
        }
        float x = PlayerPrefs.GetFloat("PlayerX");
        float y = PlayerPrefs.GetFloat("PlayerY");
        //float z = PlayerPrefs.GetFloat("PlayerZ");
        int questId = PlayerPrefs.GetInt("QuestId");
        int questActionIndex = PlayerPrefs.GetInt("QuestActionIndex");

        player.transform.position = new Vector3(x, y, -6);
        questManager.questId = questId;
        questManager.questActionIndex = questActionIndex;
        questManager.ControlObject();
    }

    public void GameExit()
    {
        Application.Quit();
    }


}
