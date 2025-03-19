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

        [SerializeField] protected MoonshadingConfig shadingConfig;

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
            shadingConfig = (MoonshadingConfig)ss;

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

        public override void SetSurfaceProperties(Material material, Vector2 heightMinMax, float bodyScale, float oceanLevel)
        {
            material.SetVector("heightMinMax", heightMinMax);
            material.SetFloat("bodyScale", bodyScale);

            // CraterBiomeSettings
            SetCraterBiome(material);

            if (shadingConfig.randomize)
            {
                // SetRandomColors();
                // ApplyColours(material, shadinConfig.RandomMoonColours);
                SetRandColors();
                ApplyColors(material, shadingConfig.randomMooncolours);
            }
            else 
            {
                // Apply colours
                ApplyColors(material, shadingConfig.baseMoonColours);
            }

            shadingConfig.mainColor = Color.gray;
        }


        private void SetCraterBiome(Material material)
        {
            PRNG prng = new PRNG(shadingConfig.seed);

            Vector4 biomesValues = new Vector4(
                prng.SignedValueBiasExtremes(0.3f),
                prng.SignedValueBiasExtremes(0.3f) * 0.4f,
                prng.SignedValueBiasExtremes(0.3f) * 0.3f,
                prng.SignedValueBiasExtremes(0.3f) * 0.7f
                );

            material.SetVector("_RandomBiomeValues", biomesValues);
            var warpStrength = prng.SignedValueBiasCentre(0.65f) * 30f;
            material.SetFloat("_BiomeBlendStrength", prng.Range(2f, 12) + Mathf.Abs (warpStrength) / 2);
            material.SetFloat("_BiomeWarpStrength", warpStrength);
        }

        private void SetRandColors()
        {
            PRNG random = new PRNG(shadingConfig.seed);
            if (shadingConfig.realisticColors)
            {
                var deltaH = random.Range(shadingConfig.colourHRange.x, shadingConfig.colourHRange.y);
                MoonColors colors = shadingConfig.baseMoonColours;

                shadingConfig.randomMooncolours.primaryColorA =
                    ColorHelper.TweakHSV(colors.primaryColorA, deltaH, 0, 0);
                shadingConfig.randomMooncolours.secondaryColorA =
                    ColorHelper.TweakHSV(colors.secondaryColorA, deltaH, 0, 0);
                shadingConfig.randomMooncolours.primaryColorB =
                    ColorHelper.TweakHSV(colors.primaryColorB, deltaH, 0, 0);
                shadingConfig.randomMooncolours.secondaryColorB =
                    ColorHelper.TweakHSV(colors.secondaryColorB, deltaH, 0, 0);
            }
            else
            {
                shadingConfig.randomMooncolours.primaryColorA =
                    ColorHelper.Random(random, 0.45f, 0.6f, 0.7f, 0.8f);
                shadingConfig.randomMooncolours.secondaryColorA =
                    ColorHelper.TweakHSV(shadingConfig.randomMooncolours.primaryColorA, random.SignedValue() * 0.2f, random.SignedValue() * 0.15f, random.Range(-0.25f, -0.2f));
                shadingConfig.randomMooncolours.primaryColorB =
                    ColorHelper.Random(random, 0.45f, 0.6f, 0.7f, 0.8f);
                shadingConfig.randomMooncolours.secondaryColorB =
                    ColorHelper.TweakHSV(shadingConfig.randomMooncolours.primaryColorB, random.SignedValue() * 0.2f, random.SignedValue() * 0.15f, random.Range(-0.25f, -0.2f));
            }
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
        public class MoonshadingConfig : ShadingConfig
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