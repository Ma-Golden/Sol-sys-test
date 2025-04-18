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
        for (int i = 1; i < bodies.Length; i++)
        {
            Vector3 r = bodies[i].Position - bodies[0].Position;
            Vector3 v = bodies[i].Velocity;
            float r_mag = r.magnitude;
            float v_mag = v.magnitude;

            // Calculate specific angular momentum
            Vector3 h = Vector3.Cross(r, v);
            float h_mag = h.magnitude;

            // Calculate eccentricity vector
            Vector3 e_vec = Vector3.Cross(v, h) / (bodies[0].Mass * 6.674f) - r.normalized;
            float e = e_vec.magnitude;

            // Calculate semi-major axis
            float a = h_mag * h_mag / (bodies[0].Mass * 6.674f * (1 - e * e));

            // Calculate periapsis and apoapsis
            float periapsis = a * (1 - e);
            float apoapsis = a * (1 + e);

            // Calculate orbital period using Kepler's third law
            float period = 2 * Mathf.PI * Mathf.Sqrt(Mathf.Pow(a, 3) / (bodies[0].Mass * 6.674f));

            orbitalElements[bodies[i]] = new OrbitalElements
            {
                Periapsis = periapsis,
                Apoapsis = apoapsis,
                InitialTime = 0,
                Period = period,
                Eccentricity = e,
                SemiMajorAxis = a
            };

            Debug.Log($"Initialized orbit for body {i}: a={a}, e={e}, P={period}, r_p={periapsis}, r_a={apoapsis}");
        }
    }

    public void UpdateBodies(VirtualBody[] bodies, float timeStep)
    {
        if (bodies.Length < 2) return; // Need at least central and orbiting body

        accumulatedTime += timeStep;
        Vector3 centralPos = bodies[0].Position;

        // For each orbiting body
        for (int i = 1; i < bodies.Length; i++)
        {
            if (!orbitalElements.ContainsKey(bodies[i])) continue;

            var elements = orbitalElements[bodies[i]];
            float t = (accumulatedTime % elements.Period) / elements.Period;
            
            // Calculate position in orbital plane
            Vector3 orbitPos = CalculatePointOnOrbit(elements, t);
            
            // Update body position
            bodies[i].Position = centralPos + orbitPos;
        }
    }

    private Vector3 CalculatePointOnOrbit(OrbitalElements elements, float t)
    {
        // Calculate mean anomaly
        float meanAnomaly = t * 2 * Mathf.PI;
        
        // Solve Kepler's equation for eccentric anomaly
        float eccentricAnomaly = (float)SolveKepler(meanAnomaly, elements.Eccentricity);
        
        // Calculate true anomaly
        float trueAnomaly = 2 * Mathf.Atan2(
            Mathf.Sqrt(1 + elements.Eccentricity) * Mathf.Sin(eccentricAnomaly / 2),
            Mathf.Sqrt(1 - elements.Eccentricity) * Mathf.Cos(eccentricAnomaly / 2)
        );
        
        // Calculate distance from focus
        float distance = elements.SemiMajorAxis * (1 - elements.Eccentricity * elements.Eccentricity) / 
                        (1 + elements.Eccentricity * Mathf.Cos(trueAnomaly));
        
        // Calculate position in orbital plane
        Vector3 position = new Vector3(
            distance * Mathf.Cos(trueAnomaly),
            0,
            distance * Mathf.Sin(trueAnomaly)
        );
        
        return position;
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

            // Update guess to value of x where the slope of the function intersects the x-axis
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
        public float Eccentricity;
        public float SemiMajorAxis;
    }
}
