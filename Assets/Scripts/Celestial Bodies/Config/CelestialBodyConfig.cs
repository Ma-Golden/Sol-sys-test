using System;
using System.Net.NetworkInformation;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CelestialBodies.Config
{
    [Serializable]
    public class CelestialBodyConfig
    {
        public string bodyName;
        private static readonly string[] testNames = { "Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune", "Pluto" };
        public CelestialBodyType bodyType;

        // Base class for setting shape, shading, and physical properties
        public Shape.Shape shape;
        public Shading.Shading shading;
        //public PhysicalProperties.PhysicalProperties physicalProperties;

        public void Init(CelestialBodyType type)
        {
            bodyName = testNames[Random.Range(0, testNames.Length)];
            UpdateCBodySettings(type);
        }

        public void SubscribeToShapeUpdates(ICelestialObserver observer)
        {
            shape.Subscribe(observer);

            observer.OnInitialUpdate();
        }



        public void UpdateCBodySettings(CelestialBodyType newType)
        {
            Debug.Log("Updating Celestial Body Settings");
            bodyType = newType;
            
            Debug.Log("shape = SystemSavingUtils.Instance.CreateFeatures(newType)");            
            (Shape.Shape sp, Shading.Shading sd) = SystemSavingUtils.Instance.CreateFeatures(newType);


            Debug.Log("shape = sp; etc");
            shape = sp;
            shading = sd;
            //physics = ph;


            Debug.Log("sd.InitConfig");
            sp.InitConfig();

            Debug.Log("shading init config");
            sd.InitSettings();
            //ph.InitSettings();

        }


        [Serializable]
        public enum CelestialBodyType // Possible types of celestial bodies
        {
            Planet,
            Moon,
            Star,
        }
    }
}