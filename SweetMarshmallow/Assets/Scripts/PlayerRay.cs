using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRay : MonoBehaviour
{
    public Rigidbody2D rigid;

    Animation ani; // 플레이어 오른쪽, 왼쪽 회전
    Animation ani_pink; // 핑크색 버튼 게임설명 화살표
    Animation ani_sky; // 하늘색 버튼 게임설명 화살표

    public GameObject Btn;

    public Animator ani_t; // timeover

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animation>();
        ani_pink = GetComponent<Animation>();
        ani_sky = GetComponent<Animation>();
    }

    private void Update()
    {
        // 게임시작시
        if (GameManager.instance.start == true)
        {
            GameObject child_pinkArrow = transform.Find("ColorBtnArrow_pink").gameObject; // 플레이어 오브젝트 하위에 있는 자식 오브젝트 찾기
            child_pinkArrow.SetActive(false); // 컬러버튼 가리키는 화살표(게임설명) 숨기기
            GameObject child_skyArrow = transform.Find("ColorBtnArrow_sky").gameObject;
            child_skyArrow.SetActive(false);
        }

        // 타이머가 4초 남았을 경우 sweat(땀) 애니메이션 실행
        if (GameManager.instance.deadTime <= 4)
        {
            ani_t.SetBool("timeover", true);
        }
        else
        {
            ani_t.SetBool("timeover", false);
        }
    }

    private void FixedUpdate() 
    {
        // 플레이어 레이캐스트 방향 확인용
        Debug.DrawRay(rigid.position, new Vector3(0, 1.5f, 0), new Color(0, 1, 0), 1f);
        Debug.DrawRay(rigid.position, new Vector3(1.5f, 0, 0), new Color(1, 0, 0), 1f);
        Debug.DrawRay(rigid.position, new Vector3(-1.5f, 0, 0), new Color(0, 0, 1), 1f);

        RaycastHit2D rayhit_Up = Physics2D.Raycast(rigid.position, new Vector3(0, 1, 0), 1.5f); // 리지드 바디 기준으로 위쪽으로 1만큼의 거리까지 raycast발사
        RaycastHit2D rayhit_right = Physics2D.Raycast(rigid.position, new Vector3(1, 0, 0), 1.5f); // 오른쪽으로 raycast발사
        RaycastHit2D rayhit_left = Physics2D.Raycast(rigid.position, new Vector3(-1, 0, 0), 1.5f); // 왼쪽으로 raycast발사

        // 게임시작 전 플레이어 앞에 오브젝트가 있을 경우 
        if (rayhit_Up.collider != null && GameManager.instance.start == false)
        {
            // 플레이어 앞에 핑크색 오브젝트가 있을 경우
            if (rayhit_Up.collider.tag == "Pink")
            {
                ani_pink.Play("pinkArrow"); // 게임설명 화살표 애니메이션 실행
                GameObject child_skyArrow = transform.Find("ColorBtnArrow_sky").gameObject;
                child_skyArrow.SetActive(false);
            }
            // 플레이어 앞에 하늘색 오브젝트가 있을 경우
            if (rayhit_Up.collider.tag == "Sky")
            {
                ani_sky.Play("skyArrow");
                GameObject child_pinkArrow = transform.Find("ColorBtnArrow_pink").gameObject; // 플레이어 오브젝트 하위에 있는 자식 오브젝트 찾기
                child_pinkArrow.SetActive(false);
            }
        }
        
        // 정면에 블럭이 존재할때 
        if (rayhit_Up.collider != null)
        {
            // 회전
            if (this.gameObject.transform.eulerAngles.z == 270)
            {
                ani.Play("right_up");
            }
            else if (this.gameObject.transform.eulerAngles.z == 90)
            {
                ani.Play("left_up");
            }
        }
        // 오른쪽에 블럭이 존재할 때 
        if (rayhit_right.collider != null) 
        {
            ani.Play("right");
            Debug.Log("오른쪽 회전");
        }
        // 왼쪽에 블럭이 존재할 때 
        if (rayhit_left.collider != null)
        {
            ani.Play("left");
            Debug.Log("왼쪽 회전");
        }
    }

    // sweat 애니메이션의 Gameover 이벤트 함수
    public void Gameover()
    {
        ani_t.SetBool("Gameover", true); // died 애니메이션 재생

        GameManager.instance.pinkBtn.interactable = false; // 색상버튼 클릭 불가능
        GameManager.instance.skyBtn.interactable = false;
    }

    // died 애니메이션의 Sound 이벤트 함수
    public void Sound()
    {
        GameManager.instance.bgmSound.Stop(); // 인게임 bgm 정지
        GameManager.instance.gameoverSound.Play(); // 게임오버 사운드 재생
    }

    // died 애니메이션의 Scoreboard 이벤트 함수
    public void Scoreboard()
    {
        GameManager.instance.panel_GameOver.SetActive(true); // GameOver 패널 On
    }
}
