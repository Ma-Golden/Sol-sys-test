using UnityEngine;

public class cBodySpawn : MonoBehaviour
{
    public GameObject cBodyPrefab; // Assign in inspector
    public int numBodies = 0; // 2 for the moment

    private void Start()
    {
        // DEBUG LOGS
        if (cBodyPrefab == null)
        {
            Debug.LogError("cBodyPrefab is Not assigned in Inspector");
        }
        if (cBodyPrefab.GetComponent<CelestialBody>() == null) 
        {
            Debug.LogError("cbodyPrefab itself is missing the CelestialBody component");
            return;
        }

        for (int i = 1; i < numBodies; i++)
        {
            SpawnRandBody();
        }
    }

    private void SpawnRandBody()
    {
        if (cBodyPrefab == null)
        {
            Debug.LogError("cBodyPrefab is NOT assigned in Inspector!");
            return;
        }

        //Vector3 randomPos = new Vector3(
        //    Random.Range(-10f, 10f),
        //    Random.Range(-10f, 10f),
        //    Random.Range(-10f, 10f)
        //);

        Vector3 randomPos = new Vector3(
            Random.Range(-10f, 10f),
            0, // Keeping flat for start
            Random.Range(-10f, 10f)            
        );

        GameObject newBody = Instantiate(cBodyPrefab, randomPos, Quaternion.identity);
        CelestialBody body = newBody.GetComponent<CelestialBody>();


        body.mass = Random.Range(1f, 10f);
        body.velocity = Random.insideUnitSphere * 2f;


        GravityManager gravityManager = FindFirstObjectByType<GravityManager>();

        if (gravityManager != null)
        {
            gravityManager.bodies.Add(body);
        }

        //// Give planets an initial velocity perpendicular to the center (0,0,0)
        //Vector3 directionToCenter = (Vector3.zero - randomPos).normalized;
        //Vector3 orbitalVelocity = Vector3.Cross(directionToCenter, Vector3.up) * Random.Range(1f, 2f);
        //body.velocity = orbitalVelocity;

        //GravityManager gravityManager = FindFirstObjectByType<GravityManager>();
        //if (gravityManager != null)
        //{
        //    gravityManager.bodies.Add(body);
        //}
    }

    // Debug function to print hierarchy of instantiated objects
    private void PrintObjectHierarchy(Transform obj)
    {
        Debug.Log("Object: " + obj.name);
        foreach (Transform child in obj)
        {
            Debug.Log(" - Child: " + child.name);
        }
    }


}
