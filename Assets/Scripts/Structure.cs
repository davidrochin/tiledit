using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure {

    public static GameObject GetPrefab(string prefabName) {
        return Resources.Load("Structure/" + prefabName) as GameObject;
    }
}
