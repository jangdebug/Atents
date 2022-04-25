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
    [SerializeField] private Transform player; // �÷��̾� ��ġ�� Ȯ���� Transform.
    [SerializeField] private float timeBetSpawnMax = 7f; // ���� �ִ� �ð� ����.
    [SerializeField] private float timeBetSpawnMin = 2f; // ���� �ּ� �ð� ���� . 
    private float timeBetSpawn; // ���� ���� ����. 
    private void Start()
    {
        // ������ ���� �ð� ���ϱ�.
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
            // ���� �ð� �� ����.
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
        spawnPosition += Vector3.up * 0.5f; // �ٴڿ��� 0.5��ŭ ���� �ø���.
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

