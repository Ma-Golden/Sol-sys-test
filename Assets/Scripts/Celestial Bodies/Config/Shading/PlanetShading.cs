using System;
using UnityEngine;
using Utilities;
using static CelestialBodies.Config.Shading.Shading;


namespace CelestialBodies.Config.Shading
{
    [Serializable][CreateAssetMenu]

    public class PlanetShading : Shading
    {
        [SerializeField] protected PlanetShadingSettings shadingSettings;

        // Memento Pattern
        public override void InitConfig()
        {
            if (Observers == null) return;
            foreach (var observer in Observers)
            {
                observer.OnShadingUpdate();
            }
        }

        public override ShadingConfig GetConfig()
        {
            return shadingSettings;
        }

        public override void SetConfig(ShadingConfig ss)
        {
            shadingSettings = (PlanetShadingSettings)ss;

            if (Observers == null) return;
            foreach (ICelestialObserver observer in Observers)
            {
                observer.OnShadingUpdate();
            }
        }

        public override void SetSurfaceProperties(Material material, Vector2 heightMinMax, float bodyScale, float oceanLevel)
        {
            
            Debug.Log("Setting surface properties");
            material.SetVector("heightMinMax", heightMinMax);
            material.SetFloat("oceanLevel", oceanLevel);
            material.SetFloat("bodyScale", bodyScale);

            SetCraterBiomesSettings(material);

            Debug.Log("Randomize shading: " + shadingSettings.randomize);
            if (shadingSettings.randomize)
            {
                SetRandomColors();
                ApplyColours(material, shadingSettings.randomColors);

                shadingSettings.mainColor = shadingSettings.randomColors.flatColLowA;
            }
            else
            {
                ApplyColours(material, shadingSettings.baseGreenColors);

                shadingSettings.mainColor = shadingSettings.baseGreenColors.flatColLowA;
            }
        }

        private void SetCraterBiomesSettings(Material material)
        {
            PRNG random = new PRNG(shadingSettings.seed);

            Vector4 biomesValues = new Vector4(
                random.SignedValueBiasExtremes(0.3f),
                random.SignedValueBiasExtremes(0.3f) * 0.4f,
                random.SignedValueBiasExtremes(0.3f) * 0.3f,
                random.SignedValueBiasCentre(0.3f) * .7f
            );
            material.SetVector("_RandomBiomeValues", biomesValues);
            var warpStrength = random.SignedValueBiasCentre(.65f) * 30;
            material.SetFloat("_BiomeBlendStrength", random.Range(2f, 12) + Mathf.Abs(warpStrength) / 2);
            material.SetFloat("_BiomeWarpStrength", warpStrength);
        }

