using System;
using Items;
using UnityEngine;

public class PlayerVitals : MonoBehaviour, IDamageable
{
    public float health = 100;
    public float maxHealth = 100;
    public int hunger = 50;
    public float foodConsumptionRate = 8;
    public int foodConsumptionAmount = 2;
    public int infection = 100;
    public int infectionRegenerationAmount = 3;
    public int infectionHealthDropAmount = 5;
    public float infectionDropRate = 2;
    public Inventory inventory;
    public Item defaultWeapon;
    [NonSerialized]
    public int InfectionStrength;
    private float _timer;
    private float _nextHungerUpdate;
    private float _nextInfectionUpdate;
    private float _nextAttack;
    public AudioClip hitClip;
    private AudioSource _audioSource;

    private Item Weapon => inventory.Weapon;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if(InputHandler.DisableInput) return;
        _timer += Time.deltaTime;
        UpdateVitals();
        if (Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Space))
        {
            Attack();
        }
    }

    private void Attack()
    {
        if (_timer > _nextAttack)
        {
            var fireRate = Weapon == null ? defaultWeapon.fireRate : Weapon.fireRate;
            _nextAttack = _timer + fireRate;
            var attackDestination =Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //var movement = GetComponent<PlayerMovement>();
            //if(movement.Velocity.magnitude < .1f) movement.SetDirection((attackDestination - transform.position).normalized);
            Attack(attackDestination);
            
        }
    }
    
    private void UpdateVitals()
    {
        if (_timer >= _nextHungerUpdate)
        {
            hunger -= foodConsumptionAmount;
            if (hunger <= 0)
            {
                hunger = 0;
                health -= 3*foodConsumptionAmount;
                if(health <= 0) Dead();
            }

            _nextHungerUpdate = foodConsumptionRate + _timer;
        }

        if (_timer >= _nextInfectionUpdate)
        {
            if (InfectionStrength > 0)
            {
                var protectionMultiplier = inventory.Protection == null ? 0 : inventory.Protection.protectionLevel; 
                infection -= (int)(InfectionStrength * (1 - protectionMultiplier));
                if (infection <= 0)
                {
                    health -= infectionHealthDropAmount;
                    if(health <= 0) Dead();
                    infection = 0;
                }
            }
            else
            {
                if(infection < 100) infection += infectionRegenerationAmount;
            }
            
            _nextInfectionUpdate = _timer + infectionDropRate;
        }
    }

    public void ApplyDamage(int damage)
    {
        float damageMultiplier = Inventory.Instance.Armor == null ? 1 : 1 - Inventory.Instance.Armor.toughness;
        health -= damage * damageMultiplier;
        _audioSource.clip = hitClip;
        _audioSource.Play();
        if(health <= 0) Dead();
    }

    public void Dead()
    {
        InputHandler.DisableInput = true;
        UIController.Instance.Death();
        Destroy(gameObject);
    }
    
    private void Attack(Vector3 destination)
    {
        var weaponMode = Weapon == null ? defaultWeapon.mode : Weapon.mode;
        //TODO: play attack anim
        switch (weaponMode)
        {
            case AttackMode.Melee:
            {
                var colliders = Physics2D.OverlapCircleAll(transform.position, Weapon == null ? defaultWeapon.range : Weapon.range);
                foreach (var coll in colliders)
                {
                    if (coll.TryGetComponent(out IDamageable damageable))
                    {
                        if(coll.CompareTag("Player")) continue;
                        damageable.ApplyDamage(Weapon == null ? defaultWeapon.damage : Weapon.damage);
                        break;
                    }
                }
                break;
            }
            case AttackMode.Ranged:
                var transform1 = transform;
                var bullet = Instantiate(Weapon.projectile, transform1.position, transform1.rotation).GetComponent<Projectile>();
                bullet.IsFriendly = true;
                bullet.Source = Weapon == null ? defaultWeapon : Weapon;
                bullet.Fired = transform1;
                bullet.StartingVelocity = GetComponent<PlayerMovement>().Velocity;
                bullet.Direction = (transform1.position - destination).normalized;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}