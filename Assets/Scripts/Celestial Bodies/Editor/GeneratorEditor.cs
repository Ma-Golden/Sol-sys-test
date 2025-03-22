// using CelestialBodies.Config;
// using CelestialBodies.Config.Shading;
// using CelestialBodies.Config.Shape;
// using PlasticPipe.PlasticProtocol.Messages;
// using System.Runtime.InteropServices;
// using UnityEditor;
// using UnityEngine;
// using UnityEngine.Rendering;
// using UnityEngine.XR;


// [CustomEditor(typeof(CelestialBodyGenerator))]
// public class GeneratorEditor : Editor
// {
//     CelestialBodyGenerator generator;
//     private UnityEditor.Editor shapeEditor;
//     private UnityEditor.Editor shadingEditor;
//     private UnityEditor.Editor oceanEditor;
//     // ocean, atmos etc.


//     private bool shapeFoldout;
//     private bool shadingFoldout;
//     private bool oceanFoldout;

//     public override void OnInspectorGUI()
//     {
//         // Get relevant config data
//         Shading.ShadingConfig shadingC = generator.bodyConfig.shading.GetConfig();
//         Shape.ShapeConfig shapeC = generator.bodyConfig.shape.GetConfig();
//         Ocean.OceanSettings oceanC = generator.bodyConfig.ocean.GetSettings();
//         // TODO: phys, ocean etc

//         CelestialBodyConfig.CelestialBodyType newValue = 
//             (CelestialBodyConfig.CelestialBodyType)EditorGUILayout.EnumPopup(generator.bodyConfig.bodyType);


//         if (generator == null)
//         {
//             Debug.LogError("Generator is null");
//             return;
//         }

//         if (generator.bodyConfig == null)
//         {
//             Debug.LogError("Generator body config is null");
//             return;
//         }


//         // TODO CHECK THIS
//         if (newValue != generator.bodyConfig.bodyType)
//         {
//             generator.bodyConfig.UpdateCBodySettings(newValue);
//             shapeC = generator.bodyConfig.shape.GetConfig();
//             shadingC = generator.bodyConfig.shading.GetConfig();
//             oceanC = generator.bodyConfig.ocean.GetSettings();
            
//             // lINK NEW SETTINGS TO GENERATOR
//             generator.bodyConfig.Subscribe(generator);
//             //Regenerate(); 

//         }

//         using (var check = new EditorGUI.ChangeCheckScope())
//         {
//             DrawDefaultInspector();
//             DrawConfigEditors(); // Adds to editor each config section
//             if (check.changed)
//             {
//                 Regenerate();
//             }
//         }

//         if (GUILayout.Button("Randomize Shape"))
//         {
// //            Debug.Log("Randomize Shape button pressed");
//             shapeC.seed = Random.Range(0, 100000);
//             shapeC.RandomizeShape(true);
//             Regenerate();
//         }

//         if (GUILayout.Button("Randomize Shading"))
//         {
// //            Debug.Log("Randomize Shading button pressed");
//             shadingC.seed = Random.Range(0, 100000);
//             shadingC.RandomizeShading(true);

//             Regenerate();
//         }

//         if (GUILayout.Button("Randomize All"))
//         {
// //            Debug.Log("Randomize All button pressed");
//             shapeC.seed = Random.Range(0, 100000);
//             shadingC.seed = Random.Range(0, 100000);
//             Regenerate();
//         }

//         SaveState();
//     }

//     void Regenerate()
//     {      

//         generator.OnShapeUpdate();
//         generator.OnShadingUpdate();
//     }


//     private void DrawConfigEditors()
//     {
//         DrawConfigEditor(generator.bodyConfig.shape, ref shapeFoldout, ref shapeEditor);
//         DrawConfigEditor(generator.bodyConfig.shading, ref shadingFoldout, ref shadingEditor);
//         DrawConfigEditor(generator.bodyConfig.ocean, ref shadingFoldout, ref shadingEditor);
//     }




//     void DrawConfigEditor(Object settings, ref bool foldout, ref Editor editor)
//     {
//         if (settings != null)
//         {
//             foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
//             if (foldout)
//             {
//                 CreateCachedEditor(settings, null, ref editor);
//                 editor.OnInspectorGUI();
//             }
//         }
//     }

//     private void OnEnable()
//     {
//         shapeFoldout = EditorPrefs.GetBool(nameof(shapeFoldout), false);
//         shadingFoldout = EditorPrefs.GetBool(nameof(shadingFoldout), false);
//         oceanFoldout = EditorPrefs.GetBool(nameof(oceanFoldout), false);
        
//         generator = (CelestialBodyGenerator) target;

//         if (generator == null)
//         {
//             Debug.LogError("Generator is null");
//             return;
//         }

//         if (generator.bodyConfig == null)
//         {
//             Debug.LogError("Generator body config is null");
//             return;
//         }

//         //shapeFoldout = true;
//         //shadingFoldout = true;
//     }

//     private void SaveState()
//     {
//         EditorPrefs.SetBool(nameof(shapeFoldout), shapeFoldout);
//         EditorPrefs.SetBool(nameof(shadingFoldout), shadingFoldout);
//         EditorPrefs.SetBool(nameof(oceanFoldout), oceanFoldout);
//     }

// }
