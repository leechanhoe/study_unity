using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public int questId;
    public int questActionIndex;
    public GameObject[] questObject;

    Dictionary<int, QuestData> questlist;

    void Awake()
    {
        questlist = new Dictionary<int, QuestData>();
        GenerateData();
    }


    void GenerateData()
    {
        questlist.Add(10, new QuestData("마을 사람들과 대화하기", new int[] {0, 2000}));
        questlist.Add(20, new QuestData("루도의 동전 찾아주기", new int[] { 5000, 2000 }));
        questlist.Add(30, new QuestData("퀘스트 올 클리어", new int[] { 0 }));
    }

    public int GetQuestTalkIndex(int id) // NPC id를 받고 퀘스트번호를 반환하는 함수
    {
        return questId + questActionIndex;
    }

    public string CheckQuest(int id)
    {
        //다음 대화 타겟
        if(id == questlist[questId].npcId[questActionIndex])
            questActionIndex++;

        //퀘스트 아이템 컨트롤
        ControlObject();

        //퀘스트 완료 & 다음 퀘스트
        if (questActionIndex == questlist[questId].npcId.Length)
            NextQuest();

        //퀘스트 이름
        return questlist[questId].questName;
    }

    public string CheckQuest()
    {
        //퀘스트 이름
        return questlist[questId].questName;
    }

    void NextQuest()
    {
        questId += 10;
        questActionIndex = 0;
    }

    public void ControlObject()
    {
        switch(questId)
        {
            case 10:
                if (questActionIndex == 2)
                    questObject[0].SetActive(true);
                break;
            case 20:
                if(questActionIndex == 0)
                    questObject[0].SetActive(true);
                else if (questActionIndex == 1)
                    questObject[0].SetActive(false);
                break;
        }
    }
}
