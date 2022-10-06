using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public static class IntruderHelper
{
    /// <summary>
    /// Returns the closest lag compensated hit from the list and chosen count.
    /// </summary>
    public static LagCompensatedHit GetClosestLagCompensatedHit(Vector3 position, List<LagCompensatedHit> hits, int count)
    {
        float closestDistance = float.MaxValue;
        int index = 0;
        for (int i = 0; i < count; i++)
        {
            float distance = Vector3.Distance(hits[i].Hitbox.Root.gameObject.transform.position, position);
            if (closestDistance > distance)
            {
                closestDistance = distance;
                index = i;
            }
        }
        return hits[index];
    }
    
    /// <summary>
    /// Returns the result count of closest lag compensated hits from the list and chosen count.
    /// </summary>
    public static List<LagCompensatedHit> GetClosestLagCompensatedHits(Vector3 position, List<LagCompensatedHit> hits, int resultCount)
    {
        if (resultCount >= hits.Count)
            return hits;
        
        hits.Sort((p1,p2)=>
        {
            float distance = Vector3.Distance(p1.Hitbox.Root.gameObject.transform.position, position);
            float distance2 = Vector3.Distance(p2.Hitbox.Root.gameObject.transform.position, position);
            return distance.CompareTo(distance2);
        });

        return hits.GetRange(0, resultCount);
    }
    
    /// <summary>
    /// Returns sorted result count of closest lag compensated hits from the list and chosen count.
    /// </summary>
    public static List<LagCompensatedHit> GetSortedClosestLagCompensatedHits(Vector3 position, List<LagCompensatedHit> hits)
    {
        hits.Sort((p1,p2)=>
        {
            float distance = Vector3.Distance(p1.Hitbox.Root.gameObject.transform.position, position);
            float distance2 = Vector3.Distance(p2.Hitbox.Root.gameObject.transform.position, position);
            return distance.CompareTo(distance2);
        });

        return hits;
    }

    public static bool CanAimAtCharacter(IntruderCharacterController character1,
        IntruderCharacterController character2, LayerMask _detectionLayerMask, float visionHeight = 1)
    {
        if (Physics.Raycast(new Vector3(character1.GetNetworkTransform().ReadPosition().x,visionHeight, character1.GetNetworkTransform().ReadPosition().z),
            (character2.GetNetworkTransform().ReadPosition() - character1.GetNetworkTransform().ReadPosition()).normalized,
            out RaycastHit hit, Vector3.Distance(character1.GetNetworkTransform().ReadPosition(), character2.GetNetworkTransform().ReadPosition()), _detectionLayerMask, QueryTriggerInteraction.Collide))
        {
            if (hit.collider.gameObject.layer != character2.gameObject.layer)
                return false;
        }
        return true;
    }
    
    public static bool IsInLayerMask(int layer, LayerMask layerMask)
    {
        return layerMask == (layerMask | (1 << layer));
    }

    public static string FormatTime(float time)
    {
        int minutes = (int) time / 60 ;
        int seconds = (int) time - 60 * minutes;
        int decaseconds = (int) ((time - minutes * 60 - seconds) * 10f);
        return $"{minutes:00}:{seconds:00}.{decaseconds:0}";
    }
    
    public static string FormatNumber(int number)
    {
        if(number <= 1000) 
            return number.ToString();
        string underThousand = "" + number % 1000;
        while (underThousand.Length < 3)
        {
            underThousand = "0" + underThousand;
        }
        return number / 1000 + "," + underThousand;
    }

    public static void ShuffleList<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++) {
            T temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
