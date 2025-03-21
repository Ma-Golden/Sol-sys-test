using System;
using UnityEngine;

namespace CelestialBodies.Config.Shape
{
    [Serializable]
    [CreateAssetMenu(fileName = "MoonShape", menuName = "Scriptable Objects/Shape/Star Shape")]
    public class StarShape : Shape
    {
        [SerializeField] public StarShapeConfig shapeConfig;

        public override void InitConfig()
        {
            if (Observers == null)
            {
                return;
            }
            
            foreach (ICelestialObserver o in Observers)
            {
                o.OnShapeUpdate();
            }            
        }

        public override void SetConfig(ShapeConfig shapeConfig)
        {
            shapeConfig = (StarShapeConfig)shapeConfig;

            if (Observers == null)
            {
                return;
            }

            foreach (ICelestialObserver o in Observers)
            {
                o.OnShapeUpdate();
            }
        }
        public override ShapeConfig GetConfig()
        {
            return shapeConfig;
        }

        [Serializable]
        public class StarShapeConfig : ShapeConfig
        {
            // Stars are all spherical
        }
    }







}