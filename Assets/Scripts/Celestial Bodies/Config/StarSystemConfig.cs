using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CelestialBodies.Config
{
    // StarSystemConfig holds the configuration for a star system.
    [Serializable]
    public class StarSystemConfig
    {
        // TODO check this
        [SerializeField]
        public string systemName;
        public List<CelestialBodyConfig> celestialBodyConfigs = new List<CelestialBodyConfig>();

        // TODO: Check these
        public Vector3 lastCamPos;
        public float lastCamZoom;

        
        // Add and init new settings to config list
        public int AddNewCelestialBodySettings(CelestialBodyConfig.CelestialBodyType type)
        {
            //CelestialBodyConfig newBody = new CelestialBodyConfig();
            
            CelestialBodyConfig newBody = ScriptableObject.CreateInstance<CelestialBodyConfig>();

            // Init config values
            newBody.Init(type);

            celestialBodyConfigs.Add(newBody);
            return celestialBodyConfigs.Count - 1;
        }



    }
}
