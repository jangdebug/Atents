using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemMgr : Spawn<BaseItem>
{
    public static ItemMgr Instance { get; private set; }
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            return;
        }
        Destroy(gameObject);
    }
    [SerializeField] private Transform player; // 플레이어 위치를 확인할 Transform.
    [SerializeField] private float timeBetSpawnMax = 7f; // 생성 최대 시간 간격.
    [SerializeField] private float timeBetSpawnMin = 2f; // 생성 최소 시간 간격 . 
    private float timeBetSpawn; // 실재 생성 간격. 
    private void Start()
    {
        // 아이템 생성 시간 구하기.
        timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
        for (int i = 0; prefabs.Length > i; i++)
        {
            MakeObjPooling(i);
        }
    }
    public override void Updates()
    {
        if (0 >= (timeBetSpawn -= Time.deltaTime) && player)
        {
            // 생성 시간 재 설정.
            timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
            SpawnObj();
        }
    }
    protected override void SpawnObj()
    {
        if (isEmptyPool)
        {
            var select = Random.Range(0, prefabs.Length);
            MakeObjPooling(select);
        }
        var item = GetObjRandom();
        Vector3 spawnPosition = GetRandomPointOnNavMesh(player.position);
        spawnPosition += Vector3.up * 0.5f; // 바닥에서 0.5만큼 위로 올린다.
        item.SetPosition(spawnPosition);
        StartCoroutine(LateInactive(item.gameObject));
    }
    private IEnumerator LateInactive(GameObject obj)
    {
        var timer = 5f;
        while (0 < (timer -= Time.deltaTime))
        {
            if (!obj.activeSelf) yield break;
            yield return null;
        }
        obj.SetActive(false);
    }
} // class ItemMgr

