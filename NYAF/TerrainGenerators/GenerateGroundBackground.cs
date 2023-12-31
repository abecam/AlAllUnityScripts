﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace TerrainGenerators
{
    public class GenerateGroundBackground : MonoBehaviour
    {
        private Mesh mesh;
        private Vector3[] vertices;

        private int currentLevel = 0;

        public static float highestGround = -100;
        public static float lowestGround = 100;

        public GameObject groundTexture;
        public GameObject referenceImage;

        Texture2D referenceTexture;

        static int sizeTexture = 1024;

        private ArrayList allFunctions = new ArrayList();

        private bool noFunctionYet = true;

        class HeightAndColor
        {
            public float height = 0;
            public Color color = Color.white;
            public bool removeMe = false;
        }

        abstract class PointFunction
        {
            public float heightHill;
            public abstract void init(float xStartHill, float yStartHill, int type, float size, float height);
            public abstract HeightAndColor calculateAtPoint(float x, float y, bool noFunctionYetIn, ref bool noFunctionYetOut);
        }
        class PointFunctionRoundHill : PointFunction
        {
            float xShift;
            float yShift;

            float sizeHill; // Surface covered is perfectly square
            float halfSizeHill;
            float minusQuarterHeightHill;
            float minusFifthHeightHill;
            bool notCountAsHill = false;

            public override void init(float x, float y, int type, float size, float height)
            {
                // Type 1: Round hill
                // Start from current point to current point + size
                sizeHill = size * 7;
                halfSizeHill = sizeHill / 2;

                // Precalculate what we can
                heightHill = height * 2;
                minusQuarterHeightHill = heightHill - (heightHill / 4);
                minusFifthHeightHill = heightHill - (heightHill / 5);

                xShift = -x - sizeHill / 2;
                yShift = -y - sizeHill / 2;

                if (type == 10)
                {
                    notCountAsHill = true; // Do not avoid other small hills
                }
            }

            public override HeightAndColor calculateAtPoint(float x, float y, bool noFunctionYetIn, ref bool noFunctionYetOut)
            {
                //noFunctionYetOut = false;

                float xShifted = (x + xShift) / halfSizeHill;
                float yShifted = (y + yShift) / halfSizeHill; // Get cut...

                HeightAndColor ourNewData = new HeightAndColor();

                float height = heightHill * (1 - (Mathf.Pow(xShifted, 2) + Mathf.Pow(yShifted, 2)));

                if (height < 0)
                {
                    height = 0;
                }
                //height = 20f * Mathf.Cos(x / 10f) * Mathf.Cos(y / 10f);
                Color newColor = Color.white;

                //if ((height > heightHill - (heightHill/7) + 0.2f*(Mathf.Cos(xShifted * 10) + Mathf.Cos(yShifted * 10))) && (height < heightHill - (heightHill * 10) + 0.2f * (Mathf.Cos(xShifted * 10) + Mathf.Cos(yShifted * 10))))
                float variationForSnow = 4f * (Mathf.Cos(xShifted * 20) * Mathf.Cos(yShifted * 20));

                ///if ((height > heightHill - (heightHill / 4)) && (height < heightHill - (heightHill / 5)))
                if ((height > minusQuarterHeightHill + variationForSnow) && (height < minusFifthHeightHill + variationForSnow))
                {
                    // Black
                    newColor = Color.black;
                    //Debug.Log("Green");
                }
                else
                {
                    // light green on one side...
                }

                if (xShifted > 1 && yShifted > 1)
                {
                    Debug.Log("Remove me at " + xShifted + ", " + yShifted);
                    ourNewData.removeMe = true;
                }
                if (notCountAsHill)
                {
                    // Do nothing :)
                }
                else if (noFunctionYetIn && (xShifted > 1 || yShifted > 1))
                {
                    noFunctionYetOut = true;
                }
                else
                {
                    noFunctionYetOut = false;
                }

                ourNewData.height = height;
                ourNewData.color = newColor;

                //Debug.Log("Calculated " + ourNewData.height + " and " + ourNewData.color);
                return ourNewData;
            }
        }

        class PointFunctionCylHill : PointFunction
        {
            float xShift;
            float yShift;

            float sizeHill; // Surface covered is perfectly square
            float minusSeventhHeight;
            float minusTenthHeight;

            public override void init(float x, float y, int type, float size, float height)
            {
                // Type 0: Round hill
                // Start from current point to current point + size
                sizeHill = size / 4;
                heightHill = height / 4;
                minusSeventhHeight = heightHill - (heightHill / 7);
                minusTenthHeight = heightHill - (heightHill / 10);

                xShift = -x - size / 2;
                yShift = -y - size / 2;
            }

            public override HeightAndColor calculateAtPoint(float x, float y, bool noFunctionYetIn, ref bool noFunctionYetOut)
            {
                noFunctionYetOut = false;

                float xShifted = (x + xShift) / sizeHill;
                float yShifted = 0; //  (y + yShift)/sizeHill; // Get cut...

                HeightAndColor ourNewData = new HeightAndColor();

                float height = heightHill - (Mathf.Pow(xShifted, 2) + Mathf.Pow(yShifted, 2));

                if (height < 0)
                {
                    height = 0;
                }
                //height = 20f * Mathf.Cos(x / 10f) * Mathf.Cos(y / 10f);
                Color newColor = Color.white;
                float variationForSnow = Mathf.Cos(xShifted / 10) + Mathf.Cos(yShifted / 10);

                if ((height > minusSeventhHeight + variationForSnow) && (height < minusTenthHeight + variationForSnow))
                {
                    // Black
                    newColor = Color.black;
                    //Debug.Log("Green");
                }
                else
                {
                    // light green on one side...
                }

                if (xShifted > 1 && yShifted > 1)
                {
                    Debug.Log("Remove me at " + xShifted + ", " + yShifted);
                    ourNewData.removeMe = true;
                }
                if (noFunctionYetIn && (xShifted > 1 || yShifted > 1))
                {
                    noFunctionYetOut = true;
                }
                ourNewData.height = height;
                ourNewData.color = newColor;

                //Debug.Log("Calculated " + ourNewData.height + " and " + ourNewData.color);
                return ourNewData;
            }
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
            Renderer imageRenderer = referenceImage.GetComponent<Renderer>();
            referenceTexture = (Texture2D)imageRenderer.material.mainTexture;

            Debug.Log("Working on " + referenceTexture);

            Generate();
        }


        private void Generate()
        {
            GetComponent<MeshFilter>().mesh = mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

            mesh.name = "Procedural Grid";

            HeightAndColor pointInfo = new HeightAndColor();

            calculateVertex(pointInfo);

            // Now the 8k texture
            clearFunctions();

            int sizeGenTexture = 2; // 2 for testing or heightmap

            var textureHeightMap = new Texture2D(sizeTexture * sizeGenTexture, sizeTexture * sizeGenTexture, TextureFormat.R16, false);
            var textureHeightMapInfo = new Texture2D(sizeTexture * sizeGenTexture, sizeTexture * sizeGenTexture, TextureFormat.RGB24, false);
            //uint[] rawData = new byte[sizeTexture * sizeGenTexture * sizeTexture * sizeGenTexture];
            int sizeHTexture = sizeTexture * sizeGenTexture;
            // Heightmap

            // Find the min/max (as somehow the function is dependent on the size it seems)
            generateHeighMap(pointInfo, sizeGenTexture, textureHeightMap, textureHeightMapInfo);

            sizeGenTexture = 4;

            // Texture
            var texture = new Texture2D(sizeHTexture * sizeGenTexture, sizeHTexture * sizeGenTexture, TextureFormat.RGB24, false);
            texture.filterMode = FilterMode.Trilinear;

            for (int y = 0, i = 0; y <= sizeHTexture * sizeGenTexture; y++)
            {
                for (int x = 0; x <= sizeHTexture * sizeGenTexture; x++, i++)
                {
                    Color whatToPlaceRGBf = textureHeightMap.GetPixel(x / sizeGenTexture, y / sizeGenTexture);
                    Color32 whatToPlaceRGB = whatToPlaceRGBf;

                    // then call all functions to get the new height and texture
                    pointInfo = new HeightAndColor();
                    pointInfo.height = 100*whatToPlaceRGB.r;

                    if (pointInfo.height < 8)
                    {
                        //Debug.Log("Addind a small hill!");

                        // Add a small one.
                        //addAFunction(x, y, 1, Random.value * 10 + 5, Random.value * 10 + 5);
                        float fading = 1 - pointInfo.height / 8; // 1 / (1 + pointInfo.height);

                        // Some grass down there
                        Color grass = new Color(Random.value * 0.1f, Random.value * 0.2f + 0.8f, Random.value * 0.1f);
                        pointInfo.color = Color.Lerp(pointInfo.color, grass, fading);
                    }

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
            for (int y = 50, i = 0; y <= sizeTexture * sizeGenTexture - 50 ; y++)
            {
                for (int x = 50; x <= sizeTexture * sizeGenTexture - 50; x++, i++)
                {
                    noFunctionYet = true;

                    if (Random.value > 0.999992f)
                    {
                        //Debug.Log("Addind a small hill!");
                        //writer.WriteLine("Addind a small hill!");
                        // Eventually add a small one.
                        addAFunction(x, y, 1, Random.value * 50, Random.value * 50 + 50);
                    }

                    // then call all functions to get the new height and texture
                    pointInfo = callAllFunctions(x, y, sizeGenTexture, ref noFunctionYet);

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
            clearFunctions();

            for (int y = 0, i = 0; y <= sizeTexture * sizeGenTexture; y++)
            {
                for (int x = 0; x <= sizeTexture * sizeGenTexture; x++, i++)
                {
                    noFunctionYet = true;

                    noFunctionYet = true;

                    if (Random.value > 0.999992f)
                    {
                        //Debug.Log("Addind a small hill!");
                        //writer.WriteLine("Addind a small hill!");
                        // Eventually add a small one.
                        addAFunction(x, y, 1, Random.value * 50, Random.value * 50 + 50);
                    }

                    // then call all functions to get the new height and texture
                    pointInfo = callAllFunctions(x, y, sizeGenTexture, ref noFunctionYet);

                    float fromNoFunction = noFunctionYet ? 1.0f : 0.0f;

                    if (noFunctionYet && Random.value > 0.9995f)
                    {
                        //Debug.Log("Addind a small hill!");

                        // Add a small one.
                        //addAFunction(x, y, 10, Random.value * 5 + 5, Random.value * 5 + 2);
                        // Eventually add a small one.
                        addAFunction(x, y, 10, Random.value * 5 + 5, Random.value * 5 + 2);
                    }

                    float heightColor = ((pointInfo.height - lowestGround) / (highestGround - lowestGround));
                    //uint heightColor16b = (uint )(65535.0 * heightColor);
                    textureHeightMap.SetPixel(x, y, new Color(heightColor, heightColor, heightColor));
                    textureHeightMapInfo.SetPixel(x, y, new Color(fromNoFunction, 0, 0));

                    //rawData[x + sizeTexture * sizeGenTexture * y] = heightColor16b;
                }
            }

            clearFunctions();
            return pointInfo;
        }

        private HeightAndColor calculateVertex(HeightAndColor pointInfo)
        {
            vertices = new Vector3[(sizeTexture + 1) * (sizeTexture + 1)];
            Vector2[] uv = new Vector2[vertices.Length];

            for (int y = 0, i = 0; y <= sizeTexture; y++)
            {
                for (int x = 0; x <= sizeTexture; x++, i++)
                {
                    noFunctionYet = true;

                    if (Random.value > 0.999992f)
                    {
                        //Debug.Log("Addind a small hill!");
                        //writer.WriteLine("Addind a small hill!");
                        // Eventually add a small one.
                        addAFunction(x, y, 1, Random.value * 50, Random.value * 50 + 50);
                    }

                    // then call all functions to get the new height and texture
                    pointInfo = callAllFunctions(x, y, 1, ref noFunctionYet);

                    if (noFunctionYet && Random.value > 0.9995f)
                    {
                        //Debug.Log("Addind a small hill!");
                        //writer.WriteLine("Addind a small hill!");
                        // Eventually add a small one.
                        addAFunction(x, y, 10, Random.value * 5 + 5, Random.value * 5 + 2);
                    }

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
            var path = name + index + ".png";
            File.WriteAllBytes(path, pngData);
            AssetDatabase.Refresh();
        }

        private static void CreateImageRawFiles(Texture2D tex, int index, string name)
        {
            var path = name + index + ".raw";
            File.WriteAllBytes(path, tex.GetRawTextureData());
            AssetDatabase.Refresh();
        }

        /*
         * Constant relief to add... Not sure if useful.
         */
        private HeightAndColor generalRelief(int x, int y)
        {
            HeightAndColor aHeightAndColor = new HeightAndColor();

            return aHeightAndColor;
        }

        private HeightAndColor callAllFunctions(int x, int y, int withSize, ref bool noFunctionYetOut)
        {
            ArrayList functionToRemove = new ArrayList();
            HeightAndColor aHeightAndColor = new HeightAndColor();
            bool isFirst = true;
            float maxCurrentHeight = 0;
            bool noFunctionYetIn = true;
            //noFunctionYetOut = true;

            foreach (PointFunction aPointFunction in allFunctions)
            {
                HeightAndColor aNewHeightAndColor = aPointFunction.calculateAtPoint(x, y, noFunctionYetIn, ref noFunctionYetOut);
                if (aNewHeightAndColor.height > 0)
                {
                    maxCurrentHeight += aPointFunction.heightHill;
                }
                if (!noFunctionYetOut)
                {
                    noFunctionYetIn = false;
                }
                //Debug.Log("All: Calculated " + aNewHeightAndColor.height + " and " + aNewHeightAndColor.color);

                if (aNewHeightAndColor.removeMe)
                {
                    functionToRemove.Add(aPointFunction);
                }
                // Add/Mix the value of the result
                aHeightAndColor.height += aNewHeightAndColor.height;

                // keep the darker color
                if (isFirst)
                {
                    aHeightAndColor.color = aNewHeightAndColor.color;
                }
                else if (aHeightAndColor.color.grayscale > aNewHeightAndColor.color.grayscale)
                {
                    aHeightAndColor.color = aNewHeightAndColor.color;
                }
                isFirst = false;
            }

            aHeightAndColor.color = getColorFromAltitude(x, y, aHeightAndColor.height, maxCurrentHeight, withSize);
            // Clean-up
            foreach (PointFunction aPointFunctionToRemove in functionToRemove)
            {
                allFunctions.Remove(aPointFunctionToRemove);
                Debug.Log("Removed one function, remaining: " + allFunctions.Count);
            }

            //Debug.Log("All: Calculated " + aHeightAndColor.height + " and " + aHeightAndColor.color);

            return aHeightAndColor;
        }

        private void addAFunction(int x, int y, int type, float size, float height)
        {
            switch (type)
            {
                case 1:
                    PointFunctionRoundHill aNewHillFct = new PointFunctionRoundHill();

                    aNewHillFct.init(x, y, type, size, height);

                    Debug.Log("Adding hill function to functions");

                    allFunctions.Add(aNewHillFct);
                    break;
                case 10:
                    PointFunctionRoundHill aNewHillFct2 = new PointFunctionRoundHill();

                    aNewHillFct2.init(x, y, type, size, height);

                    Debug.Log("Adding hill function to functions");

                    allFunctions.Add(aNewHillFct2);
                    break;
                default:
                    /*
                    PointFunctionCylHill aNewCylHillFct = new PointFunctionCylHill();

                    aNewCylHillFct.init(x, y, type, size, height);

                    Debug.Log("Adding cyl hill function to functions");

                    allFunctions.Add(aNewCylHillFct);
                    */
                    break;
            }
        }

        private void clearFunctions()
        {
            allFunctions.Clear();
        }
        public Color getColorFromAltitude(float x, float y, float height, float maxHill, int withSize)
        {
            //float sizeHill = withSize * 7;
            //float halfSizeHill = sizeHill / 2;

            //float xShift = -x - halfSizeHill;
            //float yShift = -y - halfSizeHill;

            float xShifted = x;
            float yShifted = y; //  (y + yShift)/sizeHill; // Get cut...
                                //float xShifted = (x + xShift) / halfSizeHill;
                                //float yShifted = (y + yShift) / halfSizeHill;

            if (height < 0)
            {
                height = 0;
            }
            //height = 20f * Mathf.Cos(x / 10f) * Mathf.Cos(y / 10f);
            Color newColor = Color.white;

            //if ((height > heightHill - (heightHill/7) + 0.2f*(Mathf.Cos(xShifted * 10) + Mathf.Cos(yShifted * 10))) && (height < heightHill - (heightHill * 10) + 0.2f * (Mathf.Cos(xShifted * 10) + Mathf.Cos(yShifted * 10))))
            ///if ((height > heightHill - (heightHill / 4)) && (height < heightHill - (heightHill / 5)))
            //if ((height > maxHill - (maxHill / 4) + sizeHill*4 * (Mathf.Cos(8*xShifted) * Mathf.Cos(8*yShifted))) && (height < maxHill - (maxHill / 4.5f) + sizeHill*6 * (Mathf.Cos(8*xShifted) * Mathf.Cos(8*yShifted))))
            //if ((height > maxHill - (maxHill / 4)) && (height < maxHill - (maxHill / 4.5f)))
            //if ((height > 0) && (height < maxHill))
            if ((height > maxHill - (maxHill / 4) + withSize * 28 * (Mathf.Cos(xShifted / 20) * Mathf.Cos(yShifted / 20))) && (height < maxHill - (maxHill / 4.5f) + withSize * 42 * (Mathf.Cos(xShifted / 20) * Mathf.Cos(yShifted / 20))))
            {
                // Black
                newColor = Color.black;
                //Debug.Log("Green");
            }
            else
            {
                // light green on one side...
            }

            //Debug.Log("Calculated " + ourNewData.height + " and " + ourNewData.color);
            return newColor;
        }
    }
}