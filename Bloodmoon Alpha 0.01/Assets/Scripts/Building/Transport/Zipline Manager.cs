using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.UI;

public class ZiplineManager : MonoBehaviour
{
    private GameObject startpole;
    enum mode { normal, BuildLine, OnLine}
    private mode playermode = mode.normal;
    private GameObject player;

    private List<Transform> ZiplinePoles = new List<Transform>();
    private int pointPole;

    private float velosity;
    private GameObject playerLine;

    public LayerMask linemask;
    public LayerMask zipmask;
    public Material ZiplineMat;

    private Vector3 uplift = new Vector3(0,1.25f,0);

    public List<zipline> ziplines = new List<zipline>();
    public class zipline
    {
        public GameObject line;
        public LineRenderer lineRenderer;
        public GameObject post1;
        public GameObject post2;
    }

    private void Start()
    {
        player = GameObject.Find("Character");
    }

    private void Update()
    {
        switch (playermode)
        {
            case mode.OnLine:
                int dirmult = 1;
                velosity += 0.5f * Time.deltaTime;
                if (playerLine.transform.forward.y > 0) { dirmult = -1; }
                player.transform.position += playerLine.transform.forward * velosity * dirmult * Time.deltaTime;
                if (Input.GetKeyDown(KeyCode.Space) || Vector3.Distance(player.transform.position, playerLine.transform.position) >= playerLine.GetComponent<CapsuleCollider>().height / 2)
                {
                    playermode = mode.normal;
                    player.GetComponent<PlayerController>().enabled = true;
                    player.GetComponent<Rigidbody>().useGravity = true;
                    player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                    playerLine = null;
                    velosity = 0;
                }
                break;
            case mode.BuildLine:
                bool build = false;
                if (Input.GetKeyDown(KeyCode.Mouse0)) 
                {
                    if (TryToCreateZipline(startpole, ZiplinePoles[pointPole].gameObject))
                    {
                        build = true;
                    }
                }
                if (Input.GetAxis("Mouse ScrollWheel") != 0)
                {
                    pointPole += 1;
                    if (ZiplinePoles.Count <= pointPole)
                    {
                        pointPole = 0;
                    }
                }
                Camera.main.transform.LookAt(ZiplinePoles[pointPole]);
                if (Input.GetKeyDown(KeyCode.Escape) || build)
                {
                    playermode = mode.normal;
                    player.GetComponent<PlayerController>().enabled = true;
                    player.GetComponent<Rigidbody>().useGravity = true;
                    player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
                    ZiplinePoles.Clear();
                    player.transform.GetComponentInChildren<CameraFollow>().enabled = true;
                }
                break;
            case mode.normal:
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 2f, linemask, QueryTriggerInteraction.Collide))
                {
                    if (Input.GetKeyDown(KeyCode.F))
                    {
                        playermode = mode.OnLine;
                        player.GetComponent<PlayerController>().enabled = false;
                        player.GetComponent<Rigidbody>().useGravity = false;
                        player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                        player.transform.position = hit.point - Camera.main.transform.position + player.transform.position - new Vector3(0, 0.3f, 0);
                        playerLine = hit.transform.gameObject;
                        velosity = Mathf.Abs(player.transform.forward.y) / Time.deltaTime * 10;
                    }
                }
                if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hitzip, 2f, zipmask, QueryTriggerInteraction.Collide))
                {
                    if (hitzip.transform.name == "ZipLine(Clone)" && Input.GetKeyDown(KeyCode.F))
                    {
                        startpole = hitzip.transform.gameObject;
                        playermode = mode.BuildLine;
                        player.GetComponent<PlayerController>().enabled = false;
                        player.GetComponent<Rigidbody>().useGravity = false;
                        player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                        BuildingID[] x = GetComponentsInChildren<BuildingID>();
                        pointPole = 0;
                        player.transform.GetComponentInChildren<CameraFollow>().enabled = false;
                        foreach (BuildingID building in x)
                        {
                            if (building.transform.name == "ZipLine(Clone)" && building.gameObject != hitzip.transform.gameObject)
                            {
                                ZiplinePoles.Add(building.transform);
                            }
                        }
                    }
                }
                break;
        }
    }

    private bool TryToCreateZipline(GameObject post1, GameObject post2)
    {
        bool work = true;
        foreach (zipline line in ziplines)
        {
            if ((line.post1 == post1 || line.post2 == post1) && (line.post1 == post2 || line.post2 == post2))
            {
                work = false;
            }
        }
        if (work)
        {
            CreateZipLine(post1, post2);
        }
        return work;
    }

    public void CreateZipLine(GameObject Post1, GameObject Post2)
    {
        var Line = new GameObject();

        Line.name = "(Zip)line";

        Line.layer = LayerMask.NameToLayer("Zipline");

        var Ren = Line.AddComponent<LineRenderer>();

        Ren.SetPosition(0, Post1.transform.position+uplift);
        Ren.SetPosition(1, Post2.transform.position+uplift);

        Ren.SetWidth(0.2f,0.2f);

        if (ZiplineMat != null)
        {
            Ren.material = ZiplineMat;
        }

        CapsuleCollider Box = Line.AddComponent<CapsuleCollider>();
        Line.transform.position = Box.center;
        Box.center = new Vector3();
        Box.radius = 0.1f;
        Box.isTrigger = true;

        float distance = Vector3.Distance(Post1.transform.position, Post2.transform.position);
        Box.height = distance;
        Box.direction = 2;

        Line.transform.LookAt(Post1.transform.position+uplift);

        zipline zipline = new zipline();
        zipline.line = Line;
        zipline.lineRenderer = Ren;
        zipline.post1 = Post1;
        zipline.post2 = Post2;
        ziplines.Add(zipline);
    }

    public void ZiplineDown()
    {
        foreach (zipline zipline in ziplines)
        {
            if(zipline.post1 == null || zipline.post2 == null)
            {
                Destroy(zipline.line);
                ziplines.Remove(zipline);
            }
        }
    }
}
