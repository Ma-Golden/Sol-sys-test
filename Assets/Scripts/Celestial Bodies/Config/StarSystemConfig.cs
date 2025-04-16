using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CelestialBodies.Config
{
    // StarSystemConfig holds the configuration for a star system.
    [Serializable]
    public class StarSystemConfig : ScriptableObject
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
            CelestialBodyConfig newBody = new CelestialBodyConfig();
            
            newBody.Init(type);

            var posX = celestialBodyConfigs.Select(cbs =>
                cbs.physics.GetPhysicalConfig().initialPosition.x +
                cbs.physics.GetPhysicalConfig().maxRadius +
                cbs.physics.GetPhysicalConfig().maxRadius + 100).Prepend(0.0f).Max();

            Vector3 pos = new Vector3(posX, 0, 0);
            Physics.PhysicsSettings ps = newBody.physics.GetPhysicalConfig();
            ps.initialPosition = pos;
            newBody.physics.SetSettings(ps);
            
            // CelestialBodyConfig newBody = ScriptableObject.CreateInstance<CelestialBodyConfig>();

            celestialBodyConfigs.Add(newBody);
            return celestialBodyConfigs.Count - 1;
        }



    }
}
