using UnityEngine;
using System.Linq;

public class FireTower : MonoBehaviour
{
    public float range = 8f;
    public float fireRate = 1f;

    public Transform head;
    public Transform shootPoint;
    public GameObject fireProjectilePrefab;

    public AudioClip shootAudioClip;
    private AudioSource audioSource;

    float cooldown;
    Transform currentTarget;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        cooldown = 1f / fireRate;
    }

    void Update()
    {
        cooldown -= Time.deltaTime;

        currentTarget = FindTarget();
        if (currentTarget == null) return;

        RotateTowards(currentTarget.position);

        if (cooldown <= 0f)
        {
            Shoot(currentTarget);
            cooldown = 1f / fireRate;
        }
    }

    Transform FindTarget()
    {
        return GameObject.FindGameObjectsWithTag("Enemy")
            .Where(e => Vector3.Distance(transform.position, e.transform.position) <= range)
            .OrderBy(e => Vector3.Distance(transform.position, e.transform.position))
            .Select(e => e.transform)
            .FirstOrDefault();
    }

    void RotateTowards(Vector3 pos)
    {
        Vector3 dir = pos - head.position;
        if (dir.sqrMagnitude < 0.001f) return;
        head.rotation = Quaternion.Lerp(head.rotation, Quaternion.LookRotation(dir), Time.deltaTime * 5f);
        if (shootPoint != null) shootPoint.rotation = head.rotation;
    }

    void Shoot(Transform target)
    {
        GameObject proj = Instantiate(fireProjectilePrefab, shootPoint.position, shootPoint.rotation);
        proj.GetComponent<FireProjectile>().SetTarget(target);

        if (shootAudioClip != null)
            audioSource.PlayOneShot(shootAudioClip);
    }
}
