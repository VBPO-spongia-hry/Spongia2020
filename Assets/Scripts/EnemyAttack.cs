using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public enum AttackMode {Melee, Ranged}
public class EnemyAttack : MonoBehaviour
{
    public int damage;
    public float fireRate;
    public AttackMode attackMode;
    public float meleeRange;
    public GameObject projectile;
    private EnemyMovement _movement;
    private float _timer = 0;
    private float _nextAttack;
    private void Start()
    {
        _movement = GetComponent<EnemyMovement>();
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_movement.state != EnemyMovement.EnemyState.Attack) return;
        if (_timer > _nextAttack)
        {
            _nextAttack += fireRate;
            Attack(_movement.Player.position);
        }
    }

    private void Attack(Vector2 destination)
    {
        //TODO: play attack anim
        switch (attackMode)
        {
            case AttackMode.Melee:
            {
                var colliders = Physics2D.OverlapCircleAll(transform.position, meleeRange);
                foreach (var coll in colliders)
                {
                    if (coll.CompareTag("Player"))
                    {
                        coll.GetComponent<PlayerVitals>().ApplyDamage(damage);
                        break;
                    }
                }
                break;
            }
            case AttackMode.Ranged:
                var transform1 = transform;
                Instantiate(projectile, transform1.position, transform1.rotation);
                //TODO: implement projectiles
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}