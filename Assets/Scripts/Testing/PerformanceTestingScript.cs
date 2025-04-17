using UnityEngine;
using CelestialBodies.Config;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Debug = UnityEngine.Debug;

public class PerformanceTestScript : MonoBehaviour
{
    // ───────────────────────────────────────────────────────────────────────────────
    // 1  CONFIGURATION
    // ───────────────────────────────────────────────────────────────────────────────
    [Header("Test Configuration")]
    [SerializeField] private int initialNumberOfPlanets = 5;
    [SerializeField] private int maxNumberOfPlanets = 50;
    [SerializeField] private int planetsToAddPerStage = 5;
    [SerializeField] private float minOrbitRadius = 20f;
    [SerializeField] private float maxOrbitRadius = 100f;
    [SerializeField] private float minPlanetMass = 0.1f;
    [SerializeField] private float maxPlanetMass = 5f;
    [SerializeField] private float starMass = 100f;
    
    [Header("Simulation Settings")]
    [SerializeField] [Range(0.0001f, 0.01f)] private float timeStep = 0.0001f;
    [SerializeField] [Range(1, 10)] private int simulationSpeed = 1;
    
    [Header("Performance Metrics")]
    [Tooltip("How long to measure at each stage, in seconds")]
    [SerializeField] private float measurementDuration = 10f;
    [SerializeField] private bool logPerformanceData = true;
    [Tooltip("Write raw frame times to CSV in Application.persistentDataPath")]
    [SerializeField] private bool writeCsv = true;

    // ───────────────────────────────────────────────────────────────────────────────
    // 2  STATE
    // ───────────────────────────────────────────────────────────────────────────────
    private StarSystemManager starSystemManager;
    private Stopwatch performanceTimer;
    private float elapsedTime;
    private int currentStage = 0;
    private int currentNumberOfPlanets;
    private bool isMeasuring = false;

    private readonly List<float> frameTimeSamples = new();
    private StreamWriter csvWriter;
    private string csvPath;
    private List<PerformanceStage> performanceStages = new();

    // ───────────────────────────────────────────────────────────────────────────────
    // 3  INITIALISATION
    // ───────────────────────────────────────────────────────────────────────────────
    private void Awake()
    {
        starSystemManager = StarSystemManager.Instance;
        if (starSystemManager == null)
        {
            Debug.LogError("StarSystemManager not found in scene!");
            enabled = false;
        }
    }

    private void Start()
    {
        // 3.1  Configure simulation controller
        if (starSystemManager.simulationController != null)
        {
            starSystemManager.simulationController.timeStep = timeStep;
            starSystemManager.simulationController.simulationSpeed = simulationSpeed;
            Debug.Log($"Simulation settings → Δt = {timeStep}, speed = {simulationSpeed}");
        }
        else
        {
            Debug.LogError("Simulation controller is null!");
            enabled = false;
            return;
        }

        // 3.2  Set up CSV logging
        if (logPerformanceData && writeCsv)
        {
            csvPath = Path.Combine(
                Application.persistentDataPath,
                $"perf_scaling_{SystemInfo.deviceName}_{System.DateTime.Now:yyyyMMdd_HHmmss}.csv");

            csvWriter = new StreamWriter(csvPath);
            csvWriter.WriteLine($"# Unity {Application.unityVersion}, OS {SystemInfo.operatingSystem}");
            csvWriter.WriteLine($"# Initial planets={initialNumberOfPlanets}, Max planets={maxNumberOfPlanets}, Add per stage={planetsToAddPerStage}");
            csvWriter.WriteLine($"# dt={timeStep}, speed={simulationSpeed}");
            csvWriter.WriteLine("stage,planets,frame,deltaTime");
        }

        // 3.3  Start with initial number of planets
        currentNumberOfPlanets = initialNumberOfPlanets;
        SetupTestSystem();
        StartMeasurement();
    }

