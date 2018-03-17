using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TerrainGenerator : MonoBehaviour
{
	public float seed = 0.0f;

	[Range(1, 200)]
	public int gridSize = 50;

	public Gradient coloring;

	public float frequency = 1f;

	[Range(1, 8)]
	public int octaves = 1;

	[Range(1f, 4f)]
	public float lacunarity = 2f;

	[Range(0f, 1f)]
	public float persistence = 0.5f;

	[Range(0f, 10f)]
	public float displacementScale = 1f;

	public Vector2 offset;

	public bool showNormals = false;

	public bool water = false;

	Mesh mesh;
	Color[] colors;
	Vector3[] vertices;
	Vector3[] normals;
	public void GenerateTerrain()
	{
		if (mesh == null)
		{
			mesh = new Mesh
			{
				name = "Terrain"
			};
			GetComponent<MeshFilter>().mesh = mesh;
		}
		CreateGrid();
		GenerateNoise();
	}
	void CreateGrid()
	{
		mesh.Clear();
		float stepSize = 1.0f / gridSize;
		vertices = new Vector3[(gridSize + 1) * (gridSize + 1)];
		int[] triangles = new int[gridSize * gridSize * 6];
		Vector2[] uvs = new Vector2[vertices.Length];
		normals = new Vector3[vertices.Length];
		colors = new Color[vertices.Length];
		for (int i = 0, k = 0; i <= gridSize; ++i)
		{
			for (int j = 0; j <= gridSize; ++j, ++k)
			{
				vertices[k] = new Vector3(j * stepSize - 0.5f, 0, i * stepSize - 0.5f);
				uvs[k] = new Vector2(j * stepSize, i * stepSize);
				normals[k] = Vector3.up;
				colors[k] = Color.red;
			}
		}
		for (int i = 0, k = 0, t = 0; i < gridSize; ++i, ++k) //k is incremented here to handle the extra vertex in each row
		{
			for (int j = 0; j < gridSize; ++j, ++k, t += 6)
			{
				triangles[t] = k;
				triangles[t + 1] = k + gridSize + 1;
				triangles[t + 2] = k + 1; //counter-clockwise
				triangles[t + 3] = k + 1;
				triangles[t + 4] = k + gridSize + 1;
				triangles[t + 5] = k + gridSize + 2;
			}
		}
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		mesh.normals = normals;
	}
	void GenerateNoise()
	{
		float amplitude = displacementScale / frequency;
		for (int k = 0, j = 0; j <= gridSize; ++j)
		{
			for (int i = 0; i <= gridSize; ++i, ++k)
			{
				float sample = Noise.Perlin((float)j / gridSize + offset.x, (float)i / gridSize + offset.x, seed, frequency, octaves, lacunarity, persistence) * 0.5f;
				colors[k] = coloring.Evaluate(sample + 0.5f);
				vertices[k].y = sample * amplitude;
			}
		}
		mesh.vertices = vertices;
		mesh.colors = colors;
		mesh.RecalculateNormals();
		normals = mesh.normals;
	}

	private void OnDrawGizmosSelected()
	{
		float scale = 1f / gridSize;
		if (showNormals && vertices != null)
		{
			Gizmos.color = Color.yellow;
			for (int v = 0; v < vertices.Length; v++)
			{
				Gizmos.DrawRay(transform.position + vertices[v], normals[v] * scale);
			}
		}
	}
	public void Awake()
	{
		GenerateTerrain();
	}

	public void Update()
	{
		if (water)
		{
			seed += 0.008f/frequency;
			GenerateNoise();
		}
	}
}