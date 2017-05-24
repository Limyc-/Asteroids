using SerializableCollections;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

[CreateAssetMenu(fileName = "Data", menuName = "Util/PrefabDB", order = 1000)]
[Serializable]
public class PrefabDB : ScriptableObject
{
	[SerializeField]
	public GameObject[] prefabs;

	public GameObject GetPrefab(PrefabType type)
	{
		int index = (int)type;
		Assert.IsTrue(index < prefabs.Length);

		GameObject result = prefabs[index];

		return result;
	}
}