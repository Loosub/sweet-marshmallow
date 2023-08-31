using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    public AudioSource btnSound;

    //���� ���� ��ư Ŭ����(Touch to Start)
    public void StartBtn()
    {
        SceneManager.LoadScene("Ingame");
        btnSound.Play();
        Time.timeScale = 1.0f;
    }
}
