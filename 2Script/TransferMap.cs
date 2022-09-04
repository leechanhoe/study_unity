using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransferMap : MonoBehaviour
{

    public string transferMapName;
    PlayerManager thePlayer;
    CameraManager theCamera;

    public Transform target;
    public BoxCollider2D targetBound;

    public bool flag = false;
    private FadeManager theFade;
    private OrderManager theOrder;

    public BoxCollider2D[] bounds;

    public GameObject beforeCanvas;
    public GameObject afterCanvas;
    public bool horizonMove;
    /*
        public Animator anim_1;
        public Animator anim_2;

        public int door_count;

        [Tooltip("UP, DOWN, LEFT, RIGHT")]
        public string direction; // 캐릭이 바라보는 방향
        private Vector2 vector; // getfloat("DirX")

        [Tooltip("문이 열린다:true, 문이 없으면 false")]
        public bool door = false; // 문이 있냐 없냐 체크
        */
    void Start()
    {   if (!flag)
            theCamera = FindObjectOfType<CameraManager>();
        thePlayer = FindObjectOfType<PlayerManager>();
        theFade = FindObjectOfType<FadeManager>();
        theOrder = FindObjectOfType<OrderManager>();
    }

    public void GoField() // 로비에서 필드로 
    {
        thePlayer.horizonMove = false;
        StartCoroutine(TransperCoroutine4(beforeCanvas, null, true));
    }
    public void GoOtherMap() // 로비에서 학교로
    {
        thePlayer.horizonMove = horizonMove;
        StartCoroutine(TransperCoroutine4(beforeCanvas));
    }
    public void TransperCanvas() // 버튼용(버튼은 인자에 게임오브젝트가 전달이 안됌;)
    {
        StartCoroutine(TransperCoroutine3(beforeCanvas, afterCanvas));
    }

    public void TransperCanvas2(GameObject before, GameObject after)
    { // 이건 인자전달해서 외부 스크립트에서 쉽게 사용 가능한것
        StartCoroutine(TransperCoroutine3(before, after));
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player")
        {
            if (flag)
            {
                thePlayer.currentMapName = transferMapName;
                SceneManager.LoadScene(transferMapName);
            }
            else
            {
                StartCoroutine(TransperCoroutine());
            }
        }

    }

    IEnumerator TransperCoroutine()
    {
        //박스콜라이더 닿아야 사용되는 코루틴
        //theOrder.PreLoadCharacter();
        theOrder.NotMove();
        theFade.FadeOut();

        yield return new WaitForSeconds(0.5f);

        //theOrder.SetUnTransparent("Player");

        yield return new WaitForSeconds(1f);
        thePlayer.currentMapName = transferMapName;
        thePlayer.transform.position = target.transform.position;
        theCamera.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, theCamera.transform.position.z);
        theCamera.SetBound(targetBound);
        theFade.FadeIn();
        yield return new WaitForSeconds(0.5f);
        theOrder.Move();
    }

    IEnumerator TransperCoroutine2()
    
    { // 박스 콜라이더에 안 닿아도 걍 이동할수 있게 하는 함수 (캐릭터이동)
        //theOrder.PreLoadCharacter();
        theOrder.NotMove();
        theFade.FadeOutIn();

        yield return new WaitUntil(() => theFade.completedBlack);
        thePlayer.transform.position = target.transform.position; 
        theCamera.SetBound(targetBound);
        theCamera.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, theCamera.transform.position.z);
        thePlayer.currentMapName = transferMapName;

        theFade.FadeIn();

        yield return new WaitForSeconds(0.5f);
        theOrder.Move();
    }

    IEnumerator TransperCoroutine3(GameObject _before = null, GameObject _after = null, bool canMove = false) 
    { // 뒤에 배경만 바꾸기 (캔버스만이동)
        theOrder.NotMove();
        theFade.FadeOutIn(canMove:false);
        yield return new WaitUntil(() => theFade.completedBlack);
        if (_before != null)
            _before.SetActive(false);
        if(_after != null)
            _after.SetActive(true);
        if (canMove)
            theOrder.Move();
    }

    IEnumerator TransperCoroutine4(GameObject _before = null, GameObject _after = null, bool canMove = true)
    { // 이건 캐릭이랑 캔버스 둘다 이동해야될 필요 있을때
        theOrder.NotMove();
        theFade.FadeOutIn();
        yield return new WaitUntil(() => theFade.completedBlack);

        thePlayer.transform.position = target.transform.position; // 캐릭터, 카메라이동
        theCamera.SetBound(targetBound);
        theCamera.transform.position = new Vector3(target.transform.position.x, target.transform.position.y, theCamera.transform.position.z);
        thePlayer.currentMapName = transferMapName;

        if (_before != null) // 캔버스이동
            _before.SetActive(false);
        if (_after != null)
            _after.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        if (canMove)
            theOrder.Move();
    }
}
