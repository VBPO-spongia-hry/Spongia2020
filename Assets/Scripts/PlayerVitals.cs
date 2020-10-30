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
    [NonSerialized]
    public int InfectionStrength;
    private float _timer;
    private float _nextHungerUpdate;
    private float _nextInfectionUpdate;
    
    
    private void Update()
    {
        if(InputHandler.DisableInput) return;
        _timer += Time.deltaTime;
        UpdateVitals();
    }

    private void UpdateVitals()
    {
        if (_timer >= _nextHungerUpdate)
        {
            hunger -= foodConsumptionAmount;
            if (hunger <= 0)
            {
                hunger = 0;
                //health -= 
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
        if(health <= 0) Dead();
    }

    public void Dead()
    {
        
    }
}