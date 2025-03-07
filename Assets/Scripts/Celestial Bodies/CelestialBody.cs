using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    // Essential Vars for all cBodies
    public float mass = 1f;
    public Vector3 velocity;
    public Vector3 position;
    public Transform sphereMesh; // Reference to the sphere mesh of the cbody

    private SphereCollider sphereCollider;    
    
    // TODO: name of cbody 

    // N.B CHECK METHOD FOR CALCULATING FORCES

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        position = transform.position;

        //TODO: REFACTOR
        // Make sure cbody has collifer for clicking
         sphereCollider = GetComponent<SphereCollider>();
        if (sphereCollider == null)
        {
            Debug.Log("Adding Sphere Collider to " + gameObject.name);
            sphereCollider = gameObject.AddComponent<SphereCollider>();
        }

        UpdateColliderSize(); // Set collider size to match visual mesh
    }

    private void UpdateColliderSize()
    {
        Debug.Log("Updating collider size");
        if (sphereMesh != null)
        {
            float radius = sphereMesh.localScale.x /2f;
            Debug.Log("Setting collider Radius: " + radius);
            sphereCollider.radius = radius;
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
        Debug.Log("CelestialBody clicked" + gameObject.name);
        EditorUI editorUI = FindFirstObjectByType<EditorUI>();
        if (editorUI != null) 
        {
            editorUI.SetSelectedBody(this);
        }
    }



}
