using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpAnimation : MonoBehaviour
{

    [SerializeField] private float speed = 5f;
    [SerializeField] private float height = 0.5f;
    [SerializeField] private float rotateSpeed;

    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    } 

    // Update is called once per frame
    void Update()
    {
        float sinMultiplier = Mathf.Sin(Time.time * speed);
        float yTranslation = sinMultiplier * height + startPos.y;
        transform.position = new Vector3(transform.position.x, yTranslation, transform.position.z);

        transform.Rotate(Vector3.up * rotateSpeed, Space.World);
    }
}
