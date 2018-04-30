using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialBank {

	public static Material GetWallMaterial(int index) {
        return Resources.Load("Materials/Walls/" + index) as Material;
    }

    public static Material GetFloorMaterial(int index) {
        return Resources.Load("Materials/Floors/" + index) as Material;
    }

    public static Material[] GetFloorMaterials(int[] indexes) {
        List<Material> materials = new List<Material>();
        foreach (int i in indexes) {
            materials.Add(Resources.Load("Materials/Floors/" + i) as Material);
        }
        return materials.ToArray();
    }
}
