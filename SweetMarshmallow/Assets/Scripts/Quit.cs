using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 어플리케이션 뒤로가기 연속 2회 클릭시 게임종료
public class Quit : MonoBehaviour
{
    int ClickCount = 0; //클릭 횟수

    void Update()
    {
        // 뒤로가기 버튼 클릭시
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ClickCount++; // 클릭 횟수 1증가
            if (!IsInvoking("DoubleClick")) // DoubleClick 함수 실행 도중이 아닐 경우
            {
                Invoke("DoubleClick", 1.0f); // DoubleClick 함수 1초 후 실행
            };
        }
        else if (ClickCount == 2) { // 뒤로가기 클릭 횟수가 2일 경우
            CancelInvoke("DoubleClick"); // DoubleClick 함수 실행 취소
            Application.Quit(); // 어플리케이션 종료
        }
    }

    // 더블클릭 확인
    void DoubleClick()
    {
        ClickCount = 0;
    }
}
