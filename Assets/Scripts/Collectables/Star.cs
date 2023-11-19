using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : Collectable
{
    [Header("Star Information")]
    public int uid;

    protected override void Start()
    {
        base.Start();
        if (uid < 0 || uid > 2)
        {
            Debug.LogError("star uid not in {0, 1, 2}");
        }
    }

    protected override void Collected()
    {
        inGameManager.PlayerCollectStar(uid);
        Destroy(gameObject, 0.05f);
    }
}
