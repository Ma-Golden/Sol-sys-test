using CelestialBodies;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable][CreateAssetMenu]
public class Physics : ScriptableObject
{
    [CanBeNull] private List<ICelestialObserver> _observers = new List<ICelestialObserver>();

    [SerializeField] private PhysicsSettings _physicsSettings;


    public void InitSettings()
    {
        if (_observers == null) return;

        foreach (ICelestialObserver observer in _observers)
        {
            observer.OnPhysicsUpdate();
        }
    }

    public PhysicsSettings GetPhysicalConfig()
    {
        return _physicsSettings;
    }

    public void SetSettings(PhysicsSettings settings)
    {
        _physicsSettings = settings;

        if (_observers == null) return;
        foreach (ICelestialObserver observer in _observers)
        {
            observer.OnPhysicsUpdate();
        }
    }

    public void Subscribe(ICelestialObserver observer)
    {
        _observers?.Add(observer);
    }

    public void Unsubscribe(ICelestialObserver observer)
    {
        _observers = null;
    }

    [Serializable]
    public class PhysicsSettings
    {
        public float minRadius = 1f;
        public float maxRadius = 40f;

        public float radius = 5f;
        
        public Vector3 initialPosition = Vector3.zero;

        public float minSpeed = 0.1f;
        public float maxSpeed = 20f;
        public Vector3 initialVelocity = Vector3.up * 5;

        public float minRotationSpeed = 0f;
        public float maxRotationSpeed = 5f;
        public float initialRotationSpeed = 2f;
    }
}