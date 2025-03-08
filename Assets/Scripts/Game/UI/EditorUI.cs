using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Search;

public class EditorUI : MonoBehaviour
{
    // Select body vars
    public CelestialBody selectedBody;

    // UI Buttons
    public TMP_InputField velocityXInput, velocityYInput, velocityZInput;
    public TMP_Text bodyNameText; // Display name of selected body
    public Slider massSlider;
    public Button deleteButton;
    public Button deselectButton;


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
            selectedBody.Highlight(false); // Remove highlight from previous selection
        }

        selectedBody = body;
        selectedBody.Highlight(true); // Highlight selected body

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


    public void DeselectBody()
    {
        selectedBody.Highlight(false);
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
        deleteButton.gameObject.SetActive(show);
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
