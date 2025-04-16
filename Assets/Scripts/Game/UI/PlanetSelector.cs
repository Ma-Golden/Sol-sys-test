using UnityEngine;

public class PlanetSelector : MonoBehaviour
{
    private CelestialBody _selectedPlanet;  // Stores curretnly selected planet


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (UnityEngine.Physics.Raycast(ray, out RaycastHit hit))
            {
                CelestialBody cbody = hit.collider.GetComponent<CelestialBody>();
                if (cbody != null)
                {
                    SelectCbody(cbody);
                }
            }
        }
    }

    void SelectCbody(CelestialBody cBody)
    {
        if (_selectedPlanet != null)
        {
           _selectedPlanet.Deselect();
        }

        _selectedPlanet = cBody;

        if (_selectedPlanet != null)
        {
            _selectedPlanet.Select();
        }
    }

    



}
