using System;
using UnityEngine;
using Utilities;

namespace CelestialBodies.Config.Shading
{
    [Serializable]
    [CreateAssetMenu]
    public class StarShading : Shading
    {

        /*
            public virtual void SetSurfaceProperties(Material material, Vector2 heightMinMax, float bodyScale, float oceanLevel)
            {
            // Overriden by child class
            }
         */

        [SerializeField] protected StarShadingSettings shadingSettings;



        public override void SetSurfaceProperties(Material material, Vector2 heightMinMax, float bodyScale, float oceanLevel)
        {


            Color surfaceColor = Color.black;

            if (shadingSettings.randomize)
            { 
                PRNG prng = new PRNG(shadingSettings.seed);
                var n = prng.Range(0, 3);
                var deltaH = prng.Range(-0.05f, 0.05f);

                switch (n)
                {
                    case 0: 
                        surfaceColor = ColorHelper.TweakHSV(shadingSettings.yellow, deltaH, 0, 0);
                        shadingSettings.mainColor = Color.yellow;
                        break;
                    case 1:
                        surfaceColor = ColorHelper.TweakHSV(shadingSettings.blue, deltaH, 0, 0);
                        shadingSettings.mainColor = Color.blue;
                        break;
                    case 2:
                        surfaceColor = ColorHelper.TweakHSV(shadingSettings.red, deltaH, 0, 0);
                        shadingSettings.mainColor = Color.red;
                        break;
                }
            }
            else
            {
                surfaceColor = shadingSettings.yellow;
            }

            material.SetColor("_EmissionColor", surfaceColor * 4);
        }









        public override void InitConfig()
        {
            if (Observers == null)
            {
                return;
            }

            foreach (ICelestialObserver observer in Observers)
            {
                observer.OnShadingUpdate();
            }

        }

        public override void SetConfig(ShadingConfig ss)
        {
            shadingSettings = (StarShadingSettings)ss;
            if (Observers == null)
            {
                return;
            }

            foreach (ICelestialObserver observer in Observers)
            {
                observer.OnShadingUpdate();
            }
        }

        public override ShadingConfig GetConfig()
        {
            return shadingSettings;
        }

        [Serializable]
        public class StarShadingSettings : ShadingConfig
        {
            public Color yellow;
            public Color blue;
            public Color red;
        }

    } // StarShading class
} // namespace