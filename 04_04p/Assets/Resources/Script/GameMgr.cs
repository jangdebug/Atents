using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameMgr : MonoBehaviour
{

    public static GameMgr Instance { get; private set; }
    private Player player;
    GameObject[] turrets;
    private Bullet bulletPrefab;
    private List<Bullet> listBullet;
    [SerializeField] private float spwanRateMin = 0.3f;
    [SerializeField] private float spwanRateMax = 0.8f;
    private float spawnRate = 1f;
    private float checkTime = 0;

    private float timer = 0;

    private void Awake()
    {
        if (null == Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += (scene, mode) => { Init(); };   //Dodge(SCENE)�� ȣ��
            return;
        }
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update() 
    {
        if (player && player.isLive)        // nullexception üũ
        {
            checkTime += Time.deltaTime;
            if (spawnRate <= checkTime)
            {
                checkTime = 0;
                spawnRate = Random.Range(spwanRateMin, spwanRateMax);
                SpawnBullet();
            }
            else if (Input.GetKeyDown(KeyCode.R)) SceneManager.LoadScene("Dodge");
            timer += Time.deltaTime;
            UIMgr.Instance.Timer = timer;

        }

    }

    void Init()
    {
        UIMgr.Instance.OnPlay();

        timer = 0;

        spawnRate = 1f;
        spwanRateMax = 0.8f;

        player = FindObjectOfType<Player>();
        if (player) player.Init();

        bulletPrefab = Resources.Load<Bullet>("Prefabs/Sphere");

        turrets = GameObject.FindGameObjectsWithTag("Respawn");
        // ������Ʈ Ǯ��.
        listBullet = new List<Bullet>();
        for (int i = 0; turrets.Length > i; i++)
        {
            var bullet = MakeBullet();
        }
    }

    Bullet MakeBullet()
    {
        if (bulletPrefab)
        {
            var bullet = Instantiate(bulletPrefab);
            if (bullet && player)
            {
                bullet.EventHadleOnCollisionPlayer += player.OnDamaged;
                bullet.EventHadleOnCollisionPlayer += () => { UIMgr.Instance.GameOver(timer); };
            }
            return bullet;
            
        }
        return null;
    }

    void SpawnBullet()
    {
        if (0 >= turrets.Length) return;
        // ���ǰ� ���� ����(��Ȱ��ȭ ����) źȯ�� ã�´�.
        var bullet = listBullet.Find(b => !b.gameObject.activeSelf);
        // ������ �ʴ� źȯ�� ���ٸ� �߰��� �����.
        if (!bullet) bullet = MakeBullet();
        if (bullet)
        {
            // źȯ �߻�.
            var pos_index = Random.Range(0, turrets.Length);
            var pos = turrets[pos_index].transform.position + Vector3.up * 1.5f;
            bullet.SetPosition(pos);
            var dir = (player.position - pos).normalized;
            dir.y = 0.2f;
            var force = Random.Range(3, 8);
            bullet.OnFire(dir, force * 100);
        }
    }


}
