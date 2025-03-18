using System;
using CelestialBodies;
using UnityEngine;

namespace CelestialBodies.Config.Shape
{
    [Serializable][CreateAssetMenu(fileName = "MoonShape", menuName = "Scriptable Objects/Shape/Moon Shape")]
    public class MoonShape : Shape
    {
        [SerializeField] public MoonShapeConfig shapeConfig;


        protected override void SetShapeData()
        {
            PRNG prng = new PRNG(shapeConfig.seed);

            SetCraterSettings(prng, shapeConfig.seed, shapeConfig.random);

            // TODO : use these in compute shader
            shapeConfig.shapeNoise.SetComputeValues(heightCompute, prng, "_shape");
            shapeConfig.ridgeNoise.SetComputeValues(heightCompute, prng, "_ridge");
            shapeConfig.ridgeNoise2.SetComputeValues(heightCompute, prng, "_ridge2");

            // TODO RUN WITH TESTER (SIN WAVE
            heightCompute.SetFloat("testValue", shapeConfig.testHeight);
        }


        void SetCraterSettings(PRNG prng, int seed, bool randomizeValues)
        {
            if (randomizeValues)
            {
                var chance = new Chance(prng);
                if (chance.Percent(70))
                { // Medium amount of mostly small to medium craters
                    shapeConfig.craterSettings.SetComputeValues(heightCompute, seed, prng.Range(100, 700), new Vector2(0.01f, 0.1f), 0.57f);
                }
                else if (chance.Percent(15))
                { // Many small craters
                    shapeConfig.craterSettings.SetComputeValues(heightCompute, seed, prng.Range(800, 1800), new Vector2(0.01f, 0.08f), 0.74f);
                }
                else if (chance.Percent(15))
                { // A few large craters
                    shapeConfig.craterSettings.SetComputeValues(heightCompute, seed, prng.Range(50, 150), new Vector2(0.01f, 0.2f), 0.4f);
                }
            }
            else
            {
                shapeConfig.craterSettings.SetComputeValues(heightCompute, seed);
            }
        }


        public override ShapeConfig GetConfig()
        {
            return shapeConfig;
        }

        public override void InitConfig()
        {
            if (Observers == null) return;
            foreach (ICelestialObserver observer in Observers)
            {
                observer.OnShapeUpdate();
            }
        }

        public override void SetConfig(ShapeConfig shapeConfigIn)
        {
            shapeConfig = (MoonShapeConfig)shapeConfigIn;

            if (Observers == null) return;
            foreach (ICelestialObserver observer in Observers)
            {
                observer.OnShapeUpdate();
            }

        }

        [Serializable]
        public class MoonShapeConfig : ShapeConfig
        {
            [Header("Test shape settings")]
            public float testHeight = 1f;

            [Header("Noise settings")]
            public CraterSettings craterSettings = new CraterSettings();
            public SimpleNoiseSettings shapeNoise = new SimpleNoiseSettings();
            public RidgeNoiseSettings ridgeNoise = new RidgeNoiseSettings();
            public RidgeNoiseSettings ridgeNoise2 = new RidgeNoiseSettings();
        }
    }
}
