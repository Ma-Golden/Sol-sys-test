using System.Collections.Generic;
using UnityEngine;



using UnityEngine;

public class KeplerianPhysics : IPhysicsModel
{
    public void InitializeBodies(VirtualBody[] bodies)
    {
        // Nothing needs to be done here as we'll compute orbital elements on-the-fly
    }

    public void UpdateBodies(VirtualBody[] bodies, float timeStep)
    {
        // Skip the first body (assumed to be the central body)
        for (int i = 1; i < bodies.Length; i++)
        {
            Vector3 newPosition = ComputePosition(bodies[0], bodies[i], timeStep);
            
            // Validate position before assigning
            if (!HasNaN(newPosition))
            {
                bodies[i].Position = newPosition;
            }
            else
            {
                Debug.LogWarning($"Calculated NaN position for body {i}. Skipping update.");
            }
        }
    }

    private Vector3 ComputePosition(VirtualBody centralBody, VirtualBody orbitingBody, float timeStep)
    {
        // Calculate the gravitational parameter (G * M)
        float mu = centralBody.Mass;
        
        // Get initial position and velocity relative to central body
        Vector3 r0 = orbitingBody.Position - centralBody.Position;
        Vector3 v0 = orbitingBody.Velocity;
        
        float r0Mag = r0.magnitude;
        
        // Guard against division by zero
        if (r0Mag < 1e-6f)
        {
            Debug.LogWarning("Body too close to central body. Using a minimum safe distance.");
            r0 = new Vector3(1e-6f, 0, 0);
            r0Mag = 1e-6f;
        }
        
        // Calculate orbital elements using current state
        Vector3 h = Vector3.Cross(r0, v0);                    // Angular momentum vector
        Vector3 n = Vector3.Cross(Vector3.up, h);             // Node vector
        Vector3 e = Vector3.Cross(v0, h) / mu - r0 / r0Mag;   // Eccentricity vector
        
        float eMag = e.magnitude;                             // Eccentricity
        
        // Clamp eccentricity to avoid numerical issues
        eMag = Mathf.Clamp(eMag, 0, 0.999f);
        
        // Calculate semi-major axis
        float v0Mag = v0.magnitude;
        float a;
        
        // Energy equation
        float energy = (v0Mag * v0Mag / 2) - (mu / r0Mag);
        
        // Handle different orbit types
        if (Mathf.Abs(energy) < 1e-10f)  // Parabolic
        {
            a = float.MaxValue;
            Debug.LogWarning("Near-parabolic orbit detected. Using elliptical approximation.");
            eMag = 0.9f;  // Force slightly elliptical
        }
        else if (energy >= 0)  // Hyperbolic
        {
            Debug.LogWarning("Hyperbolic orbit detected. Using elliptical approximation.");
            eMag = 0.9f;  // Force slightly elliptical
            a = 100f;     // Use large semi-major axis
        }
        else  // Elliptical
        {
            a = -mu / (2 * energy);
        }
        
        // Calculate orbital period
        float period = 2 * Mathf.PI * Mathf.Sqrt(a * a * a / mu);
        
        // Calculate initial true anomaly
        float trueAnomaly0 = Mathf.Acos(Mathf.Clamp(Vector3.Dot(e, r0) / (eMag * r0Mag), -1f, 1f));
        if (Vector3.Dot(r0, v0) < 0)
        {
            trueAnomaly0 = 2 * Mathf.PI - trueAnomaly0;
        }
        
        // Calculate initial mean anomaly
        float E0 = 2 * Mathf.Atan(Mathf.Tan(trueAnomaly0 / 2) * Mathf.Sqrt((1 - eMag) / (1 + eMag)));
        float M0 = E0 - eMag * Mathf.Sin(E0);
        
        // Advance mean anomaly by timeStep
        float M = M0 + (2 * Mathf.PI * timeStep / period);
        
        // Solve Kepler's equation for eccentric anomaly
        float E = SolveKepler(M, eMag);
        
        // Calculate new true anomaly
        float trueAnomaly = 2 * Mathf.Atan(Mathf.Sqrt((1 + eMag) / (1 - eMag)) * Mathf.Tan(E / 2));
        
        // Calculate distance from focus
        float r = a * (1 - eMag * Mathf.Cos(E));
        
        // Determine orientation vectors for the orbital plane
        Vector3 xAxis = e.normalized;
        Vector3 zAxis = h.normalized;
        Vector3 yAxis = Vector3.Cross(zAxis, xAxis);
        
        // Calculate position in orbital plane
        Vector3 positionInPlane = r * new Vector3(Mathf.Cos(trueAnomaly), Mathf.Sin(trueAnomaly), 0);
        
        // Transform to world space
        Vector3 position = centralBody.Position + 
                           positionInPlane.x * xAxis + 
                           positionInPlane.y * yAxis;
        
        return position;
    }
    
