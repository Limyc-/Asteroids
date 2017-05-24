using System;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour, IPoolable<Bullet>
{
	public Transform tf;
	public Rigidbody2D body;
	public SpriteRenderer sprite;
	public BoxCollider2D renderBoundsCollider;

	public float lifeTime = 2f;
	public float startTime = 0;
	public Vector2 size;

	public Action OnDeathAction;

	public int PoolIndex { get; set; }
	public Action<Bullet> OnDespawn { get; set; }

	private void Awake()
	{
		tf = this.transform;
		body = GetComponent<Rigidbody2D>();

		OnDeathAction = delegate { };
	}

	public void Start()
	{
		size = renderBoundsCollider.bounds.size;
		Destroy(renderBoundsCollider);
	}

	public void Init(Vector2 velocity, float angularVelocity, Action deathCallback)
	{
		Init(tf.position, tf.rotation.eulerAngles.z, velocity, angularVelocity, deathCallback);
	}

	public void Init(Vector2 position, float rotation, Vector2 velocity, float angularVelocity, Action deathCallback)
	{
		Assert.IsNotNull(deathCallback);

		tf.position = position;
		tf.rotation = Quaternion.Euler(0, 0, rotation);

		Game.SetRigidbodyState(body, position, rotation, velocity, angularVelocity);

		if (deathCallback != null)
		{
			OnDeathAction += deathCallback;
		}

		startTime = Time.time;
	}

	public void FixedUpdate()
	{
		if (Time.time - startTime >= lifeTime)
		{
			Die();

			return;
		}

		if (!GeometryUtility.TestPlanesAABB(WorldState.frustumPlanes, new Bounds(body.position, size)))
		{
			body.position = Game.WrapPosition(body.position);
		}
	}

	public void OnCollisionEnter2D(Collision2D col)
	{
		Die();
	}

	public void Die()
	{
		OnDeathAction();
		OnDespawn(this);
	}

	public void Activate(Transform parent)
	{
		tf.SetParent(parent);

		body.simulated = true;
		gameObject.SetActive(true);
	}

	public void Deactivate(Transform parent)
	{
		tf.SetParent(parent);

		body.simulated = false;
		gameObject.SetActive(false);
	}

	public void Reset()
	{
		OnDeathAction = delegate { };
		tf.position = Game.DefaultPos;
		body.velocity = Vector2.zero;
		body.angularVelocity = 0;
	}
}
