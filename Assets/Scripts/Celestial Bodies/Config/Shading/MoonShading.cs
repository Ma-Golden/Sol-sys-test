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

        public override void SetSurfaceProperties(Material material, Vector2 heightMinMax, float bodyScale)
        {
            material.SetVector("heightMinMax", heightMinMax);
            material.SetFloat("bodyScale", bodyScale);

            // CraterBiomeSettings

            if (shadingConfig.randomize)
            {
                // SetRandomColors();
                // ApplyColours(material, shadinConfig.RandomMoonColours);
                ApplyColors(material, shadingConfig.baseMoonColours);
            }
            else 
            {
                // Apply colours
                ApplyColors(material, shadingConfig.baseMoonColours);
            }

            shadingConfig.mainColor = Color.gray;
        }


        /*
         		
		float4 primaryColorA;
		float4 secondaryColorA;
		float4 primaryColorB;
		float4 secondaryColorB;
         
         */

        void ApplyColors(Material material, MoonColors colors)
        {
            material.SetColor("primaryColorA", colors.primaryColorA);
            material.SetColor("secondaryColorA", colors.secondaryColorA);
            material.SetColor("primaryColorB", colors.primaryColorB);
            material.SetColor("secondaryColorB", colors.secondaryColorB);
        }

        protected override void SetShadingDataComputeProperties()
        {
            SetShadingNoise();
        }

        private void SetShadingNoise()
        {
            const string detailWarpNoiseSuffix = "_detailWarp";
            const string detailNoiseSuffix = "_detail";

            PRNG prng = new PRNG(shadingConfig.seed);
            PRNG prng2 = new PRNG(shadingConfig.seed);


            if (shadingConfig.randomize)
            {
                SimpleNoiseSettings randomizedDetailWarpNoise = new SimpleNoiseSettings
                {
                    scale = prng.Range(1f, 3f),
                    elevation = prng.Range(1f, 5f)
                };

                randomizedDetailWarpNoise.SetComputeValues(shadingDataCompute, prng2, detailWarpNoiseSuffix);

                shadingConfig.detailNoise.SetComputeValues(shadingDataCompute, prng2, detailNoiseSuffix);
            } else
            {
                shadingConfig.detailWarpNoise.SetComputeValues(shadingDataCompute, prng, detailWarpNoiseSuffix);
                shadingConfig.detailNoise.SetComputeValues(shadingDataCompute, prng2, detailNoiseSuffix);
            }
        }

        [Serializable]
        public class MoonShadingSettings : ShadingConfig
        {
            // Dummy setup to test shape stuff
            public int dummyVar = 0;

            public MoonColors baseMoonColours;
            public MoonColors randomMooncolours;
            public Vector2 colourHRange;

            public SimpleNoiseSettings detailNoise = new SimpleNoiseSettings();
            public SimpleNoiseSettings detailWarpNoise = new SimpleNoiseSettings();
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