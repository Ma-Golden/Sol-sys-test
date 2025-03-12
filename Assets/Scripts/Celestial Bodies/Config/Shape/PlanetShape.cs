using UnityEngine;
using System.Collections.Generic;
using System;
using Unity.VisualScripting;
using System.Linq.Expressions;


namespace CelestialBodies.Config.Shape
{
    [CreateAssetMenu(fileName = "PlanetShape", menuName = "Scriptable Objects/Shape/PlanetShape")]
    public class PlanetShape : Shape
    {
        [SerializeField] public PlanetShapeConfig shapeConfig;
        protected override void SetShapeData()
        {
            // Allow reproducable 
            PRNG prng = new PRNG(shapeConfig.seed);

            // shape config
            shapeConfig.continentNoise.SetComputeValues(heightCompute, prng, "_continents");
            shapeConfig.ridgeNoise.SetComputeValues(heightCompute, prng, "_ridges");
            shapeConfig.maskNoise.SetComputeValues(heightCompute, prng, "_mask");
        }

        public override void InitConfig()
        {
            // TODO: CHECK OBSERVERS HERE
            return;
        }

        public override ShapeConfig GetConfig()
        {
            return shapeConfig;
        }


        public override void SetConfig(ShapeConfig psc)
        {
            shapeConfig = (PlanetShapeConfig)psc;
            shapeConfig.UpdateMountainHeights();

            if (Observers == null) return;
            foreach (ICelestialObserver o in Observers)
            {

                o.OnShapeUpdate();
            }
        }

        [Serializable]
        public class PlanetShapeConfig : ShapeConfig
        {
            [Header("Continent settings TODO")]
            public float oceanDepthMultiplier = 5;
            public float oceanFloorDepth = 1.4f;
            public float oceanFloorSmoothing = 0.5f;
            
            public float mountainBlend = 1f;
            public float baseMountainHeight = 0.02f;
            public float minMountainHeight = -0.8f;
            public float maxMountainHeight = 0.8f;
            
            // TODO check if needed
            public float continentFrequency = 1.0f;


            [Header("Noise settings")]
            public SimpleNoiseSettings continentNoise = new SimpleNoiseSettings();

            public SimpleNoiseSettings maskNoise = new SimpleNoiseSettings();

            public RidgeNoiseSettings ridgeNoise = new RidgeNoiseSettings();

            public Vector4 testParams = Vector4.zero;

            public void UpdateMountainHeights()
            {
                if (random)
                {
                    PRNG random = new PRNG(seed);
                    maskNoise.verticalShift = random.Range(minMountainHeight, maxMountainHeight);
                }
                else
                {
                    maskNoise.verticalShift = baseMountainHeight;
                }
            }
        } // ShapeConfig
    };
}