    // ───────────────────────────────────────────────────────────────────────────────
    // 4  MAIN LOOP
    // ───────────────────────────────────────────────────────────────────────────────
    private void Update()
    {
        if (!isMeasuring || performanceTimer == null || !performanceTimer.IsRunning) return;

        float dt = Time.deltaTime;
        frameTimeSamples.Add(dt);
        if (writeCsv) csvWriter?.WriteLine($"{currentStage},{currentNumberOfPlanets},{Time.frameCount},{dt:F6}");

        elapsedTime += dt;
        if (elapsedTime >= measurementDuration)
        {
            LogPerformanceResults();
            performanceTimer.Stop();
            isMeasuring = false;

            // Move to next stage if we haven't reached max planets
            if (currentNumberOfPlanets < maxNumberOfPlanets)
            {
                currentStage++;
                currentNumberOfPlanets = Mathf.Min(currentNumberOfPlanets + planetsToAddPerStage, maxNumberOfPlanets);
                SetupTestSystem();
                StartMeasurement();
            }
            else
            {
                // Test complete
                LogFinalResults();
                csvWriter?.Dispose();
                enabled = false;
            }
        }
    }

    private void StartMeasurement()
    {
        frameTimeSamples.Clear();
        performanceTimer = new Stopwatch();
        performanceTimer.Start();
        elapsedTime = 0f;
        isMeasuring = true;
        Debug.Log($"Starting measurement stage {currentStage} with {currentNumberOfPlanets} planets");
    }

    // ───────────────────────────────────────────────────────────────────────────────
    // 5  STAR-SYSTEM SET-UP
    // ───────────────────────────────────────────────────────────────────────────────
    private void SetupTestSystem()
    {
        // Clear existing system
        starSystemManager.StopSimulation();

        starSystemManager.systemBodies.Clear();
        
        // starSystemManager.ClearBodies();

        // Create central star
        GameObject starObj = new("Central Star");
        CelestialBody star = CreateCelestialBody(
            starObj, Vector3.zero, Vector3.zero, starMass,
            CelestialBodyConfig.CelestialBodyType.Star);

        starSystemManager.SetCentralBody(star);
        starSystemManager.AddBody(star);

        // Create orbiting planets
        for (int i = 0; i < currentNumberOfPlanets; i++)
        {
            float orbitRadius = Mathf.Lerp(minOrbitRadius, maxOrbitRadius, i / (float)currentNumberOfPlanets);

            Vector3 position = CalculateInitialPosition(orbitRadius, i);
            Vector3 velocity = CalculateOrbitalVelocity(position, starMass);

            GameObject planetObj = new($"Planet_{i:D2}");
            float planetMass = Random.Range(minPlanetMass, maxPlanetMass);

            CelestialBody planet = CreateCelestialBody(
                planetObj, position, velocity, planetMass,
                CelestialBodyConfig.CelestialBodyType.Planet);

            starSystemManager.AddBody(planet);
        }

        starSystemManager.StartSimulation();
    }

    private CelestialBody CreateCelestialBody(
        GameObject go, Vector3 position, Vector3 velocity, float mass,
        CelestialBodyConfig.CelestialBodyType bodyType)
    {
        CelestialBody body = go.AddComponent<CelestialBody>();
        body.mass     = mass;
        body.position = position;
        body.velocity = velocity;

        var generator = go.AddComponent<CelestialBodyGenerator>();
        body.celestiaBodyGenerator = generator;
        generator.body = body;

        var bodyConfig = new CelestialBodyConfig();
        bodyConfig.Init(bodyType);

        var physicsWrapper = ScriptableObject.CreateInstance<Physics>();
        var settings = new Physics.PhysicsSettings
        {
            initialPosition = position,
            initialVelocity = velocity
        };
        physicsWrapper.SetSettings(settings);

        generator.bodyConfig         = bodyConfig;
        generator.bodyConfig.physics = physicsWrapper;

        generator.OnInitialUpdate();    // render visuals

        return body;
    }

    private Vector3 CalculateInitialPosition(float radius, int index)
    {
        float angle = Mathf.Deg2Rad * ((360f / currentNumberOfPlanets) * index);
        return new Vector3(
            radius * Mathf.Cos(angle),
            0f,
            radius * Mathf.Sin(angle)
        );
    }

