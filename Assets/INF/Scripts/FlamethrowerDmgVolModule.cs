using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NeoFPS;
using NeoFPS.ModularFirearms;
using MyBox;

public class FlamethrowerDmgVolModule : MonoBehaviour
{

    [SerializeField] private Transform spawnPoint;
    [SerializeField] private FlameDmgVol flameDmgVolPrefab;
    [SerializeField] List<ParticleSystem> particleSystems;


    [Header("Damage Volume Properties")]
    [SerializeField, Range(0, 3)] private float spawnFrequency;
    [SerializeField] private float volumeSpeed;
    [SerializeField] private float startVolumeRadius;
    [SerializeField] private float maxTravelDistance;
    [SerializeField, Range(0, 10)] private float volumeGrowthRate;
    [SerializeField, Range(0, 1000)] private float maxDamage;
    [SerializeField, Range(0.1f, 0.9f)] private float damageFalloffStart;
    [SerializeField] private LayerMask nonPenetrableLayer;

    private IModularFirearm inputFirearm;
    private bool activated = true;
    private float spawnTimer = 0;
    private bool onSpawnCooldown = false;

    void Awake()
    {
        inputFirearm = GetComponent<IModularFirearm>();

        foreach (ParticleSystem ps in particleSystems)
        {
            var collisionModule = ps.collision;
            collisionModule.collidesWith = nonPenetrableLayer;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (inputFirearm.trigger.pressed) {
            if (!onSpawnCooldown) {

                Instantiate<FlameDmgVol>(flameDmgVolPrefab, spawnPoint.position, transform.rotation).
                    Init(spawnPoint.position, volumeSpeed, startVolumeRadius, maxTravelDistance, volumeGrowthRate, damageFalloffStart, nonPenetrableLayer, maxDamage);

                if (spawnFrequency > 0){
                    onSpawnCooldown = true;
                    spawnTimer = spawnFrequency;
                }
            }
            else {
                spawnTimer -= Time.deltaTime;

                if (spawnTimer <= 0) {
                    onSpawnCooldown = false;
                    spawnTimer = spawnFrequency;
                }
            }
        }
        else{
            onSpawnCooldown = false;
            spawnTimer = 0;
        }
    }
}
