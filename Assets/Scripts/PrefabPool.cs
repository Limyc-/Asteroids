using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[Serializable]
public class PrefabPool<T> where T : IPoolable<T>
{
	public GameObject prefab;
	public T[] instances;

	public Transform poolParent;
	public int totalCount = 0;
	public int activeCount = 0;

	public PrefabPool(GameObject prefab, int count, Transform poolParent = null)
	{
		Assert.IsNotNull(prefab);

		this.prefab = prefab;
		this.poolParent = poolParent;

		instances = new T[count];

		AllocateInstances();
	}

	public T SpawnInstance(Transform parent = null)
	{
		if (totalCount == activeCount)
		{
			AllocateInstances();
		}

		if (parent == null)
		{
			parent = poolParent;
		}

		T t = instances[activeCount++];

		t.Reset();
		t.Activate(parent);
		t.OnDespawn += DespawnInstance;

		return t;
	}

	public void DespawnInstance(T inst)
	{
		int index = inst.PoolIndex;

		Assert.IsTrue(index >= 0);
		Assert.IsTrue(index < instances.Length);
		Assert.IsTrue(index < activeCount);

		activeCount--;

		instances.Swap(index, activeCount);
		instances[index].PoolIndex = index;

		inst.Deactivate(poolParent);
		inst.OnDespawn = null;
		inst.PoolIndex = activeCount;
	}

	public void DespawnAllInstances()
	{
		for (int i = 0; i < activeCount; i++)
		{
			T inst = instances[i];
			inst.Deactivate(poolParent);
			inst.OnDespawn = null;
		}

		activeCount = 0;

	}

	private void AllocateInstances()
	{
		int newSize = ExpandCapacity();

		int count = newSize - totalCount;

		for (int i = 0; i < count; i++)
		{
			var go = GameObject.Instantiate(prefab) as GameObject;
			var t = go.GetComponent<T>();
			t.Deactivate(poolParent);
			t.PoolIndex = totalCount;
			instances[totalCount++] = t;
		}
	}

	private int ExpandCapacity()
	{
		int newSize = instances.Length;

		while (newSize < (totalCount + 1))
		{
			newSize = Mathf.Max(4, (int)(newSize * 1.5f));
		}

		if (newSize != instances.Length)
		{
			Array.Resize(ref instances, newSize);
		}

		return newSize;
	}

}