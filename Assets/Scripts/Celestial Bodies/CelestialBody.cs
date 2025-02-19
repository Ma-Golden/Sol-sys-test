using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    // Essential Vars for all cBodies
    public float mass = 1f;
    public Vector3 velocity;
    public Vector3 position;

    // N.B CHECK METHOD FOR CALCULATING FORCES

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        position = transform.position;
    }

    private void FixedUpdate()
    {
        position += velocity * Time.fixedDeltaTime;
        transform.position = position;
    }

    public void ApplyForce(Vector3 force)
    {
        Vector3 acceleration = force / mass;
        velocity = velocity * Time.fixedDeltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
