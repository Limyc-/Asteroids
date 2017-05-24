using UnityEngine;

public class FlickerSprite : MonoBehaviour
{
	public SpriteRenderer sprite;
	[Range(0f, 1f)]
	public float min = 0.75f;
	[Range(0f, 1f)]
	public float max = 1f;
	public float speed = 1f;
	public float dir = 1f;

	private void Awake()
	{
		sprite = GetComponent<SpriteRenderer>();
		max = Mathf.Max(max, min);
	}

	public void Update()
	{
		var color = sprite.color;

		color.a += dir * speed * Time.deltaTime;

		if (color.a >= max || color.a <= min)
		{
			dir = -dir;
			color.a = Mathf.Clamp(color.a, min, max);
		}

		sprite.color = color;
	}
}
