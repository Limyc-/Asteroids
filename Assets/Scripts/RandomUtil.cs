using UnityEngine;

public static class RandomUtil
{
	public static Vector2 OnRect()
	{
		return OnRect(Vector2.one);
	}

	public static Vector2 OnRect(Vector2 scale)
	{
		Vector2 result;
		float x = Random.Range(-1f, 1f);
		float y = Random.Range(-1f, 1f);

		float absX = Mathf.Abs(x);
		float absY = Mathf.Abs(y);

		if (absX > absY)
		{
			x = Mathf.Sign(x);
		}
		else if (absX < absY)
		{
			y = Mathf.Sign(y);
		}
		else
		{
			x = Mathf.Sign(x);
			y = Mathf.Sign(y);
		}

		result.x = x * scale.x;
		result.y = y * scale.y;

		return result;
	}
}
