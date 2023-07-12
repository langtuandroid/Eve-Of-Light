using UnityEngine;

public class PlayerTracker : MonoBehaviour
{
    public static Transform _playerTransform;
    void Start() => _playerTransform = transform;
}
