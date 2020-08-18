using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CItemDropTable : MonoBehaviour
{
    [System.Serializable]
    public class ItemObjectWithChance
    {
        public GameObject item;
        [Range(0f,1f)]
        public float[] chancesPerStage = new float[6];
    }

    public List<ItemObjectWithChance> normalItemObjectList;
    
}
