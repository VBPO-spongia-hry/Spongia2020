using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public enum AttackMode {Melee, Ranged}
public class EnemyAttack : MonoBehaviour
{
    public float damage;
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
        if (!(_timer > _nextAttack)) return;
        _nextAttack += fireRate;
        Attack(_movement._player.position);
    }

    private void Attack(Vector2 destination)
    {
        //TODO: play attack anim
        switch (attackMode)
        {
            case AttackMode.Melee:
            {
                var player = Physics2D.OverlapCircleAll(transform.position, meleeRange).First((collider2D1 => collider2D1.CompareTag("Player")));
                //TODO: apply damage to player
                //player.GetComponent<>()
                break;
            }
            case AttackMode.Ranged:
                Instantiate(projectile, transform.position, transform.rotation);
                //TODO: implement projectiles
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}