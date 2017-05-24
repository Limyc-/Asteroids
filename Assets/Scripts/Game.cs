using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;
using System;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
	public static readonly Vector2 DefaultPos = Vector2.one * 9999;

	public PrefabDB prefabDB;
	public AsteroidDB asteroidDB;
	public Text waveText;
	public Text scoreText;
	public Text beginText;

	//[Range(1, 4)]
	//public int numPlayers = 1;

	public PrefabPool<Bullet> bulletPool;
	public PrefabPool<AsteroidController> asteroidPool;

	public AsteroidSpawner asteroidSpawner;

	private GameState gameState = new GameState();
	private PlayerController player;

	private void Awake()
	{
		var min = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
		var max = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

		WorldState.bounds = new Bounds();
		WorldState.bounds.SetMinMax(min, max);
		WorldState.frustumPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
	}

	public void Start()
	{
		var bulletParent = new GameObject("Pool(Bullet)");
		bulletPool = new PrefabPool<Bullet>(prefabDB.GetPrefab(PrefabType.Bullet), 4, bulletParent.transform);



		var asteroidParent = new GameObject("Pool(Asteroid");
		asteroidPool = new PrefabPool<AsteroidController>(prefabDB.GetPrefab(PrefabType.Asteroid),
															32, asteroidParent.transform);

		asteroidSpawner = new AsteroidSpawner(asteroidDB, asteroidPool, WorldState.bounds, OnActiveAsteroidCountChanged);
	}

	public void Update()
	{
		if (!gameState.isRunning)
		{
			if (Input.GetKeyDown(KeyCode.Space))
			{
				gameState.isRunning = true;

				SpawnPlayer();
				BeginNextWave();

				RefreshUI();
			}
		}
	}

	public void SpawnPlayer()
	{
		if (player == null)
		{
			var go = Instantiate(prefabDB.GetPrefab(PrefabType.Player)) as GameObject;
			player = go.GetComponent<PlayerController>();
			player.Init(bulletPool, OnPlayerDeath);
		}
		else
		{
			player.gameObject.SetActive(true);
		}
	}

	public void BeginNextWave()
	{
		gameState.wave++;

		int numAsteroids = gameState.baseAsteroidCount + ((gameState.wave - 1) * gameState.perWaveIncrease);

		for (int i = 0; i < numAsteroids; i++)
		{
			asteroidSpawner.SpawnAsteroid(AsteroidType.Large, WorldState.bounds);
		}
	}

	public void RefreshUI()
	{
		waveText.text = "Wave: " + gameState.wave;
		scoreText.text = "Score: " + gameState.score;

		beginText.gameObject.SetActive(!gameState.isRunning);
	}

	public void OnPlayerDeath()
	{
		gameState.isRunning = false;
		gameState.score = 0;
		gameState.wave = 0;

		asteroidPool.DespawnAllInstances();
		bulletPool.DespawnAllInstances();

		player.Reset();
		player.gameObject.SetActive(false);

		RefreshUI();
	}


	public void OnActiveAsteroidCountChanged(AsteroidType type)
	{
		switch (type)
		{
			case AsteroidType.Large:
			{
				gameState.score += 50;
			}
			break;
			case AsteroidType.Medium:
			{
				gameState.score += 100;
			}
			break;
			case AsteroidType.Small:
			{
				gameState.score += 200;
			}
			break;
		}

		if (asteroidPool.activeCount == 0)
		{
			BeginNextWave();
		}

		RefreshUI();
	}

	public static Vector2 WrapPosition(Vector2 pos)
	{
		if (pos.x > WorldState.bounds.max.x ||
			pos.x < WorldState.bounds.min.x)
		{
			pos.x = -pos.x;
		}

		if (pos.y > WorldState.bounds.max.y ||
			pos.y < WorldState.bounds.min.y)
		{
			pos.y = -pos.y;
		}

		return pos;
	}

	public static void SetRigidbodyState(Rigidbody2D body, RigidbodyState state)
	{
		body.position = state.position;
		body.rotation = state.rotation;
		body.velocity = state.velocity;
		body.angularVelocity = state.angularVelocity;
	}

	public static void SetRigidbodyState(Rigidbody2D body, Vector2 position, float rotation, Vector2 velocity, float angularVelocity)
	{
		body.position = position;
		body.rotation = rotation;
		body.velocity = velocity;
		body.angularVelocity = angularVelocity;
	}
}

