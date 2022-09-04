using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubMenu : MonoBehaviour
{
    Animator anim;
    static public SubMenu instance;

    public GameObject menuButton;
    public GameObject[] subMenu;
    AudioManager audioManager;
    public string soundEffect;

    private void Start()
    {
        anim = GetComponent<Animator>();
        audioManager = FindObjectOfType<AudioManager>();
    }

    public void MenuAction()
    {
        if (anim.GetBool("Opening")) // 메뉴가 열려있으면
            MenuClose(); // 메뉴닫음
        else // 아니면 반대
            MenuOpen();
    }
    public void MenuOpen()
    {
        //audioManager.Play(soundEffect);
        anim.SetBool("Opening", true);
        for (int i = 0; i < subMenu.Length; i++)
            subMenu[i].SetActive(true);
    }

    public void MenuClose()
    {
        //audioManager.Play(soundEffect);
        StartCoroutine(MenuCloseC());
    }

    IEnumerator MenuCloseC()
    {
        anim.SetBool("Opening", false);
        yield return new WaitForSeconds(0.2f); // 서브메뉴들의 애니메이션이 다 끝난 다음에 서브메뉴 비활성화
        for (int i = 0; i < subMenu.Length; i++)
            subMenu[i].SetActive(false);
    }
}
