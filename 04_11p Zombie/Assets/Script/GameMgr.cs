using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameMgr : MonoBehaviour
{

    [SerializeField] private int maxWave = 10; // �ִ� Wave ī��Ʈ.
    public int wave { get; private set; } = 0; // ���� Wave ī��Ʈ.
                                               // �̹� Wave�� ������ Enemy�� ��.

    [SerializeField] private GameObject gameoverUI; // ���� ������ Ȱ��ȭ�� UI.

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
        // instance�� �ƴ� Instance ���� ����!!
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

    // ���� �޼��� ����.
    private int score = 0;
    public void AddScore(int value)
    {
        score += value;
        // UI�� ScoreText�� ����.
        UIMgr.Instance.UpdateScoreText(score);
    }

    private void Update()
    {
        if (ItemMgr.Instance) ItemMgr.Instance.Updates();
        if (EnemyMgr.Instance) EnemyMgr.Instance.Updates();

    }

}
