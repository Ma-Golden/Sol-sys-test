using System;
using System.IO;
using CelestialBodies.Config;
using CelestialBodies.Config.Shading;
using CelestialBodies.Config.Shape;
using Newtonsoft.Json;
using UnityEngine;
using JsonSubTypes;
using System.Collections.Generic;

public class SystemSavingUtils : MonoBehaviour
{
    public static SystemSavingUtils Instance; // single instance of this class

    public StarSystemConfig currentSystemConfig;

    public string storePath = null;
    public string testStorePath = null;

    private JsonSerializerSettings _jSonSettings;


    // N.B TESTING ONLY -> HARDCODED SHAPES
    [Header("Shapes")]
    public PlanetShape PlanetShape;
    public MoonShape MoonShape;
    public StarShape StarShape;

    [Header("Shading")]
    public PlanetShading PlanetShading;
    public MoonShading MoonShading;
    public StarShading StarShading;

    [Header("Physics")]
    public Physics basePhysics;

    [Header("Ocean")]
    public Ocean baseOcean;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);

            // Save the store path
            storePath = Application.persistentDataPath + Path.DirectorySeparatorChar;
            testStorePath = storePath + "test" + Path.DirectorySeparatorChar;
            
            // Setting the de/-serialization settings 
            _jSonSettings = new JsonSerializerSettings();
            _jSonSettings.ConstructorHandling = ConstructorHandling.Default;
            _jSonSettings.Converters.Add(JsonSubtypesConverterBuilder
                .Of<Shape.ShapeConfig>("Type") // type property is only defined here
                .RegisterSubtype<PlanetShape.PlanetShapeConfig>(CelestialBodyConfig.CelestialBodyType.Planet)
                //.RegisterSubtype<GaseousShape.GaseousShapeSettings>(CelestialBodyConfig.CelestialBodyType.Gaseous)
                .RegisterSubtype<StarShape.StarShapeConfig>(CelestialBodyConfig.CelestialBodyType.Star)
                .RegisterSubtype<MoonShape.MoonShapeConfig>(CelestialBodyConfig.CelestialBodyType.Moon)
                .SerializeDiscriminatorProperty() // ask to serialize the type property
                .Build());
            _jSonSettings.Converters.Add(JsonSubtypesConverterBuilder
                .Of<Shading.ShadingConfig>("Type") // type property is only defined here
                .RegisterSubtype<PlanetShading.PlanetShadingSettings>(CelestialBodyConfig.CelestialBodyType.Planet)
                //.RegisterSubtype<.GaseousShadingSettings>(CelestialBodyConfig.CelestialBodyType.Gaseous)
                .RegisterSubtype<StarShading.StarShadingSettings>(CelestialBodyConfig.CelestialBodyType.Star)
                .RegisterSubtype<MoonShading.MoonshadingConfig>(CelestialBodyConfig.CelestialBodyType.Moon)
                .SerializeDiscriminatorProperty() // ask to serialize the type property
                .Build());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SaveSystem(StarSystemConfig systemConfig)
    {
        if (storePath == null) return;


        var systemNamesPath = storePath + "names_of_systems.txt";
        SavedSystemNames savedNames = new SavedSystemNames();
        if (File.Exists(systemNamesPath))
        {
            StreamReader namesReader = new StreamReader(systemNamesPath);
            savedNames = JsonConvert.DeserializeObject<SavedSystemNames>(namesReader.ReadToEnd());
            namesReader.Close();
        }

        if (!savedNames.savedSystemNames.Contains(systemConfig.systemName))
        {
            savedNames.savedSystemNames.Add(systemConfig.systemName);
        }

        StreamWriter namesWriter = new StreamWriter(systemNamesPath, false);
        var namesJson = JsonConvert.SerializeObject(savedNames);
        namesWriter.WriteLine(namesJson);
        namesWriter.Close();


        // Stores cBody settings for each cBody
        CelestialBodyConfigList toStoreCBodiesSettings = new CelestialBodyConfigList();
        foreach (CelestialBodyConfig cBodySettings in systemConfig.celestialBodyConfigs)
        {
            toStoreCBodiesSettings.shapeSettingsList.Add(cBodySettings.shape.GetConfig());
            toStoreCBodiesSettings.shadingSettingsList.Add(cBodySettings.shading.GetConfig());
            toStoreCBodiesSettings.physicsSettingsList.Add(cBodySettings.physics.GetPhysicalConfig());
            toStoreCBodiesSettings.oceanSettingsList.Add(cBodySettings.ocean.GetSettings());
            // toStoreCBodiesSettings.atmosphereSettingsList.Add(cBodySettings.atmosphere.GetConfig());
            // toStoreCBodiesSettings.ringSettingsList.Add(cBodySettings.ring.GetConfig());
        }

        var cBodiesSettingsPath = storePath + systemConfig.systemName + "_cBodies_settings.txt";
        StreamWriter settingsWriter = new StreamWriter(cBodiesSettingsPath, false);
        var settingsJson = JsonConvert.SerializeObject(toStoreCBodiesSettings, _jSonSettings);
        settingsWriter.WriteLine(settingsJson);
        settingsWriter.Close();


        // Stores the system settings
        var systemPath = storePath + systemConfig.systemName + "_system_settings.txt";
        StreamWriter systemWriter = new StreamWriter(systemPath, false);
        var systemJson = JsonConvert.SerializeObject(systemConfig);
        systemWriter.WriteLine(systemJson);
        systemWriter.Close();
    }

    public void SaveTestSystem(StarSystemConfig systemSettings)
    {
        if (!Directory.Exists(testStorePath))
        {
            Directory.CreateDirectory(testStorePath);
        }
        
        var temp = storePath;
        storePath = testStorePath;
        SaveSystem(systemSettings);
        storePath = temp;
    }

    public StarSystemConfig LoadSystem(string systemName)
    {
        // var cBodiesTypesPath = storePath + systemName + "_cBodies_types.txt";
        var cBodiesSettingsPath = storePath + systemName + "_cBodies_settings.txt";
        var systemPath = storePath + systemName + "_system_settings.txt";
        
        // if (File.Exists(systemPath) && File.Exists(cBodiesTypesPath) && File.Exists(cBodiesSettingsPath))
        if (File.Exists(systemPath) && File.Exists(cBodiesSettingsPath))
        {
            // There exists already a previous saved state
            // StreamReader cBodiesTypesReader = new StreamReader(cBodiesTypesPath);
            // CBodiesTypes loadedCBodiesTypes = JsonConvert.DeserializeObject<CBodiesTypes>(cBodiesTypesReader.ReadToEnd());
            
            StreamReader cBodiesSettingsReader = new StreamReader(cBodiesSettingsPath);

            StreamReader systemSettingsReader = new StreamReader(systemPath);
            StarSystemConfig loadedSystemSettings = JsonConvert.DeserializeObject<StarSystemConfig>(systemSettingsReader.ReadToEnd());
            
        
            // if (loadedCBodiesTypes != null && loadedSystemSettings != null)
            if (loadedSystemSettings != null)
            {
                CelestialBodyConfigList loadedCelestialBodyConfigs = JsonConvert.DeserializeObject<CelestialBodyConfigList>(cBodiesSettingsReader.ReadToEnd(),_jSonSettings);
                
                for (int i = 0; i < loadedSystemSettings.celestialBodyConfigs.Count; i ++)
                {
                    (Shape shape, Shading shading, Ocean ocean, Physics physics) = Instance.CreateFeatures(loadedSystemSettings.celestialBodyConfigs[i].bodyType);

                    shape.SetConfig(loadedCelestialBodyConfigs.shapeSettingsList[i]);
                    shading.SetConfig(loadedCelestialBodyConfigs.shadingSettingsList[i]); 

                    ocean.SetSettings(loadedCelestialBodyConfigs.oceanSettingsList[i]);
                    physics.SetSettings(loadedCelestialBodyConfigs.physicsSettingsList[i]);

                    loadedSystemSettings.celestialBodyConfigs[i].shape = shape;
                    loadedSystemSettings.celestialBodyConfigs[i].shading = shading;
                    loadedSystemSettings.celestialBodyConfigs[i].ocean = ocean;
                    loadedSystemSettings.celestialBodyConfigs[i].physics = physics;
                }
                cBodiesSettingsReader.Close();
                systemSettingsReader.Close();
                
                return loadedSystemSettings;
            }
            else
            {
                cBodiesSettingsReader.Close();
                systemSettingsReader.Close();
                Debug.LogError("System types not correctly loaded");
                return null;
            }
        }
        Debug.LogError("No saved system data found");
        return null;
    }

    public StarSystemConfig LoadTestSystem(string systemName)
    {
        var temp = storePath;
        storePath = testStorePath;
        StarSystemConfig ss = LoadSystem(systemName);
        storePath = temp;
        return ss;
    }


    public void DeleteSystem(string systemNameToDelete)
    {
        // todo 
        // remove the system name from the names list 
        // delete the files containing system types and system settings 
        List<string> names = GetSystemNames();
        
        if (!names.Contains(systemNameToDelete)) return;
        
        names.Remove(systemNameToDelete);
        var systemNamesPath = storePath + "names_of_systems.txt";
        StreamWriter namesWriter = new StreamWriter(systemNamesPath, false);
        SavedSystemNames savedSystemsNames = new SavedSystemNames();
        savedSystemsNames.savedSystemNames = names;
        var namesJson = JsonConvert.SerializeObject(savedSystemsNames);
        namesWriter.WriteLine(namesJson);
        namesWriter.Close();
            
        string cBSettingsPath = storePath + systemNameToDelete + "_cBodies_settings.txt";
        if (File.Exists(cBSettingsPath))
        {
            File.Delete(cBSettingsPath);
        }
            
        string sysSettingsPath = storePath + systemNameToDelete + "_system_settings.txt";
        if (File.Exists(sysSettingsPath))
        {
            File.Delete(sysSettingsPath);
        }
    }


    public (Shape shape, Shading shading, Ocean ocean, Physics physics) CreateFeatures(CelestialBodyConfig.CelestialBodyType celestialBodyType)
    {
        Shape shape = null;
        Shading shading = null;
        Ocean ocean = null;
        Physics physics = null;

        // TODO:
        // !TODO: fix physics getphysicalconfig
        // TODO:
        // !TODO:    ADD SPECIFIC PHYSICS FOR EACH CELESTIAL BODY TYPE
        // TODO:

        ocean = (baseOcean);

        switch (celestialBodyType)
        {
            case CelestialBodyConfig.CelestialBodyType.Planet:
                shape = PlanetShape;
                shading = PlanetShading; 
                ocean = baseOcean;
                physics = basePhysics;
                break;
            case CelestialBodyConfig.CelestialBodyType.Moon:
                shape = MoonShape;
                shading = MoonShading;
                physics = basePhysics;
                break;
            case CelestialBodyConfig.CelestialBodyType.Star:
                shape = StarShape;
                shading = StarShading;
                physics = basePhysics;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        return (shape, shading, ocean, physics);
    }



    public List<string> GetSystemNames()
    {
        var systemNamesPath = storePath + "names_of_systems.txt";
        SavedSystemNames savedNames = new SavedSystemNames();
        if (!File.Exists(systemNamesPath)) return savedNames.savedSystemNames;
        StreamReader reader = new StreamReader(systemNamesPath);
        savedNames = JsonConvert.DeserializeObject<SavedSystemNames>(reader.ReadToEnd());
        reader.Close();
        return savedNames.savedSystemNames;
    }


    [Serializable]
    private class CelestialBodyConfigList
    {
        public List<Shape.ShapeConfig> shapeSettingsList = new List<Shape.ShapeConfig>();
        public List<Shading.ShadingConfig> shadingSettingsList = new List<Shading.ShadingConfig>();
        public List<Physics.PhysicsSettings> physicsSettingsList = new List<Physics.PhysicsSettings>();
        public List<Ocean.OceanSettings> oceanSettingsList = new List<Ocean.OceanSettings>();
//        public List<Atmosphere.AtmosphereSettings> atmosphereSettingsList = new List<Atmosphere.AtmosphereSettings>();
  //      public List<Ring.RingSettings> ringSettingsList = new List<Ring.RingSettings>();

    }

    [Serializable]
    private class SavedSystemNames
    {
        public List<string> savedSystemNames = new List<string>();
    }





}; // Class SystemSavingUtils
