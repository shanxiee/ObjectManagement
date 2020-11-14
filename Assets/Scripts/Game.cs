using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

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
			CreateShape();
        }
        if (Input.GetKeyDown(newGameKey))
        {
			BeginNewGame();
        }
        if (Input.GetKeyDown(saveKey))
        {
			storage.Save(this,saveVersion);

		}
        if (Input.GetKeyDown(loadKey))
        {
			BeginNewGame();
			storage.Load(this);
		}

	}

    public override  void Save(GameDataWriter writer)
    {

        writer.Writer(shapes.Count);
        for(int i = 0; i < shapes.Count; i++)
        {
            writer.Writer(shapes[i].ShapeId);
            writer.Writer(shapes[i].MaterialId);
            shapes[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int version = reader.Version;
        if (version > saveVersion)
        {
            Debug.LogError("Unsupported future save version " + version);
        }

        int count = version<=0 ? -version : reader.ReadInt();
        for(int i = 0; i < count; i++)
        {
            int shapeId = version > 0 ? reader.ReadInt() : 0;
            int materialId = version > 0 ? reader.ReadInt() : 0;
            Shape instance = shapeFactory.Get(shapeId, materialId);
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

    void CreateShape()
    {
        Shape instance = shapeFactory.GetRandom();
		Transform t = instance.transform;
        t.localPosition = UnityEngine.Random.insideUnitSphere * 5f;
        t.localRotation = UnityEngine.Random.rotation;
        t.localScale = Vector3.one * UnityEngine.Random.Range(0.1F, 1F);
        instance.SetColor(Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.25f, 1f, 1f, 1f));
        shapes.Add(instance);
	}
}
