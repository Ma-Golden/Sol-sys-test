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

    private void OnDestroy()
    {
        //SystemSavingUtils.Instance.SaveTestSystem();
    }


}
