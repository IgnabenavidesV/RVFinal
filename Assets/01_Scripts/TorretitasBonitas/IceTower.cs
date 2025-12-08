using UnityEngine;
using System.Collections.Generic;

public class IceTower : MonoBehaviour
{
    public float range = 8f;
    public float fireRate = 1f;
    public float rotationSpeed = 5f;

    public Transform head;
    public Transform shootPoint;
    public GameObject iceProjectilePrefab;

    private float fireTimer = 0f;
    private Transform currentTarget;

    void Update()
    {
        fireTimer -= Time.deltaTime;

        FindTarget();

        if (currentTarget != null)
        {
            RotateToTarget();

            if (fireTimer <= 0f)
            {
                Shoot();
                fireTimer = 1f / fireRate;
            }
        }
    }

    void FindTarget()
    {
        float closestDist = Mathf.Infinity;
        currentTarget = null;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject e in enemies)
        {
            float dist = Vector3.Distance(transform.position, e.transform.position);

            if (dist < closestDist && dist <= range)
            {
                closestDist = dist;
                currentTarget = e.transform;
            }
        }
    }

    void RotateToTarget()
    {
        Vector3 dir = currentTarget.position - head.position;
        Quaternion lookRot = Quaternion.LookRotation(dir);

        head.rotation = Quaternion.Lerp(
            head.rotation,
            lookRot,
            rotationSpeed * Time.deltaTime
        );
    }

    void Shoot()
    {
        GameObject go = Instantiate(
            iceProjectilePrefab,
            shootPoint.position,
            shootPoint.rotation
        );

        go.GetComponent<IceProjectile>().SetTarget(currentTarget);
    }
}
