using UnityEngine;
using System.Collections.Generic;
using System;


namespace CelestialBodies.Config.Shape
{
    [CreateAssetMenu(fileName = "PlanetShape", menuName = "Scriptable Objects/Shape/PlanetShape")]
    public class PlanetShape : Shape
    {

        [SerializeField] public PlaneShapeSettings shapeConfig;

        public override void InitSettings()
        {
            // TODO: CHECK OBSERVERS HERE
            return;
        }

        public override ShapeConfig GetShapeConfig()
        {
            return shapeConfig;
        }


        // TODO: replace with compute shader generation
        public override Mesh GenerateMesh(int resolution, float radius)
        {
            Mesh mesh = new Mesh(); // Instance of new mesh
            List<Vector3> vertices = new List<Vector3>(); // List of vertices
            List<int> triangles = new List<int>(); // List of triangles

            PRNG random = new PRNG(shapeConfig.seed); // Create a new random number generator

            for (int y = 0; y <= resolution; y++)
            {
                for (int x = 0; x <= resolution; x++)
                {
                    float u = (float)x / resolution * 2 - 1;
                    float v = (float)y / resolution * 2 - 1;
                    Vector3 pointOnSphere = new Vector3(u, v, 1).normalized;

                    float height = Mathf.PerlinNoise(pointOnSphere.x * shapeConfig.noiseScale,
                                                     pointOnSphere.y * shapeConfig.noiseScale)
                                                       * shapeConfig.noiseStrength;

                    vertices.Add(pointOnSphere * (radius + height));

                    if (x < resolution && y < resolution)
                    {
                        int a = y * (resolution + 1) + x;
                        int b = a + resolution + 1;
                        int c = a + 1;
                        int d = b + 1;

                        triangles.Add(a);
                        triangles.Add(b);
                        triangles.Add(c);
                        triangles.Add(c);
                        triangles.Add(b);
                        triangles.Add(d);
                    }
                }
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            return mesh;
        }

        [Serializable]
        public class PlaneShapeSettings : ShapeConfig
        {
            [Header ("Continent settings TODO")]
            public float continentFrequency = 1.0f;
        }

    };
}
