using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ElectricTower : MonoBehaviour
{
    [Header("Range")]
    public float range = 8f;
    public float chainRange = 4f;
    public int maxChainTargets = 3;

    [Header("Combat")]
    public float fireRate = 1f;
    public int damage = 1;
    public float stunDuration = 0.5f;

    [Header("Extra Effects (opcional)")]
    public bool applySlow = false;
    [Range(0f, 1f)] public float slowPercent = 0.3f;
    public float slowDuration = 1.5f;

    public bool applyBurn = false;
    public float burnDuration = 2f;
    public float burnDps = 1f;

    public bool applyPoison = false;
    public float poisonDuration = 3f;
    public float poisonDps = 1f;

    [Header("Refs")]
    public Transform head;
    public Transform shootPoint; // si no tienes, usa head
    public GameObject beamPrefab; // LightningBeam prefab

    [Header("Audio")]
    public AudioClip shootAudioClip;
    private AudioSource audioSource;

    float cooldown = 0f;
    Transform currentTarget;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        if (shootPoint == null) shootPoint = head;
        cooldown = 1f / fireRate;
    }

    void Update()
    {
        cooldown -= Time.deltaTime;

        currentTarget = FindClosestTarget(range);
        if (currentTarget == null) return;

        RotateTowards(currentTarget.position);

        if (cooldown <= 0f)
        {
            FireChain(currentTarget);
            cooldown = 1f / fireRate;
        }
    }

    Transform FindClosestTarget(float r)
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float best = float.MaxValue;
        Transform bestT = null;

        foreach (var go in enemies)
        {
            float d = Vector3.Distance(transform.position, go.transform.position);
            if (d <= r && d < best)
            {
                best = d;
                bestT = go.transform;
            }
        }
        return bestT;
    }

    void RotateTowards(Vector3 worldPos)
    {
        Vector3 dir = worldPos - head.position;
        if (dir.sqrMagnitude < 0.001f) return;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        head.rotation = Quaternion.Lerp(head.rotation, lookRot, Time.deltaTime * 8f);
    }

    void FireChain(Transform first)
    {
        // Lista de objetivos ya golpeados
        List<Transform> hit = new() { first };

        // 1) Primer impacto
        ApplyEffectsTo(first);
        SpawnBeam(shootPoint.position, first.position);

        if (shootAudioClip != null) audioSource.PlayOneShot(shootAudioClip);

        // 2) Cadena: busca cerca del último golpeado
        Transform from = first;

        for (int i = 0; i < maxChainTargets; i++)
        {
            Transform next = FindClosestAround(from.position, chainRange, hit);
            if (next == null) break;

            hit.Add(next);
            ApplyEffectsTo(next);
            SpawnBeam(from.position, next.position);

            from = next;
        }
    }

    Transform FindClosestAround(Vector3 center, float r, List<Transform> exclude)
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float best = float.MaxValue;
        Transform bestT = null;

        foreach (var go in enemies)
        {
            Transform t = go.transform;
            if (exclude.Contains(t)) continue;

            float d = Vector3.Distance(center, t.position);
            if (d <= r && d < best)
            {
                best = d;
                bestT = t;
            }
        }
        return bestT;
    }

    void ApplyEffectsTo(Transform target)
    {
        Debug.Log($"[ElectricTower] ZAP {target.name} time={Time.time:F2}");

        // ✅ Soporta collider en hijo: busca componente en el padre también
        Enemy e = target.GetComponentInParent<Enemy>();
        if (e != null)
        {
            e.TakeDamage(damage);
            e.ApplyStun(stunDuration);

            if (applySlow) e.ApplySlow(slowPercent, slowDuration);
            if (applyBurn) e.ApplyBurn(burnDuration, burnDps);
            if (applyPoison) e.ApplyPoison(poisonDuration, poisonDps);
            return;
        }

        Balloon b = target.GetComponentInParent<Balloon>();
        if (b != null)
        {
            b.TakeDamage(damage);
            b.ApplyStun(stunDuration);

            if (applySlow) b.ApplySlow(slowPercent, slowDuration);
            if (applyBurn) b.ApplyBurn(burnDuration, burnDps);
            if (applyPoison) b.ApplyPoison(poisonDuration, poisonDps);
        }
    }

    void SpawnBeam(Vector3 from, Vector3 to)
    {
        if (beamPrefab == null) return;

        GameObject go = Instantiate(beamPrefab);
        var beam = go.GetComponent<LightningBeam>();
        if (beam != null) beam.Draw(from, to);

        // Debug line por si no tienes beamPrefab
        Debug.DrawLine(from, to, Color.cyan, 0.1f);
    }
}
