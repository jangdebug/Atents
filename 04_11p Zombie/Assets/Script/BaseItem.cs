using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class BaseItem : MonoBehaviour
{
    //해당 아이템을 습득, 사용가능 대상이 되는 레이어.
    [SerializeField] protected LayerMask targetLayer;
    [SerializeField] protected int value = 0; // 아이템 사용 시, 적용되는 값.
    virtual public bool Use(GameObject target)
    {
        // *.layer는 index 값을 가지고,
        // targetLayer는 bit shift(2^index)된 값을 가진다. 
        if ((1 << target.layer).Equals(targetLayer))
        {
            gameObject.SetActive(false);
            return true;
        }
        return false;
    }
    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
        gameObject.SetActive(true);
    }

    virtual protected void OnDisable()
    {
        if (ItemMgr.Instance) ItemMgr.Instance.SetPooling(this);
    }
}

