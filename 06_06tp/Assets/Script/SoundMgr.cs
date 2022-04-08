using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMgr : MonoBehaviour
{

    public static SoundMgr Instance { get; private set; }
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
    [SerializeField] AudioSource BGM;
    [SerializeField] AudioSource SFX;
    private AudioClip[] audioClips;

    // Start is called before the first frame update
    void Start()
    {
        audioClips = Resources.LoadAll<AudioClip>("Audio");     //리소스 내 오디오 클립 모든 것을 가져옴
        if (BGM) BGM.loop = true;
        if (SFX) SFX.loop = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayBGM(string name)
    {
        if (BGM && 0 < audioClips.Length)
        {
            foreach (var clip in audioClips)
            {
                if (clip.name.ToLower().Equals(name.ToLower()))
                {
                    BGM.clip = clip;
                    BGM.Play();
                    break;
                }
            }
        }
    }
    public void StopBGM()
    {
        if (BGM)
        {
            BGM.Stop();
        }
    }


    public void PlaySFX(string name)
    {
        if (SFX && 0 < audioClips.Length)
        {
            foreach (var clip in audioClips)
            {
                if (clip.name.ToLower().Equals(name.ToLower()))
                {
                    SFX.PlayOneShot(clip);
                    break;
                }
            }
        }
    }
}
