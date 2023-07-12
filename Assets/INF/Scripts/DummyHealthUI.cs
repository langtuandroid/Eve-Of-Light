using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NeoFPS;
using TMPro;
using System;

public class DummyHealthUI : MonoBehaviour
{
    private Canvas canvas;
    private BasicHealthManager health;
    [SerializeField] private TMP_Text healthText;
    [SerializeField] private TMP_Text damageText;
    [SerializeField] private ParticleSystem onDeathEffect;

    void Awake() {
        canvas = GetComponent<Canvas>();
        health = transform.parent.GetComponent<BasicHealthManager>();
        healthText.text = string.Format($"{(int)health.healthMax}/{(int)health.health}");
        health.onHealthChanged += OnHealthChange;
    }

    private void OnHealthChange(float from, float to, bool critical, IDamageSource source)
    {
        healthText.text = string.Format($"{(int)health.healthMax}/{(int)to}");
        damageText.text = string.Format($"{(from - to).ToString("#.#")}");

        if (to <= 0)
        {
            Destroy(transform.parent.gameObject);
            Instantiate(onDeathEffect, transform.parent.position, Quaternion.identity);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        canvas.worldCamera = Camera.main;
    }

    void OnDisable() {
        health.onHealthChanged -= OnHealthChange;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerTracker._playerTransform != null)
        {
            transform.LookAt(PlayerTracker._playerTransform.position);
        }
    }
}
