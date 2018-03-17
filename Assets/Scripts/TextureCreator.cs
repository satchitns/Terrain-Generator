using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class TextureCreator : MonoBehaviour
{

	public int resolution = 256;
	private Texture2D texture;
	public Gradient coloring;

	public float seed = 0.0f;

	public float frequency = 1f;

	[Range(1, 8)]
	public int octaves = 1;

	[Range(1f, 4f)]
	public float lacunarity = 2f;

	[Range(0f, 1f)]
	public float persistence = 0.5f;
	void Awake()
	{
		texture = new Texture2D(resolution, resolution, TextureFormat.RGB24, true)
		{
			name = "Procedural texture",
			wrapMode = TextureWrapMode.Clamp,
			filterMode = FilterMode.Trilinear
		};
		GetComponent<MeshRenderer>().material.mainTexture = texture;
		FillTexture();
	}

	[ContextMenu("Fill")]
	private void FillTexture()
	{
		for (int y = 0; y < resolution; ++y)
		{
			for (int x = 0; x < resolution; ++x)
			{
				float pNoise = Noise.Perlin(((float)x) / resolution, ((float)y) / resolution, seed, frequency, octaves, lacunarity, persistence);
				texture.SetPixel(x, y, coloring.Evaluate((1.0f + pNoise)/2));
			}
		}
		texture.Apply();
	}
}