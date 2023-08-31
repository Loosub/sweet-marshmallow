using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStart : MonoBehaviour
{
    public AudioSource btnSound;

    //게임 시작 버튼 클릭시(Touch to Start)
    public void StartBtn()
    {
        SceneManager.LoadScene("Ingame");
        btnSound.Play();
        Time.timeScale = 1.0f;
    }
}
