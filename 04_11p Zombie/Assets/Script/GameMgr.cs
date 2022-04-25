using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameMgr : MonoBehaviour
{

    [SerializeField] private int maxWave = 10; // 최대 Wave 카운트.
    public int wave { get; private set; } = 0; // 현재 Wave 카운트.
                                               // 이번 Wave에 출현할 Enemy의 수.

    [SerializeField] private GameObject gameoverUI; // 게임 오버시 활성화할 UI.

    public int enemySpawnCount { get { return Mathf.RoundToInt(wave * 1.5f); } }
    public bool NextWave()
    {
        if (maxWave > wave)
        {
            wave++;
            return true;
        }
        return false;
    }


    private static GameMgr instance = null;
    public static GameMgr Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<GameMgr>();
                if (!instance) instance = new GameObject("GameMgr").AddComponent<GameMgr>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }
    private void Awake()
    {
        // instance가 아닌 Instance 임을 주의!!
        if (this != Instance)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {

        UIMgr.Instance.RestartEvent += () =>
        {
            gameoverUI.SetActive(false);
            SceneManager.LoadScene("SampleScene");
        };
    }

    // 현재 달성한 점수.
    private int score = 0;
    public void AddScore(int value)
    {
        score += value;
        // UI의 ScoreText를 갱신.
        UIMgr.Instance.UpdateScoreText(score);
    }

    private void Update()
    {
        if (ItemMgr.Instance) ItemMgr.Instance.Updates();
        if (EnemyMgr.Instance) EnemyMgr.Instance.Updates();

    }

}
