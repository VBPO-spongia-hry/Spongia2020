using System;
using System.Linq;
using UnityEngine;

public class EnemyVision : MonoBehaviour
{
    private EnemyMovement _movement;
    private bool _seePlayer;
    
    private void Start()
    {
        _movement = GetComponentInParent<EnemyMovement>();
    }

    private void Update()
    {
       // if(_seePlayer)
           // _movement.AttackIfClose();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;
        var hit = Physics2D.Raycast(transform.position, (_movement.Player.position - transform.position).normalized, float.PositiveInfinity, LayerMask.GetMask("Default","Items"));
        if (hit.collider.CompareTag("Player"))
        {
            //_movement.OnSeePlayer();
            _seePlayer = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;
        var hit = Physics2D.Raycast(transform.position, transform.position - _movement.Player.position);
        if (!_seePlayer)
        {
            if (hit.collider == null) return;
            if (hit.collider.CompareTag("Player"))
            {
                //_movement.OnSeePlayer();
                _seePlayer = true;
            }
        }
        else
        {
            if (!hit.collider.CompareTag("Player"))
            {
                //_movement.OnUnseePlayer();
                _seePlayer = false;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;
        if(_seePlayer)
        {
            //_movement.OnUnseePlayer();
            _seePlayer = false;
        }
    }
}