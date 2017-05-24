using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AsteroidSpawner
{
	public AsteroidDB db;
	public PrefabPool<AsteroidController> pool;
	public Bounds levelBounds;

	public Action<AsteroidType> OnAsteroidCountChanged;

	public AsteroidSpawner(AsteroidDB db, PrefabPool<AsteroidController> pool, Bounds levelBounds, Action<AsteroidType> asteroidCountChangedCallback)
	{
		this.db = db;
		this.pool = pool;
		this.levelBounds = levelBounds;
		this.OnAsteroidCountChanged = asteroidCountChangedCallback;
	}

	public Asteroid GetRandomAsteroid(AsteroidType type)
	{
		Asteroid asteroid = null;

		switch (type)
		{
			case AsteroidType.Large:
			asteroid = db.large[Random.Range(0, db.large.Count)];
			break;
			case AsteroidType.Medium:
			asteroid = db.medium[Random.Range(0, db.medium.Count)];
			break;
			case AsteroidType.Small:
			asteroid = db.small[Random.Range(0, db.small.Count)];
			break;
		}

		return asteroid;
	}

	public void SpawnAsteroid(AsteroidType type, Bounds levelBounds)
	{
		Asteroid asteroid = GetRandomAsteroid(type);

		RigidbodyState state = new RigidbodyState();

		Vector2 offset = Random.insideUnitCircle;
		state.position = RandomUtil.OnRect(levelBounds.max);
		state.rotation = Random.value * 360;
		state.velocity = offset.normalized * Random.Range(asteroid.minVelocity, asteroid.maxVelocity);
		state.angularVelocity = Random.Range(asteroid.minAngular, asteroid.maxAngular);

		Vector2 p = new Vector2(105, 110);
		Vector2 toOrigin = Vector2.zero - p;

		Vector2 dir = (Quaternion.AngleAxis(Random.Range(-30f, 30f), Vector3.up) * toOrigin).normalized;

		SpawnAsteroid(asteroid, state);
	}

	public void SpawnAsteroid(Asteroid asteroid, RigidbodyState state)
	{
		AsteroidController controller = pool.SpawnInstance();

		controller.Init(asteroid, state, OnAsteroidDeath);
	}

	public void OnAsteroidDeath(AsteroidType type, Vector2 position)
	{
		if (type == AsteroidType.Small) { return; }

		var spawnType = AsteroidType.Large;
		int spawnCount = 0;
		float offsetMultiplier = 1f;

		if (type == AsteroidType.Large)
		{
			spawnType = AsteroidType.Medium;
			spawnCount = 2;
			offsetMultiplier = 0.5f;
		}
		else if (type == AsteroidType.Medium)
		{
			spawnType = AsteroidType.Small;
			spawnCount = 3;
			offsetMultiplier = 0.25f;
		}

		for (int i = 0; i < spawnCount; i++)
		{
			Asteroid asteroid = GetRandomAsteroid(spawnType);
			RigidbodyState state = new RigidbodyState();

			Vector2 offset = Random.insideUnitCircle;
			state.position = position + (offset * offsetMultiplier);
			state.rotation = Random.value * 360;
			state.velocity = offset.normalized * Random.Range(asteroid.minVelocity, asteroid.maxVelocity);
			state.angularVelocity = Random.Range(asteroid.minAngular, asteroid.maxAngular);

			SpawnAsteroid(asteroid, state);
		}

		OnAsteroidCountChanged(type);
	}
}


