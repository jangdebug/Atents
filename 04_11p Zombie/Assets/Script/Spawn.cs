using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

abstract public class Spawn<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] protected T[] prefabs; // ������ ����Ʈ.
    [SerializeField] private float maxDistance = 5f; // ������ �ִ� �ݰ�.
                                                     // pooling�� ������Ʈ ���� ����.
                                                     // random�� ������ ���� �ʿ䰡 ���� ��� Queue�� ���.
    private List<T> pooling = new List<T>();
    public bool isEmptyPool { get { return !(0 < pooling.Count); } }
    abstract public void Updates();
    abstract protected void SpawnObj();
    protected void MakeObjPooling(int index) //������Ʈ�� ����� pooling ����Ʈ�� �߰��Ѵ�.
    {
        var select = Mathf.Min(prefabs.Length - 1, index);
        if (0 <= select)
        {
            var obj = Instantiate(prefabs[select]);
            if (obj)
            {
                pooling.Add(obj);
                obj.gameObject.SetActive(false);
            }
        }
    }
    // �ÿ��� ���� ������Ʈ�� �ٽ� pooling ����Ʈ�� �ִ´�.
    public void SetPooling(T obj)
    {
        if (!pooling.Contains(obj)) pooling.Add(obj);
    }
    private T GetObject(int index) // pooling ����Ʈ�� index��ġ�� ���� �����ϰ� ����.
    {
        T obj = null;
        if (0 < pooling.Count)
        {
            obj = pooling[index];
            pooling.RemoveAt(index);
        }
        return obj;
    }
    protected T GetObjRandom() // pooling ����Ʈ�� ������ ��ġ(index)�� ���� ����.
    {
        var select = Random.Range(0, pooling.Count);
        return GetObject(select);
    }
    protected T GetFirstObj() // pooling ����Ʈ�� ���� ù ��° ���� ����.
    {
        return GetObject(0);
    }
    /// <summary>
    /// NavMesh ���� ������ ��ġ�� ��ȯ�ϴ� �޼���. <br/>
    /// center�� �߽����� maxDistance �ݰ� �ȿ��� ������ ��ġ�� ã�´�.
    /// </summary>
    protected Vector3 GetRandomPointOnNavMesh(Vector3 center)
    {
        // center�� �߽����� �������� maxDistance�� �� �ȿ����� ������ ��ġ �ϳ��� ����.
        // Random.insideUnitSphere : �������� 1�� �� �ȿ����� ������ �� ���� ��ȯ.
        Vector3 randomPos = Random.insideUnitSphere * maxDistance + center;
        NavMeshHit navHit;
        // maxDistance �ݰ� �ȿ���, randomPos�� ���� ����� ����޽� ���� �� ���� ã�´�.
        NavMesh.SamplePosition(randomPos, out navHit, maxDistance, NavMesh.AllAreas);
        return navHit.position;
    }
} // abstract class Spawn