    private Vector3 CalculateOrbitalVelocity(Vector3 position, float centralMass)
    {
        float radius = position.magnitude;
        float speed  = Mathf.Sqrt(GameManager.Instance.GetGravityConstant() * centralMass / radius);
        Vector3 dir  = new Vector3(-position.z, 0f, position.x).normalized; // perpendicular
        return dir * speed;
    }

    // ───────────────────────────────────────────────────────────────────────────────
    // 6  RESULTS & CLEAN-UP
    // ───────────────────────────────────────────────────────────────────────────────
    private void LogPerformanceResults()
    {
        if (frameTimeSamples.Count == 0) return;

        float avg = frameTimeSamples.Average();
        float min = frameTimeSamples.Min();
        float max = frameTimeSamples.Max();
        var order = frameTimeSamples.OrderBy(t => t).ToList();
        float median = order[order.Count / 2];
        float p99 = order[(int)(order.Count * 0.99f)];

        var stage = new PerformanceStage
        {
            StageNumber = currentStage,
            NumberOfPlanets = currentNumberOfPlanets,
            AverageFrameTime = avg,
            MedianFrameTime = median,
            P99FrameTime = p99,
            MinFrameTime = min,
            MaxFrameTime = max
        };

        performanceStages.Add(stage);

        string summary =
            $"\n=== Performance Results - Stage {currentStage} ===\n" +
            $"Planets          : {currentNumberOfPlanets}\n" +
            $"Duration (s)     : {measurementDuration}\n" +
            $"Average FPS      : {(1f / avg):F1}\n" +
            $"Median Frame (ms): {(median * 1000f):F2}\n" +
            $"99th  Frame (ms) : {(p99 * 1000f):F2}\n" +
            $"Min    Frame (ms): {(min * 1000f):F2}\n" +
            $"Max    Frame (ms): {(max * 1000f):F2}\n";

        Debug.Log(summary);

        if (writeCsv && csvWriter != null)
        {
            csvWriter.WriteLine();         // blank line before footer
            csvWriter.WriteLine($"# stage_{currentStage}_summary");
            csvWriter.WriteLine($"average_ms,{avg * 1000f:F4}");
            csvWriter.WriteLine($"median_ms,{median * 1000f:F4}");
            csvWriter.WriteLine($"p99_ms,{p99 * 1000f:F4}");
            csvWriter.WriteLine($"min_ms,{min * 1000f:F4}");
            csvWriter.WriteLine($"max_ms,{max * 1000f:F4}");
            csvWriter.Flush();
        }
    }

    private void LogFinalResults()
    {
        if (performanceStages.Count == 0) return;

        string finalSummary = "\n=== Final Performance Summary ===\n";
        finalSummary += "Stage\tPlanets\tAvg FPS\tMedian(ms)\tP99(ms)\n";
        
        foreach (var stage in performanceStages)
        {
            finalSummary += $"{stage.StageNumber}\t{stage.NumberOfPlanets}\t" +
                           $"{(1f / stage.AverageFrameTime):F1}\t" +
                           $"{(stage.MedianFrameTime * 1000f):F2}\t" +
                           $"{(stage.P99FrameTime * 1000f):F2}\n";
        }

        Debug.Log(finalSummary);

        // Save final summary to a separate file
        string summaryPath = Path.Combine(
            Application.persistentDataPath,
            $"perf_scaling_summary_{System.DateTime.Now:yyyyMMdd_HHmmss}.txt");
        
        File.WriteAllText(summaryPath, finalSummary);
    }

    private void OnDisable() => csvWriter?.Dispose();

    private class PerformanceStage
    {
        public int StageNumber { get; set; }
        public int NumberOfPlanets { get; set; }
        public float AverageFrameTime { get; set; }
        public float MedianFrameTime { get; set; }
        public float P99FrameTime { get; set; }
        public float MinFrameTime { get; set; }
        public float MaxFrameTime { get; set; }
    }
}


