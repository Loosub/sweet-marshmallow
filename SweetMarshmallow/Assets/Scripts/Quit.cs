using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ���ø����̼� �ڷΰ��� ���� 2ȸ Ŭ���� ��������
public class Quit : MonoBehaviour
{
    int ClickCount = 0; //Ŭ�� Ƚ��

    void Update()
    {
        // �ڷΰ��� ��ư Ŭ����
        if (Input.GetKeyDown(KeyCode.Escape)) {
            ClickCount++; // Ŭ�� Ƚ�� 1����
            if (!IsInvoking("DoubleClick")) // DoubleClick �Լ� ���� ������ �ƴ� ���
            {
                Invoke("DoubleClick", 1.0f); // DoubleClick �Լ� 1�� �� ����
            };
        }
        else if (ClickCount == 2) { // �ڷΰ��� Ŭ�� Ƚ���� 2�� ���
            CancelInvoke("DoubleClick"); // DoubleClick �Լ� ���� ���
            Application.Quit(); // ���ø����̼� ����
        }
    }

    // ����Ŭ�� Ȯ��
    void DoubleClick()
    {
        ClickCount = 0;
    }
}
