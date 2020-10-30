using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using Environment;
using UnityEngine;
using UnityEngine.UI;

public enum AttackMode {Melee, Ranged}
public class EnemyAttack : MonoBehaviour, IDamageable
{
    public int maxHealth;
    private int _health;
    public Slider healthSlider;
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
        healthSlider.maxValue = maxHealth;
        _health = maxHealth;
        healthSlider.value = _health;
    }

    private void Update()
    {
        if(InputHandler.DisableInput) return;
        if (_movement.state != EnemyMovement.EnemyState.Attack) return;
        _timer += Time.deltaTime;
        if (_timer > _nextAttack)
        {
            _nextAttack += fireRate;
            Attack(_movement.Player.position);
        }
    }

    private void OnMouseDown()
    {
        ApplyDamage(5);
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
                        coll.GetComponent<IDamageable>().ApplyDamage(damage);
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

    public void ApplyDamage(int damage)
    {
        _health -= damage;
        healthSlider.value = _health;
        if(_health <= 0) Dead();
    }

    public void Dead()
    {
        transform.parent.GetComponent<EnemySpawner>().destroyed++;    
        //TODO: death anim / sound
        Destroy(gameObject);
    }
}