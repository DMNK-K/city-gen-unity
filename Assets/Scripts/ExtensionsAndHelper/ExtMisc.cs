using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtMisc
{
    //overlaps box at the position and rotation of the collider with its size
    public static List<Collider> OverlapBox(this BoxCollider col, LayerMask mask, QueryTriggerInteraction triggerInteraction = QueryTriggerInteraction.UseGlobal, ICollection<Collider> ignores = null)
    {
        Vector3 center = col.transform.position + col.center;
        Collider[] hits = Physics.OverlapBox(center, col.size * 0.5f, col.transform.rotation, mask, triggerInteraction);
        List<Collider> result = new List<Collider>();
        if (ignores != null)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (!ignores.Contains(hits[i]))
                {
                    result.Add(hits[i]);
                }
            }
        }
        else
        {
            result.AddRange(hits);
        }
        return result;
    }
}
