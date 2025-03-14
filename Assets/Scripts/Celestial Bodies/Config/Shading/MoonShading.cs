using System;
using CelestialBodies;
using UnityEngine;
using Utilities;

namespace CelestialBodies.Config.Shading
{
    [Serializable]
    [CreateAssetMenu(fileName = "MoonShading", menuName = "Celestial Bodies/Config/Shading/Moon Shading")]
    public class MoonShading : Shading
    {
        private ComputeBuffer _pointBuffer;

        [SerializeField] protected MoonShadingSettings shadingConfig;

        public override void InitConfig()
        {
            if (Observers == null) return;
            foreach (ICelestialObserver observer in Observers)
            {
                observer.OnShadingUpdate();
            }
        }

        public override void SetConfig(ShadingConfig ss)
        {
            shadingConfig = (MoonShadingSettings)ss;

            if (Observers == null) return;
            foreach (ICelestialObserver observer in Observers)
            {
                observer.OnShadingUpdate();
            }
        }
        public override ShadingConfig GetConfig()
        {
            return shadingConfig;
        }


        [Serializable]
        public class MoonShadingSettings : ShadingConfig
        {
            // Dummy setup to test shape stuff
            public int dummyVar = 0;
        }

        [Serializable]
        public struct MoonColors
        {
            public Color primaryColorA;
            public Color secondaryColorA;
            public Color primaryColorB;
            public Color secondaryColorB;
        }
        
    }
}