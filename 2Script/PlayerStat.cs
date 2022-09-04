using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStat : MonoBehaviour
{
    public static PlayerStat instance;

    public int character_Lv;
    public int[] needExp;
    public int currentExp;

    public int hp;
    public int currentHp;
    public int mp;
    public int currentMp;

    public float lowestDmg; // 숙련도(최저데미지)
    public float highestDmg; // 최대데미지
    public int atk; // 공격력
    public int def; // 방어력
    public int cri; // 크리티컬
    public int avd; // 회피율

    public int str; // 힘
    public int dex; // 민첩
    public int inte; // 지혜
    public int endu; // 지구력

    public int statPoint; //스텟포인트
    public int skillPoint; // 스킬포인트

    public string dmgSound;
    public GameObject prefabs_Floating_text;
    public GameObject parent;

    public int recover_hp; // 초당 회복력
    public int recover_mp;
    public string dmged_Sound;

    private float current_time;

    public Slider hpSlider;
    public Slider mpSlider;

    public bool playerAlive = true; // 플레이어 생존확인
    public float attackTime;

    BattleUI battleUI;

    public bool isAttacked = false; // 공격 애니메이션 다 진행될때까지 대기

    private void Awake() // 파괴방지
    {
        if (instance == null)
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
        else
            Destroy(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        currentHp = hp;
        currentMp = mp;

        battleUI = FindObjectOfType<BattleUI>();
    }

    IEnumerator HitCoroutine()
    {
        Color color = GetComponent<SpriteRenderer>().color;
        for (int i = 0; i < 3; i++)
        {
            color.a = 0;
            GetComponent<SpriteRenderer>().color = color;
            yield return new WaitForSeconds(0.3f);
            color.a = 1f;
            GetComponent<SpriteRenderer>().color = color;
            yield return new WaitForSeconds(0.3f);
        }
    }

    public void HitedMotion()
    {
        StartCoroutine(HitCoroutine());
    }

    public void LevelUp() // 레벨업시 
    {
        character_Lv++;
        int increaseHp, increaseMp, increaseAtk, increaseDef;
        increaseHp = Random.Range(27, 34);
        increaseMp = Random.Range(27, 34);
        increaseAtk = Random.Range(character_Lv + 3, character_Lv + 8);
        increaseDef = Random.Range(8, 13);

        hp += increaseHp;
        mp += increaseMp;

        currentHp += increaseHp;
        currentMp += increaseMp;

        atk += increaseAtk;
        def += increaseDef;

        statPoint += 4;

        battleUI.explaneText.text = "축하합니다! 레벨이 " + character_Lv + " 로 상승하였습니다. \n체력, 마력, 공격력, 방어력이 상승하였습니다.";
    }

        // Update is called once per frame
    void Update()
    {/*
        hpSlider.maxValue = hp;
        mpSlider.maxValue = mp;

        hpSlider.value = currentHp;
        mpSlider.value = currentMp;
        */
      
    }

    public void AttackMotion() // 공격 애니메이션
    {
        StartCoroutine(AttackMotionCoroutine());
    }

    IEnumerator AttackMotionCoroutine()
    {
        //anim.setint(type) // 일반공격이면 -1 마법이면 0~5
        //anim.setint(num) // 타입을 정한 뒤 마법번호
        yield return new WaitForSeconds(attackTime);
        isAttacked = true;
    }
}
