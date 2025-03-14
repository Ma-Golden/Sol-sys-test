using System;
using CelestialBodies.Config;
using CelestialBodies.Config.Shading;
using CelestialBodies.Config.Shape;
using UnityEngine;


public class SystemSavingUtils : MonoBehaviour
{
    public static SystemSavingUtils Instance; // single instance of this class

    public StarSystemConfig currentSystemConfig;

    public string storePath = null;
    public string testStorePath = null;


    // N.B TESTING ONLY -> HARDCODED SHAPES
    [Header("Shapes")]
    public PlanetShape PlanetShape;
    public MoonShape MoonShape;


    [Header("Shading")]
    public Shading PlanetShading;
    public Shading MoonShading;


    private void Awake()
    {
        Instance = this;
    }

    // N.B ONLY RETURNS SHAPE ATM
    public (Shape shape, Shading shading) CreateFeatures(CelestialBodyConfig.CelestialBodyType celestialBodyType)
    {
        Shape shape = null;
        Shading shading = null;
        
        // TODO: add more features

        switch (celestialBodyType)
        {
            case CelestialBodyConfig.CelestialBodyType.Planet:
                shape = PlanetShape;
                shading = PlanetShading; 
                break;
            case CelestialBodyConfig.CelestialBodyType.Moon:
                Debug.Log("Moon shape (Create Features)");
                shape = MoonShape;
                
                Debug.Log("Moon shading (Create Features)");
                shading = MoonShading;
                
                
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return (shape, shading);
    }
};
