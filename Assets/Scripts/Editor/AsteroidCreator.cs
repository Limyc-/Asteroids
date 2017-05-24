using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System;

public class AsteroidCreator : Editor
{
	[MenuItem("Util/Create Asteroids/Large")]
	public static void CreateLargeAsteroids()
	{
		AddAsteroidsAsType(AsteroidType.Large);
	}

	[MenuItem("Util/Create Asteroids/Medium")]
	public static void CreateMediumAsteroids()
	{
		AddAsteroidsAsType(AsteroidType.Medium);
	}

	[MenuItem("Util/Create Asteroids/Small")]
	public static void CreateSmallAsteroids()
	{
		AddAsteroidsAsType(AsteroidType.Small);
	}

	public static void AddAsteroidsAsType(AsteroidType type)
	{
		var obj = Selection.GetFiltered(typeof(Texture2D),
										SelectionMode.Assets | SelectionMode.DeepAssets | SelectionMode.ExcludePrefab);

		if (obj.Length == 0)
		{
			Debug.Log("No Texture2D in Selection");
			return;
		}


		AsteroidDB db = null;

		string[] str = AssetDatabase.FindAssets("AsteroidDB");

		if (str.Length > 0)
		{
			var path = AssetDatabase.GUIDToAssetPath(str[0]);
			Debug.Log("Load AsteroidDB from path '" + path + "'");
			db = AssetDatabase.LoadAssetAtPath(path, typeof(AsteroidDB)) as AsteroidDB;
		}

		if (db == null)
		{
			Debug.Log("Create new AsteroidDB");
			db = CreateAsset<AsteroidDB>();
		}

		for (int i = 0; i < obj.Length; i++)
		{
			string path = AssetDatabase.GetAssetPath(obj[i]);
			Sprite sprite = AssetDatabase.LoadAssetAtPath(path, typeof(Sprite)) as Sprite;

			var go = new GameObject(sprite.name);
			var render = go.AddComponent<SpriteRenderer>();
			render.sprite = sprite;
			var poly = go.AddComponent<PolygonCollider2D>();

			db.AddAsteroid(type, poly.points, sprite);

			DestroyImmediate(go);
		}

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		Debug.Log("Success");
	}

	private static T CreateAsset<T>() where T : ScriptableObject
	{
		T asset = CreateInstance<T>();
		Type type = typeof(T);
		string path = AssetDatabase.GenerateUniqueAssetPath("Assets/" + type.ToString() + ".asset");

		AssetDatabase.CreateAsset(asset, path);
		AssetDatabase.SaveAssets();

		return AssetDatabase.LoadAssetAtPath(path, type) as T;
	}

}