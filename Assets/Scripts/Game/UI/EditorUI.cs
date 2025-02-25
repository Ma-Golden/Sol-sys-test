using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EditorUI : MonoBehaviour
{
    public CelestialBody selectedBody;
    public Slider massSlider;
    public TMP_InputField velocityXInput, velocityYInput, velocityZInput;
    public Button deleteButton;

    public void SetSelectedBody(CelestialBody body)
    {
        selectedBody = body;

        massSlider.value = body.mass;
        velocityXInput.text = selectedBody.velocity.x.ToString("F2");
        velocityYInput.text = selectedBody.velocity.y.ToString("F2");
        velocityZInput.text = selectedBody.velocity.z.ToString("F2");
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

    public void DeleteSelectedBody()
    {
        if (selectedBody != null)
        {
            Destroy(selectedBody.gameObject);
            selectedBody = null;
        }
    }


}
