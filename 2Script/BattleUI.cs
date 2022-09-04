using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUI : MonoBehaviour
{
    public GameObject FirstPanel; // 배틀필드 입장시 제일 먼저 활성화되는 메뉴
    public GameObject attackPanel; // 공격을 눌렀을 시 메뉴
    public GameObject playerActionPanel; // 뒤에 황토색 판넬 
    public GameObject nothingPanel; // 다음버튼만 있는 패널

    public GameObject arrow1; // 몬스터 위의 화살표
    public GameObject arrow2;
    public GameObject arrow3;

    public GameObject attackButton; // 타깃을 정하고 생기는 공격버튼
    public GameObject leftButton; // 타킷을 정하는 버튼
    public GameObject rightButton;
    public GameObject upButton;
    public Text leftButtonText;
    public Text upButtonText;
    public Text rightButtonText;

    public GameObject nextButton; // 다음버튼
    bool next = false;

    public GameObject explanePanel; // 배틀할때 쓰이는 텍스트를 위한 판넬
    public Text explaneText; // 배틀할때 쓰이는 텍스트

    public Monster monster1;
    public Monster monster2;
    public Monster monster3;
    public bool isAppear1 = false; // 몬스터가 존재하는가
    public bool isAppear2 = false;
    public bool isAppear3 = false;

    public List<Monster> appearMonsters; // 배틀에 나타난 몬스터

    Foothold[] foothold;
    PlayerStat thePlayerStat;
    Monster[] monsters;
    FadeManager fadeManager;
    CameraManager cameraManager;
    SubMenu subMenu;
    DatabaseManager database;
    Inventory inventory;

    public string atkSound;

    Monster choosedMonster;

    public GameObject prefabs_Floating_Text;
    public GameObject parent;

    public int footholdId; 
    public Foothold currentFoothold; //현재 발판 정보 가져옴

    bool allMonsterDead; // 모든 몬스터가 죽었나 확인

    public BoxCollider2D FieldBound;

    public GameObject[] dropItemIcons;
    public GameObject[] ObjectPos; // 몬스터나 캐릭터 위치(텍스트 띄우기 위한)

    private void Start()
    {
        foothold = FindObjectsOfType<Foothold>();
        thePlayerStat = FindObjectOfType<PlayerStat>();
        monsters = FindObjectsOfType<Monster>();
        appearMonsters = new List<Monster>();
        fadeManager = FindObjectOfType<FadeManager>();
        cameraManager = FindObjectOfType<CameraManager>();
        subMenu = FindObjectOfType<SubMenu>();
        inventory = FindObjectOfType<Inventory>();
        database = FindObjectOfType<DatabaseManager>();
    }

    public void Run() // 도망치기
    {
        StartCoroutine(RunCoroutine());
    }

    IEnumerator RunCoroutine()
    {
        int pro = Random.Range(0, 2);
        if (pro == 1)
        {
            FirstPanel.SetActive(false);
            nothingPanel.SetActive(true);

            //도망가는 모션 애니
            yield return new WaitForSeconds(2f);
            explaneText.text = "몬스터에게서 무사히 도망쳤습니다.";
            nextButton.SetActive(true); // 이하 3줄은 세트임
            yield return new WaitUntil(() => next);
            next = false;
            GoToField();
        }
        else
        {
            FirstPanel.SetActive(false);
            nothingPanel.SetActive(true);

            //anim 도망가는 모션
            yield return new WaitForSeconds(2f);
            explaneText.text = "이런, 도망치지 못했습니다..";
            nextButton.SetActive(true); // 이하 3줄은 세트임
            yield return new WaitUntil(() => next);
            next = false;
            AttackPlayer();
        }
    }

    public void NextButton() // 다음버튼 , 누르는 즉시 버튼 사라짐
    {
        next = true;
        nextButton.SetActive(false);
    }

    public void GoToAttackMenu() // 일반공격 메뉴 띄움
    {
        FirstPanel.SetActive(false);
        attackPanel.SetActive(true);
        attackButton.SetActive(false);
        if (isAppear1)
        {
            leftButton.SetActive(true);
            leftButtonText.text = monster1.monsterName;
        }
        if (isAppear2)
        {
            upButton.SetActive(true);
            upButtonText.text = monster2.monsterName;
        }
        if (isAppear3)
        {
            rightButton.SetActive(true);
            rightButtonText.text = monster3.monsterName;
        }
    }

    public void BackToFirstMenu() // 처음 플레이어 행동고르는 메뉴 띄움
    {
        FirstPanel.SetActive(true);

        attackPanel.SetActive(false);
        arrow1.SetActive(false);
        arrow2.SetActive(false);
        arrow3.SetActive(false);
        attackButton.SetActive(false);
        nothingPanel.SetActive(false);
    }

    public void GoToField() // 전투를 마치고 다른곳으로 감
    {
        StartCoroutine(GoToFieldCoroutine());
    }
    IEnumerator GoToFieldCoroutine()
    {
        fadeManager.FadeOutIn();
        yield return new WaitUntil(() => fadeManager.completedBlack);
        playerActionPanel.SetActive(false);
        FirstPanel.SetActive(false);
        attackPanel.SetActive(false);
        arrow1.SetActive(false);
        arrow2.SetActive(false);
        arrow3.SetActive(false);
        attackButton.SetActive(false);
        nothingPanel.SetActive(false);
        explanePanel.SetActive(false);

        for (int i = 0; i < monsters.Length; i++) {
            Color color = monsters[i].GetComponent<SpriteRenderer>().color;
            color.a = 0f;
            monsters[i].GetComponent<SpriteRenderer>().color = color;
            monsters[i].isActive = false;
            monsters[i].currentHp = monsters[i].hp;
                }
        monster1 = null;
        monster2 = null;
        monster3 = null;
        choosedMonster = null;
        isAppear1 = false;
        isAppear2 = false;
        isAppear3 = false;
        footholdId = -1;
        currentFoothold.SceneDisappear();
        appearMonsters.Clear();
        currentFoothold.isOn = -2;
        thePlayerStat.transform.position = currentFoothold.transform.position; // 원래 발판있던 자리로 돌아감
        cameraManager.SetBound(FieldBound);// 카메라 이동
        cameraManager.transform.position = new Vector3(thePlayerStat.transform.position.x, thePlayerStat.transform.position.y, -10f);
        PlayerManager.instance.currentMapName = "Field";


        subMenu.menuButton.SetActive(true);
        currentFoothold = null;
    }

    public void ChooseTarget(int target) // 공격할때 어느몬스터를 때릴지 지정, 1은 왼쪽 2는 위 3은 오른쪽
    {
        arrow1.SetActive(false);
        arrow2.SetActive(false);
        arrow3.SetActive(false);
        for (int j = 0; j < foothold.Length; j++)
        {
            if (foothold[j].FootholdID == footholdId) // 많은 발판중에서 활성화된 하나의 발판 찾아서 몬스터 정보 가져오기
            {
                switch (target)
                {
                    case 1:
                        arrow1.SetActive(true);
                        choosedMonster = monster1;
                        break;
                    case 2:
                        arrow2.SetActive(true);
                        choosedMonster = monster2;
                        break;
                    case 3:
                        choosedMonster = monster3;
                        arrow3.SetActive(true);
                        break;
                }
            }
        }
        if (choosedMonster != null)
            attackButton.SetActive(true);
    }
    
    public void AttackMonster() // 플레이어가 몬스터를 공격
    {
        StartCoroutine(PlayerAttackToMonster());
    }

    IEnumerator PlayerAttackToMonster()
    {
        arrow1.SetActive(false);
        arrow2.SetActive(false);
        arrow3.SetActive(false);
        attackButton.SetActive(false);
        attackPanel.SetActive(false);
        nothingPanel.SetActive(true);
        
        thePlayerStat.AttackMotion();

        yield return new WaitUntil(() => thePlayerStat.isAttacked);
        thePlayerStat.isAttacked = false;

        int dmg = choosedMonster.Hurted(thePlayerStat.atk);
        AudioManager.instance.Play(atkSound);
        Vector3 vector;

        if (choosedMonster == monster1)
            vector = ObjectPos[1].transform.position;
        else if (choosedMonster == monster2)
            vector = ObjectPos[2].transform.position;
        else
            vector = ObjectPos[3].transform.position;
        // 이하 클론을 언급하는 10줄정도는 피격시 데미지 텍스트 띄우기
        //Istantiate(effect, vector, Quaternion.Euler(Vector3.zero));
        //vector.y += 1.5f;

        GameObject clone = Instantiate(prefabs_Floating_Text, vector, Quaternion.Euler(Vector3.zero));
        clone.GetComponent<FloatingText>().text.text = dmg.ToString();
        clone.GetComponent<FloatingText>().text.color = Color.white;
        clone.GetComponent<FloatingText>().text.fontSize = 25;
        //clone.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f);
        clone.transform.SetParent(parent.transform);

        explaneText.text = "몬스터에게 " + dmg + " 데미지를 입혔습니다";

        yield return new WaitForSeconds(thePlayerStat.attackTime);
        yield return new WaitUntil(() => next);
        next = false;

        choosedMonster = null;

        allMonsterDead = true;
        for (int i = 0; i < monsters.Length; i++)
        {
            if (monsters[i].isActive) // 필드에 몬스터가 1마리 이상이 있으면 몬스터가 행동 개시
            {
                AttackPlayer();
                allMonsterDead = false;
                i = monsters.Length;
            }
        }

        if (allMonsterDead) // 몬스터를 모두 잡아 전투에서 승리했을 때
        {
            int expSum = 0;
            List<int> dropItemsID = new List<int>();
            
            explaneText.text = "전투에서 승리하였습니다!!";
            nextButton.SetActive(true); // 이하 3줄은 세트임
            yield return new WaitUntil(() => next);
            next = false;
            
            for (int i = 0;i < appearMonsters.Count;i++) // 몬스터 드랍템이나 경험치 정보 얻는곳
            {
                expSum += appearMonsters[i].exp; // 경험치의 합
                if(appearMonsters[i].DropItemReturn() != 0)
                    dropItemsID.Add(appearMonsters[i].DropItemReturn()); // 드랍템 추가
            }

            explaneText.text = expSum + "경험치를 획득하였습니다.";
            nextButton.SetActive(true); // 이하 3줄은 세트임
            yield return new WaitUntil(() => next);
            next = false;

            if(dropItemsID.Count > 0) // 아이템이 드랍된게 있으면
            {
                explaneText.text = "아이템을 획득하였습니다.";
                for (int i = 0;i < dropItemsID.Count;i++)
                { // 아이템 이미지 표시
                    dropItemIcons[i].SetActive(true);
                    dropItemIcons[i].GetComponent<Image>().sprite = database.GetItemIcon(dropItemsID[i]);
                }
                nextButton.SetActive(true); // 이하 3줄은 세트임
                yield return new WaitUntil(() => next);
                next = false;
                for (int i = 0; i < dropItemsID.Count; i++)
                {
                    dropItemIcons[i].SetActive(false); // 아이템 이미지 다시 없애기
                    inventory.GetAnItem(dropItemsID[i]); // 아이템 인벤토리에 추가
                }
            }

            if (thePlayerStat.currentExp >= thePlayerStat.needExp[thePlayerStat.character_Lv - 1]) // 레벨업
            {
                thePlayerStat.LevelUp();
                nextButton.SetActive(true); // 이하 3줄은 세트임
                yield return new WaitUntil(() => next);
                next = false;
            }
            fadeManager.FadeOutIn();
            yield return new WaitUntil(() => fadeManager.completedBlack);
            //thePlayerStat.transform.position = currentFoothold.transform.position; // 원래 발판있던 자리로 돌아감
            cameraManager.SetBound(FieldBound);// 카메라 이동
            cameraManager.transform.position = new Vector3(thePlayerStat.transform.position.x, thePlayerStat.transform.position.y, -10f);
            PlayerManager.instance.currentMapName = "Field";
            GoToField();
        }
    }

    public void AttackPlayer()//몬스터가 플레이어를 공격
    {
        StartCoroutine(MonsterAttackToPlayer());
    }

    IEnumerator MonsterAttackToPlayer()
    {
        nothingPanel.SetActive(true);
        explaneText.text = "몬스터가 공격할 차례입니다.";
        for (int i = 0; i < monsters.Length; i++)
        {
            if (monsters[i].isActive && !monsters[i].isAttacked)
            {
                int dmg;
                monsters[i].isAttacked = true;
                dmg = monsters[i].HurtPlayer();

                yield return new WaitUntil(() => !monsters[i].isAttacked);
                Vector3 vector = ObjectPos[0].transform.position;
                //vector.y += 60;

                GameObject clone = Instantiate(prefabs_Floating_Text, vector, Quaternion.Euler(Vector3.zero)); // instantiate 함수는 prefab을 생성하는거
                clone.GetComponent<FloatingText>().text.text = dmg.ToString();
                clone.GetComponent<FloatingText>().text.color = Color.red;
                clone.GetComponent<FloatingText>().text.fontSize = 25;
                //clone.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f);
                clone.transform.SetParent(parent.transform);

                explaneText.text = "몬스터에게 " + dmg + " 데미지를 입었습니다";

                nextButton.SetActive(true); // 이하 3줄은 세트임
                yield return new WaitUntil(() => next);
                next = false;

                if (!thePlayerStat.playerAlive)
                {
                    Debug.Log("플레이어 사망");
                    //나중에 플레이어 사망시 함수 만들예정
                }
            }
        }
        BackToFirstMenu();
    }
}
