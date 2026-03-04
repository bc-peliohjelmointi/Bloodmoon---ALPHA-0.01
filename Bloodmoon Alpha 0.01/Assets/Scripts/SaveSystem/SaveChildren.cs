using System;
using System.Collections.Generic;
using System.IO;
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
            if (File.Exists(SaveSystem.SaveFileName()))
            {
                SaveSystem.Save();
                loaded = true;
            }
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
            data.buildingsID = new List<GameObject>();
            data.isgrounded = new List<bool>();
            data.ListID = new List<int>();
            for (int i = 1; trans.Length > i; i++)
            {
                data.locations.Add(trans[i].position);
                data.rotations.Add(trans[i].rotation);
                data.names.Add(trans[i].name);
                data.buildingsID.Add(trans[i].gameObject);
                data.isgrounded.Add(trans[i].GetComponent<BuildingID>().IsOnGround);
                data.ListID.Add(trans[i].GetComponent<BuildingID>().BuildingListID);
            }
            Debug.Log(Convert.ToString(trans.Count()));
            data.buildings = new List<GameObject>();
            data.listnum = new List<int>();
            for (int i = 0; i < transform.GetComponent<BuildingColapse>().buildings.Count; i++)
            {
                for (int x = 0; x < transform.GetComponent<BuildingColapse>().buildings[i].Count; x++)
                {
                    data.buildings.Add(transform.GetComponent<BuildingColapse>().buildings[i][x].Me);
                    data.listnum.Add(i);
                }
            }
        }
    }

    public void Load(ChildSaveData data)
    {
        BuildingColapse coll = transform.GetComponent<BuildingColapse>();
        coll.buildings = new List<List<BuildingColapse.Structure>>();
        GameObject builder = GameObject.Find("Builder");
        for (int i = 0; data.names.Count > i; i++)
        {
            bool found = false;
            for (int x = 0; x < builder.GetComponent<Builder>().buildings.Count; x++)
            {
                if (builder.GetComponent<Builder>().buildings[x].name + "(Clone)" == data.names[i])
                {
                    found = true;
                    GameObject instance = Instantiate(builder.GetComponent<Builder>().buildings[x], data.locations[i], data.rotations[i], transform);
                    if (null == instance)
                    {
                        Debug.Log("load " + i + " Faild");
                    }
                    else
                    {
                        instance.GetComponent<BuildingID>().IsOnGround = data.isgrounded[i];
                        instance.GetComponent<BuildingID>().BuildingListID = data.ListID[i];

                        for(int h = 0; h < data.buildings.Count; h++)
                        {
                            if (data.buildingsID[i] == data.buildings[h])
                            {
                                while (coll.buildings.Count <= instance.GetComponent<BuildingID>().BuildingListID)
                                {
                                    coll.buildings.Add(new List<BuildingColapse.Structure>());
                                }
                                BuildingColapse.Structure structure = new BuildingColapse.Structure();
                                structure.Me = instance.gameObject;
                                structure.onGround = data.isgrounded[i];
                                coll.buildings[instance.GetComponent<BuildingID>().BuildingListID].Add(structure);
                            }
                        }
                    }
                }
            }
            if (!found && i != 0)
            {
                Debug.Log("Object " + i + " not found");
            }
        }
        builder.GetComponent<Builder>().update.NavUpdate();
        Debug.Log(coll.buildings.Count);
        foreach (List<BuildingColapse.Structure> build in coll.buildings)
        {
            Debug.Log(build.Count);
        }
    }
}
[System.Serializable]
public struct ChildSaveData
{
    public List<Vector3> locations;
    public List<Quaternion> rotations;
    public List<string> names;
    public List<bool> isgrounded;
    public List<GameObject> buildingsID;
    public List<int> ListID;

    public List<GameObject> buildings;
    public List<int> listnum;
}
