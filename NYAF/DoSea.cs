using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoSea : MonoBehaviour
{
	private Mesh mesh;
	private Vector3[] vertices;
	private int xSize = 100;
	private int zSize = 100;

	private int currentLevel = 0;

	// Start is called before the first frame update
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	private void Awake()
	{
		if (LocalSave.HasIntKey("FindingNYAF_currentLevel"))
		{
			currentLevel = LocalSave.GetInt("FindingNYAF_currentLevel");
		}

		Generate();
	}

	private void Generate()
	{
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.name = "Procedural Grid";

		vertices = new Vector3[(xSize + 1) * (zSize + 1)];
		for (int i = 0, z = 0; z <= zSize; z++)
		{
			for (int x = 0; x <= xSize; x++, i++)
			{
				float xShift = ((float)(x - xSize / 2)) / 10;
				float zShift = ((float)z) / 5 - 7;

				float xCurr, yCurr;

				float zOut; // Ignored here

				PlaceCharacters.applyFunction(xShift, 0, zShift, out xCurr, out yCurr, out zOut, currentLevel);

				vertices[i] = new Vector3(xCurr, yCurr, zShift - 4);
			}
		}
		mesh.vertices = vertices;

		int[] triangles = new int[xSize * zSize * 6];
		for (int ti = 0, vi = 0, y = 0; y < zSize; y++, vi++)
		{
			for (int x = 0; x < xSize; x++, ti += 6, vi++)
			{
				triangles[ti] = vi;
				triangles[ti + 3] = triangles[ti + 2] = vi + 1;
				triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
				triangles[ti + 5] = vi + xSize + 2;
			}
		}
		mesh.triangles = triangles;
		MeshCollider meshCollider = GetComponent<MeshCollider>();
		meshCollider.sharedMesh = mesh;
	}
}
