using System.Collections;
using UnityEngine;

public class LightningBeam : MonoBehaviour
{
    public LineRenderer line;
    public float life = 0.08f;

    void Awake()
    {
        if (line == null) line = GetComponent<LineRenderer>();
    }

    public void Draw(Vector3 from, Vector3 to)
    {
        line.positionCount = 2;
        line.SetPosition(0, from);
        line.SetPosition(1, to);
        StopAllCoroutines();
        StartCoroutine(Die());
    }

    IEnumerator Die()
    {
        yield return new WaitForSeconds(life);
        Destroy(gameObject);
    }
}
