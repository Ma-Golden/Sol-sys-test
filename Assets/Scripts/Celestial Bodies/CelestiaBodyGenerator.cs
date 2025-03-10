using CelestialBodies.Config;
using CelestialBodies.Config.Shape;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class CelestiaBodyGenerator : MonoBehaviour
{
    [Header ("Body config")]
    public CelestialBody body;  // Celestial body to generate
    public CelestialBodyConfig bodyConfig;
    // Config settings

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
    private Mesh _generatedMesh;


    // for different LOD levels
    private static Dictionary<int, SphereMesh> _sphereGenerators;
    
    
    
    //[Header("Body Settings")]
    //public Shape shape; // Shape of the body
    //public int resolution = 100;        // Mesh res
    //public float radius = 10f;           // Base radius

    // handles in shape config
    //public float noiseStrength = 1f;    // Strength of noise
    //public float noiseScale = 3.0f;     // Frequency of noise


    // Meshes
    //private GameObject _terrainHolder = null;
    //private GameObject _colliderHolder = null;

    //private Mesh _previewMesh;
    //private Mesh[] _lodMeshes;


    private void Start()
    {
        if (bodyConfig == null)
        {
            Debug.LogWarning("No CelestialBodyConfig assigned to generator!");
            return;
        }

        InitializeMeshComponents();
        GenerateBody();
    }



    private void InitializeMeshComponents()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        meshRenderer = gameObject.AddComponent<MeshRenderer>();
        meshCollider = gameObject.AddComponent<MeshCollider>();

        if (meshRenderer.sharedMaterial == null)
        {
            meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
        }
    }

    private void GenerateBody()
    {
        if (bodyConfig.shape != null)
        {
            _generatedMesh = bodyConfig.shape.GenerateMesh(100, 10f);

            meshFilter.mesh = _generatedMesh;
            meshCollider.sharedMesh = _generatedMesh;
        }
    }



    // Util class for resolution settings
    public class ResolutionSettings
    {
        public const int NumLodLevels = 4;
        private const int MaxResolution = 500;

        public int lod0 = 250;
        public int lod1 = 100;
        public int lod2 = 50;
        public int lod3 = 25;
        public int collider = 100;

        public int GetLODResolution(int lodlevel)
        {
            return lodlevel switch
            {
                0 => lod0,
                1 => lod1,
                2 => lod2,
                3 => lod3,
                _ => 0
            };
        }

        public void ClampResolutions()
        {
            lod0 = Mathf.Min(MaxResolution, lod0);
            lod1 = Mathf.Min(MaxResolution, lod1);
            lod2 = Mathf.Min(MaxResolution, lod2);
            collider = Mathf.Min(MaxResolution, collider);
        }
    }
}
