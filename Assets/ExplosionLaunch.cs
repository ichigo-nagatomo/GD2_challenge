using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ExplosionLaunch : Explosion {
    protected override void SetExpand() {
        move = new Vector2(1f, 0f);
        targetTime = 3f;
    }
}

