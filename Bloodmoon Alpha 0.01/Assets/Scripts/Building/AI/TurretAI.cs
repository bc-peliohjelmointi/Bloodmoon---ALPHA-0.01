using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TurretAI : IDamageable
{
    [Header("Targeting Settings")]
    public float range = 1;
    public LayerMask Enemys;
    public float ShotDamage = 1;
    public float ShotPerSec = 1;
    public float RotationSpeed = 1;

    float LastShotTime = 0;

    Collider[] EnemyList;
    GameObject target;
    [Header("Turret Parts")]
    public Transform Head;
    public Transform Neck;

    Wolf Animal = null;

    // Update is called once per frame
    void Update()
    {
        if (LookForEnemys())
        {
            if (PointAtTarget())
            {
                if (CanC())
                {
                    Shoot();
                }
            }
        }
    }

    bool LookForEnemys()
    {
        if (Physics.CheckSphere(transform.position, range, Enemys))
        {
            return true;
        }
        return false;
    }

    bool PointAtTarget()
    {
        if (Animal == null)
        {
            if (target == null)
            {
                EnemyList = Physics.OverlapSphere(transform.position, range, Enemys);
                target = EnemyList[0].gameObject;
            }
            Animal =target.GetComponent<Wolf>();
        }
        if (target == null || !Animal.Alive())
        {
            EnemyList = Physics.OverlapSphere(transform.position, range, Enemys);
            for (int i = 0; i < EnemyList.Length; i++)
            {
                target = EnemyList[i].gameObject;
                if (target.GetComponent<Wolf>().Alive())
                {
                    break;
                }
            }
            Animal = target.GetComponent<Wolf>();
        }
        int xyHit = 0;
        if (Head != null)
        {
            Vector3 CurentRotation = Head.rotation.eulerAngles;
            Head.LookAt(target.transform.position);
            Vector3 EnemyPos = Head.rotation.eulerAngles;
            Head.rotation = Quaternion.Euler(CurentRotation);
            if ((EnemyPos.y - CurentRotation.y) >= RotationSpeed * Time.deltaTime)
            {
                Head.Rotate(0, RotationSpeed * Time.deltaTime, 0);
            }
            else if ((EnemyPos.y - CurentRotation.y) <= -RotationSpeed * Time.deltaTime)
            {
                Head.Rotate(0, -RotationSpeed * Time.deltaTime, 0);
            }
            else
            {
                Head.Rotate(0, EnemyPos.y - CurentRotation.y, 0);
                xyHit += 1;
            }
            if (Neck != null)
            {
                Neck.Rotate(0, Head.rotation.eulerAngles.y - Neck.rotation.eulerAngles.y, 0);
            }
            if ((EnemyPos.x - CurentRotation.x) % 360 >= RotationSpeed * Time.deltaTime)
            {
                Head.Rotate(RotationSpeed * Time.deltaTime, 0, 0);
            }
            else if ((EnemyPos.x - CurentRotation.x) % 360 <= -RotationSpeed * Time.deltaTime)
            {
                Head.Rotate(-RotationSpeed * Time.deltaTime, 0, 0);
            }
            else
            {
                Head.Rotate(EnemyPos.x - CurentRotation.x, 0, 0);
                xyHit += 1;
            }
            Vector3 now = Head.rotation.eulerAngles;
            now.z = 0;
            Head.rotation = Quaternion.Euler(now);
        }
        else
        {
            Vector3 CurentRotation = transform.rotation.eulerAngles;
            transform.LookAt(target.transform.position);
            Vector3 EnemyPos = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(CurentRotation);
            if ((EnemyPos.y - CurentRotation.y) >= RotationSpeed * Time.deltaTime)
            {
                transform.Rotate(0,RotationSpeed * Time.deltaTime, 0);
            }
            else if ((EnemyPos.y - CurentRotation.y) <= -RotationSpeed * Time.deltaTime)
            {
                transform.Rotate(0, -RotationSpeed * Time.deltaTime, 0);
            }
            else 
            {
                transform.Rotate(0, EnemyPos.y - CurentRotation.y, 0);
                xyHit += 1;
            }

            if ((EnemyPos.x - CurentRotation.x) % 360 >= RotationSpeed * Time.deltaTime)
            {
                transform.Rotate(RotationSpeed * Time.deltaTime, 0, 0);
            }
            else if ((EnemyPos.x - CurentRotation.x) % 360 <= -RotationSpeed * Time.deltaTime)
            {
                transform.Rotate(-RotationSpeed * Time.deltaTime, 0, 0);
            }
            else
            {
                transform.Rotate(EnemyPos.x - CurentRotation.x, 0, 0);
                xyHit += 1;
            }
            Vector3 now = Head.rotation.eulerAngles;
            now.z = 0;
            Head.rotation = Quaternion.Euler(now);
        }
        if (xyHit == 2)
        {
            return true;
        }
        return false;
    }

    private bool CanC()
    {
        if (Head != null)
        {
            if (!Physics.Linecast(Head.position, target.transform.position))
            {
                return true;
            }
        }
        else
        {
            if (!Physics.Linecast(transform.position, target.transform.position))
            {
                return true;
            }
        }
        return false;
    }

    void Shoot()
    {
        Vector3 KnockBack = transform.forward;
        if (Head != null)
        {
            KnockBack = Head.transform.forward;
        }
        if (LastShotTime + (1 / ShotPerSec) < Time.time)
        {
            if (1 / ShotPerSec < Time.deltaTime)
            {
                for (int i = 0; (1 / ShotPerSec) / Time.deltaTime > i; i++)
                {
                    DealDamage(ShotDamage, target, KnockBack);
                }
                LastShotTime = Time.time;
            }
            else
            {
                DealDamage(ShotDamage, target, KnockBack);
                LastShotTime = Time.time;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
