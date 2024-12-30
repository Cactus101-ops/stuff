using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

//=== assign a unique ID to each Collider so that it can be serialized and saved ===\\
public class ColliderLink : MonoBehaviour
{
    private static int idcounter = 0;
    public ActionManager associatedcollider;

    public int colliderID { get; private set; }

    private void Awake()
    {
        colliderID = idcounter++;

    }
}

