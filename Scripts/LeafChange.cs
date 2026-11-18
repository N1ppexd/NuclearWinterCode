using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NuclearWinter
{
    public class LeafChange : MonoBehaviour
    {
        [SerializeField] int currentCycle;

        [SerializeField] Material leafMaterial;
        [SerializeField] Color[] leafColors;
        [SerializeField] Mesh[] leafMesh;

        private MeshFilter mf;

        private void Awake()
        {
            mf = GetComponent<MeshFilter>();
        }

        public void CycleChange()
        {
            leafMaterial.color = leafColors[currentCycle];

            mf.mesh = leafMesh[currentCycle];
        }
    }
}
