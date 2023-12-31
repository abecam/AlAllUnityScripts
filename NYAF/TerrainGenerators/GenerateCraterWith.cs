using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TerrainGenerators
{
    public class GenerateCraterWith : MonoBehaviour
    {
        private Mesh mesh;
        private Vector3[] vertices;

        private int currentLevel = 0;

        public static float highestGround = -100;
        public static float lowestGround = 100;

        public GameObject groundTexture;

        static int sizeTexture = 1024;
        static float currentSize = sizeTexture;
        static float middle = currentSize / 2;

        private ArrayList allFunctions = new ArrayList();

        private bool noFunctionYet = true;

        private static string timeStamp;

        private static bool wasStarted = false;

        class HeightAndColor
        {
            public float height = 0;
            public Color color = Color.white;
            public bool removeMe = false;
        } 

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
            if (!wasStarted)
            { 
                DateTime dt = DateTime.Now;
                timeStamp = dt.ToString("yyyy-MM-dd-HH-mm");

                Generate();

                wasStarted = true; // Run the generation only once!
            }
        }


        private void Generate()
        {
            GetComponent<MeshFilter>().mesh = mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

            mesh.name = "Procedural Grid";

            HeightAndColor pointInfo = new HeightAndColor();

            calculateVertex(pointInfo);

            // Now the 8k texture

            int sizeGenTexture = 2; // 2 for testing or heightmap
            currentSize = sizeTexture * sizeGenTexture;
            middle = (int)(currentSize / 2);

            var textureHeightMap = new Texture2D(sizeTexture * sizeGenTexture, sizeTexture * sizeGenTexture, TextureFormat.R16, false);
            var textureHeightMapInfo = new Texture2D(sizeTexture * sizeGenTexture, sizeTexture * sizeGenTexture, TextureFormat.RGB24, false);
            //uint[] rawData = new byte[sizeTexture * sizeGenTexture * sizeTexture * sizeGenTexture];

            // Heightmap

            // Find the min/max (as somehow the function is dependent on the size it seems)
            generateHeighMap(pointInfo, sizeGenTexture, textureHeightMap, textureHeightMapInfo);

            sizeGenTexture = 4; // 4 for first plan texture
            currentSize = sizeTexture * sizeGenTexture;
            middle = (int)(currentSize / 2);

            // Texture
            var texture = new Texture2D(sizeTexture * sizeGenTexture, sizeTexture * sizeGenTexture, TextureFormat.RGB24, false);
            texture.filterMode = FilterMode.Trilinear;

            for (int y = 0, i = 0; y <= sizeTexture * sizeGenTexture; y++)
            {
                for (int x = 0; x <= sizeTexture * sizeGenTexture; x++, i++)
                {
                    // then call all functions to get the new height and texture
                    pointInfo = callAllFunctionsWColors(x, y, sizeGenTexture);

                    texture.SetPixel(x, y, pointInfo.color);
                }
            }


            texture.Apply();
            CreateImagePngFiles(texture, 0, "TextureTerrain");

            CreateImagePngFiles(textureHeightMap, 0, "HeightmapTerrainPng");
            CreateImageRawFiles(textureHeightMap, 0, "HeightmapTerrain");
            CreateImagePngFiles(textureHeightMapInfo, 0, "HeightmapTerrainInfo");

            // connect texture to material of GameObject this script is attached to
            Renderer imageRenderer = groundTexture.GetComponent<Renderer>();
            imageRenderer.material.mainTexture = texture;
        }

        private HeightAndColor generateHeighMap(HeightAndColor pointInfo, int sizeGenTexture, Texture2D textureHeightMap, Texture2D textureHeightMapInfo)
        {
            for (int y = 0, i = 0; y <= sizeTexture * sizeGenTexture; y++)
            {
                for (int x = 0; x <= sizeTexture * sizeGenTexture; x++, i++)
                {
                    // then call all functions to get the new height and texture
                    pointInfo = callAllFunctions(x, y);

                    if (pointInfo.height > highestGround)
                    {
                        highestGround = pointInfo.height;
                    }
                    else if (pointInfo.height < lowestGround)
                    {
                        lowestGround = pointInfo.height;
                    }
                }
            }

            // Stop before the end so the small hills are not cut too.
            for (int y = 0, i = 0; y <= sizeTexture * sizeGenTexture; y++)
            {
                for (int x = 0; x <= sizeTexture * sizeGenTexture; x++, i++)
                {
                    // then call all functions to get the new height and texture
                    pointInfo = callAllFunctions(x, y);

                    float heightColor = ((pointInfo.height - lowestGround) / (highestGround - lowestGround));
                    //uint heightColor16b = (uint )(65535.0 * heightColor);
                    textureHeightMap.SetPixel(x, y, new Color(heightColor, heightColor, heightColor));
                    //textureHeightMapInfo.SetPixel(x, y, new Color(fromNoFunction, 0, 0));

                    //rawData[x + sizeTexture * sizeGenTexture * y] = heightColor16b;
                }
            }

            return pointInfo;
        }

        private HeightAndColor calculateVertex(HeightAndColor pointInfo)
        {
            vertices = new Vector3[(sizeTexture + 1) * (sizeTexture + 1)];
            Vector2[] uv = new Vector2[vertices.Length];

            int maxSizeSmallHill = sizeTexture - 50;

            for (int y = 0, i = 0; y <= sizeTexture; y++)
            {
                for (int x = 0; x <= sizeTexture; x++, i++)
                {
                    // then call all functions to get the new height and texture
                    pointInfo = callAllFunctions(x, y);

                    vertices[i] = new Vector3(x, pointInfo.height, y);
                    //Debug.Log("Height is " + pointInfo.height);

                    uv[i] = new Vector2(x / (float)sizeTexture, y / (float)sizeTexture);
                }
            }

            mesh.vertices = vertices;

            int[] triangles = new int[sizeTexture * sizeTexture * 6];

            for (int ti = 0, vi = 0, y = 0; y < sizeTexture; y++, vi++)
            {
                for (int x = 0; x < sizeTexture; x++, ti += 6, vi++)
                {
                    triangles[ti] = vi;
                    triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                    triangles[ti + 4] = triangles[ti + 1] = vi + sizeTexture + 1;
                    triangles[ti + 5] = vi + sizeTexture + 2;
                }
            }

            mesh.triangles = triangles;
            mesh.uv = uv;
            //Unwrapping.GenerateSecondaryUVSet(mesh);
            //mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            //mesh.RecalculateTangents();
            MeshCollider meshCollider = GetComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;

            return pointInfo;
        }

        private static void CreateImagePngFiles(Texture2D tex, int index, string name)
        {
            var pngData = tex.EncodeToPNG();
            if (pngData.Length < 1)
            {
                return;
            }
            var path = name + timeStamp + index + ".png";
            File.WriteAllBytes(path, pngData);
            AssetDatabase.Refresh();
        }

        private static void CreateImageRawFiles(Texture2D tex, int index, string name)
        {
            var path = name + index + timeStamp + ".raw";
            File.WriteAllBytes(path, tex.GetRawTextureData());
            AssetDatabase.Refresh();
        }

        private HeightAndColor callAllFunctions(int x, int y)
        {
            HeightAndColor aHeightAndColor = new HeightAndColor();
            //noFunctionYetOut = true;

            float xShifted = ((float)x - middle) / middle;
            float yShifted = ((float)y - middle) / middle;

            float heightCrater = 500*(Mathf.Pow(0.8f*xShifted, 2) + Mathf.Pow(0.8f*yShifted, 2)) - 100;
            if (heightCrater < 40)
            {
                // Some weird underwater
                heightCrater = 4 * (Mathf.Pow(Mathf.Cos(40*xShifted), 2) + Mathf.Pow(Mathf.Sin(40*yShifted), 2));
            }
            aHeightAndColor.height = heightCrater;
            //Debug.Log("All: Calculated " + aHeightAndColor.height + " and " + aHeightAndColor.color);

            return aHeightAndColor;
        }

        private HeightAndColor callAllFunctionsWColors(int x, int y, int withSize)
        {
            HeightAndColor aHeightAndColor = callAllFunctions(x,y);

            float maxCurrentHeight = 0;
            //noFunctionYetOut = true;

            aHeightAndColor.color = getColorFromAltitude(x, y, aHeightAndColor.height, maxCurrentHeight, withSize);
            // Clean-up
           

            //Debug.Log("All: Calculated " + aHeightAndColor.height + " and " + aHeightAndColor.color);

            return aHeightAndColor;
        }

        public Color getColorFromAltitude(float x, float y, float height, float maxHill, int withSize)
        {
            // Less green, more blue with the altitude
            float blue = 0.5f*Mathf.Cos(x / (2*withSize*height));
            float green = 0.5f*Mathf.Cos(y / (2*withSize*height));

            Color newColor = new Color(0.4f+blue, 0.2f, 0.5f + green); 

            return newColor;
        }
    }
}