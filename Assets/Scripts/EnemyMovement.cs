using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public enum EnemyMode { Idle, Defend, Patrol}
public class EnemyMovement : MonoBehaviour
{
    public EnemyMode mode;
    public float speed;
    public float followSpeed;
    public float attackRange;
    public float observeTime;
    public Transform[] waypoints;
    public float maxDefendRadius;
    public enum EnemyState {Patrol, Observe, Follow, Attack}
    [HideInInspector]
    public EnemyState state;
    private bool _seePlayer;
    private Rigidbody2D _rb;
    private int _waypointIndex;
    private Vector2 _spawn;
    private Vector2 _destination;
    [HideInInspector]
    public Transform _player;

    private Coroutine _observing;
    private Queue<Vector2> _path;
    private Pathfinding _pathfinding;
    private Vector2 _pathDestination;
    private void Start()
    {
        _seePlayer = false;
        _rb = GetComponent<Rigidbody2D>();
        _spawn = transform.position;
        _player = GameObject.FindWithTag("Player").transform;
        state = EnemyState.Patrol;
        _destination = _spawn;
        _path = new Queue<Vector2>();
        _pathDestination = _spawn;
        _pathfinding = Pathfinding.GetPathfinding(_spawn);
        StartCoroutine(OnDestinationArrived(false));
    }

    private void Update()
    {
        if (InputHandler.DisableInput) return;
        if (_seePlayer) Moveto(_player.position);
        else if (state == EnemyState.Patrol)
        {
            if (_path.Count == 0) StartCoroutine(OnDestinationArrived(true));
            if (Vector2.Distance(_rb.position, _pathDestination) < .2f) _pathDestination = _path.Dequeue();
            Moveto(_pathDestination);
        }
    }

    private void Moveto(Vector2 destination)
    {
        var velocity = state != EnemyState.Follow ? speed : followSpeed;
        var position = _rb.position;
        var movement = (destination - position).normalized;
        _rb.MovePosition(position + movement * (velocity * Time.deltaTime));
        //Debug.Log(movement.magnitude);
        //if (Vector2.Distance(transform.position, destination) < .3f && triggerOnArrive) _observing = StartCoroutine(OnDestinationArrived());
    }

    private IEnumerator OnDestinationArrived(bool observe)
    {
        if (observe)
        {
            state = EnemyState.Observe;
            _rb.velocity = Vector2.zero;
            Debug.Log("Arrived");
            //TODO: otacanie a hladanie hraca
            yield return new WaitForSeconds(observeTime);
        }
        switch (mode)
        {
            case EnemyMode.Patrol:
            {
                _waypointIndex++;
                if (_waypointIndex == waypoints.Length) _waypointIndex = 0;
                _destination = waypoints[_waypointIndex].position;
                break;
            }
            case EnemyMode.Defend:
                _destination = _spawn + Random.insideUnitCircle * maxDefendRadius;
                Debug.Log(_destination);
                break;
            case EnemyMode.Idle:
                _destination = _spawn;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        AssignNewPath();
        state = EnemyState.Patrol;
        yield break;
    }

    private void AssignNewPath()
    {
        foreach (var dest in _pathfinding.GetPath(Vector2Int.RoundToInt(_rb.position), Vector2Int.RoundToInt(_destination)))
        {
            _path.Enqueue(dest);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;
        Debug.Log("I see player");
        _seePlayer = true;
        if(_observing != null) StopCoroutine(_observing);
        _observing = null;
        state = EnemyState.Follow;
        _destination = _player.position;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        Debug.Log("Player escaped");
        _seePlayer = false;
        state = EnemyState.Patrol;
        StartCoroutine(OnDestinationArrived(false));
        if (mode == EnemyMode.Idle || mode == EnemyMode.Defend) _destination = _spawn;
        else _destination = waypoints[_waypointIndex].position;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;
        _seePlayer = true;
        if (Vector2.Distance(_rb.position, _player.position) < attackRange)
        {
            state = EnemyState.Attack;
            _rb.velocity = Vector2.zero;
        }
        else
        {
            state = EnemyState.Follow;
        }
    }
}