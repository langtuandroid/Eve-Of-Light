using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NeoFPS;

[RequireComponent(typeof(PooledObject))]
public class FlameDmgVol : MonoBehaviour
{
    private PooledObject pooledObject;

    // pre-defined vector and distance values
    private Vector3 origin;
    private float maxDistance;
    private float startDistanceFallOff_N; // normalized


    // properties
    private float speed = 5;
    private float growthRate;
    private LayerMask collisionLayer;
    private float maxDamage;


    // updated distances values
    private float distanceFromMax = 0;

    /// <summary>
    /// 0 - 1 increment normalized
    /// </summary>
    private float distanceFromMax_N
    {
        get => Vector3.Distance(origin, transform.position) / maxDistance;
    }

    /// <summary>
    /// 1 - 0 decrement normalized
    /// </summary>
    private float distanceFromMax_N_Inverse
    {
        get => 1 - distanceFromMax_N;
    }

    /// <summary>
    /// a decrementing 1 - 0 normalized value based on the distance from the origin and where the falloff damage starts on the normalized 0-1 value
    /// </summary>
    /// <value></value>
    private float originBasedDistanceDecrementingDamageFallOffMultiplier
    {
        get => distanceFromMax_N_Inverse / (1 - startDistanceFallOff_N);
    }


    void Awake() {
        pooledObject = GetComponent<PooledObject>();
    }

    public void Init(
        Vector3 origin,
        float speed,
        float startRadius,
        float maxTravelDistance,
        float growthRate,
        float startDistanceDamageFallOff_N,
        LayerMask collisionLayer,
        float maxDamage)
    {
        this.origin = origin;
        this.speed = speed;
        transform.localScale = Vector3.one * startRadius;
        this.maxDistance = maxTravelDistance;
        this.growthRate = growthRate;
        this.startDistanceFallOff_N = startDistanceDamageFallOff_N;
        this.collisionLayer = collisionLayer;
        this.maxDamage = maxDamage;
    }

    // Update is called once per frame
    void Update()
    {
        MoveAndGrow();
        CollisionCheck();
        DistanceUpdates();

    }

    private void DistanceUpdates()
    {
        distanceFromMax = Vector3.Distance(transform.position, origin);
        if (distanceFromMax >= maxDistance) {
            pooledObject.ReturnToPool();
        }
    }

    private void MoveAndGrow()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
        transform.localScale += Vector3.one * Time.deltaTime * growthRate;
    }

    private void CollisionCheck()
    {
        Collider[] result = Physics.OverlapSphere(transform.position, transform.localScale.y / 2);

        // check for hits
        if (result.Length > 0 && result[0] != null) {
            // calculate current distance from origin in normalized form against the target distance
            // commence damage application 
            if (result[0].TryGetComponent<IHealthManager>(out IHealthManager health)) {

                float damageApplied = distanceFromMax_N >= startDistanceFallOff_N 
                    ? damageApplied = maxDamage * originBasedDistanceDecrementingDamageFallOffMultiplier
                    : damageApplied = maxDamage;

                health.AddDamage(damageApplied);
                // Debug.Log(damageApplied);

                if (collisionLayer.ContainsLayer(result[0].gameObject.layer)){
                    pooledObject.ReturnToPool();
                }
            }
            else
            {
                if (collisionLayer.ContainsLayer(result[0].gameObject.layer)){
                    pooledObject.ReturnToPool();
                }
            }
        }
    }
}
