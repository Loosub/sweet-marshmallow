using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRay : MonoBehaviour
{
    public Rigidbody2D rigid;

    Animation ani; // �÷��̾� ������, ���� ȸ��
    Animation ani_pink; // ��ũ�� ��ư ���Ӽ��� ȭ��ǥ
    Animation ani_sky; // �ϴû� ��ư ���Ӽ��� ȭ��ǥ

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
        // ���ӽ��۽�
        if (GameManager.instance.start == true)
        {
            GameObject child_pinkArrow = transform.Find("ColorBtnArrow_pink").gameObject; // �÷��̾� ������Ʈ ������ �ִ� �ڽ� ������Ʈ ã��
            child_pinkArrow.SetActive(false); // �÷���ư ����Ű�� ȭ��ǥ(���Ӽ���) �����
            GameObject child_skyArrow = transform.Find("ColorBtnArrow_sky").gameObject;
            child_skyArrow.SetActive(false);
        }

        // Ÿ�̸Ӱ� 4�� ������ ��� sweat(��) �ִϸ��̼� ����
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
        // �÷��̾� ����ĳ��Ʈ ���� Ȯ�ο�
        Debug.DrawRay(rigid.position, new Vector3(0, 1.5f, 0), new Color(0, 1, 0), 1f);
        Debug.DrawRay(rigid.position, new Vector3(1.5f, 0, 0), new Color(1, 0, 0), 1f);
        Debug.DrawRay(rigid.position, new Vector3(-1.5f, 0, 0), new Color(0, 0, 1), 1f);

        RaycastHit2D rayhit_Up = Physics2D.Raycast(rigid.position, new Vector3(0, 1, 0), 1.5f); // ������ �ٵ� �������� �������� 1��ŭ�� �Ÿ����� raycast�߻�
        RaycastHit2D rayhit_right = Physics2D.Raycast(rigid.position, new Vector3(1, 0, 0), 1.5f); // ���������� raycast�߻�
        RaycastHit2D rayhit_left = Physics2D.Raycast(rigid.position, new Vector3(-1, 0, 0), 1.5f); // �������� raycast�߻�

        // ���ӽ��� �� �÷��̾� �տ� ������Ʈ�� ���� ��� 
        if (rayhit_Up.collider != null && GameManager.instance.start == false)
        {
            // �÷��̾� �տ� ��ũ�� ������Ʈ�� ���� ���
            if (rayhit_Up.collider.tag == "Pink")
            {
                ani_pink.Play("pinkArrow"); // ���Ӽ��� ȭ��ǥ �ִϸ��̼� ����
                GameObject child_skyArrow = transform.Find("ColorBtnArrow_sky").gameObject;
                child_skyArrow.SetActive(false);
            }
            // �÷��̾� �տ� �ϴû� ������Ʈ�� ���� ���
            if (rayhit_Up.collider.tag == "Sky")
            {
                ani_sky.Play("skyArrow");
                GameObject child_pinkArrow = transform.Find("ColorBtnArrow_pink").gameObject; // �÷��̾� ������Ʈ ������ �ִ� �ڽ� ������Ʈ ã��
                child_pinkArrow.SetActive(false);
            }
        }
        
        // ���鿡 ���� �����Ҷ� 
        if (rayhit_Up.collider != null)
        {
            // ȸ��
            if (this.gameObject.transform.eulerAngles.z == 270)
            {
                ani.Play("right_up");
            }
            else if (this.gameObject.transform.eulerAngles.z == 90)
            {
                ani.Play("left_up");
            }
        }
        // �����ʿ� ���� ������ �� 
        if (rayhit_right.collider != null) 
        {
            ani.Play("right");
            Debug.Log("������ ȸ��");
        }
        // ���ʿ� ���� ������ �� 
        if (rayhit_left.collider != null)
        {
            ani.Play("left");
            Debug.Log("���� ȸ��");
        }
    }

    // sweat �ִϸ��̼��� Gameover �̺�Ʈ �Լ�
    public void Gameover()
    {
        ani_t.SetBool("Gameover", true); // died �ִϸ��̼� ���

        GameManager.instance.pinkBtn.interactable = false; // �����ư Ŭ�� �Ұ���
        GameManager.instance.skyBtn.interactable = false;
    }

    // died �ִϸ��̼��� Sound �̺�Ʈ �Լ�
    public void Sound()
    {
        GameManager.instance.bgmSound.Stop(); // �ΰ��� bgm ����
        GameManager.instance.gameoverSound.Play(); // ���ӿ��� ���� ���
    }

    // died �ִϸ��̼��� Scoreboard �̺�Ʈ �Լ�
    public void Scoreboard()
    {
        GameManager.instance.panel_GameOver.SetActive(true); // GameOver �г� On
    }
}
