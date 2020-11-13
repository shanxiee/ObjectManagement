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

	public PersistableObject prefab;
	public PersistentStorage storage;

	List<PersistableObject> objects;
	string savePath;

	void Awake()
    {
		objects = new List<PersistableObject>(100);

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
        writer.Writer(objects.Count);
        for(int i = 0; i < objects.Count; i++)
        {
            objects[i].Save(writer);
        }
    }

    public override void Load(GameDataReader reader)
    {
        int count = reader.ReadInt();
        for(int i = 0; i < count; i++)
        {
            PersistableObject o = Instantiate(prefab);
            o.Load(reader);
            objects.Add(o);
        }
    }

    private void BeginNewGame()
    {
		for(int i = 0; i < objects.Count; i++)
        {
			Destroy(objects[i].gameObject);
        }
		objects.Clear();
    }

    void CreateObject()
    {
		PersistableObject o =Instantiate(prefab);
		Transform t = o.transform;
        t.localPosition = UnityEngine.Random.insideUnitSphere * 5f;
        t.localRotation = UnityEngine.Random.rotation;
        t.localScale = Vector3.one * UnityEngine.Random.Range(0.1F, 1F);
        objects.Add(o);
	}
}
