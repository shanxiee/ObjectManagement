using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RegisterLevelObjectMenuItem
{

    const string mentItem = "GameObject/Register Level Object";

    [MenuItem(mentItem, true)]
    static bool ValidateRegisterLevelObject()
    {
        if (Selection.objects.Length == 0)
        {
            return false;
        }
        foreach (Object o in Selection.objects)
        {
            if (!(o is GameObject))
            {
                return false;
            }
        }
        return true;
    }

    [MenuItem(mentItem, false, 1)]
    static void RegisterLevelObject()
    {
        foreach (Object o in Selection.objects)
        {
            Register(o as GameObject);
        }
    }

    static void Register(GameObject o)
    {
        if (PrefabUtility.GetPrefabAssetType(o) == PrefabAssetType.Regular)
        {
            Debug.LogWarning(o.name + " si a prefab asset .", o);
        }

        var levelObject = o.GetComponent<GameLevelObject>();
        if (levelObject == null)
        {
            Debug.LogWarning(o.name + " isn't a game level object .", o);
            return;
        }

        foreach (GameObject rootObject in o.scene.GetRootGameObjects())
        {
            var gameLevel = rootObject.GetComponent<GameLevel>();
            if (gameLevel != null)
            {
                if (gameLevel.HasLevelObject(levelObject))
                {
                    Debug.LogWarning(o.name + " is aleady registered. ", o);
                    return;
                }
                Undo.RecordObject(gameLevel, "Register Level Object.");
                gameLevel.RegisterLevelObject(levelObject);
                Debug.Log(o.name + " registered to game level " + gameLevel.name + "  in scene " + o.scene.name + ".", o);
                return;
            }
        }
        Debug.LogWarning(o.name + " isn't part of a game level .", o);
    }
}
