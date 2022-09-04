using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Equipment : MonoBehaviour
{
    private OrderManager theOrder;
    private AudioManager theAudio;
    private PlayerStat thePlayerStat;
    private Inventory theInven;
    private OkOrCancel theOOC;
    
    public string key_sound;
    public string enter_sound;
    public string opne_sound;
    public string close_sound;
    public string takeoff_sound;
    public string equip_sound;

    private const int WEAPON = 0, ARMOR = 1, ACCESSORY = 2, POCKET = 3;
    private const int HP = 0, MP = 1, ATK = 2, DEF = 3, CRI = 4, AVD = 5;
    private const int STR = 6, ENDU = 7, DEX = 8, INTE = 9, SP = 10;

    public int added_atk, added_def, added_hp, added_mp, added_cri, added_avd;

    public GameObject equipWeapon;
    public GameObject go;
    public GameObject go_OOC; // 예 아니오 창
    public Text OOCItemText; // 장비슬롯 누르면 나오는 그 템의 설명

    public Text[] statText; // 스텟
    public Image[] img_slots; // 장비 아이콘들
    public Text[] equipName; // 장비의 이름
    public GameObject go_selected_Slot_UI; // 선택된 장비 슬롯 UI

    public Item[] equipItemList; // 장착된 장비 리스트.

    private int selectedSlot; // 선택된 장비 슬롯

    // Start is called before the first frame update
    void Start()
    {
        theAudio = FindObjectOfType<AudioManager>();
        theInven = FindObjectOfType<Inventory>();
        theOrder = FindObjectOfType<OrderManager>();
        thePlayerStat = FindObjectOfType<PlayerStat>();
        theOOC = FindObjectOfType<OkOrCancel>();
    }

    public void ShowText()
    {
        if (added_hp == 0)
            statText[HP].text = "체력 : " + thePlayerStat.hp.ToString();
        else
            statText[HP].text = "체력 : " + thePlayerStat.hp.ToString() + "(+" + added_hp + ")";
        if (added_mp == 0)
            statText[MP].text = "마나 : " + thePlayerStat.mp.ToString();
        else
            statText[MP].text = "마나 : " + thePlayerStat.mp.ToString() + "(+" + added_mp + ")";
        if (added_atk == 0)
            statText[ATK].text = "공격력 : " + thePlayerStat.atk.ToString();
        else
            statText[ATK].text = "공격력 : " + thePlayerStat.atk.ToString() + "(+" + added_atk + ")";
        if(added_def == 0)
            statText[DEF].text = "방어력 : " + thePlayerStat.def.ToString();
        else
            statText[DEF].text = "방어력 : " + thePlayerStat.def.ToString() + "(+" + added_def + ")";
        if (added_cri == 0)
            statText[CRI].text = "크리율 : " + thePlayerStat.cri.ToString();
        else
            statText[CRI].text = "크리율 : " + thePlayerStat.cri.ToString() + "(+" + added_cri + ")";
        if (added_avd == 0)
            statText[AVD].text = "회피율 : " + thePlayerStat.avd.ToString();
        else
            statText[AVD].text = "회피율 : " + thePlayerStat.avd.ToString() + "(+" + added_avd + ")";

        statText[STR].text = "힘 : " + thePlayerStat.str.ToString();
        statText[ENDU].text = "지구력 : " + thePlayerStat.endu.ToString();
        statText[DEX].text = "민첩 : " + thePlayerStat.dex.ToString();
        statText[INTE].text = "지혜 : " + thePlayerStat.inte.ToString();
        statText[SP].text = thePlayerStat.statPoint.ToString();
    }
  /*  public void SelectedSlot()
    {
        go_selected_Slot_UI.transform.position = img_slots[selectedSlot].transform.position;
    }
*/
    public void ClearEquip() // 장비템 초기화
    {
        Color color = img_slots[0].color;
        color.a = 0f;

        for (int i = 0; i < img_slots.Length; i++)
        {
            img_slots[i].sprite = null;
            img_slots[i].color = color;
        }
    }

    public void EquipItem(Item _item) // 장비템 장착
    {
        string temp = _item.itemID.ToString();
        temp = temp.Substring(0, 3); // 문자열의 0~2번쨰 인덱스 가져옴
        
        switch (temp)
        {
            case "200": // 무기
                EquipItemCheck(WEAPON, _item);
                equipName[WEAPON].text = _item.itemName;
                //equipWeapon.SetActive(true);
                //equipWeapon.GetComponent<SpriteRenderer>().sprite = _item.itemIcon;
                break;
            case "201": // 갑옷
                EquipItemCheck(ARMOR, _item);
                equipName[ARMOR].text = _item.itemName;
                break;
            case "202": // 장신구
                EquipItemCheck(ACCESSORY, _item);
                equipName[ACCESSORY].text = _item.itemName;
                break;
            case "203": // 주머니
                EquipItemCheck(POCKET, _item);
                equipName[POCKET].text = _item.itemName;
                break;
        }
    }

    public void EquipItemCheck(int _count, Item _item)
    {
        if(equipItemList[_count].itemID == 0)
        {
            equipItemList[_count] = _item;
        }
        else
        {
            theInven.EquipToInventory(equipItemList[_count]); // 이미 템이 있으면 입던템 인벤토리로
            equipItemList[_count] = _item;
        }
        EquipEffect(_item);
        theAudio.Play(equip_sound);
        ShowText();
    }

    public void ShowEquip()
    {
        Color color = img_slots[0].color;
        color.a = 1f;

        for (int i = 0; i < img_slots.Length; i++)
        {
            if(equipItemList[i].itemID != 0)
            {
                img_slots[i].sprite = equipItemList[i].itemIcon;
                img_slots[i].color = color;
            }
        }
    }

    private void EquipEffect(Item _item) // 아이템 장착시 능력치 오르기
    {
        thePlayerStat.atk += _item.atk;
        thePlayerStat.def += _item.def;
        thePlayerStat.hp += _item.add_hp;
        thePlayerStat.mp += _item.add_mp;

        added_atk += _item.atk;
        added_def += _item.def;
        added_hp += _item.add_hp;
        added_mp += _item.add_mp;
    }

    private void TakeOffEffect(Item _item) // 아이템 헤제시 능력치 감소
    {
        thePlayerStat.atk -= _item.atk;
        thePlayerStat.def -= _item.def;
        thePlayerStat.hp -= _item.add_hp;
        thePlayerStat.mp -= _item.add_mp;

        added_atk -= _item.atk;
        added_def -= _item.def;
        added_hp -= _item.add_hp;
        added_mp -= _item.add_mp;
    }

    public void ShowEquipmentUI() // 장비창 열기
    {
        theOrder.NotMove();
        theAudio.Play(opne_sound);
        go.SetActive(true);
        ClearEquip();
        ShowEquip();
        ShowText();
    }
    public void CloseEquipmentUI() // 장비창 닫기
    {
        theOrder.Move();
        theAudio.Play(close_sound);
        go.SetActive(false);
        ClearEquip();
    }
    
    public void TouchEquip(int itemType) // 장착템 슬롯 터치시
    {
        if (equipItemList[itemType].itemID == 0)
            return;
        go_OOC.SetActive(true);
        selectedSlot = itemType;
        OOCItemText.text = equipItemList[selectedSlot].itemDescription;
    }

    public void TakeOff() // 장착 헤제 터치시
    {
        theInven.EquipToInventory(equipItemList[selectedSlot]); 
        TakeOffEffect(equipItemList[selectedSlot]);
        equipName[selectedSlot].text = "";
        go_OOC.SetActive(false);

        ShowText();
        equipItemList[selectedSlot] = new Item(0, "", "", Item.ItemType.Equip);
        theAudio.Play(takeoff_sound);
        ClearEquip();
        ShowEquip();
    }

    public void CancelTakeOff() // 취소 터치시
    {
        go_OOC.SetActive(false);
    }

    IEnumerator OOCCoroutine(string _up, string _down)
    {
        go_OOC.SetActive(true);
        theOOC.ShowTwoChoice(_up, _down);
        yield return new WaitUntil(() => !theOOC.activated);
        if (theOOC.GetResult())
        {
            theInven.EquipToInventory(equipItemList[selectedSlot]);
            TakeOffEffect(equipItemList[selectedSlot]);
            if (selectedSlot == WEAPON)
                equipWeapon.SetActive(false);

            ShowText();
            equipItemList[selectedSlot] = new Item(0, "", "", Item.ItemType.Equip);
            theAudio.Play(takeoff_sound);
            ClearEquip();
            ShowEquip();
        }
        //inputKey = true;
        go_OOC.SetActive(false);
    }
}
