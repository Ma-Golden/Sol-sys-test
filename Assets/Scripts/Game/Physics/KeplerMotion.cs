using System.Collections.Generic;
using UnityEngine;


public class KeplerianPhysics : IPhysicsModel
{
    public void InitializeBodies(VirtualBody[] bodies)
    {
        throw new System.NotImplementedException();
    }

    public static class OrbitalElements
    {
        public static float mu = 1; // TODO: use acutal G * M for central body


        public struct Elements
        {
            public float semiMajorAxis;
            public float eccentricity;
            public float meanAnomalyAtEpoch;
            public float orbitalPeriod;
            public float initialTime;
            public Vector3 orbitCenter; // For elliptical orbits
            public Vector3 periapisDirection; // Unit vector to periapis
        }

        private static Dictionary<VirtualBody, Elements> bodyElements = new();

        // Calculates the keplerian parameters of a body given its initial state
        public static void ComputeFromState(VirtualBody body)
        {

            // LEARN WHAT THE F IS GOING ON HERE
            // LEARN WHAT THE F IS GOING ON HERE
            // LEARN WHAT THE F IS GOING ON HERE

            Vector3 r = body.Position;
            Vector3 v = body.Velocity;
            
            float rMag = r.magnitude;
            float vMag = v.magnitude;

            float specificEngery = (vMag * vMag) / 2 - mu / rMag;
            float a = -mu / (2f * specificEngery); // Semi-major axis

            Vector3 h = Vector3.Cross(r, v); // Specifi angular momentum
            Vector3 eVec = (Vector3.Cross(v, h) / mu) - (r / rMag); // Eccentricity vector
            float e = eVec.magnitude; // Eccentricity

            float period = 2 * Mathf.PI * Mathf.Sqrt(a * a * a / mu);

            float trueAnomaly = Mathf.Acos(Vector3.Dot(eVec.normalized, r.normalized));

            if (Vector3.Dot(r, v) < 0)
            {
                trueAnomaly = 2f * Mathf.PI - trueAnomaly;
            }

            float E = 2f * Mathf.Atan(Mathf.Tan(trueAnomaly / 2f) / Mathf.Sqrt((1f + e) / (1f - e))); // Eccentric anomaly

            float M = E - e * Mathf.Sin(E);


            bodyElements[body] = new Elements
            {
                semiMajorAxis = a,
                eccentricity = e,
                meanAnomalyAtEpoch = M,
                orbitalPeriod = period,
                initialTime = Time.time,
                // TODO : CHECK THIS
                orbitCenter = Vector3.zero, // ORIGIN ASSUMED
                //orbitCenter = body.Position,
                periapisDirection = eVec.normalized
            };
        }

        public static Vector3 ComputePositionAtTime(VirtualBody body, float timeStep)
        {
            if (!bodyElements.TryGetValue(body, out Elements el))
            {
                return body.Position;
            }

            float t = Time.time - el.initialTime;
            float M = el.meanAnomalyAtEpoch + 2f * Mathf.PI * (t / el.orbitalPeriod);
            M = M % (2f * Mathf.PI);

//            float E = SolveKeplerEquation




            return Vector3.zero;
        }

        private static float SolveKeplersEquation(float M, float e, float tolerance)
        {

            // CONTINUE HERE
            // CONTINUE HERE
            // CONTINUE HERE
            return 0f;
        }



    }



}