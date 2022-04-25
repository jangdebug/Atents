using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : BaseItem
{
    public override bool Use(GameObject target)
    {
        if (base.Use(target))
        {
            GameMgr.Instance.AddScore(value);
            return true;
        }
        return false;
    }
}
