using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMgr : Spawn<Enemy>
{
    public static EnemyMgr Instance { get; private set; }
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            return;
        }
        Destroy(gameObject);
    }

    [SerializeField] private float damageMax = 40f; // �ִ� ���ݷ�.
    [SerializeField] private float damageMin = 20f; // �ּ� ���ݷ�.
    [SerializeField] private float healthMax = 200f; // �ִ� ü��.
    [SerializeField] private float healthMin = 100f; // �ּ� ü��.
    [SerializeField] private float speedMax = 3f; // �ִ� �ӵ�.
    [SerializeField] private float speedMin = 1f; // �ּ� �ӵ�.
    [SerializeField] private Color strongColor = Color.red; // ���� �� AI�� ������ �� �Ǻλ�.
    private int spawnCount = 0; // �ʵ忡 �����ϴ� Enemy�� ��.
    private void Start()
    {
        for (int i = 0; 50 > i; i++)
        {
            MakeObjPooling(0);
        }
    }
    public override void Updates()
    {
        if (0 >= spawnCount)
        {
            SpawnObj();
        }
    }
    private void SetupEnemy(float intensity)
    {
        // pooling list�� ��밡���� ������Ʈ�� ���ٸ� �߰��� ����.
        if (isEmptyPool) MakeObjPooling(0);
        var enemy = GetFirstObj();
        if (enemy)
        {
            // �ʵ忡 �����ϴ� Enemy�� ���� Ȯ���ϱ� ���Ͽ� ���.
            spawnCount++;
            // Lerp(a, b, t):�ּ�(a), �ִ�(b) �� ������ ���� t��(0~1)�� �����Ͽ� ����.
            var damage = Mathf.Lerp(damageMin, damageMax, intensity);
            var health = Mathf.Lerp(healthMin, healthMax, intensity);
            var speed = Mathf.Lerp(speedMin, speedMax, intensity);
            var color = Color.Lerp(Color.white, strongColor, intensity);
            var pos = GetRandomPointOnNavMesh(Vector3.zero);
            enemy.Setup(damage, health, speed, color, pos);
        }
    }
    public void DecreaseSpawnCount()
    {
        // spawnCount�� �����ϰ� UI������ �����Ѵ�.
        UIMgr.Instance.UpdateWaveText(GameMgr.Instance.wave, --spawnCount);
    }
    protected override void SpawnObj()
    {
        // wave ������ �������� Ȯ��.
        if (GameMgr.Instance.NextWave())
        {
            // ���� wave�� �����Ǵ� enemy�� ���� Ȯ���Ͽ� ����.
            var count = GameMgr.Instance.enemySpawnCount;
            for (int i = 0; count > i; i++) SetupEnemy(Random.value);
            // ���� wave UI������ ����.
            UIMgr.Instance.UpdateWaveText(GameMgr.Instance.wave, spawnCount);
        }
    }
}
