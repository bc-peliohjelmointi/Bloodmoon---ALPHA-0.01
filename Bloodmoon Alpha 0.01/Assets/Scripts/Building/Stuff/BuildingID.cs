using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BuildingID : IDamageable
{
    public int BuildingListID;
    public bool IsOnGround = false;
    public LayerMask Mask;

    private void Awake()
    {
        Mask = LayerMask.GetMask("Ground");
        if (transform.tag == "Wall")
        {
            if (transform.rotation.eulerAngles.y % 180 == 0)
            {
                Vector3 scale = transform.localScale;
                scale.z = 2;
                if (Physics.OverlapBox(transform.position, scale / 100 * 2, new Quaternion(), Mask).Length >= 1)
                {
                    Debug.LogWarning(Physics.OverlapBox(transform.position, scale / 100 * 2, new Quaternion(), Mask).Length);
                    IsOnGround = true;
                }
            }
            else
            {
                Vector3 scale = transform.localScale;
                scale.x = 2;
                if (Physics.OverlapBox(transform.position, scale / 100 * 2, new Quaternion(), Mask).Length >= 1)
                {
                    Debug.LogWarning(Physics.OverlapBox(transform.position, scale / 100 * 2, new Quaternion(), Mask).Length);
                    IsOnGround = true;
                }
            }
        }
        else if (transform.tag == "Floor")
        {
            Vector3 scale = transform.localScale;
            scale.y = 2;
            if (Physics.OverlapBox(transform.position, scale / 100 * 2, new Quaternion(), Mask).Length >= 1)
            {
                Debug.LogWarning(Physics.OverlapBox(transform.position, scale / 100 * 2, new Quaternion(), Mask).Length);
                IsOnGround = true;
            }
        }
        else
        {
            if (Physics.OverlapBox(transform.position, transform.localScale / 100 * 2, new Quaternion(), Mask).Length >= 1)
            {
                Debug.LogWarning(Physics.OverlapBox(transform.position, transform.localScale / 100 * 4, new Quaternion(), Mask).Length);
                IsOnGround = true;
            }
        }
    }

    private void OnDestroy()
    {
        if (gameObject.layer != LayerMask.NameToLayer("Ghoust"))
        {
            transform.GetComponentInParent<BuildingColapse>().Colapse(BuildingListID, gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        if (transform.tag == "Wall")
        {
            if (transform.rotation.eulerAngles.y % 180 == 0)
            {
                Vector3 scale = transform.localScale;
                scale.z = 2;
                Gizmos.DrawWireCube(transform.position, scale / 100 * 4);
            }
            else
            {
                Vector3 scale = transform.localScale;
                scale.x = 2;
                Gizmos.DrawWireCube(transform.position, scale / 100 * 4);
            }
        }
        else if (transform.tag == "Floor")
        {
            Vector3 scale = transform.localScale;
            scale.y = 2;
            Gizmos.DrawWireCube(transform.position, scale / 100 * 4);
        }
        else
        {
            Gizmos.DrawWireCube(transform.position, transform.localScale / 100 * 4);
        }
    }
}

