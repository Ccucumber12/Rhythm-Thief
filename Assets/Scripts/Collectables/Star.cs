using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : Collectable
{
    protected override void Collected()
    {
        gameManager.PlayerCollectStar();
        Destroy(gameObject, 0.05f);
    }
}
