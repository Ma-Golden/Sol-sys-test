using UnityEditor;
using UnityEngine;

public class NBodyPhysics : IPhysicsModel
{
    public float gravityConstant = 6.674f;

    public void InitializeBodies(VirtualBody[] bodies)
    {
        // No updates needed;
        // Updates calculated at each step
    }

    public void UpdateBodies(VirtualBody[] bodies, float timeStep)
    {
        int numBodies = bodies.Length;
        Vector3[] accerlerations = new Vector3[numBodies];

        // Compute accelerations due to gravity from all other bodies

        for (int i = 0; i < numBodies; i++)
        {
            Vector3 acceleration = Vector3.zero;
            for (int j = 0; j < numBodies; j++)
            {
                if (i == j) continue; // Skip self

                // Newton's Law of universal gravitation: F = G * m1 * m2 / r^2

                Vector3 direction = bodies[j].Position - bodies[i].Position;
                float sqrDist = direction.sqrMagnitude + 0.001f; // Avoid div by zero error
                float dist = Mathf.Sqrt(sqrDist);

                acceleration += direction.normalized * gravityConstant * (bodies[j].Mass / sqrDist);
            }

            accerlerations[i] = acceleration;
        }

        // Apply accelerations to update positions

        for (int i = 0; i < numBodies; i++)
        {
            bodies[i].Velocity += accerlerations[i] * timeStep;
            bodies[i].Position += bodies[i].Velocity * timeStep;
        }

    }

}
