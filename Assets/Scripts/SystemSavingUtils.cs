using System;
using CelestialBodies.Config;
using CelestialBodies.Config.Shape;
using UnityEngine;


public class SystemSavingUtils : MonoBehaviour
{
    public static SystemSavingUtils Instance; // single instance of this class

    public StarSystemConfig currentSystemConfig;

    public string storePath = null;
    public string testStorePath = null;

    [Header("Shapes")]
    public PlanetShape PlanetShape;

    private void Awake()
    {
        Instance = this;
    }

    // N.B ONLY RETURNS SHAPE ATM
    public Shape CreateFeatures(CelestialBodyConfig.CelestialBodyType celestialBodyType)
    {
        Shape shape = null;
        // TODO: add more features

        switch (celestialBodyType)
        {
            case CelestialBodyConfig.CelestialBodyType.Planet:
                shape = PlanetShape;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return shape;
    }
};
