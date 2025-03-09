using UnityEngine;
using System;
using System.Collections.Generic;


namespace CelestialBodies.Config.Shape
{
    [Serializable][CreateAssetMenu(fileName = "Shape", menuName = "Scriptable Objects/Shape")]
    public abstract class Shape : ScriptableObject
    {

        //public ShapeConfig shapeConfig; // set in editor
        public abstract Mesh GenerateMesh(int resolution, float radius);

        // todo: OBSERVERS

        //public ComputeShader perturbCompute;
        //public ComputeShader heightMapCompute;
        //private ComputeBuffer _heightMapBuffer;

        //private static System.Random _prng = new System.Random();

        public abstract void InitSettings();

        public abstract ShapeConfig GetShapeConfig();


        public abstract class ShapeConfig
        {
            public bool random = false;
            public int seed = 0;
            public bool perturbVertices = false;
            [Range(0, 1)] public float perturbStrength = 0.36f;

            public float noiseScale = 3.0f;
            public float noiseStrength = 2.0f;

            public void RandomizeShape(bool rand)
            {
                random = rand;
                seed = random ? UnityEngine.Random.Range(0, 100000) : 0;
            }
        }
    }
}