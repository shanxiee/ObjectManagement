﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpawnZone : PersistableObject
{

    [System.Serializable]
    public struct SpawnConfigureation
    {
        public enum MovementDirection
        {
            Forward,
            Upward,
            Outward,
            Random,
        }
        public MovementDirection movementDirection;
        public FloatRange speed;
        public FloatRange angularSpeed;
        public FloatRange scale;
        public ColorRangeHSV color;
    }

    [SerializeField]
    SpawnConfigureation spawnConfig;

    public abstract Vector3 SpawnPoint { get; }
    public virtual void ConfigureSpawn(Shape shape)
    {
        Transform t = shape.transform;
        t.localPosition = SpawnPoint;
        t.localRotation = UnityEngine.Random.rotation;
        t.localScale = Vector3.one * spawnConfig.scale.RandomValueInRange;
        shape.SetColor(spawnConfig.color.RandomInRange);
        shape.AngularVelocity = Random.onUnitSphere * spawnConfig.angularSpeed.RandomValueInRange;
        Vector3 direction;
        switch (spawnConfig.movementDirection)
        {
            case SpawnConfigureation.MovementDirection.Upward:
                direction = transform.up;
                break;
            case SpawnConfigureation.MovementDirection.Outward:
                direction = (t.localPosition - transform.position).normalized;
                break;
            case SpawnConfigureation.MovementDirection.Random:
                direction = Random.onUnitSphere;
                break;
            default:
                direction = transform.forward;
                break;
        }
        shape.Velocity = direction * spawnConfig.speed.RandomValueInRange;
    }
}
