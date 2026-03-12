using Mono.Cecil.Cil;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Builder : MonoBehaviour
{
    /// <summary>
    /// Onko pelaaja rakentamassa?
    /// </summary>
    public bool building = false;
    /// <summary>
    /// Rakennettavan asian esikatselu objecti
    /// </summary>
    public GameObject Ghoust;

    public Material valid;
    public Material invalid;
    /// <summary>
    /// Lista rakennettavia muotoja
    /// </summary>
    public List<GameObject> buildings;
    /// <summary>
    /// Mihin kerroksiin kelaaja voi rakentaa
    /// </summary>
    public LayerMask mask;
    /// <summary>
    /// Mit‰ pelaaja rakentaa numerona. Referoi buildings listaan
    /// </summary>
    public int build = 0;

    public int rotat = 0;

    public LocalNavUpdate update;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) // Togle building mode
        {
            building = !building;
        }
        if (building)//Jos rakentamassa
        {
            if (Input.GetMouseButtonDown(1)) // jos painat oikeaa hiiren nappia vaihda rakennettavaa esinett‰ listan sis‰ll‰
            {
                build = (build + 1) % buildings.Count;
                Destroy(Ghoust);
            }
            if (Ghoust == null) // Jos haamu puuttuu, luo uusi haamu ja poista haamut colliderit
            {
                Ghoust = Instantiate(buildings[build]);
                Ghoust.GetComponentInChildren<Renderer>().material = valid;
                Ghoust.layer = LayerMask.NameToLayer("Ghoust");
                if (Ghoust.GetComponent<MeshCollider>() != null) {
                    Ghoust.GetComponent<MeshCollider>().enabled = false;
                }
                if (Ghoust.GetComponent<BoxCollider>() != null) {
                    Ghoust.AddComponent<BoxCollider>().size /= 1.1f;
                }
                Ghoust.AddComponent<Validation>();
                Ghoust.AddComponent<Rigidbody>();
                Destroy(Ghoust.GetComponent<BuildingID>());
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                rotat += 90;
                if (rotat >= 360)
                {
                    rotat = 0;
                }
            }
            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * 50);
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 50f, mask, QueryTriggerInteraction.Ignore)) // Katso minne pelaaja katsoo ja tallenna raycast hitinfo
            {
                Snap(hit);
                SpinMeRightRound(hit);
                Ghoust.SetActive(true);
            }
            else { Ghoust.SetActive(false); }
            bool can = Valid();
            if (Ghoust.active && Input.GetMouseButtonDown(0) && can) // Mik‰li haamun pystyy laittaa nykyiseen siaintiinsa ja pelaaja painaa vasenta hiiren nappia, luo uusi rakennelma valittua tyyppi‰ haamun kohdalle, "Builder"in lapsi objectina
            {
                GameObject go = Instantiate(buildings[build], Ghoust.transform.position, Ghoust.transform.rotation, transform);
                update.NavUpdate();
                rotat = 0;
                if (hit.transform.tag != "Ground")
                {
                    GetComponent<BuildingColapse>().newObject(go, hit.transform.GetComponent<BuildingID>().BuildingListID);
                }
                else
                {
                    GetComponent<BuildingColapse>().newObject(go, -1);
                }
            }
        }
        else if (Ghoust != null) // Jos ei rakentamassa ja haamu on olemassa, tuhoa haamu
        {
            Destroy(Ghoust);
            Ghoust = null;
        }
    }
    /// <summary>
    /// Jos Pystyy yhdist‰m‰‰n haamun katsottuun rakennelmaan, yhdist‰ se siihen. Jos ei ole rakennelma, laita mihin katsotaan 
    /// </summary>
    /// <param name="hit"></param>
    public void Snap(RaycastHit hit) // Ota ray niin tiet‰‰ mit‰ katsoo
    {
        if (hit.transform.tag == "Floor") // Jos katsot lattiaa, yhdisty siihen.
        {
            Vector3 dir = hit.transform.position - hit.point;
            dir.y = 0;
            Vector3 uplift = new Vector3();
            if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z)) // Katso mihin suuntaan yhdistyt, x vai z
            {
                dir.z = 0;
                if (dir.x < 0) // Katso kumpi reuna, positiivinen vai negatiivinen
                {
                    dir.x = 4 * Ghoust.transform.localScale.x / 100;
                }
                else
                {
                    dir.x = -4 * Ghoust.transform.localScale.x / 100;
                }
            }
            else
            {
                dir.x = 0;
                if (dir.z < 0) // Katso kumpi reuna, positiivinen vai negatiivinen
                {
                    dir.z = 4 * Ghoust.transform.localScale.x / 100;
                }
                else
                {
                    dir.z = -4 * Ghoust.transform.localScale.x / 100;
                }
            }
            if (Ghoust.tag == "Wall") // mik‰li yrit‰t laittaa sein‰‰, s‰‰d‰ siainti t‰ydellist‰ sn‰ppi‰ varten ja k‰‰nn‰ oikeaan suuntaan
            { 
                dir.z /= 2;
                dir.x /= 2;
                uplift.y = 2 * Ghoust.transform.localScale.x / 100;
                Ghoust.transform.position = hit.transform.position + dir + uplift;
                Vector3 target = hit.transform.position;
                target.y += 2 * Ghoust.transform.localScale.x / 100;
                Ghoust.transform.LookAt(target);
            }
            else if (Ghoust.tag == "Stairs" || Ghoust.tag == "StairsRight" || Ghoust.tag == "StairsLeft" || Ghoust.tag == "StairsLoop") 
            {
                Ghoust.transform.rotation = new Quaternion();
                uplift.y = 2 * Ghoust.transform.localScale.x / 100;
                Ghoust.transform.position = hit.transform.position + uplift;
            }
            else // Jos et ole luomassa mit‰‰n edellisist‰, k‰yt‰ default
            {
                Ghoust.transform.rotation = hit.transform.rotation;
                Ghoust.transform.position = hit.transform.position + dir;
            }
        }
        else if (hit.transform.tag == "Wall") // jos katsoo sein‰‰ snap sein‰‰n
        {
            Vector3 dir = hit.transform.position - hit.point;
            Ghoust.transform.position = hit.transform.position + dir;
            if (Ghoust.tag == "Floor") // jos olet valinnut lattian, rajaa snap suunnat
            {
                if (dir.y > 0)
                {
                    dir = new Vector3();
                    dir.y = -2 * Ghoust.transform.localScale.x / 100;
                    dir += hit.transform.forward * 2 * Ghoust.transform.localScale.x / 100;
                }
                else
                {
                    dir = new Vector3();
                    dir.y = 2 * Ghoust.transform.localScale.x / 100;
                    dir += hit.transform.forward * 2 * Ghoust.transform.localScale.x / 100;
                }
                Ghoust.transform.position = hit.transform.position + dir;
            }
            else if(Ghoust.tag == "Stairs" || Ghoust.tag == "StairsRight" || Ghoust.tag == "StairsLeft" || Ghoust.tag == "StairsLoop")
            {
                dir = hit.transform.forward * 2 * Ghoust.transform.localScale.x / 100;
            }
            else
            {
                if (Mathf.Abs(dir.x) > Mathf.Abs(dir.z)) // valitse suunta mihin snap tapahtuu
                {
                    if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
                    {
                        if (dir.x > 0)
                        {
                            dir = new Vector3();
                            dir.x = -4 * Ghoust.transform.localScale.x / 100;
                        }
                        else
                        {
                            dir = new Vector3();
                            dir.x = 4 * Ghoust.transform.localScale.x / 100;
                        }
                    }
                    else
                    {
                        if (dir.y > 0)
                        {
                            dir = new Vector3();
                            dir.y = -4 * Ghoust.transform.localScale.x / 100;
                        }
                        else
                        {
                            dir = new Vector3();
                            dir.y = 4 * Ghoust.transform.localScale.x / 100;
                        }
                    }
                }
                else
                {
                    if (Mathf.Abs(dir.y) < Mathf.Abs(dir.z))
                    {
                        if (dir.z > 0)
                        {
                            dir = new Vector3();
                            dir.z = -4 * Ghoust.transform.localScale.x / 100;
                        }
                        else
                        {
                            dir = new Vector3();
                            dir.z = 4 * Ghoust.transform.localScale.x / 100;
                        }
                    }
                    else
                    {
                        if (dir.y > 0)
                        {
                            dir = new Vector3();
                            dir.y = -4 * Ghoust.transform.localScale.x / 100;
                        }
                        else
                        {
                            dir = new Vector3();
                            dir.y = 4 * Ghoust.transform.localScale.x / 100;
                        }
                    }
                }
            }
            Ghoust.transform.rotation = hit.transform.rotation;
            Ghoust.transform.position = hit.transform.position + dir;
        }
        else if (hit.transform.tag == "Stairs" || hit.transform.tag == "StairsRight" || hit.transform.tag == "StairsLeft" || hit.transform.tag == "StairsLoop")
        {
            Vector3 dir = hit.transform.position - hit.point;
            if (Ghoust.tag == "Floor")
            {
                if (dir.y < 0)
                {
                    switch (hit.transform.tag)
                    {
                        case "StairsLeft":
                            dir = -hit.transform.right * 4 * Ghoust.transform.localScale.x / 100;
                            Ghoust.transform.rotation = hit.transform.rotation;
                            Ghoust.transform.Rotate(0, 90, 0);
                            break;
                        case "StairsLoop":
                            dir = -hit.transform.forward * 4 * Ghoust.transform.localScale.x / 100;
                            Ghoust.transform.rotation = hit.transform.rotation;
                            break;
                        case "StairsRight":
                            dir = hit.transform.right * 4 * Ghoust.transform.localScale.x / 100;
                            Ghoust.transform.rotation = hit.transform.rotation;
                            Ghoust.transform.Rotate(0, 90, 0);
                            break;
                        default:
                            dir = hit.transform.forward * 4 * Ghoust.transform.localScale.x / 100;
                            Ghoust.transform.rotation = hit.transform.rotation;
                            break;
                    }
                    dir.y = 2 * Ghoust.transform.localScale.x / 100;
                    Ghoust.transform.position = hit.transform.position + dir;
                }
                else
                {
                    dir = hit.transform.forward * 4 * Ghoust.transform.localScale.x / 100;
                    dir.y = 2 * Ghoust.transform.localScale.x / 100;
                    Ghoust.transform.position = hit.transform.position - dir;
                    Ghoust.transform.rotation = hit.transform.rotation;
                }
            }
            else if (Ghoust.tag == "Wall")
            {
                if (dir.y < 0)
                {
                    switch (hit.transform.tag)
                    {
                        case "StairsLeft":
                            dir = -hit.transform.right * 2 * Ghoust.transform.localScale.x / 100;
                            Ghoust.transform.rotation = hit.transform.rotation;
                            Ghoust.transform.Rotate(0, 90, 0);
                            break;
                        case "StairsLoop":
                            dir = -hit.transform.forward * 2 * Ghoust.transform.localScale.x / 100;
                            Ghoust.transform.rotation = hit.transform.rotation;
                            break;
                        case "StairsRight":
                            dir = hit.transform.right * 2 * Ghoust.transform.localScale.x / 100;
                            Ghoust.transform.rotation = hit.transform.rotation;
                            Ghoust.transform.Rotate(0, 90, 0);
                            break;
                        default:
                            dir = hit.transform.forward * 2 * Ghoust.transform.localScale.x / 100;
                            Ghoust.transform.rotation = hit.transform.rotation;
                            break;
                    }
                    dir.y = 4 * Ghoust.transform.localScale.x / 100;
                    Ghoust.transform.position = hit.transform.position + dir;
                }
                else
                {
                    dir = hit.transform.forward * 2 * Ghoust.transform.localScale.x / 100;
                    dir.y = 4 * Ghoust.transform.localScale.x / 100;
                    Ghoust.transform.position = hit.transform.position - dir;
                    Ghoust.transform.rotation = hit.transform.rotation;
                }
            }
            else
            {
                if (Mathf.Abs(dir.y) > 0.5f || Ghoust.tag != "Stairs")
                {
                    if (dir.y < 0)
                    {
                        switch (hit.transform.tag)
                        {
                            case "StairsLeft":
                                dir = -hit.transform.right * 4 * Ghoust.transform.localScale.x / 100;
                                Ghoust.transform.rotation = hit.transform.rotation;
                                Ghoust.transform.Rotate(0, -90, 0);
                                break;
                            case "StairsLoop":
                                dir = -hit.transform.forward * 4 * Ghoust.transform.localScale.x / 100;
                                Ghoust.transform.rotation = hit.transform.rotation;
                                Ghoust.transform.Rotate(0, 180, 0);
                                break;
                            case "StairsRight":
                                dir = hit.transform.right * 4 * Ghoust.transform.localScale.x / 100;
                                Ghoust.transform.rotation = hit.transform.rotation;
                                Ghoust.transform.Rotate(0, 90, 0);
                                break;
                            default:
                                dir = hit.transform.forward * 4 * Ghoust.transform.localScale.x / 100;
                                Ghoust.transform.rotation = hit.transform.rotation;
                                break;
                        }
                        dir.y = 4 * Ghoust.transform.localScale.x / 100;
                        Ghoust.transform.position = hit.transform.position + dir;
                    }
                    else
                    {
                        dir = hit.transform.forward * -4 * Ghoust.transform.localScale.x / 100;
                        dir.y = -4 * Ghoust.transform.localScale.x / 100;
                        Ghoust.transform.rotation = hit.transform.rotation;
                        Ghoust.transform.position = hit.transform.position + dir;
                    }
                }
                else
                {
                    if (Mathf.Abs(hit.transform.right.x) > Mathf.Abs(hit.transform.right.z)) // valitse suunta mihin snap tapahtuu
                    {
                        if (dir.x > 0)
                        {
                            dir = new Vector3(4 * Ghoust.transform.localScale.x / 100, 0, 0);
                        }
                        else
                        {
                            dir = new Vector3(-4 * Ghoust.transform.localScale.x / 100, 0, 0);
                        }
                    }
                    else
                    {
                        if (dir.z > 0)
                        {
                            dir = new Vector3(0, 0, 4 * Ghoust.transform.localScale.x / 100);
                        }
                        else
                        {
                            dir = new Vector3(0, 0, -4 * Ghoust.transform.localScale.x / 100);
                        }
                    }
                    Ghoust.transform.rotation = hit.transform.rotation;
                    Ghoust.transform.position = hit.transform.position - dir;
                }
            }
        }
        else // mik‰li et osu mihink‰‰n mihin snap toimii, laita mihin ray osoittaa
        {
            Ghoust.transform.rotation = new Quaternion();
            Ghoust.transform.position = hit.point;
        }
    }

    private bool Valid()
    {
        Validation val = Ghoust.GetComponentInChildren<Validation>();
        bool valid_bool = val.valid;
        if (valid_bool)
        {
            val.transform.GetComponentsInChildren<Renderer>()[0].material = valid;
        }
        else
        {
            val.transform.GetComponentsInChildren<Renderer>()[0].material = invalid;
        }
        return valid_bool;
    }

    private void SpinMeRightRound(RaycastHit hit)
    {
        if (Ghoust.tag != "Wall")
        {
            if (Ghoust.tag == "Stairs" && hit.transform.tag == "Stairs")
            {
                if ((rotat / 90) % 2 == 1 && hit.transform.position.y != Ghoust.transform.position.y)
                {
                    Ghoust.transform.position -= new Vector3(0, 4 * Ghoust.transform.localScale.x / 100, 0);
                    Ghoust.transform.Rotate(0,180,0);
                }
            }
            else
            {
                Ghoust.transform.Rotate(0, rotat, 0);
            }
        }
        else if (rotat != 0 && hit.transform.tag == "Wall")
        {
            if (rotat == 180)
            {
                rotat += 90;
            }
            float x = (hit.transform.position.x - Ghoust.transform.position.x) / 2;
            float z = (hit.transform.position.z - Ghoust.transform.position.z) / 2;
            if (hit.transform.position.y - Ghoust.transform.position.y == 0)
                {
                if (x == 0)
                {
                    if (rotat == 90)
                    {
                        x += 2 * Ghoust.transform.localScale.x / 100;
                    }
                    else
                    {
                        x -= 2 * Ghoust.transform.localScale.x / 100;
                    }
                }
                else
                {
                    if (rotat == 90)
                    {
                        z += 2 * Ghoust.transform.localScale.z / 100;
                    }
                    else
                    {
                        z -= 2 * Ghoust.transform.localScale.z / 100;
                    }
                }
                Ghoust.transform.position += new Vector3(x, 0, z);
                Ghoust.transform.Rotate(0, 90, 0);
            }
        }
        else if (rotat != 0 && hit.transform.tag == "Floor")
        {
            Ghoust.transform.Rotate(0, 0, rotat);
        }
    }
}