    private float SolveKepler(float M, float e)
    {
        // Ensure M is in the range [0, 2π)
        M = M % (2 * Mathf.PI);
        if (M < 0) M += 2 * Mathf.PI;
        
        // Initial guess
        float E = M;
        
        // For low eccentricity, M is a good approximation
        if (e < 0.1f)
        {
            E = M;
        }
        // For high eccentricity, better initial guess
        else if (M < Mathf.PI)
        {
            E = M + e / 2;
        }
        else
        {
            E = M - e / 2;
        }
        
        // Newton-Raphson iteration
        for (int i = 0; i < 10; i++)
        {
            float E_next = E - (E - e * Mathf.Sin(E) - M) / (1 - e * Mathf.Cos(E));
            
            // Check for convergence
            if (Mathf.Abs(E_next - E) < 1e-6f)
            {
                return E_next;
            }
            
            E = E_next;
        }
        
        return E;  // Return best approximation after iterations
    }
    
    private bool HasNaN(Vector3 v)
    {
        return float.IsNaN(v.x) || float.IsNaN(v.y) || float.IsNaN(v.z);
    }
}

// public class KeplerianPhysics : IPhysicsModel
// {
//     public void InitializeBodies(VirtualBody[] bodies)
//     {
//         // Get keplerian params from initial state of the body
//         foreach (var body in bodies)
//         {
//             OrbitalElements.ComputeFromState(body);
//         }
//     }

//     public void UpdateBodies(VirtualBody[] bodies, float timeStep)
//     {
//         foreach (var body in bodies)
//         {
//             body.Position = OrbitalElements.ComputePositionAtTime(body, timeStep);
//         }
//     }

//     public static class OrbitalElements
//     {
//         public static float mu = 1; // TODO: use acutal G * M for central body


//         public struct Elements
//         {
//             public float semiMajorAxis;
//             public float eccentricity;
//             public float meanAnomalyAtEpoch;
//             public float orbitalPeriod;
//             public float initialTime;
//             public Vector3 orbitCenter; // For elliptical orbits
//             public Vector3 periapisDirection; // Unit vector to periapis
//         }

//         private static Dictionary<VirtualBody, Elements> bodyElements = new();

//         // Calculates the keplerian parameters of a body given its initial state
//         public static void ComputeFromState(VirtualBody body)
//         {

//             // LEARN WHAT THE F IS GOING ON HERE
//             // LEARN WHAT THE F IS GOING ON HERE
//             // LEARN WHAT THE F IS GOING ON HERE

//             Vector3 r = body.Position;
//             Vector3 v = body.Velocity;
            
//             float rMag = r.magnitude;
//             float vMag = v.magnitude;

//             float specificEngery = (vMag * vMag) / 2 - mu / rMag;
//             float a = -mu / (2f * specificEngery); // Semi-major axis

//             Vector3 h = Vector3.Cross(r, v); // Specifi angular momentum
//             Vector3 eVec = (Vector3.Cross(v, h) / mu) - (r / rMag); // Eccentricity vector
//             float e = eVec.magnitude; // Eccentricity

//             float period = 2 * Mathf.PI * Mathf.Sqrt(a * a * a / mu);

//             float trueAnomaly = Mathf.Acos(Vector3.Dot(eVec.normalized, r.normalized));

//             if (Vector3.Dot(r, v) < 0)
//             {
//                 trueAnomaly = 2f * Mathf.PI - trueAnomaly;
//             }

//             float E = 2f * Mathf.Atan(Mathf.Tan(trueAnomaly / 2f) / Mathf.Sqrt((1f + e) / (1f - e))); // Eccentric anomaly

//             float M = E - e * Mathf.Sin(E);


//             bodyElements[body] = new Elements
//             {
//                 semiMajorAxis = a,
//                 eccentricity = e,
//                 meanAnomalyAtEpoch = M,
//                 orbitalPeriod = period,
//                 initialTime = Time.time,
//                 // TODO : CHECK THIS
//                 orbitCenter = Vector3.zero, // ORIGIN ASSUMED
//                 //orbitCenter = body.Position,
//                 periapisDirection = eVec.normalized
//             };
//         }

//         public static Vector3 ComputePositionAtTime(VirtualBody body, float timeStep)
//         {
//             if (!bodyElements.TryGetValue(body, out Elements el))
//             {
//                 return body.Position;
//             }

//             float t = Time.time - el.initialTime;
//             float M = el.meanAnomalyAtEpoch + 2f * Mathf.PI * (t / el.orbitalPeriod);
//             M = M % (2f * Mathf.PI);

//             float E = SolveKeplersEquation(M, el.eccentricity, 1e-6f);

//             float x = el.semiMajorAxis * (Mathf.Cos(E) - el.eccentricity);
//             float y = el.semiMajorAxis * Mathf.Sqrt(1 - el.eccentricity * el.eccentricity) * Mathf.Sin(E);

//             Vector3 pos = new Vector3(x, y, 0);
//             Quaternion rotation = Quaternion.FromToRotation(Vector3.right, el.periapisDirection);
//             return el.orbitCenter + rotation * pos;
//         }

//         private static float SolveKeplersEquation(float M, float e, float tolerance)
//         {
//             // LEARN
//             // LEARN

//             float E = M;

//             // TODO: WHY 10
//             for (int i = 0; i < 10; i++)
//             {
//                 float delta = (E - e * Mathf.Sin(E) - M) / (1 - e * Mathf.Cos(E));
//                 E -= delta;
//                 if (Mathf.Abs(delta) < tolerance)
//                 {
//                     break;
//                 }
//             }
//             return E;
//         }



//     }



// }