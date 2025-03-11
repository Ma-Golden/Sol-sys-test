using UnityEngine;
using System;
using System.Collections.Generic;
using JetBrains.Annotations;


namespace CelestialBodies.Config.Shape
{
    [Serializable][CreateAssetMenu(fileName = "Shape", menuName = "Scriptable Objects/Shape")]
    public abstract class Shape : ScriptableObject
    {
        // Observers
        [CanBeNull] protected List<ICelestialObserver> Observers = new List<ICelestialObserver>();

        public ComputeShader perturbCompute;
        public ComputeShader heightCompute;
        private ComputeBuffer _heightBuffer;


        //public ShapeConfig shapeConfig; // set in editor
        public abstract Mesh GenerateMesh(int resolution, float radius);

        //private static System.Random _prng = new System.Random();

        public virtual float[] CalculateHeights (ComputeBuffer vertexBuffer)
        {
            // Set data
            SetShapeData();
            heightCompute.SetInt("numVertices", vertexBuffer.count);
            heightCompute.SetBuffer(0, "vertices", vertexBuffer);
            ComputeHelper.CreateAndSetBuffer<float>(ref _heightBuffer, vertexBuffer.count, heightCompute, "heights");

            // Run
            ComputeHelper.Run(heightCompute, vertexBuffer.count);

            // Get & return heights from shader
            float[] heights = new float[vertexBuffer.count];
            _heightBuffer.GetData(heights);
            return heights;
        }

        protected virtual void SetShapeData ()
        {
            // overriden by child class
        }

        public virtual void ReleaseBuffers ()
        {
            ComputeHelper.Release(_heightBuffer);
        }


        public void Subscribe(ICelestialObserver observer)
        {
            Observers?.Add(observer);
        }

        public void UnsubscribeAll()
        {
           Observers?.Clear();
        }

        // Abstract methods - to be overridden by derived classes
        public abstract void InitConfig();

        public abstract ShapeConfig GetShapeConfig();
        public abstract void SetConfig(ShapeConfig shapeConfig);

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