        private void SetRandomColors()
        {
            PRNG random = new PRNG(shadingSettings.seed);
            if (shadingSettings.realisticColors)
            {
                var deltaH = 0.0f;
                PlanetColors colors;

                var n = random.Range(0, 3);
                switch (n)
                {
                    case 0:
                        Debug.Log("Random Green");
                        colors = shadingSettings.baseGreenColors;
                        deltaH = random.Range(shadingSettings.greenHRange.x, shadingSettings.greenHRange.y);
                        break;
                    case 1:
                        Debug.Log("Random Red");
                        colors = shadingSettings.baseRedColors;
                        deltaH = random.Range(shadingSettings.redHRange.x, shadingSettings.redHRange.y);
                        break;
                    case 2:
                        colors = shadingSettings.baseBlueColors;
                        deltaH = random.Range(shadingSettings.blueHRange.x, shadingSettings.blueHRange.y);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                };

                shadingSettings.randomColors.shoreColLow =
                    ColorHelper.TweakHSV(colors.shoreColLow, deltaH, 0, 0);
                shadingSettings.randomColors.shoreColHigh =
                    ColorHelper.TweakHSV(colors.shoreColHigh, deltaH, 0, 0);
                shadingSettings.randomColors.flatColLowA =
                    ColorHelper.TweakHSV(colors.flatColLowA, deltaH, 0, 0);
                shadingSettings.randomColors.flatColHighA =
                    ColorHelper.TweakHSV(colors.flatColHighA, deltaH, 0, 0);
                shadingSettings.randomColors.flatColLowB =
                    ColorHelper.TweakHSV(colors.flatColLowB, deltaH, 0, 0);
                shadingSettings.randomColors.flatColHighB =
                    ColorHelper.TweakHSV(colors.flatColHighB, deltaH, 0, 0);
                shadingSettings.randomColors.steepLow =
                    ColorHelper.TweakHSV(colors.steepLow, deltaH, 0, 0);
                shadingSettings.randomColors.steepHigh =
                    ColorHelper.TweakHSV(colors.steepHigh, deltaH, 0, 0);

            }
            else
            {
                shadingSettings.randomColors.flatColLowA =
                    ColorHelper.Random(random, 0.45f, 0.6f, 0.7f, 0.8f);
                shadingSettings.randomColors.flatColHighA =
                    ColorHelper.TweakHSV(shadingSettings.randomColors.flatColLowA, random.SignedValue() * 0.2f, random.SignedValue() * 0.15f, random.Range(-0.25f, -0.2f));
                shadingSettings.randomColors.flatColLowB =
                    ColorHelper.Random(random, 0.45f, 0.6f, 0.7f, 0.8f);
                shadingSettings.randomColors.flatColHighB =
                    ColorHelper.TweakHSV(shadingSettings.randomColors.flatColLowB, random.SignedValue() * 0.2f, random.SignedValue() * 0.15f, random.Range(-0.25f, -0.2f));
                shadingSettings.randomColors.shoreColLow =
                    ColorHelper.Random(random, 0.2f, 0.3f, 0.9f, 1);
                shadingSettings.randomColors.shoreColHigh =
                    ColorHelper.TweakHSV(shadingSettings.randomColors.shoreColLow, random.SignedValue() * 0.2f, random.SignedValue() * 0.2f, random.Range(-0.3f, -0.2f));
                shadingSettings.randomColors.steepLow =
                    ColorHelper.Random(random, 0.3f, 0.7f, 0.4f, 0.6f);
                shadingSettings.randomColors.steepHigh =
                    ColorHelper.TweakHSV(shadingSettings.randomColors.steepLow, random.SignedValue() * 0.2f, random.SignedValue() * 0.2f, random.Range(-0.35f, -0.2f));
            }

            shadingSettings.mainColor = shadingSettings.randomColors.flatColLowA;
        }

        void ApplyColours(Material material, PlanetColors colors)
        {
            material.SetColor("_ShoreLow", colors.shoreColLow);
            material.SetColor("_ShoreHigh", colors.shoreColHigh);

            material.SetColor("_FlatLowA", colors.flatColLowA);
            material.SetColor("_FlatHighA", colors.flatColHighA);

            material.SetColor("_FlatLowB", colors.flatColLowB);
            material.SetColor("_FlatHighB", colors.flatColHighB);

            material.SetColor("_SteepLow", colors.steepLow);
            material.SetColor("_SteepHigh", colors.steepHigh);
        }

        [Serializable]
        public class PlanetShadingSettings : ShadingConfig
        {
            public PlanetColors baseGreenColors;
            public PlanetColors baseRedColors;
            public PlanetColors baseBlueColors;

            public PlanetColors randomColors;

            public Vector2 greenHRange;
            public Vector2 redHRange;
            public Vector2 blueHRange;
        }

        [System.Serializable]
        public struct PlanetColors
        {
            public Color shoreColLow;
            public Color shoreColHigh;
            public Color flatColLowA;
            public Color flatColHighA;
            public Color flatColLowB;
            public Color flatColHighB;
            public Color steepLow;
            public Color steepHigh;
        }


        [System.Serializable]
        public enum ColorType
        {
            Green,
            Red,
            Blue
        }
    }

}