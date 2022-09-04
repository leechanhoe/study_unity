using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    private DatabaseManager theDatabase;
    private OrderManager theOrder;
    private AudioManager theAudio;
    private Equipment theEquip;

    public string cancel_sound;
    public string open_sound;
    public string beep_sound; // 잘못된 행동 했을 시 사운드
    public string touch_sound;
    public string use_sound;

    private InventorySlot[] slots;

    public List<Item> inventoryItemList; // 플레이어가 소지한 아이템 리스트
    private List<Item> inventoryTabList; // 선택된 탭에 따라 다르게 보여질 아이템 리스트

    public Text Description_Text; // 부연설명

    public Transform tf; // slot 부모객체 (grid slot)

    public GameObject go; // 인벤토리 활성화 비활성화
    public GameObject[] selectedTabImage; // 소모 장비 이런 탭들
    public GameObject useItemButton; // 아이템 사용버튼
    public Text useItemText; // 사용 텍스트
    public GameObject DumpItemButton; // 아이템 버리기 버튼
    public Text DumpItemText; // 버리기 텍스트

    public Item selectedItem; // 선택된 아이템
    public int selectedTab; // 선택된 텝

    public GameObject prefab_floating_Text;

    private WaitForSeconds waitTime = new WaitForSeconds(0.01f);
    


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        theAudio = FindObjectOfType<AudioManager>();
        theOrder = FindObjectOfType<OrderManager>();
        theDatabase = FindObjectOfType<DatabaseManager>();
        theEquip = FindObjectOfType<Equipment>();

        inventoryItemList = new List<Item>();
        inventoryTabList = new List<Item>();
        slots = tf.GetComponentsInChildren<InventorySlot>(); // grid slot 밑에 있는 슬롯들이 다 얘한테 들어감. tf가 그리드 슬롯

        inventoryItemList.Add(new Item(10001, "빨간 포션", "체력을 50 회복시켜주는 마법의 물약", Item.ItemType.Use));
        inventoryItemList.Add(new Item(10002, "파란 포션", "마나를 15 회복시켜주는 마법의 물약", Item.ItemType.Use));
        inventoryItemList.Add(new Item(10003, "농축 빨간 포션", "체력을 350 회복시켜주는 마법의 농축 물약", Item.ItemType.Use));
        inventoryItemList.Add(new Item(10004, "농축 파란 포션", "마나를 80 회복시켜주는 마법의  농축 물약", Item.ItemType.Use));
        inventoryItemList.Add(new Item(11001, "랜덤 상자", "랜덤으로 포션이 나온다. 낮은 확률로 꽝", Item.ItemType.Use));
        inventoryItemList.Add(new Item(20001, "짧은 검", "기본적인 용사의 검", Item.ItemType.Equip, 3));
        inventoryItemList.Add(new Item(20301, "사파이어 반지", "1초에 hp 1을 회복시켜주는 마법 반지", Item.ItemType.Equip, 0, 0, 1));
        inventoryItemList.Add(new Item(30001, "고대 유물의 조각 1", "반으로 쪼개진 고대 유물의 파편", Item.ItemType.Quest));
        inventoryItemList.Add(new Item(30002, "고대 유물의 조각 2", "반으로 쪼개진 고대 유물의 파편", Item.ItemType.Quest));
        inventoryItemList.Add(new Item(30003, "고대 유물", "고대 유적에 잠들어있던 고대의 유물", Item.ItemType.Quest));
    }

    public void ShowInventory()
    {
        theAudio.Play(open_sound);
        theOrder.NotMove();
        go.SetActive(true);
        ShowTab();
    }

    public void CloseInventory()
    {
        theAudio.Play(cancel_sound);
        StopAllCoroutines();
        go.SetActive(false);
        theOrder.Move();
    }

    public List<Item> SaveItem()
    {
        return inventoryItemList;
    }

    public void LoadItem(List<Item> _itemList)
    {
        inventoryItemList = _itemList;
    }

    public void EquipToInventory(Item _item)
    {
        inventoryItemList.Add(_item);
    }
    public void GetAnItem(int _itemID, int _count = 1)
    {
        for(int i = 0;i < theDatabase.itemList.Count;i++) // 데이터베이스 아이템 검색
        {
            if (_itemID == theDatabase.itemList[i].itemID) // 데이터베이스에 아이템 발견
            {
                /*
                var clone = Instantiate(prefab_floating_Text, PlayerManager.instance.transform.position, Quaternion.Euler(Vector3.zero)); // 정확한 형식을 모를 떄 var , instantiate 는 프리펩 생성해서 클론에 넘
                // instantiate(대상,위치,각도)
                clone.GetComponent<FloatingText>().text.text = theDatabase.itemList[i].itemName + " " + _count + "개 획득";
                clone.transform.SetParent(this.transform);*/

                for(int j = 0;j < inventoryItemList.Count;j++) //소지품에 같은 아이템이 있는지 검색
                {
                    if(inventoryItemList[j].itemID == _itemID) // 소지품에 같은 템이 있으니 개수만 증감
                    {   if (inventoryItemList[j].itemType == Item.ItemType.Use)
                        {
                            inventoryItemList[j].itemCount += _count;
                            return;
                        }
                        else
                        {
                            inventoryItemList.Add(theDatabase.itemList[i]);
                        }
                        return;
                    }
                }
                inventoryItemList.Add(theDatabase.itemList[i]); // 소지품에 해당 아이템 추가.
                inventoryItemList[inventoryItemList.Count - 1].itemCount = _count;
                return;
            }
        }
        Debug.LogError("데이터베이스에 해당 ID값을 가진 아이템이 존재하지 않습니다."); // 데이터베이스에 itemID 없음
    }

    public void ShowTab() // 탭 활성화
    {
        Description_Text.text = "";
        useItemButton.SetActive(false);
        DumpItemButton.SetActive(false);
        for (int i = 0; i < slots.Length; i++) // 선택된 탭 흐리게하는 효과 일단 다 헤제
            slots[i].selected_Item.SetActive(false);
        RemoveSlot();
        SelectedTab();
        ShowItem();
    }

    public void RemoveSlot() // 인벤토리 슬롯 초기화
    {
        for(int i = 0;i < slots.Length;i++)
        {   
            slots[i].RemoveItem();
            slots[i].gameObject.SetActive(false);
        }
    }

    public void SelectedTab() // 선택된 탭을 제외하고 다른 모든 텝의 컬러 알파값 0으로 조정.
    {
        Color color = selectedTabImage[selectedTab].GetComponent<Image>().color;
        color.a = 0f;
        for(int i = 0;i < selectedTabImage.Length;i++)
        {
            selectedTabImage[i].GetComponent<Image>().color = color;
        }
        color.a = 0.5f;
        selectedTabImage[selectedTab].GetComponent<Image>().color = color;
    }

    IEnumerator SelectedTabEffectCoroutine() //선택된 탭 반짝임 효과
    {
        //while(tabActivated)
        //{
            Color color = selectedTabImage[selectedTab].GetComponent<Image>().color;
            while(color.a < 0.5f)
            {
                color.a += 0.03f;
                selectedTabImage[selectedTab].GetComponent<Image>().color = color;
                yield return waitTime;
            }
            while (color.a > 0f)
            {
                color.a -= 0.03f;
                selectedTabImage[selectedTab].GetComponent<Image>().color = color;
                yield return waitTime;
            }

            yield return new WaitForSeconds(0.3f);
        //}
    }

    public void ShowItem() // 아이템 활성화 (inventoryTabList에 조건에 맞는 아이템들만 넣어주고, 인벤토리 슬롯에 출력
    {
        inventoryTabList.Clear();
        RemoveSlot();

        switch (selectedTab) // 탭에 따른 아이템 분류, 그것을 인벤토리 탭 리스트에 추가
        {
            case 0:
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.Use == inventoryItemList[i].itemType)
                        inventoryTabList.Add(inventoryItemList[i]);
                }
                break;
            case 1:
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.Equip == inventoryItemList[i].itemType)
                        inventoryTabList.Add(inventoryItemList[i]);
                }
                break;
            case 2:
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.Quest == inventoryItemList[i].itemType)
                        inventoryTabList.Add(inventoryItemList[i]);
                }
                break;
            case 3:
                for (int i = 0; i < inventoryItemList.Count; i++)
                {
                    if (Item.ItemType.ETC == inventoryItemList[i].itemType)
                        inventoryTabList.Add(inventoryItemList[i]);
                }
                break;
        }
        for (int i = 0;i < inventoryTabList.Count; i++) // 인벤토리 탭 리스트의 내용을, 인벤토리 슬롯에 추가
        {
            slots[i].gameObject.SetActive(true);
            slots[i].Additem(inventoryTabList[i]);
        }
    }

    public void ChooseTab(int _itemType) // 모바일 버튼 변환, 소비 0 장비 1 퀘스트 2 기타 3
    {
        theAudio.Play(touch_sound);
        useItemButton.SetActive(false);
        DumpItemButton.SetActive(false);
        switch (_itemType)
        {
            case 0:
                selectedTab = 0;
                ShowTab();
                break;
            case 1:
                selectedTab = 1;
                ShowTab();
                break;
            case 2:
                selectedTab = 2;
                ShowTab();
                break;
            case 3:
                selectedTab = 3;
                ShowTab();
                break;
        }
    }
    public void TouchItem2() // 아이템 아이콘 터치시 
    {
        theAudio.Play(touch_sound);
        Description_Text.text = selectedItem.itemDescription;
        useItemButton.SetActive(true);
        DumpItemButton.SetActive(true);
        if (selectedTab == 0) // 소비창
            useItemText.text = "사용";
        else if (selectedTab == 1) // 장비창
            useItemText.text = "장착";
        else if (selectedTab == 2) // 퀘스트
        {
            useItemButton.SetActive(false);
            DumpItemButton.SetActive(false);
        }
        else if (selectedTab == 3) // 기타창
            useItemButton.SetActive(false);
    }

    public void ItemUse() // 아이템 사용버튼 누를시
    {
        theAudio.Play(use_sound);
        if (selectedTab == 0) // 소비템 소모
        {
            theDatabase.UseItem(selectedItem.itemID);
            if (selectedItem.itemCount > 1)
                selectedItem.itemCount--;
            else
                inventoryItemList.Remove(selectedItem);

            //theAudio.Play() 아이템 먹는 소리 출력
            ShowItem();
        }
        else if (selectedTab == 1) // 아이템 장착
        {
            theEquip.EquipItem(selectedItem);
            inventoryItemList.Remove(selectedItem);
            ShowItem();
        }
        useItemButton.SetActive(false);
        DumpItemButton.SetActive(false);
        Description_Text.text = "";
        for (int i = 0; i < slots.Length; i++) // 선택된 탭 흐리게하는 효과 일단 다 헤제
            slots[i].selected_Item.SetActive(false);
    }
}
