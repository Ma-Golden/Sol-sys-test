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
    public StarShape StarShape;

    [Header("Shading")]
    public PlanetShading PlanetShading;
    public MoonShading MoonShading;
    public StarShading StarShading;

    [Header("Physics")]
    public Physics basePhysics;

    [Header("Ocean")]
    public Ocean baseOcean;


    private void Awake()
    {
        Instance = this;
    }

    // N.B ONLY RETURNS SHAPE ATM
    public (Shape shape, Shading shading, Ocean ocean, Physics physics) CreateFeatures(CelestialBodyConfig.CelestialBodyType celestialBodyType)
    {
        Shape shape = null;
        Shading shading = null;
        Ocean ocean = null;
        Physics physics = null;

        // TODO:
        // !TODO: fix physics getphysicalconfig
        // TODO:
        // !TODO:    ADD SPECIFIC PHYSICS FOR EACH CELESTIAL BODY TYPE
        // TODO:
        // TODO:
        // TODO:

        ocean = (baseOcean);

        switch (celestialBodyType)
        {
            case CelestialBodyConfig.CelestialBodyType.Planet:
                shape = PlanetShape;
                shading = PlanetShading; 
                ocean = baseOcean;
                physics = basePhysics;
                break;
            case CelestialBodyConfig.CelestialBodyType.Moon:
                shape = MoonShape;
                shading = MoonShading;
                physics = basePhysics;
                break;
            case CelestialBodyConfig.CelestialBodyType.Star:
                shape = StarShape;
                shading = StarShading;
                physics = basePhysics;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return (shape, shading, ocean, physics);
    }
};
