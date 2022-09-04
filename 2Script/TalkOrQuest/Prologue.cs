using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Prologue : MonoBehaviour
{
    FadeManager fadeManager;
    public TypeEffect typeEffect;
    public Sprite[] scenes;
    GameManager2 gm;
    public Image background;
    TransferMap transferMap;
    AudioManager audioManager;

    public GameObject TalkPanel;
    public Text TalkPanelText;
    public GameObject nextButton;
    public GameObject ifEndWhereGo;// 프롤로그가 끝나면 어디로 가나
    public GameObject subMenuButton;
    public GameObject mainMenu;

    public string[] bgm;

    bool next = true;
    int dialogueIndex = 0; // 대화내용 순번
    // 대사 (맨끝에 " $0$#" 넣어야댐
    public string[] prologueDialogue = { "땡~땡~땡~$0",
    "수업 종이 울린다.$0$#",
    "오늘 수업이 모두 끝났음을 알리는 소리다.$0",
    "에리:엔젤라~ 오컬트 동아리 같이 가자니까~~ 마법이라든가 마술이라든가.. 쨋든 신기한 것들이 많다구.$0",
    "얘는 같은 반 친한 친구인 에리다. 마법이나 마술같은 환상적인 것들에 관심이 많은 친구다.$0",
    "마법같은건 세상에 있을리도 없고, 마술도 다 속임수에 불과한데 말이지..$0",
    "엔젤라:그런건 어렸을 때나 만화 주인공들 따라하던 거 아니야? 마법이 있을리가 없잖아..$1",
    "에리:치... 혹시 모르잖아! 우리 주위에 마법을 쓸수있는 마법사가 숨어있을지, 아니면 마법이 일상인 이세계가 있을지!!$0",
    "어쩌다가 저런거에 빠져버린걸까,, 구제가 안된다.$0",
    "엔젤라:하하..뭐래,, 소설쓰니? 난 이제 학원가야해. 며칠 있으면 기말고사잖아? 너도 공부 슬슬 시작해야하는거 아니야?$1",
    //index 10
    "에리:힝.. 엔젤라는 낭만이 없어 ㅠ.ㅠ 내일은 꼭 엔젤라를 동아리에 가입시키고야 말겠어!!$0",
    " $0$#"
    };

    // Start is called before the first frame update
    void Start()
    {
        fadeManager = FindObjectOfType<FadeManager>();
        gm = FindObjectOfType<GameManager2>();
        transferMap = FindObjectOfType<TransferMap>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    public void StartPrologue()
    {
        StartCoroutine(PrologueCoroutine());
    }

    public void NextButton()
    {
        next = true;
        if (typeEffect.isAnim)
            gm.Talk2("");
        else
        {
            if (prologueDialogue[dialogueIndex + 1].Contains("#")) // 잠깐 대화 멈추는 트리거, 다음 대사에 있는지 미리 확인
            {
                next = false; 
                dialogueIndex++;
                return;
            }
            dialogueIndex++;
            gm.Talk2(prologueDialogue[dialogueIndex]);
        }
    }

    IEnumerator PrologueCoroutine()
    {
        fadeManager.FadeOutIn(0.01f, 1f, false);
        yield return new WaitUntil(() => fadeManager.completedBlack);
        mainMenu.SetActive(false);
        TalkPanel.SetActive(true);
        gm.Talk2(prologueDialogue[dialogueIndex]);
        yield return new WaitUntil(() => !next); //index 1
        next = true;

        background.sprite = scenes[0]; // testImage
        audioManager.Play(bgm[0]); // school bgm
        fadeManager.ImageGraduallyAppear(background);
        gm.Talk2(prologueDialogue[dialogueIndex]);
        yield return new WaitUntil(() => !next); // index 11
        transferMap.TransperCanvas2(TalkPanel, ifEndWhereGo); // 끝으로 집으로 감

        yield return new WaitUntil(() => fadeManager.completedBlack); // 완전히 어두워질 때 까지 기다렸다가
        subMenuButton.SetActive(true);
    }



    /*
    void SceneAppear(string sceneName)
    {
        for(int i = 0;i < scenes.Length;i++)
        {
            if (scenes[i].gameObject.name == sceneName) {
                Color color = scenes[i].GetComponent<SpriteRenderer>().color;
                color.a = 1f;
                scenes[i].GetComponent<SpriteRenderer>().color = color;
            }
        }
    }


    void SceneDisAppear(string sceneName)
    {
        for (int i = 0; i < scenes.Length; i++)
        {
            if (scenes[i].gameObject.name == sceneName)
            {
                Color color = scenes[i].GetComponent<SpriteRenderer>().color;
                color.a = 0f;
                scenes[i].GetComponent<SpriteRenderer>().color = color;
            }
        }
    }*/
}
