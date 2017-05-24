
using System.IO;
using UnityEditor;
using UnityEngine;

public class ConfigurePrefabDB : Editor
{
	[MenuItem("Util/Update PrefabDB")]
	public static void UpdatePrefabDB()
	{
		var obj = Selection.GetFiltered(typeof(PrefabDB), SelectionMode.Assets);

		if (obj.Length == 0) { return; }

		var db = obj[0] as PrefabDB;

		if (db != null)
		{
			GameObject[] prefabs = db.prefabs;

			WritePrefabTypeEnum(prefabs);
		}

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		Debug.Log("Complete");

	}

	public static void WritePrefabTypeEnum(GameObject[] objects)
	{
		string enumName = "PrefabType";
		string enumFilePath = Path.Combine(Path.Combine(Application.dataPath, "Scripts"), enumName + ".cs");

		if (!File.Exists(enumFilePath))
		{
			Debug.Log("Creating file: '" + enumFilePath + "'");
			File.CreateText(enumFilePath);
		}

		Debug.Log("Writing file: '" + enumFilePath + "'");

		using (var writer = new StreamWriter(enumFilePath))
		{
			writer.WriteLine("[System.Serializable]");
			writer.WriteLine("public enum " + enumName);
			writer.WriteLine("{");


			for (int i = 0; i < objects.Length; i++)
			{
				if (objects[i] == null) { continue; }

				string path = AssetDatabase.GetAssetPath(objects[i]);
				var name = Path.GetFileNameWithoutExtension(path);

				writer.WriteLine("\t{0} = {1},", name, i);
			}

			writer.WriteLine("}");
		}
	}
}