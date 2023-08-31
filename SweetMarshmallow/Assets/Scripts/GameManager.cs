using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // 다른 스크립트에서 게임매니저 불러오기
    public bool start; // 시작할 경우(게임오버시 score가 0인 경우 게임설명 화살표 사라짐)

    /* 컬러블록 랜덤맵 */
    public GameObject[] colorBlock; // 컬러블록 프리팹을 넣는 배열
    private Queue<GameObject> queueBlock; // 컬러블록들을 담는 큐, 담은 순서대로 꺼냄
    private int generateCount = 50; // 초기에 생성하는 컬러블록의 수
    private int minCount = 40; // 최소 유지할 컬러블록 수
    float x = 0; // 초기 컬러블록 생성 가로 위치
    float y = -2f; // 초기 컬러블록 생성 세로 위치
    bool left = false; // 컬러블록 왼쪽 생성
    bool right = false; // 컬러블록 오른쪽 생성

    /* 점수 */
    public int score = 0; // 점수
    public Text scoreText; // 점수 텍스트
    public int highScore; // 최고 점수
    public Text highScore_Text; // 최고 점수 텍스트

    /* 게임오버 */
    public GameObject panel_GameOver; // 게임오버 패널
    public Text gameOver_scoreText; // 게임오버시 게임오버 패널에 뜨는 마지막 점수 텍스트
    public Button pinkBtn; // 인게임 왼쪽 하단 색상 버튼
    public Button skyBtn; // 인게임 오른쪽 하단 색상 버튼

    /* 일시정지 */
    public GameObject panel_Pause; // 일시정지 화면
    public bool isPause = false;

    /* 제한시간 */
    public GameObject player; // 플레이어 오브젝트
    public float deadTime = 5.0f; // 플레이어가 컬러버튼 클릭을 안할 경우 실행되는 게임오버 타이머
    public float deadTime_decrease = 0.25f; // 타이머 수치 감소량
    public Animator anim_sweat;

    /* 콤보 게이지 */
    public Slider comboGauge; // 상단 콤보 게이지바
    private bool comboGaugeMax; // 게이지바의 수치 값이 최대
    public float comboMaxTime = 5.0f; // 게이지바 최대일 경우 시작되는 타이머
    public float comboMaxTime_decrease; // 게이지바 타이머 감소량

    /*사운드*/
    public AudioMixer masterMixer;
    public Slider bgmSlider; // pause패널 bgm슬라이더
    public Slider sfxSlider; // pause패널 sfx슬라이더
    public AudioSource bgmSound; // 인게임 BGM 사운드
    public AudioSource gameoverSound; // 게임오버 사운드
    public AudioSource candySound; // 캔디 획득 효과음
    public AudioSource btnSound; // 버튼 클릭 효과음
    public static float bgmValue; // 슬라이더 value값 임시 저장
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

        bgmSlider.value = bgmValue; // 게임 재시작 전 슬라이더 value 값 불러오기
        sfxSlider.value = sfxValue;

        //comboGauge.value = 0; // 상단 게이지바 수치: 0
        //comboGaugeMax = false;
    }

    void Update()
    {
        // 버튼 클릭 테스트용
        if (Input.GetKeyDown(KeyCode.A))
        {
            CompareBlock("Pink");
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            CompareBlock("Sky");
        }

        // 제한시간
        Timer();

        // 콤보 게이지
        // Combo();

        // 최고점수 갱신 및 저장
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("highScore", score);
            PlayerPrefs.Save();
        }
        // 최고점수 가져오기
        highScore = PlayerPrefs.GetInt("highScore");
        // 최고점수 표시
        highScore_Text.text = highScore.ToString("F0");
    }

    // 컬러블록 생성
    private void GenerateBlock()
    {
        for (int i = 0; i < generateCount; i++)
        {
            // 컬러블록 프리팹을 랜덤하게 생성
            GameObject generateBlock = Instantiate(colorBlock[Random.Range(0, 10)]);
            // 큐 안에 컬러블록 담기
            queueBlock.Enqueue(generateBlock);
        }
        RepositionBlock();
    }

    // 컬러블록 추가 생성
    // 기존 블록을 재정렬할 경우 블록이 사라지므로 추가 생성을 통해 블록들이 이어지도록 함
    private void AddGenerateBlock()
    {
        GameObject generateBlock = Instantiate(colorBlock[Random.Range(0, 10)]);
        queueBlock.Enqueue(generateBlock);

        // randomDir: 세 방향(오른쪽, 왼쪽, 위) 중 어느 방향으로 생성할지 랜덤으로 결정
        int randomDir = Random.Range(0, 3);

        //오른쪽  
        if (randomDir == 1 && right && !left)
        {
            x += 1.1f;
            left = false;
        }
        //왼쪽
        else if (randomDir == 2 && left && !right)
        {
            x -= 1.1f;
            right = false;
        }
        //위
        else if (!left && !right)
        {
            // randomLR: 두 방향(왼쪽, 오른쪽) 중 어느 방향으로 생성할지 랜덤으로 결정
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
        // 왼쪽에서 위로 이동시 왼쪽이나 위로만 이동 가능(오른쪽으로 이동 불가)
        else if (!right && left)
        {
            // randomLU: 두 방향(왼쪽, 위) 중 어느 방향으로 생성할지 랜덤으로 결정
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
        // 오른쪽에서 위로 이동시 오른쪽이나 위로만 이동 가능(왼쪽으로 이동 불가)
        else if (!left && right)
        {
            // randomRU: 두 방향(오른쪽, 위) 중 어느 방향으로 생성할지 랜덤으로 결정
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
        // 블록 생성 위치
        generateBlock.transform.position = new Vector3(x, y, 0);
    }

    // 정렬-컬러블록 x, y 위치 정하기
    private void RepositionBlock()
    {
        foreach (GameObject generateBlock in queueBlock)
        {
            // randomDir: 세 방향(오른쪽, 왼쪽, 위) 중 어느 방향으로 생성할지 랜덤으로 결정
            int randomDir = Random.Range(0, 3);

            //오른쪽  
            if (randomDir == 1 && right && !left)
            {
                x += 1.1f;
                left = false;
            }
            //왼쪽
            else if (randomDir == 2 && left && !right)
            {
                x -= 1.1f;
                right = false;
            }
            //위
            else if (!left && !right)
            {
                // randomLR: 두 방향(왼쪽, 오른쪽) 중 어느 방향으로 생성할지 랜덤으로 결정
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
            // 왼쪽에서 위로 이동시 왼쪽이나 위로만 이동 가능(오른쪽으로 이동 불가)
            else if (!right && left)
            {
                // randomLU: 두 방향(왼쪽, 위) 중 어느 방향으로 생성할지 랜덤으로 결정
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
            // 오른쪽에서 위로 이동시 오른쪽이나 위로만 이동 가능(왼쪽으로 이동 불가)
            else if (!left && right)
            {
                // randomRU: 두 방향(오른쪽, 위) 중 어느 방향으로 생성할지 랜덤으로 결정
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
            // 블록 생성 위치
            generateBlock.transform.position = new Vector3(x, y, 0);
        }
    }

    // 비교-색깔 버튼 클릭시 컬러블록의 오브젝트 이름을 비교
    private void CompareBlock(string tag)
    {
        if (queueBlock.Count > 0)
        {
            PlayerRay playerr = GameObject.Find("Player").GetComponent<PlayerRay>();
            playerr.ani_t.SetBool("timeover", false);

            GameObject generateBlock = queueBlock.Dequeue();

            // 사탕과 동일한 색상의 컬러버튼 클릭시
            if (generateBlock.tag.Contains(tag))
            {
                if (comboGaugeMax == false) // 일반 점수 증가량
                {
                    score += 10;
                }
                else if (comboGaugeMax == true) // 콤보모드 점수 증가량
                {
                    score += 100;
                }
                scoreText.text = score.ToString("F0");
                gameOver_scoreText.text = score.ToString("F0");

                // 피버타임일 경우
                if (comboGaugeMax == false)
                {
                    comboGauge.value += 0.025f; // 콤보 게이지 증가량
                }

                // 제한시간 초기화
                deadTime = 5.0f;
            }
            else
            {
                // 블록 구분 잘못했을 경우 게임 오버
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
                // 추가생성
                AddGenerateBlock();
            }
        }
    }

    // 콤보 게이지
    private void Combo()
    {
        comboMaxTime_decrease = 0.3f;

        // 콤보게이지가 max일 경우 피버타임 실행
        if (comboGauge.value >= 1)
        {
            comboGaugeMax = true;
        }
        // 피버타임 후 게이지가 다시 0이 되었을 경우
        else if (comboGauge.value <= 0)
        {
            comboGaugeMax = false;
        }

        // 콤보게이지가 최대일 경우 게이지바 수치 0으로 점차 감소
        if (comboGaugeMax == true)
        {
            comboGauge.value -= Time.deltaTime * comboMaxTime_decrease;
        }
    }

    // 제한시간
    public void Timer()
    {
        // 게임시작시 타이머 실행
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

    // Bgm 슬라이더 Volume 조절
    public void BgmAudioControl()
    {
        float sound = bgmSlider.value;
        bgmValue = bgmSlider.value; // 슬라이더 값 저장

        if (sound == -40f) masterMixer.SetFloat("BGM", -80); // value가 -40일 경우 -80으로 만들어서 음소거
        if (sound == -40f) masterMixer.SetFloat("Title", -80); // value가 -40일 경우 -80으로 만들어서 음소거
        else masterMixer.SetFloat("BGM", sound);
    }

    // Effect 슬라이더 Volume 조절
    public void SfxAudioControl()
    {
        float sound = sfxSlider.value;
        sfxValue = sfxSlider.value;

        if (sound == -40f) masterMixer.SetFloat("SFX", -80);
        else masterMixer.SetFloat("SFX", sound);
    }

    // 핑크색 버튼 클릭시
    public void PinkBtn()
    {
        CompareBlock("Pink");
        candySound.Play();
    }

    // 하늘색 버튼 클릭시
    public void SkyBtn()
    {
        CompareBlock("Sky");
        candySound.Play();
    }

    // 일시정지 버튼 클릭시
    public void PauseBtn()
    {
        bgmSound.Stop();
        panel_Pause.SetActive(true);
        Time.timeScale = 0f;
    }

    // 재시작 버튼 클릭시
    public void Restart()
    {
        Time.timeScale = 1.0f;
        // 씬 초기화 딜레이
        StartCoroutine(DelaySceneLoad());
        Time.timeScale = 1.0f;
        btnSound.Play();
        pinkBtn.interactable = true;
        skyBtn.interactable = true;
    }

    // btnSound 재생을 위해 씬 초기화 딜레이
    IEnumerator DelaySceneLoad()
    {
        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene("Ingame");
    }

    // 재생 버튼 클릭시
    public void PlayBtn()
    {
        bgmSound.Play();
        panel_Pause.SetActive(false);
        Time.timeScale = 1.0f;
        btnSound.Play();
    }

    // 홈 버튼 클릭시(게임오버 패널)
    public void HomeBtn()
    {
        SceneManager.LoadScene("Title");
        Time.timeScale = 1.0f;
        btnSound.Play();
    }
}