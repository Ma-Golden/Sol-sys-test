using CelestialBodies.Config;
using Mono.Cecil.Cil;
using Unity.VisualScripting;
using UnityEngine;

public class TestSceneScript : MonoBehaviour
{
    [Header("Sphere Mesh vars")]
    public int resolution = 3;

    public CelestialBody celestialBody;
    public CelestialBodyConfig.CelestialBodyType cBodyType;

    private CelestialBodyConfig _cs;


    private void Start()
    {
        cBodyType = CelestialBodyConfig.CelestialBodyType.Planet;

        _cs = new CelestialBodyConfig();
        _cs.Init(cBodyType);

        // Intialize Celestial Body
        celestialBody = new GameObject("TestCbody").AddComponent<CelestialBody>();
        celestialBody.celestiaBodyGenerator = celestialBody.gameObject.AddComponent<CelestiaBodyGenerator>();
        celestialBody.celestiaBodyGenerator.bodyConfig = _cs;
        celestialBody.celestiaBodyGenerator.body = celestialBody;
        Debug.Log("Celestial Body Created");

        celestialBody.celestiaBodyGenerator.HandleEditModeGeneration();

        //// Create spher mesh with resolution
        //SphereMesh sphereMesh = new SphereMesh(resolution);
    }
        //// Create new mesh & assign vertices and triangles
        //Mesh mesh = new Mesh();
        //mesh.vertices = sphereMesh.Vertices;
        //mesh.triangles = sphereMesh.Triangles;
        //mesh.RecalculateNormals();

        //// Create a neaw Gameobject to hold mesh
        //GameObject sphereObject = new GameObject("SphereMesh", typeof(MeshFilter), typeof(MeshRenderer));
        //sphereObject.GetComponent<MeshFilter>().mesh = mesh;

        //sphereObject.GetComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));

        //// Set postion of sphere object to centre of space
        //sphereObject.transform.position = new Vector3(0f, 0f, 0f);
   // }




    //private void Start()
    //{
    //    // "depth texture stuff"

    //    // System to lad

    //    //// If system not loaded or loaded cBody has different type from the one set in the inspector
    //    //// create a new system
    //    //if (_ss == null || _ss.cBodiesSettings[0].cBodyType != type)
    //    //{
    //    //    _ss = new SystemSettings
    //    //    {
    //    //        systemName = "Test"
    //    //    };
    //    //    _ss.AddNewCBodySettings(type);
    //    //}

    //    //_cs = _ss.cBodiesSettings[0];
    //    //cBody.cBodyGenerator.cBodySettings = _cs;
    //    //_cs.Subscribe(cBody.cBodyGenerator);

    //    // insert new cBody config
    //    // _cs


    //    celestialBody.celestiaBodyGenerator.bodyConfig = _cs;


    //    // TODO: INSERT SYSTEM CONFIG FILE
    //    SystemSavingUtils.Instance.currentSystemConfig = null;

    //}

    private void OnDestroy()
    {
        //SystemSavingUtils.Instance.SaveTestSystem();
    }


}
