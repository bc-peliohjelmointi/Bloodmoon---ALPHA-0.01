using System.Collections.Generic;
using UnityEngine;

public class BuildingColapse : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public class Structure
    {
        public bool onGround;
        public GameObject Me;
    }
    public List<List<Structure>> buildings = new List<List<Structure>>();
    
    public void newObject(GameObject go, int listNum = -1)
    {
        if (listNum == -1)
        {
            newBuilding(go);
        }
        else
        {
            Structure struc = new Structure();
            struc.onGround = go.GetComponent<BuildingID>().IsOnGround; 
            struc.Me = go;
            buildings[listNum].Add(struc);
            go.GetComponent<BuildingID>().BuildingListID = listNum;
        }
        Debug.Log(buildings.Count);
    }

    public void newBuilding(GameObject go)
    {
        bool listOpen = false;
        int openlist = -1;
        for (int i = 0; listOpen != true && buildings.Count - 1 > i ;i++)
        {
            if (buildings[i].Count == 0)
            {
                listOpen = true;
                openlist = i;
            }
        }
        if (listOpen)
        {
            Structure struc = new Structure();
            struc.onGround = true; struc.Me = go;
            buildings[openlist].Add(struc);
            go.GetComponent<BuildingID>().BuildingListID = openlist;
        }
        else
        {
            List<Structure> list = new List<Structure>();
            Structure struc = new Structure();
            struc.onGround = true; struc.Me = go;
            list.Add(struc);
            buildings.Add(list);
            go.GetComponent<BuildingID>().BuildingListID = buildings.Count - 1;
        }
    }

    public void Colapse(int listnum, GameObject go)
    {
        bool colapse = true;
        for (int i = 0; i < buildings[listnum].Count; i++)
        {
            if (buildings[listnum][i].Me == go)
            {
                buildings[listnum].RemoveAt(i);
            }
        }
        foreach (Structure struc in buildings[listnum]) 
        {
            if (struc.Me.GetComponent<BuildingID>().IsOnGround)
            {
                colapse = false;
            }
        }
        if (colapse)
        {
            foreach (Structure struc in buildings[listnum])
            {
                if (struc.Me.GetComponent<IDamageable>().bloodEffect != null)
                {
                    Instantiate(struc.Me.GetComponent<IDamageable>().bloodEffect, struc.Me.transform.position, struc.Me.transform.rotation);
                }
                struc.Me.GetComponent<BuildingID>().enabled = false;
                Destroy(struc.Me);
            }
            buildings[listnum].Clear();
        }
    }
}
