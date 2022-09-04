using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public int monsterID; // 몬스터 고유 아이디
    public string monsterName;
    public int monsterPosition;
    public bool isActive = false;
    public bool isAttacked = false;
    public float animTime;

    public string attack_Sound;

    public Property property;

    public enum Property
    {
        Not,
        Fire,
        Ice,
        Electricity,
        Earth,
        Light,
        Darkness,
    }

    public int hp;
    public int currentHp;
    public int atk;
    public int def;
    public int avd; // 회피율
    public int Lv;
    public int exp;
    float dmgRange;

    public DropItem[] dropItem;

    PlayerStat playerStat;
    BattleUI battleUI;
    Inventory inventory;
    //Animator anim;

    private void Start()
    {
        playerStat = FindObjectOfType<PlayerStat>();
        battleUI = FindObjectOfType<BattleUI>();
        inventory = FindObjectOfType<Inventory>();
        //anim = GetComponent<Animator>();
    }

    public int Hurted(int _playerAtk) // 플레이어에게 공격당함
    {
        int playerAtk = _playerAtk;
        float dmg;

        if (def >= playerAtk)
            dmg = 1;
        else
        {
            dmg = playerAtk - def;
            dmg *= Random.Range(playerStat.lowestDmg, playerStat.highestDmg);
        }
        currentHp -= (int)dmg;
        
        StartCoroutine(HurtedCoroutine());

        return (int)dmg;
    }
    public int HurtPlayer() // 플레이어를 공격
    {
        float dmg;

        if (playerStat.def >= atk)
            dmg = 1f;
        else
        {
            dmg = atk - playerStat.def;
            dmgRange = Random.Range(0.8f, 1.2f);
            dmg *= dmgRange;
        }

        AudioManager.instance.Play(attack_Sound);

        StartCoroutine(HurtCoroutine((int)dmg));
        return (int)dmg;
    }
    IEnumerator HurtedCoroutine() // 피격당한 객체 깜빡깜빡거리기
    {
        Color color = GetComponent<SpriteRenderer>().color;
        color.a = 0.1f;
        GetComponent<SpriteRenderer>().color = color;
        yield return new WaitForSeconds(0.3f);
        color.a = 1f;
        GetComponent<SpriteRenderer>().color = color;
        yield return new WaitForSeconds(0.3f);
        color.a = 0.1f;
        GetComponent<SpriteRenderer>().color = color;
        yield return new WaitForSeconds(0.3f);
        color.a = 1f;
        GetComponent<SpriteRenderer>().color = color;
        yield return new WaitForSeconds(0.3f);
        color.a = 0.1f;
        GetComponent<SpriteRenderer>().color = color;
        yield return new WaitForSeconds(0.3f);
        color.a = 1f;
        GetComponent<SpriteRenderer>().color = color;
        battleUI.nextButton.SetActive(true);

        if (currentHp <= 0)
        {
            yield return new WaitForSeconds(0.1f);
            Debug.Log("몬스터 죽음");
            color.a = 0f;
            this.GetComponent<SpriteRenderer>().color = color;
            PlayerStat.instance.currentExp += exp;
            currentHp = hp; // 다음에 만날땔 대비해서 체력 다시 채움

            isActive = false;
            if (battleUI.monster1 == this)
            {
                battleUI.isAppear1 = false;
                battleUI.leftButton.SetActive(false);
            }
            else if (battleUI.monster2 == this)
            {
                battleUI.isAppear2 = false;
                battleUI.upButton.SetActive(false);
            }
            else if (battleUI.monster3 == this)
            {
                battleUI.isAppear3 = false;
                battleUI.rightButton.SetActive(false);
            }
        }
    }

    IEnumerator HurtCoroutine(int _dmg) // 몬스터 공격 애니메이션
    {
        //anim.SetBool("Attack", true);
        yield return new WaitForSeconds(animTime);
        //anim.SetBool("Attack", false);

        playerStat.HitedMotion();

        playerStat.currentHp -= _dmg;

        if (playerStat.currentHp <= 0)
        {
            Debug.Log("체력 0 이하");
            playerStat.playerAlive = false;
        }

        //나중에 몬스터 공격모션 넣기
        isAttacked = false;
    }

    public int DropItemReturn()
    {
        int random = Random.Range(0,100);
        for (int i = 0;i < dropItem.Length;i++)
        { // 랜덤숫자가 범위안에 들어가면 그 범위에 맞는 템 드롭됨
            if (dropItem[i].minValue <= random && random < dropItem[i].maxValue)
            {
                return dropItem[i].itemID;
            }
        }
        return 0;
    }
}
