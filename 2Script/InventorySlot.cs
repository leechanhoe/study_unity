using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class InventorySlot : MonoBehaviour
{
    public Item item;
    public Image icon;
    public Text itemName_Text;
    public Text itemCount_Text;
    public GameObject selected_Item;
    public GameObject TouchButton;
    InventorySlot[] slots;

    Inventory inventory;
    AudioManager theAudio;
    DatabaseManager theDatabase;
    Equipment theEquip;

    private void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        theAudio = FindObjectOfType<AudioManager>();
        theDatabase = FindObjectOfType<DatabaseManager>();
        theEquip = FindObjectOfType<Equipment>();
        slots = FindObjectsOfType<InventorySlot>();
        
    }

    public void Additem(Item _item)
    {
        item = _item;
        itemName_Text.text = _item.itemName;
        icon.sprite = _item.itemIcon;
        if(Item.ItemType.Use == _item.itemType)
        {
            if (_item.itemCount > 0)
                itemCount_Text.text = "x " + _item.itemCount.ToString();
            else
                itemCount_Text.text = "";
        }
    }

    public void RemoveItem()
    {
        itemCount_Text.text = "";
        itemName_Text.text = "";
        icon.sprite = null;
    }

    public void TouchItem() // 아이템 아이콘 터치시 
    {
        for (int i = 0; i < slots.Length; i++)
            slots[i].selected_Item.SetActive(false); // 일단 선택된 탭 흐려지는거 초기화
        selected_Item.SetActive(true); // 터치된 애만 흐려지는 효과
        inventory.selectedItem = item;
        inventory.TouchItem2();
    }
}
