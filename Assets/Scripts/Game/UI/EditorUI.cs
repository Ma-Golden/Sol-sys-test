using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Search;

public class EditorUI : MonoBehaviour
{
    // Select body vars
    public CelestialBody selectedBody;
    public Button deselectButton;
    public TMP_Text bodyNameText; // Display name of selected body

    // Edit body vars
    public Slider massSlider;
    public TMP_InputField velocityXInput, velocityYInput, velocityZInput;
    public Button deleteButton;

    private Color originalColor;
    private Renderer selectedRenderer;


    private void Start()
    {
        // Add event listeners for UI elements
        massSlider.onValueChanged.AddListener(UpdateMass);
        deleteButton.onClick.AddListener(DeleteSelectedBody);
        deselectButton.onClick.AddListener(DeselectBody);
    
        velocityXInput.onEndEdit.AddListener(delegate { UpdateVelocity(); });
        velocityYInput.onEndEdit.AddListener(delegate { UpdateVelocity(); });
        velocityZInput.onEndEdit.AddListener(delegate { UpdateVelocity(); });

        // Initially hide UI elements
        ShowUI(false);
    }

    public void SetSelectedBody(CelestialBody body)
    {
        if (selectedBody != null)
        {
            ResetPreviousSelection();
        }

        selectedBody = body;
        selectedRenderer = body.GetComponent<Renderer>();

        // Store original colour and apply highlight
        if (selectedRenderer != null)
        {
            originalColor = selectedRenderer.material.color;
            selectedRenderer.material.color = Color.blue;
        }

        // Update UI elements with planets data
        bodyNameText.text = selectedBody.gameObject.name;
        massSlider.value = body.mass;
        velocityXInput.text = selectedBody.velocity.x.ToString("F2");
        velocityYInput.text = selectedBody.velocity.y.ToString("F2");
        velocityZInput.text = selectedBody.velocity.z.ToString("F2");

        ShowUI(true);
    }
    public void DeleteSelectedBody()
    {
        if (selectedBody != null)
        {
            Destroy(selectedBody.gameObject);
            selectedBody = null;
        }
    }

    private void ResetPreviousSelection()
    {

       if (selectedRenderer != null)
        {
            selectedRenderer.material.color = originalColor; // Reset color
        }
    }

    public void DeselectBody()
    {
        ResetPreviousSelection();
        selectedBody = null;
        ShowUI(false);
    }

    // Show or hide UI elements based on selection
    private void ShowUI(bool show)
    {
        massSlider.gameObject.SetActive(show);
        velocityXInput.gameObject.SetActive(show);
        velocityYInput.gameObject.SetActive(show);
        velocityZInput.gameObject.SetActive(show);
        deleteButton.gameObject.SetActive(show);
        deselectButton.gameObject.SetActive(show);
        bodyNameText.gameObject.SetActive(show);
    }

    public void UpdateMass(float newMass)
    {
        if (selectedBody != null)
        {
            selectedBody.SetMass(newMass);       
        }
    }

    public void UpdateVelocity()
    {
        if (selectedBody != null)
        {
            float x = float.Parse(velocityXInput.text);
            float y = float.Parse(velocityYInput.text);
            float z = float.Parse(velocityZInput.text);
            selectedBody.SetVelocity(new Vector3(x, y, z));
        }
    }

}
