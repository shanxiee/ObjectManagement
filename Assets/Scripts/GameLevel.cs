﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLevel : PersistableObject
{

    [SerializeField]
    int populationLimit;

    public int PopulationLimit
    {
        get
        {
            return populationLimit;
        }
    }

    [SerializeField]
    SpawnZone spawnZone;
    [SerializeField]
    PersistableObject[] persistableObjects;

    public static GameLevel Current { get; private set; }


    void OnEnable()
    {
        Current = this;
        if (persistableObjects == null)
        {
            persistableObjects = new PersistableObject[0];
        }
    }

    public override void Save(GameDataWriter writer)
    {
        writer.Write(persistableObjects.Length);
        for (int i = 0; i < persistableObjects.Length; i++)
        {
            persistableObjects[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int savedCount = reader.ReadInt();
        for (int i = 0; i < savedCount; i++)
        {
            persistableObjects[i].Load(reader);
        }
    }

    public void SpawnShapes()
    {
        spawnZone.SpawnShapes();
    }
}
