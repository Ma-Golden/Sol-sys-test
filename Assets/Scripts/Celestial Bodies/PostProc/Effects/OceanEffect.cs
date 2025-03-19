using UnityEngine;

public class OceanEffect
{
    private SunShadowCaster light;
    protected Material material;

    public void UpdateSettings(CelestialBodyGenerator generator, Shader shader)
    {
        if (material == null || material.shader != shader)
        {
            material = new Material(shader);
        }

        if (light == null)
        {
            light = Object.FindObjectOfType<SunShadowCaster>();
        }

        Vector3 centre = generator.transform.position;
        float radius = generator.GetOceanRadius();
        material.SetVector("oceanCentre", centre);
        material.SetFloat("oceanRadius", radius);

        material.SetFloat("planetScale", generator.BodyScale);

        if (light)
        {
            Vector3 dirFromPlanetToSun = (light.transform.position - generator.transform.position).normalized;
            material.SetVector("dirToSun", dirFromPlanetToSun);
        } 
        else
        {
            material.SetVector("dirToSun", Vector3.up);
            Debug.Log("No sunshadowCaster found!");
        }
        generator.bodyConfig.ocean.SetOceanProperties(material);
    }

    public Material GetMaterial()
    {
        return material;
    }
}