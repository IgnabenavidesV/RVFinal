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
    [Header("Rotation")]
    public float rotationSpeed = 5f;

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

    [Header("Hit VFX (Impacto)")]
    public GameObject hitVfxPrefab;              // tu particula de impacto rayo
    public Vector3 hitVfxOffset = Vector3.zero;
    public bool spawnHitVfxForEachChain = true;  // si quieres impacto por cada salto


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

        RotateHead();


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

 
    void RotateHead()
    {
        if (currentTarget == null || head == null)
            return;

        // Dirección hacia el enemigo (solo horizontal)
        Vector3 dir = currentTarget.position - head.position;
        dir.y = 0; // Mantener solo rotación horizontal

        if (dir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            head.rotation = Quaternion.Lerp(head.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
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
        if (beamPrefab != null)
        {
            GameObject go = Instantiate(beamPrefab);
            var beam = go.GetComponent<LightningBeam>();
            if (beam != null) beam.Draw(from, to);
        }

        // ✅ VFX de impacto en el enemigo (punto "to")
        if (hitVfxPrefab != null)
        {
            GameObject vfx = Instantiate(hitVfxPrefab, to + hitVfxOffset, Quaternion.identity);

            // fuerza one-shot y autodestruye
            var systems = vfx.GetComponentsInChildren<ParticleSystem>(true);
            float maxLife = 0f;

            foreach (var ps in systems)
            {
                var main = ps.main;
                main.loop = false;

                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                ps.Play(true);

                float life = main.duration;
                if (main.startLifetime.mode == ParticleSystemCurveMode.TwoConstants)
                    life += main.startLifetime.constantMax;
                else if (main.startLifetime.mode == ParticleSystemCurveMode.Constant)
                    life += main.startLifetime.constant;

                if (life > maxLife) maxLife = life;
            }

            Destroy(vfx, maxLife + 0.3f);
        }

        Debug.DrawLine(from, to, Color.cyan, 0.1f);
    }

}
