using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CelestialBodies.Config.Shading
{
    [Serializable]
    [CreateAssetMenu]

    public abstract class Shading : ScriptableObject
    {
        // OOBSERVER
        [CanBeNull] protected List<ICelestialObserver> Observers = new List<ICelestialObserver>();

        public ComputeShader shadingDataCompute;
        public Material terrainMaterial = null;

        protected Vector4[] CachedShadingData;
        private ComputeBuffer _shadingDataBuffer;

        private static System.Random _prng = new System.Random();

        public virtual void ReleaseBuffers()
        {
            ComputeHelper.Release(_shadingDataBuffer);
        }

        public virtual void Initialize(Shape.Shape shape) { }

        // Set shading properties on terrain
        public virtual void SetSurfaceProperties(Material material, Vector2 heightMinMax, float bodyScale, float oceanLevel)
        {
            // Overriden by child class
        }

        // Generate Vector4[] of shading data. This is stored in mesh uvs and used to help shade the body
        public Vector4[] GenerateShadingData(ComputeBuffer vertexBuffer)
        {
            int numVertices = vertexBuffer.count;
            Vector4[] shadingData = new Vector4[numVertices];

            if (shadingDataCompute)
            {
                // Set data
                SetShadingDataComputeProperties();

                shadingDataCompute.SetInt("numVertices", numVertices);
                shadingDataCompute.SetBuffer(0, "vertices", vertexBuffer);
                ComputeHelper.CreateAndSetBuffer<Vector4>(ref _shadingDataBuffer, numVertices, shadingDataCompute, "shadingData");

                // Run
                ComputeHelper.Run(shadingDataCompute, numVertices);

                // Get data
                _shadingDataBuffer.GetData(shadingData);
            }
            CachedShadingData = shadingData;
            return shadingData;
        }

        protected virtual void SetShadingDataComputeProperties()
        {
            // Overriden by child class
        }

        // OBSERVER PATTERN
        public void Subscribe(ICelestialObserver observer)
        {
            Observers?.Add(observer);
        }

        public void UnsubscribeAll()
        {
            Observers = null;
        }

        // MEMENTO PATTERN
        public abstract void InitConfig();
        public abstract ShadingConfig GetConfig();

        public abstract void SetConfig(ShadingConfig ss);

        [Serializable]
        public abstract class ShadingConfig
        {
            public bool randomize;
            public int seed = 0;

            public bool realisticColors = true;

            public Color mainColor = Color.white;

            public void RandomizeShading(bool rand)
            {
                randomize = rand;
                seed = rand ? _prng.Next(-10000, 10000) : 0;
            }
        }

    }
}