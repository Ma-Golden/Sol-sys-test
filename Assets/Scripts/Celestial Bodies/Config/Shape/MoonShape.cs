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
            // TODO : use these in compute shader
            shapeConfig.shapeNoise.SetComputeValues(heightCompute, prng, "_shape");
            shapeConfig.ridgeNoise.SetComputeValues(heightCompute, prng, "_ridges");
            shapeConfig.ridgeNoise2.SetComputeValues(heightCompute, prng, "_ridges2");

            // TODO RUN WITH TESTER (SIN WAVE
            heightCompute.SetFloat("testValue", shapeConfig.testHeight);
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
            public SimpleNoiseSettings shapeNoise = new SimpleNoiseSettings();
            public RidgeNoiseSettings ridgeNoise = new RidgeNoiseSettings();
            public RidgeNoiseSettings ridgeNoise2 = new RidgeNoiseSettings();
        }
    }
}
