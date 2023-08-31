using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // �ٸ� ��ũ��Ʈ���� ���ӸŴ��� �ҷ�����
    public bool start; // ������ ���(���ӿ����� score�� 0�� ��� ���Ӽ��� ȭ��ǥ �����)

    /* �÷���� ������ */
    public GameObject[] colorBlock; // �÷���� �������� �ִ� �迭
    private Queue<GameObject> queueBlock; // �÷���ϵ��� ��� ť, ���� ������� ����
    private int generateCount = 50; // �ʱ⿡ �����ϴ� �÷������ ��
    private int minCount = 40; // �ּ� ������ �÷���� ��
    float x = 0; // �ʱ� �÷���� ���� ���� ��ġ
    float y = -2f; // �ʱ� �÷���� ���� ���� ��ġ
    bool left = false; // �÷���� ���� ����
    bool right = false; // �÷���� ������ ����

    /* ���� */
    public int score = 0; // ����
    public Text scoreText; // ���� �ؽ�Ʈ
    public int highScore; // �ְ� ����
    public Text highScore_Text; // �ְ� ���� �ؽ�Ʈ

    /* ���ӿ��� */
    public GameObject panel_GameOver; // ���ӿ��� �г�
    public Text gameOver_scoreText; // ���ӿ����� ���ӿ��� �гο� �ߴ� ������ ���� �ؽ�Ʈ
    public Button pinkBtn; // �ΰ��� ���� �ϴ� ���� ��ư
    public Button skyBtn; // �ΰ��� ������ �ϴ� ���� ��ư

    /* �Ͻ����� */
    public GameObject panel_Pause; // �Ͻ����� ȭ��
    public bool isPause = false;

    /* ���ѽð� */
    public GameObject player; // �÷��̾� ������Ʈ
    public float deadTime = 5.0f; // �÷��̾ �÷���ư Ŭ���� ���� ��� ����Ǵ� ���ӿ��� Ÿ�̸�
    public float deadTime_decrease = 0.25f; // Ÿ�̸� ��ġ ���ҷ�
    public Animator anim_sweat;

    /* �޺� ������ */
    public Slider comboGauge; // ��� �޺� ��������
    private bool comboGaugeMax; // ���������� ��ġ ���� �ִ�
    public float comboMaxTime = 5.0f; // �������� �ִ��� ��� ���۵Ǵ� Ÿ�̸�
    public float comboMaxTime_decrease; // �������� Ÿ�̸� ���ҷ�

    /*����*/
    public AudioMixer masterMixer;
    public Slider bgmSlider; // pause�г� bgm�����̴�
    public Slider sfxSlider; // pause�г� sfx�����̴�
    public AudioSource bgmSound; // �ΰ��� BGM ����
    public AudioSource gameoverSound; // ���ӿ��� ����
    public AudioSource candySound; // ĵ�� ȹ�� ȿ����
    public AudioSource btnSound; // ��ư Ŭ�� ȿ����
    public static float bgmValue; // �����̴� value�� �ӽ� ����
    public static float sfxValue;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        queueBlock = new Queue<GameObject>();
        GenerateBlock();

        panel_GameOver.SetActive(false);
        panel_Pause.SetActive(false);

        scoreText.text = score.ToString("F0");
        start = false;

        bgmSlider.value = bgmValue; // ���� ����� �� �����̴� value �� �ҷ�����
        sfxSlider.value = sfxValue;

        //comboGauge.value = 0; // ��� �������� ��ġ: 0
        //comboGaugeMax = false;
    }

    void Update()
    {
        // ��ư Ŭ�� �׽�Ʈ��
        if (Input.GetKeyDown(KeyCode.A))
        {
            CompareBlock("Pink");
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            CompareBlock("Sky");
        }

        // ���ѽð�
        Timer();

        // �޺� ������
        // Combo();

        // �ְ����� ���� �� ����
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("highScore", score);
            PlayerPrefs.Save();
        }
        // �ְ����� ��������
        highScore = PlayerPrefs.GetInt("highScore");
        // �ְ����� ǥ��
        highScore_Text.text = highScore.ToString("F0");
    }

    // �÷���� ����
    private void GenerateBlock()
    {
        for (int i = 0; i < generateCount; i++)
        {
            // �÷���� �������� �����ϰ� ����
            GameObject generateBlock = Instantiate(colorBlock[Random.Range(0, 10)]);
            // ť �ȿ� �÷���� ���
            queueBlock.Enqueue(generateBlock);
        }
        RepositionBlock();
    }

    // �÷���� �߰� ����
    // ���� ����� �������� ��� ����� ������Ƿ� �߰� ������ ���� ��ϵ��� �̾������� ��
    private void AddGenerateBlock()
    {
        GameObject generateBlock = Instantiate(colorBlock[Random.Range(0, 10)]);
        queueBlock.Enqueue(generateBlock);

        // randomDir: �� ����(������, ����, ��) �� ��� �������� �������� �������� ����
        int randomDir = Random.Range(0, 3);

        //������  
        if (randomDir == 1 && right && !left)
        {
            x += 1.1f;
            left = false;
        }
        //����
        else if (randomDir == 2 && left && !right)
        {
            x -= 1.1f;
            right = false;
        }
        //��
        else if (!left && !right)
        {
            // randomLR: �� ����(����, ������) �� ��� �������� �������� �������� ����
            int randomLR = Random.Range(0, 2);

            if (randomLR == 0)
            {
                y += 1.1f;
                left = false;
                right = true;
            }
            else if (randomLR == 1)
            {
                y += 1.1f;
                left = true;
                right = false;
            }
        }
        // ���ʿ��� ���� �̵��� �����̳� ���θ� �̵� ����(���������� �̵� �Ұ�)
        else if (!right && left)
        {
            // randomLU: �� ����(����, ��) �� ��� �������� �������� �������� ����
            int randomLU = Random.Range(0, 2);

            if (randomLU == 0)
            {
                y += 1.1f;
                left = false;
                right = false;
            }
            else if (randomLU == 1)
            {
                x -= 1.1f;
                left = true;
                right = false;
            }
        }
        // �����ʿ��� ���� �̵��� �������̳� ���θ� �̵� ����(�������� �̵� �Ұ�)
        else if (!left && right)
        {
            // randomRU: �� ����(������, ��) �� ��� �������� �������� �������� ����
            int randomRU = Random.Range(0, 2);

            if (randomRU == 0)
            {
                y += 1.1f;
                left = false;
                right = false;
            }
            else if (randomRU == 1)
            {
                x += 1.1f;
                left = false;
                right = true;
            }
        }
        // ��� ���� ��ġ
        generateBlock.transform.position = new Vector3(x, y, 0);
    }

    // ����-�÷���� x, y ��ġ ���ϱ�
    private void RepositionBlock()
    {
        foreach (GameObject generateBlock in queueBlock)
        {
            // randomDir: �� ����(������, ����, ��) �� ��� �������� �������� �������� ����
            int randomDir = Random.Range(0, 3);

            //������  
            if (randomDir == 1 && right && !left)
            {
                x += 1.1f;
                left = false;
            }
            //����
            else if (randomDir == 2 && left && !right)
            {
                x -= 1.1f;
                right = false;
            }
            //��
            else if (!left && !right)
            {
                // randomLR: �� ����(����, ������) �� ��� �������� �������� �������� ����
                int randomLR = Random.Range(0, 2);

                if (randomLR == 0)
                {
                    y += 1.1f;
                    left = false;
                    right = true;
                }
                else if (randomLR == 1)
                {
                    y += 1.1f;
                    left = true;
                    right = false;
                }
            }
            // ���ʿ��� ���� �̵��� �����̳� ���θ� �̵� ����(���������� �̵� �Ұ�)
            else if (!right && left)
            {
                // randomLU: �� ����(����, ��) �� ��� �������� �������� �������� ����
                int randomLU = Random.Range(0, 2);

                if (randomLU == 0)
                {
                    y += 1.1f;
                    left = false;
                    right = false;
                }
                else if (randomLU == 1)
                {
                    x -= 1.1f;
                    left = true;
                    right = false;
                }
            }
            // �����ʿ��� ���� �̵��� �������̳� ���θ� �̵� ����(�������� �̵� �Ұ�)
            else if (!left && right)
            {
                // randomRU: �� ����(������, ��) �� ��� �������� �������� �������� ����
                int randomRU = Random.Range(0, 2);

                if (randomRU == 0)
                {
                    y += 1.1f;
                    left = false;
                    right = false;
                }
                else if (randomRU == 1)
                {
                    x += 1.1f;
                    left = false;
                    right = true;
                }
            }
            // ��� ���� ��ġ
            generateBlock.transform.position = new Vector3(x, y, 0);
        }
    }

    // ��-���� ��ư Ŭ���� �÷������ ������Ʈ �̸��� ��
    private void CompareBlock(string tag)
    {
        if (queueBlock.Count > 0)
        {
            PlayerRay playerr = GameObject.Find("Player").GetComponent<PlayerRay>();
            playerr.ani_t.SetBool("timeover", false);

            GameObject generateBlock = queueBlock.Dequeue();

            // ������ ������ ������ �÷���ư Ŭ����
            if (generateBlock.tag.Contains(tag))
            {
                if (comboGaugeMax == false) // �Ϲ� ���� ������
                {
                    score += 10;
                }
                else if (comboGaugeMax == true) // �޺���� ���� ������
                {
                    score += 100;
                }
                scoreText.text = score.ToString("F0");
                gameOver_scoreText.text = score.ToString("F0");

                // �ǹ�Ÿ���� ���
                if (comboGaugeMax == false)
                {
                    comboGauge.value += 0.025f; // �޺� ������ ������
                }

                // ���ѽð� �ʱ�ȭ
                deadTime = 5.0f;
            }
            else
            {
                // ��� ���� �߸����� ��� ���� ����
                playerr.ani_t.SetBool("Gameover", true);
                pinkBtn.interactable = false;
                skyBtn.interactable = false;
            }
            Debug.Log(string.Format(" Score : {0}", score));
            start = true;

            Destroy(generateBlock);
            player.transform.position = generateBlock.transform.position;
            if (queueBlock.Count < minCount)
            {
                // �߰�����
                AddGenerateBlock();
            }
        }
    }

    // �޺� ������
    private void Combo()
    {
        comboMaxTime_decrease = 0.3f;

        // �޺��������� max�� ��� �ǹ�Ÿ�� ����
        if (comboGauge.value >= 1)
        {
            comboGaugeMax = true;
        }
        // �ǹ�Ÿ�� �� �������� �ٽ� 0�� �Ǿ��� ���
        else if (comboGauge.value <= 0)
        {
            comboGaugeMax = false;
        }

        // �޺��������� �ִ��� ��� �������� ��ġ 0���� ���� ����
        if (comboGaugeMax == true)
        {
            comboGauge.value -= Time.deltaTime * comboMaxTime_decrease;
        }
    }

    // ���ѽð�
    public void Timer()
    {
        // ���ӽ��۽� Ÿ�̸� ����
        if (start == true && deadTime > 0)
        {
            deadTime -= Time.deltaTime * deadTime_decrease;

            if (score > 0)
            {
                deadTime_decrease = 0.25f;
                anim_sweat.SetFloat("sweat_speed", 0.5f);
                bgmSound.pitch = 0.96f;
            }
            if (score > 50)
            {
                deadTime_decrease = 0.5f;
                anim_sweat.SetFloat("sweat_speed", 0.5f);
                bgmSound.pitch = 0.965f;
            }
            if (score > 100)
            {
                deadTime_decrease = 1.0f;
                anim_sweat.SetFloat("sweat_speed", 0.5f);
                bgmSound.pitch = 0.97f;
            }
            if (score > 200)
            {
                deadTime_decrease = 1.5f;
                anim_sweat.SetFloat("sweat_speed", 0.7f);
                bgmSound.pitch = 0.975f;
            }
            if (score > 300)
            {
                deadTime_decrease = 2.0f;
                anim_sweat.SetFloat("sweat_speed", 0.8f);
                bgmSound.pitch = 0.98f;
            }
            if (score > 350)
            {
                deadTime_decrease = 3.0f;
                anim_sweat.SetFloat("sweat_speed", 0.9f);
                bgmSound.pitch = 0.985f;
            }
            if (score > 400)
            {
                deadTime_decrease = 3.5f;
                anim_sweat.SetFloat("sweat_speed", 1.0f);
                bgmSound.pitch = 0.99f;
            }
            if (score > 450)
            {
                deadTime_decrease = 4.5f;
                anim_sweat.SetFloat("sweat_speed", 1.1f);
                bgmSound.pitch = 0.995f;
            }
            if (score > 500)
            {
                deadTime_decrease = 5.0f;
                anim_sweat.SetFloat("sweat_speed", 1.15f);
                bgmSound.pitch = 1.0f;
            }
            if (score > 550)
            {
                deadTime_decrease = 5.5f;
                anim_sweat.SetFloat("sweat_speed", 1.2f);
                bgmSound.pitch = 1.005f;
            }
            if (score > 600)
            {
                deadTime_decrease = 6.0f;
                anim_sweat.SetFloat("sweat_speed", 1.3f);
                bgmSound.pitch = 1.01f;
            }
            if (score > 650)
            {
                deadTime_decrease = 6.5f;
                anim_sweat.SetFloat("sweat_speed", 1.4f);
                bgmSound.pitch = 1.015f;
            }
            if (score > 700)
            {
                deadTime_decrease = 7.0f;
                anim_sweat.SetFloat("sweat_speed", 1.45f);
                bgmSound.pitch = 1.02f;
            }
            if (score > 750)
            {
                deadTime_decrease = 7.5f;
                anim_sweat.SetFloat("sweat_speed", 1.5f);
                bgmSound.pitch = 1.025f;
            }
            if (score > 800)
            {
                deadTime_decrease = 8.0f;
                anim_sweat.SetFloat("sweat_speed", 1.55f);
                bgmSound.pitch = 1.03f;
            }
            if (score > 850)
            {
                deadTime_decrease = 8.5f;
                anim_sweat.SetFloat("sweat_speed", 1.65f);
                bgmSound.pitch = 1.035f;
            }
            if (score > 900)
            {
                deadTime_decrease = 9.0f;
                anim_sweat.SetFloat("sweat_speed", 1.75f);
                bgmSound.pitch = 1.04f;
            }
            if (score > 950)
            {
                deadTime_decrease = 9.5f;
                anim_sweat.SetFloat("sweat_speed", 1.8f);
                bgmSound.pitch = 1.045f;
            }
            if (score > 1000)
            {
                deadTime_decrease = 10.0f;
                anim_sweat.SetFloat("sweat_speed", 1.85f);
                bgmSound.pitch = 1.050f;
            }
            if (score > 1200)
            {
                deadTime_decrease = 10.5f;
                anim_sweat.SetFloat("sweat_speed", 1.9f);
                bgmSound.pitch = 1.055f;
            }
            if (score > 1400)
            {
                deadTime_decrease = 11.0f;
                anim_sweat.SetFloat("sweat_speed", 1.95f);
                bgmSound.pitch = 1.060f;
            }
            if (score > 1600)
            {
                deadTime_decrease = 11.5f;
                anim_sweat.SetFloat("sweat_speed", 2.0f);
                bgmSound.pitch = 1.065f;
            }
            if (score > 1800)
            {
                deadTime_decrease = 12.0f;
                anim_sweat.SetFloat("sweat_speed", 2.05f);
                bgmSound.pitch = 1.07f;
            }
            if (score > 2000)
            {
                deadTime_decrease = 12.5f;
                anim_sweat.SetFloat("sweat_speed", 2.25f);
                bgmSound.pitch = 1.075f;
            }
            if (score > 2500)
            {
                deadTime_decrease = 13.0f;
                anim_sweat.SetFloat("sweat_speed", 2.5f);
                bgmSound.pitch = 1.08f;
            }
            if (score > 3000)
            {
                deadTime_decrease = 13.5f;
                anim_sweat.SetFloat("sweat_speed", 2.75f);
                bgmSound.pitch = 1.08f;
            }
            if (score > 3500)
            {
                deadTime_decrease = 14.0f;
                anim_sweat.SetFloat("sweat_speed", 3.0f);
                bgmSound.pitch = 1.08f;
            }
            if (score > 4000)
            {
                deadTime_decrease = 15.0f;
                anim_sweat.SetFloat("sweat_speed", 4.0f);
                bgmSound.pitch = 1.08f;
            }
        }
    }

    // Bgm �����̴� Volume ����
    public void BgmAudioControl()
    {
        float sound = bgmSlider.value;
        bgmValue = bgmSlider.value; // �����̴� �� ����

        if (sound == -40f) masterMixer.SetFloat("BGM", -80); // value�� -40�� ��� -80���� ���� ���Ұ�
        if (sound == -40f) masterMixer.SetFloat("Title", -80); // value�� -40�� ��� -80���� ���� ���Ұ�
        else masterMixer.SetFloat("BGM", sound);
    }

    // Effect �����̴� Volume ����
    public void SfxAudioControl()
    {
        float sound = sfxSlider.value;
        sfxValue = sfxSlider.value;

        if (sound == -40f) masterMixer.SetFloat("SFX", -80);
        else masterMixer.SetFloat("SFX", sound);
    }

    // ��ũ�� ��ư Ŭ����
    public void PinkBtn()
    {
        CompareBlock("Pink");
        candySound.Play();
    }

    // �ϴû� ��ư Ŭ����
    public void SkyBtn()
    {
        CompareBlock("Sky");
        candySound.Play();
    }

    // �Ͻ����� ��ư Ŭ����
    public void PauseBtn()
    {
        bgmSound.Stop();
        panel_Pause.SetActive(true);
        Time.timeScale = 0f;
    }

    // ����� ��ư Ŭ����
    public void Restart()
    {
        Time.timeScale = 1.0f;
        // �� �ʱ�ȭ ������
        StartCoroutine(DelaySceneLoad());
        Time.timeScale = 1.0f;
        btnSound.Play();
        pinkBtn.interactable = true;
        skyBtn.interactable = true;
    }

    // btnSound ����� ���� �� �ʱ�ȭ ������
    IEnumerator DelaySceneLoad()
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene("Ingame");
    }

    // ��� ��ư Ŭ����
    public void PlayBtn()
    {
        bgmSound.Play();
        panel_Pause.SetActive(false);
        Time.timeScale = 1.0f;
        btnSound.Play();
    }

    // Ȩ ��ư Ŭ����(���ӿ��� �г�)
    public void HomeBtn()
    {
        SceneManager.LoadScene("Title");
        Time.timeScale = 1.0f;
        btnSound.Play();
    }
}