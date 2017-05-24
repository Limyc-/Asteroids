using System;
using UnityEngine;

public class AsteroidController : MonoBehaviour, IPoolable<AsteroidController>
{
	public Transform tf;
	public Rigidbody2D body;
	public PolygonCollider2D polyCollider;
	public SpriteRenderer render;
	public AsteroidType type;
	public LayerMask deathMask;
	public Action<AsteroidType, Vector2> OnDeath = delegate { };

	public int PoolIndex { get; set; }
	public Action<AsteroidController> OnDespawn { get; set; }

	private void Awake()
	{
		if (tf == null) { tf = GetComponent<Transform>(); }
		if (body == null) { body = GetComponent<Rigidbody2D>(); }
		if (polyCollider == null) { polyCollider = GetComponent<PolygonCollider2D>(); }
		if (render == null) { render = GetComponent<SpriteRenderer>(); }
	}

	public void Init(Asteroid asteroid, RigidbodyState state, Action<AsteroidType, Vector2> deathCallback)
	{
		this.polyCollider.SetPath(0, asteroid.points);
		this.render.sprite = asteroid.sprite;
		this.type = asteroid.type;
		this.OnDeath = deathCallback;

		Game.SetRigidbodyState(body, state);
	}

	private void FixedUpdate()
	{
		if (!GeometryUtility.TestPlanesAABB(WorldState.frustumPlanes, new Bounds(body.position, polyCollider.bounds.size)))
		{
			body.position = Game.WrapPosition(body.position);
		}
	}

	public void OnCollisionEnter2D(Collision2D col)
	{
		int otherLayer = (1 << col.gameObject.layer);

		if ((deathMask.value & otherLayer) > 0)
		{
			OnDeath(type, body.position);
			OnDespawn(this);
		}
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
		OnDeath = delegate { };
		tf.position = Game.DefaultPos;
		body.velocity = Vector2.zero;
		body.angularVelocity = 0;
	}
}