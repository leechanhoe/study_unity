using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Foothold : MonoBehaviour
{
    [SerializeField]
    public int FootholdID; // 발판 고유 ID
    public string mapname; // 밟으면 들어가지는곳
    public bool isMap;
    public string []monsterName; // 발판을 밟으면 나오는 몬스터
    //public bool PlayerOnFoothold = false;
    List<string> monsterNames;

    public string sceneName; // 활성화시킬 배경 이름
    public int AppearPro = 25;
    private int random_int;
    private int random_int2;
    public int isOn = 0; // 발판 위에 있는지 체크 (아니 트리거엔터쓰면 3번씩 들어갔다 인식됨;)
    public int monsterNum; // 몬스터의 마릿수

    private Scene[] scenes;
    Scene currentScene;
    Monster[] monsters;
    Foothold[] footholds;

    OrderManager orderManager;
    CameraManager cameraManager;
    PlayerManager playerManager;
    BattleUI battleUI;
    Bound2 bound;
    FadeManager fadeManager;
    SubMenu subMenu;
    GameManager2 gameManager2;

    // Start is called before the first frame update
    void Start()
    {
        monsterNames = new List<string>();
        for (int i = 0; i < monsterName.Length; i++)
            monsterNames.Add(monsterName[i]);

        bound = FindObjectOfType<Bound2>();
        scenes = FindObjectsOfType<Scene>();
        monsters = FindObjectsOfType<Monster>();
        cameraManager = FindObjectOfType<CameraManager>();
        playerManager = FindObjectOfType<PlayerManager>();
        battleUI = FindObjectOfType<BattleUI>();
        fadeManager = FindObjectOfType<FadeManager>();
        footholds = FindObjectsOfType<Foothold>();
        subMenu = FindObjectOfType<SubMenu>();
        gameManager2 = FindObjectOfType<GameManager2>();
    }

    private void SceneAppear()
    {
        for (int i = 0; i < scenes.Length; i++)
        {
            if (scenes[i].gameObject.name == sceneName)
            {
                currentScene = scenes[i];
                Color color = scenes[i].GetComponent<SpriteRenderer>().color;
                color.a = 1f;
                scenes[i].GetComponent<SpriteRenderer>().color = color;
            }
        }
    }

    public void SceneDisappear()
    {
        Color color = currentScene.GetComponent<SpriteRenderer>().color;
        color.a = 0f;
        currentScene.GetComponent<SpriteRenderer>().color = color;
        currentScene = null;
    }


    void MonsterAppear()
    {
        battleUI.leftButton.SetActive(false);
        battleUI.rightButton.SetActive(false);
        battleUI.upButton.SetActive(false);
        battleUI.appearMonsters.Clear();

        monsterNum = Random.Range(1, 4);
        for (int j = 0; j < monsters.Length; j++)
        {
            random_int2 = Random.Range(0, monsterNames.Count);
            if (monsters[j].gameObject.name == monsterNames[random_int2] && !battleUI.isAppear1)
            {
                Color color2 = monsters[j].GetComponent<SpriteRenderer>().color;
                color2.a = 1f;
                monsters[j].GetComponent<SpriteRenderer>().color = color2;
                monsters[j].isActive = true;

                battleUI.appearMonsters.Add(monsters[j]); // 배틀유아이에 출현 몬스터 정보 저장
                battleUI.monster1 = monsters[j];

                battleUI.isAppear1 = true;
            }

            random_int2 = Random.Range(0, monsterNames.Count);
            if (monsters[j].gameObject.name == monsterNames[random_int2]+"2" && monsterNum >= 2 && !battleUI.isAppear2)
            {
                Color color2 = monsters[j].GetComponent<SpriteRenderer>().color;
                color2.a = 1f;
                monsters[j].GetComponent<SpriteRenderer>().color = color2;
                monsters[j].isActive = true;

                battleUI.monster2 = monsters[j];
                battleUI.appearMonsters.Add(monsters[j]); // 배틀유아이에 출현 몬스터 정보 저장

                battleUI.isAppear2 = true;
            }

            random_int2 = Random.Range(0, monsterNames.Count);
            if (monsters[j].gameObject.name == monsterNames[random_int2]+"3" && monsterNum >= 3 && !battleUI.isAppear3)
            {
                Color color2 = monsters[j].GetComponent<SpriteRenderer>().color;
                color2.a = 1f;
                monsters[j].GetComponent<SpriteRenderer>().color = color2;
                monsters[j].isActive = true;

                battleUI.monster3 = monsters[j];
                battleUI.appearMonsters.Add(monsters[j]); // 배틀유아이에 출현 몬스터 정보 저장

                battleUI.isAppear3 = true;
            }
        }
        if (!battleUI.isAppear1)
            MonsterAppear();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isOn++;
        if (isOn == 1)
        {
            StartCoroutine(GoToBattle());
        }
        if (isOn > 1)
            isOn = 0;
    }

    IEnumerator GoToBattle()
    {
        random_int = Random.Range(0, 101);
        if (random_int < AppearPro)
        {

            fadeManager.FadeOutIn(canMove: false);
            yield return new WaitUntil(() => fadeManager.completedBlack);
            subMenu.menuButton.SetActive(false); //메뉴 버튼 없애기

            cameraManager.SetBound(bound.GetComponent<BoxCollider2D>());//플레이어와 카메라 이동
            playerManager.currentMapName = "Battle Field";
            playerManager.transform.position = new Vector3(1.5f, -18f, 0f);

            SceneAppear(); // 배경 불러오기
            MonsterAppear(); // 몬스터 불러오기

            battleUI.playerActionPanel.SetActive(true);
            battleUI.FirstPanel.SetActive(true);
            battleUI.explanePanel.SetActive(true);

            battleUI.explaneText.text = "전투를 시작합니다.";
            battleUI.footholdId = FootholdID;
            battleUI.currentFoothold = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
