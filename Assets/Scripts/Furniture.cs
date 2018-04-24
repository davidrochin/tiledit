using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Furniture : MonoBehaviour {

    public string name;
    public Vector3 size;

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(new Vector3(transform.position.x, transform.position.y + (size.y * 0.5f), transform.position.z), size);
    }
}
