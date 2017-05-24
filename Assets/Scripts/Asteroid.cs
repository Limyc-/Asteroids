using UnityEngine;

[System.Serializable]
public class Asteroid
{
	public AsteroidType type;
	public Vector2[] points;
	public Sprite sprite;
	public float minVelocity;
	public float maxVelocity;
	public float minAngular;
	public float maxAngular;
}