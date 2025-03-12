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
    private StarSystemConfig _ssc;

    private void Start()
    {

        // 'Load' / create new test system

        //_ssc = SystemSavingUtils.Instance.LoadTestSystem("TestSystem");
        if (_ssc == null || _ssc.celestialBodyConfigs[0].bodyType != cBodyType)
        {
            _ssc = new StarSystemConfig
            {
                systemName = "TestSystem"
            };
            Debug.Log("Adding new celestial body settings");
            _ssc.AddNewCelestialBodySettings(cBodyType);
        }

        // Check if celestialBody is null
        if (celestialBody == null)
        {
            Debug.LogError("celestialBody is null");
            return;
        }

        // Check if celestialBody.celestiaBodyGenerator is null
        if (celestialBody.celestiaBodyGenerator == null)
        {
            Debug.LogError("celestialBody.celestiaBodyGenerator is null");
            return;
        }



        //        cBodyType = CelestialBodyConfig.CelestialBodyType.Planet;

        _cs = _ssc.celestialBodyConfigs[0];
        celestialBody.celestiaBodyGenerator.bodyConfig = _cs;
        _cs.Subscribe(celestialBody.celestiaBodyGenerator);


        _cs.Init(cBodyType);
        
        SystemSavingUtils.Instance.currentSystemConfig = _ssc;


        //// Intialize Celestial Body
        //celestialBody = new GameObject("TestCbody").AddComponent<CelestialBody>();
        //celestialBody.celestiaBodyGenerator = celestialBody.gameObject.AddComponent<CelestiaBodyGenerator>();
        //celestialBody.celestiaBodyGenerator.bodyConfig = _cs;
        //celestialBody.celestiaBodyGenerator.body = celestialBody;

        //// TODO CHECK THIS
        //celestialBody.celestiaBodyGenerator.HandleEditModeGeneration();

        ////// Create spher mesh with resolution
        ////SphereMesh sphereMesh = new SphereMesh(resolution);
    }

    private void OnDestroy()
    {
        //SystemSavingUtils.Instance.SaveTestSystem();
    }


}
