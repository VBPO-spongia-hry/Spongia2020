using System;
using Items;
using TreeEditor;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class Projectile : MonoBehaviour
{
    
    [NonSerialized] public Vector3 Direction;
    public float speed;
    [NonSerialized] public Vector3 StartingVelocity = Vector3.zero;
    private Vector3 _start;
    [NonSerialized] public Item Source;
    [NonSerialized] public Transform Fired;
    [NonSerialized] public bool IsFriendly;
    private bool _hasHit;
    
    private void Start()
    {
        _hasHit = false;
        _start = transform.position;
        transform.rotation = Quaternion.Euler(0,0,Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg / 2 - 90);
    }

    private void Update()
    {
        var velocity = Direction * (speed * Time.deltaTime);
        transform.Translate(transform.right * speed * Time.deltaTime + StartingVelocity * Time.deltaTime);
        if (Fired == null)
        {
            Destroy(gameObject);
            return;
        }
        if (Vector2.Distance(Fired.position, transform.position) > Source.range)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(_hasHit) return;
        if (other.TryGetComponent(out IDamageable hit))
        {
            if (other.TryGetComponent(out EnemyMovement _) && !IsFriendly) return;
            if (other.TryGetComponent(out PlayerMovement _) && IsFriendly) return;
            hit.ApplyDamage(Source.damage);
            Debug.Log(other.name);
            _hasHit = true;
            Destroy(gameObject);
        }
    }
}
