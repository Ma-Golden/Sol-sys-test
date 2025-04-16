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

    private bool isSelected = false;

    //    private Rigidbody _rb;


    private void Awake()
    {
        //_rb = gameObject.AddComponent<Rigidbody>();
        //_rb.iskinematic = true;

        celestiaBodyGenerator = gameObject.AddComponent<CelestialBodyGenerator>();
        celestiaBodyGenerator.body = this;
    }

    //private void UpdateColliderSize()
    //{
    //    Debug.Log("Updating collider size");
    //    if (sphereMesh != null)
    //    {
    //        float radius = sphereMesh.localScale.x /2f;
    //        Debug.Log("Setting collider Radius: " + radius);
    //        sphereCollider.radius = radius;
    //    }
    //}

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
        Debug.Log("CelestialBody clicked: " + gameObject.name);

        // Check if generator and bodyconfig are not null
        if (celestiaBodyGenerator == null || celestiaBodyGenerator.bodyConfig == null)
        {
            Debug.LogError($"Body {gameObject.name} has no generator or bodyConfig");
            return;
        }


        BodyEditorPanel bodyEditorPanel = FindFirstObjectByType<BodyEditorPanel>();
        if (bodyEditorPanel != null)
        {
            Debug.Log("bodyPanel.selectbody called");
            bodyEditorPanel.SelectBody(this);
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

    public void Select()
    {
        isSelected = true;

        Highlight(true);

        // Notify editor
        BodyEditorPanel editorPanel = FindAnyObjectByType<BodyEditorPanel>();
        if (editorPanel != null)
        {
            editorPanel.SelectBody(this);
        }
    }

    public void Deselect()
    {
        isSelected = false;
        // Remove visuals

    }


}
