using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameMgr : MonoBehaviour
{
    private static GameMgr instance = null;

    private Text score;
    private float scoreCount = 0;

    private GameObject gameOver;

    public static GameMgr Instance
    {
        get
        {
            if (null == instance)   //만약 히어라이키에 GameMgr이 없다면 임시로 생성해준다(실행 시)
            {
                instance = new GameObject("GameMgr").AddComponent<GameMgr>();
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }
    }
    private void Awake()
    {
        SoundMgr.Instance.PlayBGM("music");

        var canvas = FindObjectOfType<Canvas>();
        if (canvas)
        {
            var scoreTr = canvas.transform.Find("Score");
            if (scoreTr) score = scoreTr.GetComponent<Text>();

            var gameOverTr = canvas.transform.Find("GameOver");
            if (gameOverTr) gameOver = gameOverTr.gameObject;
        }

        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(instance.gameObject);
            return;
        }
        Destroy(gameObject);
    }
    public bool isDead { get; private set; }
    public void OnDie()
    {
        isDead = true;
        SoundMgr.Instance.StopBGM();

    }

    public void GameOver()
    {
        if (gameOver) gameOver.SetActive(true);
    }



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (false == isDead && score)
        {
            scoreCount += Time.deltaTime * 100;
            score.text = string.Format("Score : {0:N0}", Mathf.RoundToInt(scoreCount));
        }
    }
}
