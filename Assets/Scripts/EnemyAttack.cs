using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using Environment;
using Items;
using UnityEngine;
using UnityEngine.UI;

public enum AttackMode {Melee, Ranged}
public class EnemyAttack : MonoBehaviour, IDamageable
{
    public int maxHealth;
    private int _health;
    public Slider healthSlider;
    public Item weapon;
    public GameObject[] itemsDropped;
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
            _nextAttack += weapon.fireRate;
            Attack(_movement.Player.position);
        }
    }
    
    private void Attack(Vector3 destination)
    {
        //TODO: play attack anim
        switch (weapon.mode)
        {
            case AttackMode.Melee:
            {
                var colliders = Physics2D.OverlapCircleAll(transform.position, weapon.range);
                foreach (var coll in colliders)
                {
                    if (coll.CompareTag("Player"))
                    {
                        coll.GetComponent<IDamageable>().ApplyDamage(weapon.damage);
                        break;
                    }
                }
                break;
            }
            case AttackMode.Ranged:
                var transform1 = transform;
                var bullet = Instantiate(weapon.projectile, transform1.position, transform1.rotation).GetComponent<Projectile>();
                bullet.Source = weapon;
                bullet.Direction = (transform1.position - destination).normalized;
                bullet.Fired = transform1;
                bullet.IsFriendly = false;
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
        foreach (var o in itemsDropped)
        {
            Instantiate(o, transform.position, Quaternion.identity);
        }
        //TODO: death anim / sound
        Destroy(gameObject);
    }
}