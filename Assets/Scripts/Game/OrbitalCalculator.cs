using UnityEngine;
using System;
using CelestialBodies.Config;

/// <summary>
/// Provides utilities for calculating orbital parameters
/// </summary>
public static class OrbitalCalculator
{
    // Gravitational constant (scaled for the simulation)
    private static readonly float G = GameManager.Instance.GetGravityConstant();

    /// <summary>
    /// Calculates the velocity needed for a circular orbit at a given distance from a central body
    /// </summary>
    /// <param name="centralMass">Mass of the central body</param>
    /// <param name="orbitRadius">Distance from the central body</param>
    /// <returns>The orbital velocity magnitude</returns>
    public static float CalculateCircularOrbitVelocity(float centralMass, float orbitRadius)
    {
        if (orbitRadius <= 0)
            throw new ArgumentException("Orbit radius must be positive");

        return Mathf.Sqrt(G * centralMass / orbitRadius);
    }

    /// <summary>
    /// Creates a planet in a circular orbit around a central body
    /// </summary>
    /// <param name="centralBody">The central body (usually a star)</param>
    /// <param name="orbitRadius">Distance from the central body</param>
    /// <param name="orbitInclination">Inclination of the orbit in degrees</param>
    /// <param name="startAngle">Starting angle around the orbit in degrees</param>
    /// <param name="planetMass">Mass of the planet</param>
    /// <param name="planetRadius">Radius of the planet</param>
    /// <returns>Position and velocity vectors for the planet</returns>
    public static (Vector3 position, Vector3 velocity) CalculateOrbitPositionVelocity(
        CelestialBody centralBody,
        float orbitRadius,
        float orbitInclination,
        float startAngle,
        float planetMass,
        float planetRadius)
    {
        if (centralBody == null)
            throw new ArgumentNullException(nameof(centralBody));

        // Convert angles to radians
        float inclinationRad = orbitInclination * Mathf.Deg2Rad;
        float startAngleRad = startAngle * Mathf.Deg2Rad;

        // Calculate position in orbital plane
        float x = orbitRadius * Mathf.Cos(startAngleRad);
        float z = orbitRadius * Mathf.Sin(startAngleRad);

        // Apply inclination
        float y = z * Mathf.Sin(inclinationRad);
        z = z * Mathf.Cos(inclinationRad);

        // Calculate position vector
        Vector3 position = centralBody.position + new Vector3(x, y, z);

        // Calculate orbital speed for circular orbit
        float orbitalSpeed = CalculateCircularOrbitVelocity(centralBody.mass, orbitRadius);

        // Calculate velocity direction (perpendicular to position vector in orbital plane)
        Vector3 velocityDirection = new Vector3(-Mathf.Sin(startAngleRad), 0, Mathf.Cos(startAngleRad));

        // Apply inclination to velocity
        Vector3 velocity = orbitalSpeed * new Vector3(
            velocityDirection.x,
            velocityDirection.z * Mathf.Sin(inclinationRad),
            velocityDirection.z * Mathf.Cos(inclinationRad)
        );

        return (position, velocity);
    }

    /// <summary>
    /// Calculates orbital period using Kepler's third law
    /// </summary>
    /// <param name="centralMass">Mass of the central body</param>
    /// <param name="semiMajorAxis">Semi-major axis of the orbit (for circular orbit equals radius)</param>
    /// <returns>Orbital period</returns>
    public static float CalculateOrbitalPeriod(float centralMass, float semiMajorAxis)
    {
        return 2 * Mathf.PI * Mathf.Sqrt(Mathf.Pow(semiMajorAxis, 3) / (G * centralMass));
    }

