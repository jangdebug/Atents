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

    [SerializeField] private float damageMax = 40f; // 최대 공격력.
    [SerializeField] private float damageMin = 20f; // 최소 공격력.
    [SerializeField] private float healthMax = 200f; // 최대 체력.
    [SerializeField] private float healthMin = 100f; // 최소 체력.
    [SerializeField] private float speedMax = 3f; // 최대 속도.
    [SerializeField] private float speedMin = 1f; // 최소 속도.
    [SerializeField] private Color strongColor = Color.red; // 강한 적 AI가 가지게 될 피부색.
    private int spawnCount = 0; // 필드에 존재하는 Enemy의 수.
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
        // pooling list에 사용가능한 오브젝트가 없다면 추가로 생성.
        if (isEmptyPool) MakeObjPooling(0);
        var enemy = GetFirstObj();
        if (enemy)
        {
            // 필드에 존재하는 Enemy의 수를 확인하기 위하여 사용.
            spawnCount++;
            // Lerp(a, b, t):최소(a), 최대(b) 값 범위의 값을 t값(0~1)을 참조하여 보간.
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
        // spawnCount를 감소하고 UI정보를 갱신한다.
        UIMgr.Instance.UpdateWaveText(GameMgr.Instance.wave, --spawnCount);
    }
    protected override void SpawnObj()
    {
        // wave 생성이 가능한지 확인.
        if (GameMgr.Instance.NextWave())
        {
            // 현재 wave에 생성되는 enemy의 수를 확인하여 생성.
            var count = GameMgr.Instance.enemySpawnCount;
            for (int i = 0; count > i; i++) SetupEnemy(Random.value);
            // 현재 wave UI정보를 갱신.
            UIMgr.Instance.UpdateWaveText(GameMgr.Instance.wave, spawnCount);
        }
    }
}
