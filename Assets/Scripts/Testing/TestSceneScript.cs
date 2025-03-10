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
        // "depth texture stuff"

        // System to lad

        //// If system not loaded or loaded cBody has different type from the one set in the inspector
        //// create a new system
        //if (_ss == null || _ss.cBodiesSettings[0].cBodyType != type)
        //{
        //    _ss = new SystemSettings
        //    {
        //        systemName = "Test"
        //    };
        //    _ss.AddNewCBodySettings(type);
        //}

        //_cs = _ss.cBodiesSettings[0];
        //cBody.cBodyGenerator.cBodySettings = _cs;
        //_cs.Subscribe(cBody.cBodyGenerator);

        // insert new cBody config
        // _cs


        celestialBody.celestiaBodyGenerator.bodyConfig = _cs;


        // TODO: INSERT SYSTEM CONFIG FILE
        SystemSavingUtils.Instance.currentSystemConfig = null;

    }

    private void OnDestroy()
    {
        //SystemSavingUtils.Instance.SaveTestSystem();
    }


}
