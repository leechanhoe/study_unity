using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    float h, v;
    bool isHorizonMove;
    public float PlayerSpeed;
    

    Vector2 dirVec;
    GameObject scanObject;

    Rigidbody2D rigid;
    Animator anim;

    //모바일 키 값
    int up_Value;
    int down_Value;
    int left_Value;
    int right_Value;
    bool up_Down;
    bool down_Down;
    bool left_Down;
    bool right_Down;
    bool up_Up;
    bool down_Up;
    bool left_Up;
    bool right_Up;


    public GameManager2 manager;
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        //기본 움직임
        //PC                                                        모바일 버튼
        h = manager.isAction ? 0 : Input.GetAxisRaw("Horizontal") + right_Value + left_Value;
        //v = manager.isAction ? 0 : Input.GetAxisRaw("Vertical") + up_Value + down_Value;

        //대각선 이동 제한
        //PC                                                                    //모바일
        bool hDown = manager.isAction ? false : Input.GetButtonDown("Horizontal") || right_Down || left_Down;
        bool vDown = manager.isAction ? false : Input.GetButtonDown("Vertical") || up_Down || down_Down;
        bool hUp = manager.isAction ? false : Input.GetButtonUp("Horizontal") || right_Up || left_Up;
        bool vUp = manager.isAction ? false : Input.GetButtonUp("Vertical") || up_Up || down_Up;

        if (hDown)
            isHorizonMove = true;
        else if (vDown)
            isHorizonMove = false;
        else if (hUp || vUp)
            isHorizonMove = h != 0;

        //이거해야 걷는애니나옴  
        /*
        if (anim.GetInteger("hAxisRaw") != h)
        {
            anim.SetBool("isChange", true);
            anim.SetInteger("hAxisRaw", (int)h);
        }
        else if (anim.GetInteger("vAxisRaw") != v)
        {
            anim.SetBool("isChange", true);
            anim.SetInteger("vAxisRaw", (int)v);
        }
        else
            anim.SetBool("isChange", false);
            */
        //단위벡터
        if (vDown && v == 1)
            dirVec = Vector2.up;
        else if (vDown && v == -1)
            dirVec = Vector2.down;
        else if (hDown && h == -1)
            dirVec = Vector2.left;
        else if (hDown && h == 1)
            dirVec = Vector2.right;

        //물체스캔
        if (Input.GetButtonDown("Jump") && scanObject != null)
            manager.Action(scanObject);

        //모바일 다운업부울값 초기화(그래야 매 프레임마다 움직일수있음)
        up_Down = false;
        down_Down = false;
        left_Down = false;
        right_Down = false;
        up_Up = false;
        down_Up = false;
        left_Up = false;
        right_Up = false;
    }

    void FixedUpdate()
    {
        //대각선 이동 제한
        Vector2 moveVec = isHorizonMove ? new Vector2(h, 0) : new Vector2(0, v);

        rigid.velocity = moveVec * PlayerSpeed;

        //Ray 물체탐색
        Debug.DrawRay(rigid.position, dirVec * 0.7f, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position,dirVec, 0.7f, LayerMask.GetMask("Object"));

        if (rayHit.collider != null)
            scanObject = rayHit.collider.gameObject;
        else
            scanObject = null;
    }

    public void ButtonDown(string type)
    {
        switch (type)
        {
            case "U":
                up_Value = 1;
                up_Down = true;
                break;
            case "D":
                down_Value = -1;
                down_Down = true;
                break;
            case "L":
                left_Value = -1;
                left_Down = true;
                break;
            case "R":
                right_Value = 1;
                right_Down = true;
                break;
            case "A":
                if (scanObject != null)
                    manager.Action(scanObject);
                break;
            case "C":
                //manager.SubMenuActive();
                break;
        }
    }

    public void ButtonUp(string type)
    {
        switch (type)
        {
            case "U":
                up_Value = 0;
                up_Up = true;
                break;
            case "D":
                down_Value = 0;
                down_Up = true;
                break;
            case "L":
                left_Value = 0;
                left_Up = true;
                break;
            case "R":
                right_Value = 0;
                right_Up = true;
                break;
        }
    }
}
