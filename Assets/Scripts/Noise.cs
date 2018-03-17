using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
	private static readonly int[] p = //Ken Perlin's hash look-up table
	{ 151,160,137,91,90,15,
	131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
	190,6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
	88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
	77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
	102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
	135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
	5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
	223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
	129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
	251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
	49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
	138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,
	151,160,137,91,90,15,
	131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
	190,6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
	88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
	77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
	102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
	135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
	5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
	223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
	129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
	251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
	49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
	138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
	};

	public static float Perlin(float x, float y, float z, float frequency = 5, int octaves = 1, float lacunarity = 2f, float persistence = 0.5f)
	{
		float result =  PerlinValue(x, y, z, frequency);
		float amplitude = 1f;
		float range = 1f;
		for(int i = 1; i < octaves; ++i)
		{
			frequency *= lacunarity;
			amplitude *= persistence;
			range += amplitude;
			result += PerlinValue(x, y, z, frequency) * amplitude;
		}
		return result / range;
	}
	private static float PerlinValue(float x, float y, float z, float frequency)
	{
		x *= frequency;
		y *= frequency;
		z *= frequency;
		int xCube = (int)x & 255;       //Restricting values to [0, 255]
		int yCube = (int)y & 255;
		int zCube = (int)z & 255;

		x -= (int)x;                    //Finding point location within unit cube
		y -= (int)y;
		z -= (int)z;

		float u = Fade(x);
		float v = Fade(y);
		float w = Fade(z);

		//Hash function - Hashes the 8 corner points of the unit cube
		//by performing a look-up on X, adding the Y value to that, performing
		//a look-up on that value, adding the Z value to that and performing 
		//a final look-up
		int aaa, aba, aab, abb, baa, bba, bab, bbb; 
		aaa = p[p[p[xCube    ] + yCube    ] + zCube    ];
		aba = p[p[p[xCube    ] + yCube + 1] + zCube    ];
		aab = p[p[p[xCube    ] + yCube    ] + zCube + 1];
		abb = p[p[p[xCube    ] + yCube + 1] + zCube + 1];
		baa = p[p[p[xCube + 1] + yCube    ] + zCube    ];
		bba = p[p[p[xCube + 1] + yCube + 1] + zCube    ];
		bab = p[p[p[xCube + 1] + yCube    ] + zCube + 1];
		bbb = p[p[p[xCube + 1] + yCube + 1] + zCube + 1];

		//the gradient function calculates the dot-product
		//between the pseudo-random gradient vector and the vector
		//from the input co-ordinate to the eight surrounding points 
		//in its cube
		return Mathf.Lerp
		(
			Mathf.Lerp
			(
				Mathf.Lerp(grad(aaa, x, y, z),
					grad(baa, x - 1, y, z),
					u),
				Mathf.Lerp(grad(aba, x, y - 1, z),
					grad(bba, x - 1, y - 1, z),
					u),
				v
			),
			Mathf.Lerp
			(
				Mathf.Lerp(grad(aab, x, y, z - 1),
					grad(bab, x - 1, y, z - 1),
					u),
				Mathf.Lerp(grad(abb, x, y - 1, z - 1),
					grad(bbb, x - 1, y - 1, z - 1),
					u),
				v
			),
			w
		);
	}
	static float Fade(float t)
	{
		//Improved fade function that Perlin recommends 
		//(avoids problems of second order derivative having non-zero value 
		//at zero and one). The fade function eases the values towards integral
		//values, leading to improved smoothness versus lerp
		//6t^5 - 15t^4 + 10t^3
		return t * t * t * (t * (t * 6 - 15) + 10); 
	}

	static float grad(int hash, float x, float y, float z)
	{
		//picks out of 16 gradient directions 
		//with 4 repeated (the vectors are from center to edges of cube)
		//(1, 1, 0), (-1, 1, 0), (1, -1, 0), (-1, -1, 0),
		//(1, 0, 1), (-1, 0, 1), (1, 0, -1), (-1, 0, -1),
		//(0, 1, 1), (0, -1, 1), (0, 1, -1), (0, -1, -1)
		int h = hash & 15;                                          //get low 4 bits
		float u = h < 8 ? x : y;									//set u = x if MSB is 0, else y
		float v = h < 4 ? y : h == 12 || h == 14 ? x : z;			//set v = y if MSB and second  bit are zero, v = x if MSB and second bit are 1,
																	//v = z if MSB and second bit are opposite
		return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);	//LSB and second-LSB are used to dictate sign for u and v
	}
}