using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Game : PersistableObject {

	public KeyCode createKey = KeyCode.C;
	public KeyCode newGameKey = KeyCode.N;
	public KeyCode saveKey = KeyCode.S;
	public KeyCode loadKey = KeyCode.L;

	public ShapeFactory shapeFactory;
	public PersistentStorage storage;

	List<Shape> shapes;
	string savePath;

    const int saveVersion = 1;

	void Awake()
    {
		shapes = new List<Shape>(100);

    }
	
	void Update () {
        if (Input.GetKeyDown(createKey))
        {
			CreateObject();
        }
        if (Input.GetKeyDown(newGameKey))
        {
			BeginNewGame();
        }
        if (Input.GetKeyDown(saveKey))
        {
			storage.Save(this);

		}
        if (Input.GetKeyDown(loadKey))
        {
			BeginNewGame();
			storage.Load(this);
		}

	}

    public override  void Save(GameDataWriter writer)
    {
        writer.Writer(-saveVersion);
        writer.Writer(shapes.Count);
        for(int i = 0; i < shapes.Count; i++)
        {
            writer.Writer(shapes[i].ShapeId);
            shapes[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int version = -reader.ReadInt();
        if (version > saveVersion)
        {
            Debug.LogError("Unsupported future save version " + version);
        }

        int count = version<=0 ? -version : reader.ReadInt();
        for(int i = 0; i < count; i++)
        {
            int shapeId = version > 0 ? reader.ReadInt() : 0;
            Shape instance = shapeFactory.Get(shapeId);
            instance.Load(reader);
            shapes.Add(instance);
        }
    }

    private void BeginNewGame()
    {
		for(int i = 0; i < shapes.Count; i++)
        {
			Destroy(shapes[i].gameObject);
        }
		shapes.Clear();
    }

    void CreateObject()
    {
        Shape instance = shapeFactory.GetRandom();
		Transform t = instance.transform;
        t.localPosition = UnityEngine.Random.insideUnitSphere * 5f;
        t.localRotation = UnityEngine.Random.rotation;
        t.localScale = Vector3.one * UnityEngine.Random.Range(0.1F, 1F);
        shapes.Add(instance);
	}
}
