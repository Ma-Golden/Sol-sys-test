using CelestialBodies.Config;
using CelestialBodies.Config.Shading;
using CelestialBodies.Config.Shape;
using PlasticPipe.PlasticProtocol.Messages;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;


[CustomEditor(typeof(CelestiaBodyGenerator))]
public class GeneratorEditor : Editor
{
    CelestiaBodyGenerator generator;
    Editor shapeEditor;
    Editor shadingEditor;

    private bool shapeFoldout;
    private bool shadingFoldout;

    public override void OnInspectorGUI()
    {
        // Get relevant config data
        Shading.ShadingSettings shadingC = generator.bodyConfig.shading.GetConfig();
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
            //            generator.bodyConfig.shape.shapeConfig.seed = Random.Range(0, 100000);

            shapeC.seed = Random.Range(0, 100000);




//            shapeC.RandomizeShape(true);
            // ocean && rings
            //Regenerate(shapeC);

        }







            base.OnInspectorGUI();
    }



    void Regenerate()
    {      
        var shapeC = generator.bodyConfig.shape.GetConfig();
        var shadingC = generator.bodyConfig.shading.GetConfig();

        generator.OnShapeUpdate();
        generator.OnShadingUpdate();

//        EditorApplication.QueuePlayerLoopUpdate();
    }



}
