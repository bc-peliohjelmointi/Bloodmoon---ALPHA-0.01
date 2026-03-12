using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class SaveChildren : MonoBehaviour
{
    // Update is called once per frame
    bool loaded = false;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            SaveSystem.Save();
            loaded = true;
        }
        if (Input.GetKeyDown(KeyCode.P) && !loaded)
        {
            SaveSystem.Load();
            loaded = true;
        }
    }

    public void Save(ref ChildSaveData data)
    {
        if (transform.childCount > 0)
        {
            data.names = new List<string>();
            Transform[] trans = transform.GetComponentsInChildren<Transform>();
            data.locations = new List<Vector3>();
            data.rotations = new List<Quaternion>();
            for (int i = 0; trans.Length > i; i++)
            {
                data.locations.Add(trans[i].position);
                data.rotations.Add(trans[i].rotation);
            }
            Debug.Log(Convert.ToString(trans.Count()));
            for (int i = 0; trans.Length > i; i++)
            {
                data.names.Add(trans[i].name);
            }
        }
    }

    public void Load(ChildSaveData data)
    {
        GameObject builder = GameObject.Find("Builder");
        for (int i = 0; data.names.Count > i; i++)
        {
            bool found = false;
            for (int x = 0; x < builder.GetComponent<Builder>().buildings.Count; x++)
            {
                if (builder.GetComponent<Builder>().buildings[x].name + "(Clone)" == data.names[i])
                {
                    found = true;
                    bool work = Instantiate(builder.GetComponent<Builder>().buildings[x], data.locations[i], data.rotations[i], transform);
                    if (!work)
                    {
                        Debug.Log("load " + i + " Faild");
                    }
                }
            }
            if (!found && i != 0)
            {
                Debug.Log("Object " + i + " not found");
            }
        }
        builder.GetComponent<Builder>().update.NavUpdate();
    }
}
[System.Serializable]
public struct ChildSaveData
{
    public List<Vector3> locations;
    public List<Quaternion> rotations;
    public List<string> names;
}
