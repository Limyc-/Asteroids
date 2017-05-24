using System;
using UnityEngine;

public interface IPoolable<T>
{
	int PoolIndex { get; set; }
	Action<T> OnDespawn { get; set; }

	void Activate(Transform parent);
	void Deactivate(Transform parent);
	void Reset();
}
