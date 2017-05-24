using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
	public Transform tf;
	public Rigidbody2D body;
	public SpriteRenderer rocketFlame;
	public BoxCollider2D renderBoundsCollider;

	public float moveForce = 5;
	public float rotateForce = 5;

	public float moveDir = 0;
	public float rotateDir = 0;
	public bool fire = false;

	public PrefabPool<Bullet> bulletPool;
	public int maxBullets = 4;
	public int activeBulletsCount = 0;

	public Vector2 size;

	public Action OnDeath = delegate { };

	private void Awake()
	{
		tf = this.transform;
		body = GetComponent<Rigidbody2D>();
	}

	private void Start()
	{
		size = renderBoundsCollider.size;
		Destroy(renderBoundsCollider);
	}

	public void Init(PrefabPool<Bullet> bulletPool, Action deathCallback)
	{
		this.bulletPool = bulletPool;
		OnDeath += deathCallback;
	}

	private void Update()
	{
		PollInput();

		if (fire && activeBulletsCount < maxBullets)
		{
			Bullet b = bulletPool.SpawnInstance();
			b.Init(tf.position, tf.rotation.eulerAngles.z, tf.up * 10f, 0f, OnBulletHit);

			activeBulletsCount++;
		}

		rocketFlame.gameObject.SetActive(moveDir != 0);
	}

	private void FixedUpdate()
	{
		Vector2 force = tf.up * moveDir * moveForce;
		float torque = rotateDir * rotateForce;

		body.AddForce(force);
		body.AddTorque(torque);

		if (!GeometryUtility.TestPlanesAABB(WorldState.frustumPlanes, new Bounds(body.position, size)))
		{
			body.position = Game.WrapPosition(body.position);
		}
	}

	public void PollInput()
	{
		moveDir = Input.GetAxisRaw("Vertical");
		rotateDir = -Input.GetAxisRaw("Horizontal");
		fire = Input.GetKeyDown(KeyCode.Space);
	}

	private void OnBulletHit()
	{
		activeBulletsCount--;
	}

	private void OnCollisionEnter2D(Collision2D col)
	{
		OnDeath();
	}

	public void Reset()
	{
		Game.SetRigidbodyState(body, Vector2.zero, 0, Vector2.zero, 0);
		activeBulletsCount = 0;
	}
}