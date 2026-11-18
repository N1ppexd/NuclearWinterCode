using System.Collections.Generic;
using UnityEngine;

namespace NuclearWinter
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class HexRenderer : MonoBehaviour
    {
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private MeshFilter meshFilter;

        [SerializeField] private Material material;


        [SerializeField] private float innerRadius, outerRadious, height;


        [SerializeField] private HexType hexType;


        private Mesh mesh;

        private List<Face> faces = new List<Face>();

        public float Height 
        { 
            get => height;
            set
            {
                height = value;

                faces.Clear();
                DrawFace();
                CombineFaces();
            } 
        }


        public HexType HexType
        {
            get => hexType;

            set => hexType = value;
        }


        private void OnValidate()
        {
            //temporary to do it in the inspector.
            
            faces.Clear();


            DrawFace();
            CombineFaces();
        }

        private void DrawFace()
        {

            for(int i = 0; i < 6 ; i++)
            {
                faces.Add(CreateFace(innerRadius / 2f, outerRadious / 2f, height / 2f, height / 2f, i));
            }

            for(int i = 0; i < 6 ; i++)
            {
                faces.Add(CreateFace(innerRadius / 2f, outerRadious / 2f, -height / 2f, -height / 2f, i, true));
            }

            for (int i = 0; i < 6; i++)
            {
                faces.Add(CreateFace(outerRadious / 2f, outerRadious / 2f, height / 2f, -height / 2f, i, true));
            }

            for (int i = 0; i < 6; i++)
            {
                faces.Add(CreateFace(innerRadius / 2f, innerRadius / 2f, height / 2f, -height / 2f, i));
            }
        }


        private void CombineFaces()
        {
            List<Vector3> vertices = new();
            List<int> triangles = new();
            List<Vector2> uvs = new();


            for(int i = 0; i < faces.Count; i++)
            {
                
                vertices.AddRange(faces[i].Vertices);
                uvs.AddRange(faces[i].UVs);

                int offset = 4 * i;

                foreach(int triangle in faces[i].Triangles)
                {
                    triangles.Add(triangle + offset);
                }
            }

            mesh = new Mesh();

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.RecalculateNormals();

            meshFilter.mesh = mesh;
            meshRenderer.material = material;

        }


        private Face CreateFace(float innerRad, float outerRad, float heightA, float heightB, int point, bool reverse = false)
        {
            Vector3 pointA = GetPoint(innerRad, heightB, point);
            Vector3 pointB = GetPoint(innerRad, heightB, (point < 5) ? point + 1 : 0);
            Vector3 pointC = GetPoint(outerRad, heightA, (point < 5) ? point + 1 : 0);
            Vector3 pointD = GetPoint(outerRad, heightA, point);


            List<Vector3> vertices = new List<Vector3>() { pointA, pointB, pointC, pointD};
            List<int> triangles = new List<int>() { 0, 1, 2, 2, 3, 0 };
            List<Vector2> uvs = new List<Vector2>() { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) };

            if (!reverse)
            {
                triangles.Reverse();
            }

            return new Face(vertices, triangles, uvs);
        }


        Vector3 GetPoint(float size, float height, int index)
        {
            float angleDeg = hexType == HexType.flatTop? 60f * index : 60f * index - 30f;

            float angleRad = Mathf.PI / 180 * angleDeg;

            return new Vector3(size * Mathf.Cos(angleRad), height, size * Mathf.Sin(angleRad));
        }
    }




    //face struct for the hecagons
    public struct Face
    {
        public List<Vector3> Vertices { get; private set; }
        public List<int> Triangles { get; private set; }
        public List<Vector2> UVs { get; private set; }

        public Face(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
        {
            Vertices = vertices;
            Triangles = triangles;
            UVs = uvs;
        }
    }
}
