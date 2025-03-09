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
        private List<CelestialBody> celestialBodies;

        // TODO: Check these
        public Vector3 lastCamPos;
        public float lastCamZoom;

        // TODO: add body settings



    }
}
