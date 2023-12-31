using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class CreateShopLineCsharp : MonoBehaviour
{
    public string rootName;

    private List<ManageOneShopLine> allShopEntries = new List<ManageOneShopLine>();

    // Start is called before the first frame update
    void Start()
    {
        /*
        foreach (Transform childShop in transform)
        {
            ManageOneShopLine oneLineScript = childShop.gameObject.GetComponent<ManageOneShopLine>();

            allShopEntries.Add(oneLineScript);
        }

        writeAllOneLine();
        */
        /*
        foreach (Transform childShop in transform)
        {
            ManageOneShopLine oneLineScript = childShop.gameObject.GetComponent<ManageOneShopLine>();

            allShopEntries.Add(oneLineScript);
        }

        writeAllOneLineDeleteSave();
        */
    }

    private void writeAllOneLine()
    {
        string docPath = Application.persistentDataPath;

        // Write the string array to a new file named "WriteLines.txt".
        using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, rootName+".cs")))
        {
            outputFile.WriteLine("public class ShopEntry\n{\n public double price;\n public double euroPrice;\n public string key;\n public string name;\n public string description;\n  public ShopEntry(double price, double euroPrice, string key, string name, string description)\n {\n ");
            foreach (ManageOneShopLine line in allShopEntries)
            {
                outputFile.WriteLine("      new ShopEntry("+line.price.ToString().Replace(",",".")+", "+line.euroPrice.ToString().Replace(",", ".") + ", \""+line.item+ "\", \"" + line.name + "\", \"" + line.desc + "\"),");
            }
            outputFile.WriteLine("   };\n}");
        }

        using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, rootName + "-desc.cs")))
        {
            outputFile.WriteLine("List<string> allDesc = new List<string>()\n{\n");
            foreach (ManageOneShopLine line in allShopEntries)
            {
                outputFile.WriteLine("    \"" + line.desc + "\",");
            }
            outputFile.WriteLine("   };\n}\n");
        }

        using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, rootName + "-cond.cs")))
        {
            outputFile.WriteLine("    bool hasKey = false;\n");

            foreach (ManageOneShopLine line in allShopEntries)
            {
                outputFile.WriteLine("    hasKey = LocalSave.HasBoolKey(\"" + line.item + "\");");

                outputFile.WriteLine("    if (hasKey)\n    {");
                outputFile.WriteLine("        bool isBought = LocalSave.GetBool(\"" + line.item + "\");");
                outputFile.WriteLine("        if (isBought)\n        {");
                outputFile.WriteLine("            // "+line.desc+"");
                outputFile.WriteLine("            // Do something here");
                outputFile.WriteLine("        }");
                outputFile.WriteLine("    }\n");
            }
        }
    }

    private void writeAllOneLineDeleteSave()
    {
        string docPath = Application.persistentDataPath;

        
        using (StreamWriter outputFile = new StreamWriter(Path.Combine(docPath, rootName + "-del.cs")))
        {
            foreach (ManageOneShopLine line in allShopEntries)
            {
                outputFile.WriteLine("    LocalSave.DeleteKey(\"" + line.item + "\");");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
