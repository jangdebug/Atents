using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

abstract public class Spawn<T> : MonoBehaviour where T : MonoBehaviour
{
    [SerializeField] protected T[] prefabs; // 생성할 리스트.
    [SerializeField] private float maxDistance = 5f; // 생성될 최대 반경.
                                                     // pooling한 오브젝트 저장 공간.
                                                     // random한 순서로 꺼낼 필요가 없을 경우 Queue를 사용.
    private List<T> pooling = new List<T>();
    public bool isEmptyPool { get { return !(0 < pooling.Count); } }
    abstract public void Updates();
    abstract protected void SpawnObj();
    protected void MakeObjPooling(int index) //오브젝트를 만들고 pooling 리스트에 추가한다.
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
    // 시용이 끝난 오브젝트를 다시 pooling 리스트에 넣는다.
    public void SetPooling(T obj)
    {
        if (!pooling.Contains(obj)) pooling.Add(obj);
    }
    private T GetObject(int index) // pooling 리스트의 index위치의 값을 제거하고 리턴.
    {
        T obj = null;
        if (0 < pooling.Count)
        {
            obj = pooling[index];
            pooling.RemoveAt(index);
        }
        return obj;
    }
    protected T GetObjRandom() // pooling 리스트의 랜덤한 위치(index)의 값을 리턴.
    {
        var select = Random.Range(0, pooling.Count);
        return GetObject(select);
    }
    protected T GetFirstObj() // pooling 리스트의 가장 첫 번째 값을 리턴.
    {
        return GetObject(0);
    }
    /// <summary>
    /// NavMesh 위의 랜덤한 위치를 반환하는 메서드. <br/>
    /// center를 중심으로 maxDistance 반경 안에서 랜덤한 위치를 찾는다.
    /// </summary>
    protected Vector3 GetRandomPointOnNavMesh(Vector3 center)
    {
        // center를 중심으로 반지름이 maxDistance인 구 안에서의 랜덤한 위치 하나를 지정.
        // Random.insideUnitSphere : 반지름이 1인 구 안에서의 랜덤한 한 점을 반환.
        Vector3 randomPos = Random.insideUnitSphere * maxDistance + center;
        NavMeshHit navHit;
        // maxDistance 반경 안에서, randomPos에 가장 가까운 내비메시 위의 한 점을 찾는다.
        NavMesh.SamplePosition(randomPos, out navHit, maxDistance, NavMesh.AllAreas);
        return navHit.position;
    }
} // abstract class Spawn
