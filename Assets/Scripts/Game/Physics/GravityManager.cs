using UnityEngine;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

public class GravityManager : MonoBehaviour
{
    public List<CelestialBody> bodies;

    private void Awake()
    {
        bodies = new List<CelestialBody>();
    }

    //public List<CelestialBody> bodies;
    public float gravityConstant = 6.674f;

    private void FixedUpdate()
    {
        ApplyGravity();
    }

    private void ApplyGravity()
    {
        foreach (CelestialBody bodyA in bodies)
        {
            foreach (CelestialBody bodyB in bodies)
            {
                if (bodyA != bodyB)
                {
                    Vector3 dir = bodyB.position - bodyA.position;
                    float distanceSqr = dir.sqrMagnitude;
                    if (distanceSqr == 0) continue;

                    float forceMagnitude = gravityConstant * (bodyA.mass *  bodyB.mass) / distanceSqr;
                    Vector3 force = dir.normalized * forceMagnitude;
                    bodyA.ApplyForce(force);
                }
            }
        }
    }
} 
