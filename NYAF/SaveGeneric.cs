using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class SaveGeneric
{
    public Dictionary<string, float> floatValues = new Dictionary<string, float>();
    public Dictionary<string, int> intValues = new Dictionary<string, int>();
    public Dictionary<string, bool> boolValues = new Dictionary<string, bool>();

    public SaveGenericList toSaveInList()
    {
        SaveGenericList ourNewSaveInList = new SaveGenericList();

        foreach (string keyFloat in floatValues.Keys)
        {
            ourNewSaveInList.keysFloat.Add(keyFloat);
            ourNewSaveInList.valuesFloat.Add(floatValues[keyFloat]);
        }

        foreach (string keyInt in intValues.Keys)
        {
            ourNewSaveInList.keysInt.Add(keyInt);
            ourNewSaveInList.valuesInt.Add(intValues[keyInt]);
        }

        foreach (string keyBool in boolValues.Keys)
        {
            ourNewSaveInList.keysBool.Add(keyBool);
            ourNewSaveInList.valuesBool.Add(boolValues[keyBool]);
        }

        return ourNewSaveInList;
    }

    public void fromSaveInList(SaveGenericList saveInList)
    {
        floatValues = new Dictionary<string, float>();

        for (int iFloat = 0; iFloat < saveInList.keysFloat.Count; iFloat++)
        {
            string key = saveInList.keysFloat[iFloat];
            float value = saveInList.valuesFloat[iFloat];

            floatValues.Add(key, value);
        }

        intValues = new Dictionary<string, int>();

        for (int iInt = 0; iInt < saveInList.keysInt.Count; iInt++)
        {
            string key = saveInList.keysInt[iInt];
            int value = saveInList.valuesInt[iInt];

            intValues.Add(key, value);
        }

        boolValues = new Dictionary<string, bool>();

        for (int iBool = 0; iBool < saveInList.keysBool.Count; iBool++)
        {
            string key = saveInList.keysBool[iBool];
            bool value = saveInList.valuesBool[iBool];

            boolValues.Add(key, value);
        }
    }
}
