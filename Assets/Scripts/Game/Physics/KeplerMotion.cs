using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering;


public class KeplerMotion : IPhysicsModel
{
    private float accumulatedTime = 0f;
    private Dictionary<VirtualBody, OrbitalElements> orbitalElements;
    
    public void InitializeBodies(VirtualBody[] bodies)
    {
        orbitalElements = new Dictionary<VirtualBody, OrbitalElements>();
        accumulatedTime = 0f;

        // Central body must be first in array
        for (int i = 0; i < bodies.Length; i++)
        {
            Vector3 centralPos = bodies[0].Position;
            Vector3 orbitingPos = bodies[i].Position;

            // Calculate orbital elements for each body
            Vector3 r = bodies[i].Position - bodies[0].Position;
            Vector3 v = bodies[i].Velocity;

            float periapsis = r.magnitude;    // Initial distance as periapsis (Closest point)
            //float apoapsis = periapsis;       // Estimate for apoapsis // TODO: More robust calculation of apoapsis
            float apoapsis = periapsis * 2f;


            orbitalElements[bodies[i]] = new OrbitalElements{
                Periapsis = periapsis,
                Apoapsis = apoapsis,
                InitialTime = 0, // Start 'counting' from now
                Period = 2 * Mathf.PI * Mathf.Sqrt(Mathf.Pow((periapsis + apoapsis) / 2, 3) / (bodies[0].Mass * 6.674f))
            };
        }
    }

    public void UpdateBodies(VirtualBody[] bodies, float timeStep)
    {
        if (bodies.Length < 2) return; // Need at least central and orbiting body

        // timeStep already includes simulation speed from bodySimulation
        accumulatedTime += timeStep;
        Vector3 centalPos = bodies[0].Position; // Set sun as center

        // For each orbiting body
        for (int i = 1; i < bodies.Length; i++)
        {
            if (!orbitalElements.ContainsKey(bodies[i])) continue; // Skip if body not initialized

            var elements = orbitalElements[bodies[i]];

            // Calculate position on orbit using simulation time
            float t = (accumulatedTime % elements.Period) / elements.Period;
            Vector3 orbitPos = CalculatePointOnOrbit(elements.Periapsis, elements.Apoapsis, t);
            bodies[i].Position = centalPos + new Vector3(orbitPos.x, 0, orbitPos.y);
        }
    }

    // Try double for increased accuracy
    private Vector2 CalculatePointOnOrbit (double periapsis, double apoapsis, float t)
    {
        double semiMajorLength = (apoapsis + periapsis) / 2;
        double linearEccentricity = semiMajorLength - periapsis; // Distance bet centre and focus
        double eccentricity = linearEccentricity / semiMajorLength; // (0 = perfect circle)
        double semiMinorLength = Math.Sqrt(Math.Pow(semiMajorLength, 2) - Math.Pow(linearEccentricity, 2));

        double meanAnomaly = t * Mathf.PI * 2;
        double eccentricAnomaly = SolveKepler(meanAnomaly, eccentricity);

        // Calculate position on the ellipse
        double pointX = semiMajorLength * (Math.Cos(eccentricAnomaly) - eccentricity);
        double pointY = semiMinorLength * Math.Sin(eccentricAnomaly);

        return new Vector2((float)pointX, (float)pointY);
    }

    private double SolveKepler(double meanAnomaly, double eccentricity, int maxIterations = 100)
    {
        const double h = 0.0001;
        const double acceptableError = 0.00000001;
        double guess = meanAnomaly;

        // Newton-Rhapson method
        for (int i = 0; i < maxIterations; i++)
        {
            double y = KeplerEquation(guess, meanAnomaly, eccentricity);

            // If within error bounds
            if (Math.Abs(y) < acceptableError)
            {
                break;
            }

            // Update guess to vlaue of x where the slope of the function intersects the x-axis
            double slope = (KeplerEquation(guess + h, meanAnomaly, eccentricity) - y) / h;
            double step = y / slope;
            guess -= step;
        }
        return guess;
    }

    // Kepler's equation: M = E - e * sin(E)
    // M = mean anomaly (angle to where body would be if its orbit was actually circular)
    // E is the Eccentric Anomaly (angle to where the body is on the ellipse)
    // e : eccentricity (0 = perfect circle etc.)
    private double KeplerEquation(double E, double M, double e)
    {
        return M - E + e * Math.Sin(E);
    }

    private struct OrbitalElements
    {
        public float Periapsis { get; set; }
        public float Apoapsis { get; set; }
        public float InitialTime;
        public float Period;
    }
}
