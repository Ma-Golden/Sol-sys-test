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


        // TODO: check placement of radous + other physical properties
        public float radius = 5;



        public void Init(CelestialBodyType type)
        {
            bodyName = testNames[Random.Range(0, testNames.Length)];
            UpdateCBodySettings(type);
        }


        public void Subscribe(ICelestialObserver observer)
        {
            shape.Subscribe(observer);
            shading.Subscribe(observer);
            //physicalProperties.Subscribe(observer);

            observer.OnInitialUpdate();
        }


        public void SubscribeToShapeUpdates(ICelestialObserver observer)
        {
            shape.Subscribe(observer);
            shading.Subscribe(observer);

            observer.OnInitialUpdate();
        }



        public void UpdateCBodySettings(CelestialBodyType newType)
        {
            bodyType = newType;
            
            (Shape.Shape sp, Shading.Shading sd) = SystemSavingUtils.Instance.CreateFeatures(newType);

            shape = sp;
            shading = sd;
            //physics = ph;


            if (shape == null)
            {
                Debug.LogError("Shape is null");
            }

            if (shading == null)
            {
                Debug.LogError("Shading is null");
            }


            Debug.Log("shape init config");
            sp.InitConfig();
            
            Debug.Log("shading init config");
            sd.InitConfig();
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