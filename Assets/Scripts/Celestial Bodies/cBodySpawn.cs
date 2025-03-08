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

    }
}
