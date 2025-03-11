using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    public CelestiaBodyGenerator celestiaBodyGenerator;

    // Essential Vars for all cBodies
    public float mass = 1f;     // TODO: ADJUST MASS SCALIN
    public Vector3 velocity;
    public Vector3 position;
    public Transform sphereMesh; // Reference to the sphere mesh of the cbody

    private SphereCollider sphereCollider;
    private Renderer sphereRenderer; // Renderer for sphere mesh
    private Material originalMaterial;
    private Material outlineMaterial; // Hightlighting selected body


    public Material surfaceMaterial;




    // TODO: name of cbody 
    // N.B CHECK METHOD FOR CALCULATING FORCES

    //void Start()
    //{
    //    position = transform.position;

    //    if (sphereMesh != null)
    //    {
    //        sphereRenderer = sphereMesh.GetComponent<Renderer>();
    //        if (sphereRenderer != null)
    //        {
    //            originalMaterial = sphereRenderer.material;
    //        }
    //    }
    //    else
    //    {
    //        Debug.LogError("SphereMesh not assigned in " + gameObject.name);
    //    }
            
    //    // make sure body has a collider
    //    sphereCollider = GetComponent<SphereCollider>();

    //    if (sphereCollider == null)
    //    {
    //        Debug.Log("Adding Sphere Collider to " + gameObject.name);
    //        sphereCollider = gameObject.AddComponent<SphereCollider>();
    //    }

    //    UpdateColliderSize(); // Set collider size to match visual mesh
    //}

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

    // Set visual cue that body is highlighted
    public void Highlight(bool isHighlighted)
    {
        if (sphereRenderer != null)
        {
            sphereRenderer.material = isHighlighted ? outlineMaterial : originalMaterial;
        }
    }

}
