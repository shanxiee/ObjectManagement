using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Game : MonoBehaviour {

	public KeyCode createKey = KeyCode.C;
	public KeyCode newGameKey = KeyCode.N;
	public KeyCode saveKey = KeyCode.S;
	public KeyCode loadKey = KeyCode.L;

	public Transform prefab;
	List<Transform> objects;
	string savePath;

	void Awake()
    {
		objects = new List<Transform>(100);
		savePath = Path.Combine(Application.persistentDataPath,"saveFile");
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
			Save();
        }
        if (Input.GetKeyDown(loadKey))
        {
			Load();
        }

	}

    private void Save()
    {
		using(var writer = new BinaryWriter(File.Open(savePath, FileMode.Create)))
        {
			writer.Write(objects.Count);
			for(int i = 0; i < objects.Count; i++)
            {
				Transform t = objects[i];
				writer.Write(t.localPosition.x);
				writer.Write(t.localPosition.y);
				writer.Write(t.localPosition.z);
			}
        }
    }

	private void Load()
    {
		BeginNewGame();
		using (var reader = new BinaryReader(File.Open(savePath, FileMode.Open)))
		{
			int count = reader.ReadInt32();
			for(int i =0;i< count; i++)
            {
				Vector3 p;
				p.x = reader.ReadSingle();
				p.y = reader.ReadSingle();
				p.z = reader.ReadSingle();
				Transform t = Instantiate(prefab);
				t.localPosition = p;
				objects.Add(t);
			}
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
		Transform t =Instantiate(prefab);
		t.localPosition = UnityEngine.Random.insideUnitSphere * 5f;
		t.localRotation = UnityEngine.Random.rotation;
		t.localScale = Vector3.one * UnityEngine.Random.Range(0.1F, 1F);
		objects.Add(t);
	}
}
