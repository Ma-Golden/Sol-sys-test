using CelestialBodies;
using CelestialBodies.Config;
using CelestialBodies.Config.Shape;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class CelestialBodyGenerator : MonoBehaviour, ICelestialObserver
{
    [Header ("Body config")]
    public CelestialBody body;  // Celestial body to generate
    public CelestialBodyConfig bodyConfig;
    // Config settings

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;
    private MeshCollider meshCollider;
    private Mesh _generatedMesh;

    // Observer vars
    private bool _shapeUpdated;
    private bool _shadingUpdated;
    private bool _physicsUpdated;

    // Store max and min height values
    private Vector2 _heightMinMax;

    // Compute buffer to stores vertices sent to GPU
    private ComputeBuffer _vertexBuffer;

    // Store different sphere meshes for different LOD levels
    private static Dictionary<int, SphereMesh> _sphereGenerators;

    //[Header("Body Settings")]
    //public Shape shape; // Shape of the body
    //public int resolution = 100;        // Mesh res
    //public float radius = 10f;           // Base radius

    // handles in shape config
    //public float noiseStrength = 1f;    // Strength of noise
    //public float noiseScale = 3.0f;     // Frequency of noise


    // Meshes
    private GameObject _terrainHolder = null;
//    private GameObject _colliderHolder = null;

    private Mesh _previewMesh;
    //private Mesh[] _lodMeshes;


    public float BodyScale => bodyConfig.radius;


    // Allow update of shape, shading etc from edit mode
    public void HandleEditModeGeneration()
    {
        // TODO: Implement this

        if (_shapeUpdated)
        {
            Debug.Log("Shape updated");
            _shapeUpdated = false;

            // _shadingUpdated = false;
            _heightMinMax = GenerateShapeShading(ref _previewMesh, 250);

            // TODO SHADING

            Material terrainMatInstance = new Material(bodyConfig.shading.terrainMaterial);

            body.surfaceMaterial = terrainMatInstance;

            //body.surfaceMaterial = new Material(Shader.Find("Standard"));

            _terrainHolder = GetOrCreateMeshObject(_previewMesh, body.surfaceMaterial);

        }
        else if (_shadingUpdated)
        {
            _shadingUpdated = false;
            GenerateShading(_previewMesh);
        }

        if (_physicsUpdated)
        {
            SetPhysicalProperties();
        }


        if (bodyConfig.shading != null && body.surfaceMaterial != null)
        {
            SetPhysicalProperties();
            
            bodyConfig.shading.Initialize(bodyConfig.shape);


            Debug.Log("Getting ocean level");
            float testOceanLevel = bodyConfig.ocean.GetSettings().GetOceanLevel();

            Debug.Log("Setting surface properties");
            bodyConfig.shading.SetSurfaceProperties(body.surfaceMaterial, _heightMinMax, BodyScale, testOceanLevel);
        
            
        }

        ReleaseAllBuffers();
    }


    private Vector2 GenerateShapeShading(ref Mesh surfaceMesh, int resolution)
    {
        var (vertices, triangles) = CreateSphereVertsTris(resolution);    
        ComputeHelper.CreateStructuredBuffer<Vector3>(ref _vertexBuffer, vertices);

        float edgeLength = (vertices[triangles[0]] - vertices[triangles[1]]).magnitude;

        // Set heights
        float[] heights = bodyConfig.shape.CalculateHeights(_vertexBuffer);

        Shape.ShapeConfig shapeCon = bodyConfig.shape.GetConfig();

        // Perturb vertices to give rougher appearance
        if (shapeCon.perturbVertices && bodyConfig.shape.perturbCompute)
        {
            ComputeShader perturbShader = bodyConfig.shape.perturbCompute;
            float maxPerturbStrength = shapeCon.perturbStrength * edgeLength / 2;

            perturbShader.SetBuffer(0, "points", _vertexBuffer);
            perturbShader.SetInt("numPoints", vertices.Length);
            perturbShader.SetFloat("maxStrength", maxPerturbStrength);

            ComputeHelper.Run(perturbShader, vertices.Length);
            Vector3[] pertData = new Vector3[vertices.Length];
            _vertexBuffer.GetData(pertData);
        }

        // Calculate terrain min/max height and set height of vertices
        float minHeight = float.MaxValue;
        float maxHeight = float.MinValue;

        for (int i = 0; i < heights.Length; i++)
        {
            float height = heights[i];
            vertices[i] *= height;
            minHeight = Mathf.Min(minHeight, height);
            maxHeight = Mathf.Max(maxHeight, height);
        }

        _heightMinMax = new Vector2(minHeight, maxHeight);

        // Create mesh with calculated vertices
        CreateMesh(ref surfaceMesh, vertices.Length);
        surfaceMesh.SetVertices(vertices);
        surfaceMesh.SetTriangles(triangles, 0, true);
        surfaceMesh.RecalculateNormals();


        // Shading noise data
        bodyConfig.shading.Initialize(bodyConfig.shape);
        Vector4[] shadingData = bodyConfig.shading.GenerateShadingData(_vertexBuffer);
        surfaceMesh.SetUVs(0, shadingData);

        var normals = surfaceMesh.normals;
        var crudeTangents = new Vector4[surfaceMesh.vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 normal = normals[i];
            crudeTangents[i] = new Vector4(-normal.z, 0, normal.x, 1);
        }
        surfaceMesh.SetTangents(crudeTangents);


        return new Vector2(minHeight, maxHeight);
    }

    private void GenerateShading(Mesh mesh)
    {
        ComputeHelper.CreateStructuredBuffer<Vector3>(ref _vertexBuffer, mesh.vertices);
        bodyConfig.shading.Initialize(bodyConfig.shape);
        Vector4[] shadingData = bodyConfig.shading.GenerateShadingData(_vertexBuffer);
        mesh.SetUVs(0, shadingData);
    }



    (Vector3[] vertices, int[] triangles) CreateSphereVertsTris(int resolution)
    {
        // If not created, create a dict storing sphere meshes
        if (_sphereGenerators == null)
        {
            _sphereGenerators = new Dictionary<int, SphereMesh>();
        }

        if (!_sphereGenerators.ContainsKey(resolution))
        {
            _sphereGenerators.Add(resolution, new SphereMesh(resolution));
        }

        // Create sphere mesh
        SphereMesh sphereGenerator = _sphereGenerators[resolution];

        var vertices = new Vector3[sphereGenerator.Vertices.Length];    // Init vertices =size to sphere
        var triangles = new int[sphereGenerator.Triangles.Length];      // "" triangles
        Array.Copy(sphereGenerator.Vertices, vertices, vertices.Length);    // Copy vertices
        Array.Copy(sphereGenerator.Triangles, triangles, triangles.Length); // "" Triangles
        return (vertices, triangles);
    }


    void CreateMesh(ref Mesh mesh, int numVertices)
    {
        const int vertexLimit16Bit = 1 << 16 - 1;
        // Create mesh if needed, otherwise clear old one
        if (mesh == null)
        {
            mesh = new Mesh();
        }
        else
        {
            mesh.Clear();
        }

        mesh.indexFormat = (numVertices < vertexLimit16Bit)
            ? UnityEngine.Rendering.IndexFormat.UInt16
            : UnityEngine.Rendering.IndexFormat.UInt32;
    }

    // TODO: LOOK INTO PHYSICS CONFIG AS SEPERATE

    private void SetPhysicalProperties()
    {
        // Updates physical properties of the bodies transform.
        Transform bodyTransform = transform;
        bodyTransform.position = body.position;
        bodyTransform.localScale = bodyConfig.radius * Vector3.one;

        // TODO SET MASS
    }




    public void OnShapeUpdate()
    {
        _shapeUpdated = true;
        //

        HandleEditModeGeneration();
    }

    public void OnPhysicsUpdate()
    {
        //_physicsUpdated = true;
        Debug.LogWarning("PhysicsUpdate not implemented");
    }

    public void OnShadingUpdate()
    {
        Debug.Log("Shading update");
        _shadingUpdated = true;
        HandleEditModeGeneration();
    }


    public void OnInitialUpdate()
    {
        _shapeUpdated = true;
        _shadingUpdated = true;
//        _physicsUpdated = true;


        // TODO IMPLEMENT CHECKING OF GAME MODE HERE
        HandleEditModeGeneration();

    }

    public float GetOceanRadius()
    {
        if (!bodyConfig.ocean.GetSettings().hasOcean)
        {
            return 0;
        }
        return UnscaledOceanRadius * BodyScale;
    }

    private float UnscaledOceanRadius => Mathf.Lerp(_heightMinMax.x, 1, bodyConfig.ocean.GetSettings().GetOceanLevel());


    
    // Get child object with specified name
    // If none exists, then creates object with that name
    GameObject GetOrCreateMeshObject (Mesh surfaceMesh, Material material)
    {
        // Find/create object
        Transform child = transform.Find("Terrain Mesh");
        if (!child)
        {
            Debug.Log("Creating new terrain mesh object");
            child = new GameObject("Terrain Mesh").transform;
            child.parent = transform;
            child.localPosition = Vector3.zero;
            child.localRotation = Quaternion.identity;
            child.localScale = Vector3.one;
            child.gameObject.layer = gameObject.layer;
        }
        // Add mesh components
        MeshFilter filter;
        if (!child.TryGetComponent<MeshFilter>(out filter))
        {
            filter = child.gameObject.AddComponent<MeshFilter>();
        }
        filter.sharedMesh = surfaceMesh;

        MeshRenderer meshRenderer;
        if (!child.TryGetComponent<MeshRenderer>(out meshRenderer))
        {
            meshRenderer = child.gameObject.AddComponent<MeshRenderer>();
        }
        meshRenderer.sharedMaterial = material;

        return child.gameObject;
    }


    //private void InitializeMeshComponents()
    //{
    //    meshFilter = gameObject.AddComponent<MeshFilter>();
    //    meshRenderer = gameObject.AddComponent<MeshRenderer>();
    //    meshCollider = gameObject.AddComponent<MeshCollider>();

    //    if (meshRenderer.sharedMaterial == null)
    //    {
    //        meshRenderer.sharedMaterial = new Material(Shader.Find("Standard"));
    //    }
    //}

    
    private void ReleaseAllBuffers()
    {
        ComputeHelper.Release(_vertexBuffer);
        bodyConfig.shape.ReleaseBuffers();
        bodyConfig.shading.ReleaseBuffers();
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
