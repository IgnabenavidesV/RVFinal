using UnityEngine;

public class PoisonProjectile : MonoBehaviour
{
    public float speed = 12f;

    [Header("Damage (Inspector)")]
    public int impactDamage = 2;
    public float poisonDuration = 6f;
    public float poisonDPS = 1.5f;

    public float lifeTime = 3f;
    public float explosionRadius = 0f;

    [Header("Hit VFX")]
    [SerializeField] private GameObject hitVfxPrefab;      // ✅ GoopSprayEffect prefab
    [SerializeField] private Vector3 hitVfxOffset = Vector3.zero;
    [SerializeField] private bool spawnVfxOnAoE = true;    // ✅ si explota en área, spawnea 1 VFX en el centro

    private Transform target;

    public void SetTarget(Transform t) => target = t;

    // ✅ La torre puede llamarlo al instanciar
    public void Configure(int dmg, float duration, float dps, float aoeRadius)
    {
        impactDamage = dmg;
        poisonDuration = duration;
        poisonDPS = dps;
        explosionRadius = aoeRadius;
    }

    void Start() => Destroy(gameObject, lifeTime);

    void Update()
    {
        if (target == null) { Destroy(gameObject); return; }

        Vector3 dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;
        transform.forward = dir;
    }

    private void OnTriggerEnter(Collider col)
    {
        if (!col.CompareTag("Enemy")) return;

        // ✅ VFX solo cuando pega
        SpawnHitVFX(col.ClosestPoint(transform.position));


        if (explosionRadius > 0f) Explode();
        else ApplyToOne(col);

        Destroy(gameObject);
    }

    void ApplyToOne(Collider col)
    {
        Enemy e = col.GetComponentInParent<Enemy>();
        if (e != null) { e.TakeDamage(impactDamage); e.ApplyPoison(poisonDuration, poisonDPS); return; }

        Balloon b = col.GetComponentInParent<Balloon>();
        if (b != null) { b.TakeDamage(impactDamage); b.ApplyPoison(poisonDuration, poisonDPS); }
    }

    void Explode()
    {
        if (spawnVfxOnAoE)
            SpawnHitVFX(transform.position);

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            Enemy e = hit.GetComponentInParent<Enemy>();
            if (e != null) { e.TakeDamage(impactDamage); e.ApplyPoison(poisonDuration, poisonDPS); continue; }

            Balloon b = hit.GetComponentInParent<Balloon>();
            if (b != null) { b.TakeDamage(impactDamage); b.ApplyPoison(poisonDuration, poisonDPS); }
        }
    }

    private void SpawnHitVFX(Vector3 pos)
    {
        if (hitVfxPrefab == null) return;

        GameObject vfx = Instantiate(hitVfxPrefab, pos + hitVfxOffset, Quaternion.identity);

        // fuerza que se reproduzca y se autodestruya
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
}
