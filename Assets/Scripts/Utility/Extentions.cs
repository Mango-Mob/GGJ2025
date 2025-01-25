using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class Extentions
{ 
    public static Collider GetClosestCollider(GameObject userObj, RaycastHit[] hits)
    {
        float dist = float.MaxValue;
        int closest = -1;
        for (int i = 0; i < hits.Length; i++)
        {
            float currDist = Vector3.Distance(userObj.transform.position, hits[i].point);
            if(dist > currDist && hits[i].collider.gameObject != userObj)
            {
                dist = currDist;
                closest = i;
            }
        }

        if (closest < 0)
            return null;

        return hits[closest].collider;
    }

    public static Vector3 DirectionTo(this Vector3 from, Vector3 to)
    {
        return (to - from).normalized;
    }
     
    public static void SetEnabled(this MonoBehaviour comp, bool status)
    {
        comp.enabled = status;
    }

    //This is terrible for performance, but who cares
    public static float EvaluateInverse(this AnimationCurve curve, float value)
    {
        var inverseCurve = new AnimationCurve();
        for (int i = 0; i < curve.length; i++)
        {
            Keyframe inverseKey = new Keyframe(curve.keys[i].value, curve.keys[i].time);
            inverseCurve.AddKey(inverseKey);
        }
        
        return inverseCurve.Evaluate(value);
    }


    public static Vector2 OnUnitSquare(Vector2 sphere)
    {
       if(sphere == Vector2.zero)
           sphere = Random.insideUnitCircle;

       sphere = sphere.normalized;

       if (1.0f - Mathf.Abs(sphere.x) < 1.0f - Mathf.Abs(sphere.y))
       {
           //x side
           sphere.x = (sphere.x < 0) ? -1f : 1f;
       }
       else
       {
           //y side
           sphere.y = (sphere.y < 0) ? -1f : 1f;
       }

       return sphere;
    }
    public static Vector3 MidPoint(Vector3 A, Vector3 B)
    {
        return new Vector3( (A.x + B.x) / 2f, (A.y + B.y) / 2f, (A.z + B.z) / 2f);
    }

    public static Vector2 FromAngle( float radians, float distance)
    {
        return new Vector2(Mathf.Cos(radians), Mathf.Sin(radians)) * distance;
    }

    public static void GizmosDrawCircle(Vector3 point, float radius)
    {
        Gizmos.matrix = Matrix4x4.TRS(point, Quaternion.identity, new Vector3(1f, 0f, 1f));

        Gizmos.DrawWireSphere(Vector3.zero, radius);

        Gizmos.matrix = Matrix4x4.identity;
    }

    public static void GizmosDrawSquare(Vector3 point, Quaternion rotation, Vector2 size)
    {
        Gizmos.matrix = Matrix4x4.TRS(point, rotation, new Vector3(1f, 0f, 1f));

        Gizmos.DrawWireCube(Vector3.zero, new Vector3(size.x, 0, size.y));

        Gizmos.matrix = Matrix4x4.identity;
    }
    public static bool LineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, ref Vector2 intersection)
    {
        float Ax, Bx, Cx, Ay, By, Cy, d, e, f, num;
        float x1lo, x1hi, y1lo, y1hi;

        Ax = p2.x - p1.x;
        Bx = p3.x - p4.x;

        // X bound box test/
        if (Ax < 0)
        {
            x1lo = p2.x; x1hi = p1.x;
        }
        else
        {
            x1hi = p2.x; x1lo = p1.x;
        }

        if (Bx > 0)
        {
            if (x1hi < p4.x || p3.x < x1lo) return false;
        }
        else
        {
            if (x1hi < p3.x || p4.x < x1lo) return false;
        }

        Ay = p2.y - p1.y;
        By = p3.y - p4.y;

        // Y bound box test//
        if (Ay < 0)
        {
            y1lo = p2.y; y1hi = p1.y;
        }
        else
        {
            y1hi = p2.y; y1lo = p1.y;
        }

        if (By > 0)
        {
            if (y1hi < p4.y || p3.y < y1lo) return false;
        }
        else
        {
            if (y1hi < p3.y || p4.y < y1lo) return false;
        }

        Cx = p1.x - p3.x;
        Cy = p1.y - p3.y;
        d = By * Cx - Bx * Cy;  // alpha numerator//
        f = Ay * Bx - Ax * By;  // both denominator//

        // alpha tests//
        if (f > 0)
        {
            if (d < 0 || d > f) return false;
        }
        else
        {
            if (d > 0 || d < f) return false;
        }

        e = Ax * Cy - Ay * Cx;  // beta numerator//

        // beta tests //
        if (f > 0)
        {
            if (e < 0 || e > f) return false;
        }
        else
        {
            if (e > 0 || e < f) return false;
        }

        // check if they are parallel
        if (f == 0) return false;

        // compute intersection coordinates //
        num = d * Ax; // numerator //
        intersection.x = p1.x + num / f;

        num = d * Ay;
        intersection.y = p1.y + num / f;

        return true;
    }

    private static bool same_sign(float a, float b)
    {
        return ((a * b) >= 0f);
    }

    public static bool NavMeshOverlapSphere(Vector3 sampleLoc, float radius, LayerMask layerMask, out Vector3 navPoint)
    {
        navPoint = Vector3.zero;
        Collider[] rHit;
        NavMeshHit nHit;

        //First sample location using navmesh
        if(!NavMesh.SamplePosition(sampleLoc, out nHit, radius, -1))
            return false;

        //Secondly sample location using physics
        rHit = Physics.OverlapSphere(nHit.position, radius, layerMask);

        if (rHit.Length > 0)
            return false;

        navPoint = nHit.position;
        return true;
    }

    public static bool CircleVsCircle(Vector2 a, Vector2 b, float rA, float rB)
    {
        return rA + rB > Vector2.Distance(a, b);
    }


    [System.Serializable]
    public struct WeightedOption<T>
    {
        [SerializeField] public T data;
        [SerializeField] public uint weight;
    }

    public static T GetFromList<T>(List<WeightedOption<T>> options)
    {
        if(options == null || options.Count == 0)
            return default(T);

        uint totalWeight = 0;
        foreach (var item in options)
            totalWeight += item.weight;

        int weightSelect = Random.Range(1, (int)totalWeight + 1); //1 to t
        for (int i = 0; i < options.Count; i++)
        {
            weightSelect -= (int)options[i].weight;
            if (weightSelect <= 0)
            {
                return options[i].data;
            }
        }

        return default(T);
    }

    public static void SetWeightAt<T>(this List<WeightedOption<T>> options, int index, uint weight)
    {
        WeightedOption<T> replacer = new WeightedOption<T>();

        replacer.data = options[index].data;
        replacer.weight = weight;

        options[index] = replacer;

    }

    public static bool Roll(int chance)
    {
        return chance > 100 || Random.Range(0, 99) > chance;
    }

    public static float RandomGaussian(float minValue = 0.0f, float maxValue = 1.0f)
    {
        float u, v, S;

        do
        {
            u = 2.0f * UnityEngine.Random.value - 1.0f;
            v = 2.0f * UnityEngine.Random.value - 1.0f;
            S = u * u + v * v;
        }
        while (S >= 1.0f);

        // Standard Normal Distribution
        float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

        // Normal Distribution centered between the min and max value
        // and clamped following the "three-sigma rule"
        float mean = (minValue + maxValue) / 2.0f;
        float sigma = (maxValue - mean) / 3.0f;
        return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
    }

    public static float RandomNormalDistribution( this System.Random random, float mean = 0.0f, float std = 1.0f)
    {
        float x1, x2, w, y1;
        do
        {
            
            x1 = 2f * (float)random.NextDouble() - 1f;
            x2 = 2f * (float)random.NextDouble() - 1f;
            w = x1 * x1 + x2 * x2;
        } while (w >= 1f);

        w = Mathf.Sqrt((-2f * Mathf.Log(w)) / w);
        y1 = x1 * w;

        return (y1 * std) + mean;
    }
}