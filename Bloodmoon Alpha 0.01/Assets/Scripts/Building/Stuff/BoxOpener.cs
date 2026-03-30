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

    private GameObject MainInventory;

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
        if (MainInventory == null)
        {
            MainInventory = GameObject.Find("MainInventory");
        }
        open = !open;
        if (loopCoroutine != null)
        {
            StopLoop();
        }
        loopCoroutine = StartCoroutine(Loop());
    }

    private void StopLoop()
    {
        if (loopCoroutine != null)
            StopCoroutine(loopCoroutine);
    }

    IEnumerator Loop()
    {
        while (true)
        {
            if (MainInventory.active == false)
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
