using SerializableCollections;
using UnityEngine;

[System.Serializable]
public class PrefabDictionary : SerializableDictionary<PrefabType, GameObject>
{

}

#if UNITY_EDITOR

[UnityEditor.CustomPropertyDrawer(typeof(PrefabDictionary))]
public class ExtendedSerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer
{

}

#endif