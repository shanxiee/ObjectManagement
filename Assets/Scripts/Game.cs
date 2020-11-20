using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : PersistableObject {

	public KeyCode createKey = KeyCode.C;
	public KeyCode newGameKey = KeyCode.N;
	public KeyCode saveKey = KeyCode.S;
	public KeyCode loadKey = KeyCode.L;
    public KeyCode destroyKey = KeyCode.X;

    [SerializeField]
	ShapeFactory shapeFactory;
    [SerializeField]
    PersistentStorage storage;
    [SerializeField]
    int levelCount;
    [SerializeField]
    bool reseedOnLoad;

    List<Shape> shapes;
	string savePath;
    const int saveVersion = 5;

    int loadedLevelBuildIndex;

    public float CreationSpeed { get; set; }
    float creationProgress,destructionProgress;

    public float DestructionSpeed { get; set; }

    Random.State mainRandomState;

    [SerializeField]
    Slider creationSpeedSlider;
    [SerializeField]
    Slider destructionSpeedSlider;

    void Start()
    {
        mainRandomState = Random.state;
        shapes = new List<Shape>(100);
        if (Application.isEditor)
        {

            for(int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(i);
                if (loadedScene.name.Contains("Level "))
                {
                    SceneManager.SetActiveScene(loadedScene);
                    loadedLevelBuildIndex = loadedScene.buildIndex;
                    return;
                }
            }
        }

        BeginNewGame();
        StartCoroutine(LoadLevel(1));
    }
	
	void Update () {
        if (Input.GetKeyDown(createKey))
        {
			CreateShape();
        }
        else if (Input.GetKeyDown(newGameKey))
        {
			BeginNewGame();
            StartCoroutine(LoadLevel(loadedLevelBuildIndex));
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
        else
        {
            for(int i = 1; i <= levelCount; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i))
                {
                    BeginNewGame();
                    StartCoroutine(LoadLevel(i));
                    return;
                }
            }
        }


    }

    void FixedUpdate()
    {
        for (int i = 0; i < shapes.Count; i++)
        {
            shapes[i].GameUpdate();
        }
        creationProgress += Time.deltaTime * CreationSpeed;
        while (creationProgress >= 1f)
        {
            creationProgress -= 1f;
            CreateShape();
        }

        destructionProgress += Time.deltaTime * DestructionSpeed;
        while (destructionProgress >= 1f)
        {
            destructionProgress -= 1f;
            DestroyShape();
        }
    }

    public override  void Save(GameDataWriter writer)
    {

        writer.Writer(shapes.Count);
        writer.Writer(Random.state);
        writer.Writer(CreationSpeed);
        writer.Writer(creationProgress);
        writer.Writer(DestructionSpeed);
        writer.Writer(destructionProgress);
        writer.Writer(loadedLevelBuildIndex);
        GameLevel.Current.Save(writer);
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
            return;
        }
        StartCoroutine(LoadGame(reader));
    }

    IEnumerator LoadGame(GameDataReader reader) 
    {
        int version = reader.Version;
        int count = version<=0 ? -version : reader.ReadInt();

        if(version >= 3)
        {
            Random.State state = reader.ReadRandomState();
            if (!reseedOnLoad)
            {
                Random.state = state;
            }
            creationSpeedSlider.value = CreationSpeed = reader.ReadFloeat();
            creationProgress = reader.ReadFloeat();
            destructionSpeedSlider.value = DestructionSpeed = reader.ReadFloeat();
            destructionProgress = reader.ReadFloeat();
        }

        yield return LoadLevel(version < 2 ? 1 : reader.ReadInt());
        if(version >= 3)
        {
            GameLevel.Current.Load(reader);
        }

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
        Random.state = mainRandomState;
        int seed = Random.Range(0, int.MaxValue)^(int)Time.unscaledDeltaTime;
        mainRandomState = Random.state;
        Random.InitState(seed);
        CreationSpeed = 0;
        creationSpeedSlider.value = 0;
        DestructionSpeed = 0;
        destructionSpeedSlider.value = 0;

        for (int i = 0; i < shapes.Count; i++)
        {
            shapeFactory.Reclaim(shapes[i]);
        }
		shapes.Clear();
    }

    void CreateShape()
    {
        Shape instance = shapeFactory.GetRandom();
        GameLevel.Current.ConfigureSpawn(instance);
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

    IEnumerator LoadLevel(int levelBuildIndex)
    {
        enabled = false;
        if(loadedLevelBuildIndex > 0)
        {
            yield return SceneManager.UnloadSceneAsync(loadedLevelBuildIndex);
        }
        yield return SceneManager.LoadSceneAsync(levelBuildIndex, LoadSceneMode.Additive);
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelBuildIndex));
        loadedLevelBuildIndex = levelBuildIndex;
        enabled = true;
    }
}