    /// <summary>
    /// Calculates parameters for an elliptical orbit
    /// </summary>
    /// <param name="centralBody">The central body</param>
    /// <param name="semiMajorAxis">Semi-major axis of the ellipse</param>
    /// <param name="eccentricity">Eccentricity of the ellipse (0-1)</param>
    /// <param name="inclination">Inclination of the orbit in degrees</param>
    /// <param name="longitudeOfAscendingNode">Longitude of ascending node in degrees</param>
    /// <param name="argumentOfPeriapsis">Argument of periapsis in degrees</param>
    /// <param name="trueAnomaly">True anomaly (angle from periapsis) in degrees</param>
    /// <returns>Position and velocity vectors for the orbiting body</returns>
    public static (Vector3 position, Vector3 velocity) CalculateEllipticalOrbit(
        CelestialBody centralBody,
        float semiMajorAxis,
        float eccentricity,
        float inclination,
        float longitudeOfAscendingNode,
        float argumentOfPeriapsis,
        float trueAnomaly)
    {
        if (centralBody == null)
            throw new ArgumentNullException(nameof(centralBody));

        if (eccentricity < 0 || eccentricity >= 1)
            throw new ArgumentException("Eccentricity must be in range [0,1)");

        // Convert angles to radians
        float incRad = inclination * Mathf.Deg2Rad;
        float loanRad = longitudeOfAscendingNode * Mathf.Deg2Rad;
        float aopRad = argumentOfPeriapsis * Mathf.Deg2Rad;
        float taRad = trueAnomaly * Mathf.Deg2Rad;

        // Calculate distance from focus
        float distance = semiMajorAxis * (1 - eccentricity * eccentricity) / (1 + eccentricity * Mathf.Cos(taRad));

        // Position in orbital plane
        Vector3 positionOrbital = new Vector3(
            distance * Mathf.Cos(taRad),
            0,
            distance * Mathf.Sin(taRad)
        );

        // Calculate the standard gravitational parameter
        float mu = G * centralBody.mass;

        // Calculate flight path angle (angle between velocity vector and local horizontal)
        float flightPathAngle = Mathf.Atan(eccentricity * Mathf.Sin(taRad) / (1 + eccentricity * Mathf.Cos(taRad)));

        // Calculate orbital speed at this point
        float p = semiMajorAxis * (1 - eccentricity * eccentricity); // semi-latus rectum
        float speed = Mathf.Sqrt(mu * (2f / distance - 1f / semiMajorAxis));

        // Velocity direction in orbital plane
        float vx = -Mathf.Sin(taRad) * Mathf.Cos(flightPathAngle) - Mathf.Cos(taRad) * Mathf.Sin(flightPathAngle);
        float vz = Mathf.Cos(taRad) * Mathf.Cos(flightPathAngle) - Mathf.Sin(taRad) * Mathf.Sin(flightPathAngle);

        Vector3 velocityOrbital = speed * new Vector3(vx, 0, vz);

        // Perform 3D rotations for orbital orientation
        Matrix4x4 rotMatrix = CalculateRotationMatrix(incRad, loanRad, aopRad);

        // Apply rotation to position and velocity
        Vector3 positionWorld = rotMatrix.MultiplyPoint3x4(positionOrbital) + centralBody.position;
        Vector3 velocityWorld = rotMatrix.MultiplyVector(velocityOrbital);

        return (positionWorld, velocityWorld);
    }

    // Calculate rotation matrix for orbital elements
    private static Matrix4x4 CalculateRotationMatrix(float inclination, float longitudeOfAscendingNode, float argumentOfPeriapsis)
    {
        // Create rotation matrices for each angle
        Matrix4x4 rotLoan = Matrix4x4.Rotate(Quaternion.Euler(0, 0, longitudeOfAscendingNode * Mathf.Rad2Deg));
        Matrix4x4 rotInc = Matrix4x4.Rotate(Quaternion.Euler(inclination * Mathf.Rad2Deg, 0, 0));
        Matrix4x4 rotAop = Matrix4x4.Rotate(Quaternion.Euler(0, 0, argumentOfPeriapsis * Mathf.Rad2Deg));

        // Combine rotations: first rotate by LOAN, then by inclination, then by AOP
        return rotAop * rotInc * rotLoan;
    }

    /// <summary>
    /// Sets up a moon in orbit around a planet
    /// </summary>
    /// <param name="planetBody">The planet to orbit</param>
    /// <param name="orbitRadius">Orbit radius from planet center</param>
    /// <param name="moonMass">Mass of the moon</param>
    /// <param name="moonRadius">Radius of the moon</param>
    /// <param name="startAngle">Initial angle in degrees</param>
    /// <returns>Position and velocity for the moon</returns>
    public static (Vector3 position, Vector3 velocity) SetupMoonOrbit(
        CelestialBody planetBody,
        float orbitRadius,
        float moonMass,
        float moonRadius,
        float startAngle)
    {
        // Use the same functions as for planets, but with the planet as the central body
        return CalculateOrbitPositionVelocity(
            planetBody,
            orbitRadius,
            0f, // No inclination for simplicity
            startAngle,
            moonMass,
            moonRadius
        );
    }

    /// <summary>
    /// Automatically calculates a stable orbit distance based on body sizes
    /// </summary>
    /// <param name="centralBody">The central body</param>
    /// <param name="bodyRadius">Radius of the orbiting body</param>
    /// <returns>A recommended orbit distance</returns>
    public static float RecommendOrbitDistance(CelestialBody centralBody, float bodyRadius)
    {
        if (centralBody == null || centralBody.celestiaBodyGenerator == null ||
            centralBody.celestiaBodyGenerator.bodyConfig == null)
            return 20f; // Default fallback

        // Get central body radius
        float centralRadius = centralBody.celestiaBodyGenerator.bodyConfig.radius;

        // Establish minimum safe distance (e.g., 3x the combined radii)
        float minDistance = (centralRadius + bodyRadius) * 3f;

        // For stars, scale based on mass
        if (centralBody.celestiaBodyGenerator.bodyConfig.bodyType == CelestialBodyConfig.CelestialBodyType.Star)
        {
            return minDistance * Mathf.Sqrt(centralBody.mass / 10f);
        }

        return minDistance;
    }
}