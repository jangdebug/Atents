using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIMgr : MonoBehaviour
{
    public static UIMgr Instance { get; private set; }

    [SerializeField] private Text scoreText; // ���� ǥ�ÿ� �ؽ�Ʈ.
    [SerializeField] private Text waveText; // �� ���̺� ǥ�ÿ� �ؽ�Ʈ.
    [SerializeField] private GameObject gameoverUI; // ���� ������ Ȱ��ȭ�� UI.
    public event System.Action RestartEvent;


    private void Awake()
    {
        if (null == Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        Destroy(gameObject);
    }

    [SerializeField] private Text ammoText; // ź�� ǥ�ÿ� �ؽ�Ʈ.
    public void UpdateAmmoText(int magAmmo, int remainAmmo)
    {
        //var strBuilder = new System.Text.StringBuilder();
        //strBuilder.Append(magAmmo);
        //strBuilder.Append(" / ");
        //strBuilder.Append(remainAmmo);
        //ammoText.text = strBuilder.ToString(); �Ǵ�,
        ammoText.text = string.Format("{0} / {1}", magAmmo, remainAmmo);
    }

    public void UpdateScoreText(int newScore)
    {
        scoreText.text = string.Format("Score : {0}", newScore);
    }
    public void UpdateWaveText(int waves, int count)
    {
        waveText.text = string.Format("Wave : {0}\nEnemy Left : {1}", waves, count);
    }
    public void GameOver()
    {
        gameoverUI.SetActive(true);
    }
    public void Restart()
    {
        if (null != RestartEvent) RestartEvent();
    }



}
