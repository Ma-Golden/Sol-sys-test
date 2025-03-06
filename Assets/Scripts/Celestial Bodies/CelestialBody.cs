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

        //TODO: REFACTOR
        // Make sure cbody has collifer for clicking
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<SphereCollider>();
        }
    }

    private void FixedUpdate()
    {

        position += velocity * Time.fixedDeltaTime;
        transform.position = position;
    }

    public void ApplyForce(Vector3 force)
    {
        Vector3 acceleration = force / mass;
        velocity += acceleration * Time.fixedDeltaTime;
    }

    // UI Updates
    public void SetMass(float newMass) => mass = newMass;
    public void SetVelocity(Vector3 newVelocity) => velocity = newVelocity;
    public void SetPosition(Vector3 newPosition)
    {
        position = newPosition;
        transform.position = newPosition;
    }


    private void OnMouseDown()
    {
        EditorUI editorUI = FindFirstObjectByType<EditorUI>();
        if (editorUI != null) 
        {
            editorUI.SetSelectedBody(this);
        }
    }



}
