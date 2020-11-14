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
    public KeyCode destroyKey = KeyCode.X;

	public ShapeFactory shapeFactory;
	public PersistentStorage storage;

	List<Shape> shapes;
	string savePath;
    const int saveVersion = 1;

    public float CreationSpeed { get; set; }
    float creationProgress,destructionProgress;

    public float DestructionSpeed { get; set; }
    


	void Awake()
    {
		shapes = new List<Shape>(100);

    }
	
	void Update () {
        if (Input.GetKeyDown(createKey))
        {
			CreateShape();
        }
        else if (Input.GetKeyDown(newGameKey))
        {
			BeginNewGame();
        }
        else if (Input.GetKeyDown(saveKey))
        {
			storage.Save(this,saveVersion);

		}
        else if (Input.GetKeyDown(loadKey))
        {
			BeginNewGame();
			storage.Load(this);
		}
        else if (Input.GetKeyDown(destroyKey))
        {
            DestroyShape();
        }

        creationProgress += Time.deltaTime * CreationSpeed;
        while(creationProgress >= 1f)
        {
            creationProgress -= 1f;
            CreateShape();
        }

        destructionProgress += Time.deltaTime * DestructionSpeed;
        while(destructionProgress >= 1f)
        {
            destructionProgress -= 1f;
            DestroyShape();
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
            shapeFactory.Reclaim(shapes[i]);
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

    void DestroyShape()
    {
        if (shapes.Count > 0)
        {
            int index = Random.Range(0, shapes.Count);
            shapeFactory.Reclaim(shapes[index]);
            int lastIndex = shapes.Count - 1;
            shapes[index] = shapes[lastIndex];
            shapes.RemoveAt(lastIndex);
        }
    }
}
