using UnityEngine;

public class CelestialBody : MonoBehaviour
{
    public CelestialBodyGenerator celestiaBodyGenerator;

    // Essential Vars for all cBodies
    public float mass = 1f;     // TODO: ADJUST MASS SCALING
    public Vector3 velocity;
    public Vector3 position;
    public Transform sphereMesh; // Reference to the sphere mesh of the cbody

    private SphereCollider sphereCollider;
    private Renderer sphereRenderer; // Renderer for sphere mesh
    private Material originalMaterial;
    private Material outlineMaterial; // Hightlighting selected body

    public Material surfaceMaterial;

    //    private Rigidbody _rb;


    private void Awake()
    {
        //_rb = gameObject.AddComponent<Rigidbody>();
        //_rb.iskinematic = true;

        celestiaBodyGenerator = gameObject.AddComponent<CelestialBodyGenerator>();
        celestiaBodyGenerator.body = this;
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

    // Set visual cue that body is highlighted
    public void Highlight(bool isHighlighted)
    {
        if (sphereRenderer != null)
        {
            sphereRenderer.material = isHighlighted ? outlineMaterial : originalMaterial;
        }
    }

}
