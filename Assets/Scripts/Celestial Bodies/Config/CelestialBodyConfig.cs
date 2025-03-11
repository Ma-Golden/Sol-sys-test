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
        public CelestialBodyType bodyType;


        // Base class for setting shape, shading, and physical properties
        public Shape.Shape shape;
        //public Shading.Shading shading;
        //public PhysicalProperties.PhysicalProperties physicalProperties;


        private static readonly string[] testNames = { "Earth", "Mars", "Jupiter", "Saturn", "Uranus", "Neptune", "Pluto" };

        public void Init(CelestialBodyType type)
        {
            bodyName = testNames[Random.Range(0, testNames.Length)];
            UpdateCBodySettings(type);
        }

        public void UpdateCBodySettings(CelestialBodyType newType)
        {
            bodyType = newType;
            Shape.Shape sh = SystemSavingUtils.Instance.CreateFeatures(newType);

            shape = sh;

            sh.InitConfig();
            //shading = sd;
            //physics = ph;

            //sd.InitSettings();
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