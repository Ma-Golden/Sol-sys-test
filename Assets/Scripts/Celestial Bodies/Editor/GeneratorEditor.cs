using CelestialBodies.Config;
using CelestialBodies.Config.Shading;
using CelestialBodies.Config.Shape;
using PlasticPipe.PlasticProtocol.Messages;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.XR;


[CustomEditor(typeof(CelestiaBodyGenerator))]
public class GeneratorEditor : Editor
{
    CelestiaBodyGenerator generator;
    private UnityEditor.Editor shapeEditor;
    private UnityEditor.Editor shadingEditor;
    

    private bool shapeFoldout;
    private bool shadingFoldout;

    public override void OnInspectorGUI()
    {
        Debug.Log("OnInspectorGUI");

        // Get relevant config data
        Debug.Log("Getting shading config");
        Shading.ShadingSettings shadingC = generator.bodyConfig.shading.GetConfig();
        Debug.Log("Getting shape config");
        Shape.ShapeConfig shapeC = generator.bodyConfig.shape.GetConfig();
        // TODO: phys, ocean etc

        CelestialBodyConfig.CelestialBodyType newValue = 
            (CelestialBodyConfig.CelestialBodyType)EditorGUILayout.EnumPopup(generator.bodyConfig.bodyType);

        // TODO CHECK THIS
        if (newValue != generator.bodyConfig.bodyType)
        {
            generator.bodyConfig.UpdateCBodySettings(newValue);
            shapeC = generator.bodyConfig.shape.GetConfig();
            shadingC = generator.bodyConfig.shading.GetConfig();
            

            // lINK NEW SETTINGS TO GENERATOR
            generator.bodyConfig.Subscribe(generator);
            //Regenerate(); 

        }

        using (var check = new EditorGUI.ChangeCheckScope())
        {
            DrawDefaultInspector();
            if (check.changed)
            {
              //  Regenerate(shapeC);
            }
        }

        if (GUILayout.Button("Randomize Shape"))
        {
            Debug.Log("Randomize Shape button pressed");
            
            //generator.bodyConfig.shape.shapeConfig.seed = Random.Range(0, 100000);
            shapeC.seed = Random.Range(0, 100000);



            Regenerate();
//            shapeC.RandomizeShape(true);
            // ocean && rings
            //Regenerate(shapeC);

        }
        SaveState();
    }

    void Regenerate()
    {      
        //var shapeC = generator.bodyConfig.shape.GetConfig();
        //var shadingC = generator.bodyConfig.shading.GetConfig();

        generator.OnShapeUpdate();
        generator.OnShadingUpdate();

//        EditorApplication.QueuePlayerLoopUpdate();
    }


    void DrawsettingsEditor(Object settings, ref bool foldout, ref Editor editor)
    {
        if (settings != null)
        {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
            if (foldout)
            {
                CreateCachedEditor(settings, null, ref editor);
                editor.OnInspectorGUI();
            }
        }
    }

    private void OnEnable()
    {
        shapeFoldout = EditorPrefs.GetBool(nameof(shapeFoldout), false);
        shadingFoldout = EditorPrefs.GetBool(nameof(shadingFoldout), false);
        
        generator = (CelestiaBodyGenerator)target;
        generator.bodyConfig.Subscribe(generator);

        //shapeFoldout = true;
        //shadingFoldout = true;
    }

    private void SaveState()
    {
        EditorPrefs.SetBool(nameof(shapeFoldout), shapeFoldout);
        EditorPrefs.SetBool(nameof(shadingFoldout), shadingFoldout);
    }

}
