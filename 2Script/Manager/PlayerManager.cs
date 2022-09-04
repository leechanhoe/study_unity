using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MovingObject
{
    static public PlayerManager instance;

    public string currentMapName;
    public string currentSceneName;

    public string walkSound_1;
    public string walkSound_2;
    public string walkSound_3;
    public string walkSound_4;

    private AudioManager theAudio;
    private SaveNLoad thesaveNLoad;

    bool canMove = true;

    public bool notMove = false;
    
    public bool horizonMove = true;
    public GameManager2 manager2;
    GameObject scanObject;
    public float PlayerSpeed;
    public float h;
    Rigidbody2D rigid;
    Vector2 dirVec;

    void Start()
    {
        // 맵이동하면 캐릭터 중복으로 생기는거 방지
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            rigid = GetComponent<Rigidbody2D>();
            queue = new Queue<string>();
            DontDestroyOnLoad(this.gameObject);
            animator = GetComponent<Animator>();
            boxCollider = GetComponent<BoxCollider2D>();
            theAudio = FindObjectOfType<AudioManager>();
            thesaveNLoad = FindObjectOfType<SaveNLoad>();
            instance = this;
        }
    }

    IEnumerator MoveCoroutine()
    {
        while (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0 && !notMove)
        {

            vector.Set(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), transform.position.z);
            //버튼 두개 동시에 누를때 생기는 오류막음
            if (vector.x != 0)
                vector.y = 0;

            animator.SetFloat("DirX", vector.x);
            animator.SetFloat("DirY", vector.y);

            bool checkCollisionFlag = base.CheckCollision();
            if (checkCollisionFlag)
                break;

            animator.SetBool("Walking", true);


            int temp = Random.Range(1, 4);
            switch (temp)
            {
                case 1:
                    theAudio.Play(walkSound_1);
                    break;
                case 2:
                    theAudio.Play(walkSound_2);
                    break;
                case 3:
                    theAudio.Play(walkSound_3);
                    break;
                case 4:
                    theAudio.Play(walkSound_4);
                    break;
            }
            //theAudio.SetVolumn(walkSound_2, 0.5f); 볼륨수정
            //여기가 앞에 미리 박스콜라이더 깔아서 장애물 있으면 못가게 하는거
            //boxCollider.offset = new Vector2(vector.x * 0.8f * speed * walkCount, vector.y * 0.8f * speed * walkCount);

            //1칸씩 움직이게 하기위함
            while (currentWalkCount < walkCount)
            {
                if (vector.x != 0)
                {
                    transform.Translate(vector.x * speed, 0, 0);
                }
                else if (vector.y != 0)
                {
                    transform.Translate(0, vector.y * speed, 0);
                }
                currentWalkCount++;
                //if (currentWalkCount == 6)
                    //boxCollider.offset = Vector2.zero;
                yield return new WaitForSeconds(0.01f);
            }
            currentWalkCount = 0;
        }
        animator.SetBool("Walking", false);
        canMove = true;
    }

    public void ButtonDown(string type)
    {
        switch (type)
        {
            case "A":
                if (scanObject != null)
                    manager2.Action(scanObject);
                break;
        }
    }

    void Update()
    {
        if (canMove && !notMove && horizonMove)
            h = Input.GetAxisRaw("Horizontal");

        //한번에 한칸씩만 움직이게 하기위함
        if (canMove && !notMove  && !horizonMove)
        {
            if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            {
                canMove = false;
                StartCoroutine(MoveCoroutine());
            }
        }
        if (h == -1)
            dirVec = Vector2.left;
        else if (h == 1)
            dirVec = Vector2.right;

        if (Input.GetButtonDown("Jump") && scanObject != null) // 대화
            manager2.Action(scanObject);
    }

    void FixedUpdate()
    {
        Vector2 moveVec = new Vector2(h, 0f);

        rigid.velocity = moveVec * PlayerSpeed;

        //Ray 물체탐색
        Debug.DrawRay(rigid.position, dirVec * 0.7f, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, dirVec, 0.7f, LayerMask.GetMask("Object"));

        if (rayHit.collider != null)
            scanObject = rayHit.collider.gameObject;
        else
            scanObject = null;
    }
}
