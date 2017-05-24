using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "Data", menuName = "Create AsteroidDB", order = int.MaxValue)]
public class AsteroidDB : ScriptableObject
{
	[SerializeField]
	private AsteroidDefaultValue smallDefault = new AsteroidDefaultValue();
	[SerializeField]
	private AsteroidDefaultValue mediumDefault = new AsteroidDefaultValue();
	[SerializeField]
	private AsteroidDefaultValue largeDefault = new AsteroidDefaultValue();

	public List<Asteroid> small = new List<Asteroid>();
	public List<Asteroid> medium = new List<Asteroid>();
	public List<Asteroid> large = new List<Asteroid>();

#if UNITY_EDITOR
	public void AddAsteroid(AsteroidType type, Vector2[] points, Sprite sprite)
	{
		AsteroidDefaultValue values = null;
		List<Asteroid> list = null;

		switch (type)
		{
			case AsteroidType.Large:
				list = large;
				values = largeDefault;
				break;
			case AsteroidType.Medium:
				list = medium;
				values = mediumDefault;
				break;
			case AsteroidType.Small:
				list = small;
				values = smallDefault;
				break;
		}

		Asteroid asteroid = new Asteroid();
		asteroid.type = type;
		asteroid.points = points;
		asteroid.sprite = sprite;
		asteroid.minVelocity = values.minVelocity;
		asteroid.maxVelocity = values.maxVelocity;
		asteroid.minAngular = values.minAngular;
		asteroid.maxAngular = values.maxAngular;

		if (list.FirstOrDefault(x => x.sprite == sprite) == null)
		{
			list.Add(asteroid);
		}
		else
		{
			Debug.Log("Asteroid with Sprite '" + sprite.name + "' already exists");
		}
	}
#endif
}

[System.Serializable]
public class AsteroidDefaultValue
{
	public float minVelocity = 1;
	public float maxVelocity = 5;
	public float minAngular = 5;
	public float maxAngular = 20;
}
