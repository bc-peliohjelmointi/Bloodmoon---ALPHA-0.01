using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoxOpener : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject PivotPoint;
    public bool open = false;
    public float progress = 0;
    private Quaternion ClosedRotation = new Quaternion();
    private Quaternion OpendRotation = new Quaternion();

    private GameObject Storage;

    public bool test = false;

    private void Start()
    {
        ClosedRotation.eulerAngles += new Vector3(0,180,90);
        OpendRotation.eulerAngles += new Vector3(0,180,0); 
    }

    private Coroutine loopCoroutine;

    private void Update()
    {
        if (test)
        {
            test = false;
            StartLoop();
        }
    }

    public void StartLoop()
    {
        if (Storage == null)
        {
            Storage = GameObject.Find("Storage");
        }
        open = !open;
        if (loopCoroutine != null)
        {
            StopLoop();
        }
        loopCoroutine = StartCoroutine(Loop());
    }

    public void StopLoop()
    {
        if (loopCoroutine != null)
            StopCoroutine(loopCoroutine);
    }

    IEnumerator Loop()
    {
        while (true)
        {
            if (Storage.active == false)
            {
                open = false;
            }
            OpenUpdate();
            yield return null; // next frame
        }
    }

    private void OpenUpdate()
    {
        if (open)
        {
            progress += 3 * Time.fixedDeltaTime;
            if (progress > 1) 
            {
                progress = 1;
            }
        }
        else
        {
            progress -= 3 * Time.fixedDeltaTime;
            if (progress < 0)
            {
                progress = 0;
                StopLoop();
            }
        }
        PivotPoint.transform.rotation = Quaternion.Lerp(ClosedRotation, OpendRotation, progress);
    }
}
