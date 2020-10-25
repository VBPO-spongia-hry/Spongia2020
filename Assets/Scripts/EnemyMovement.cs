using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEditor;
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
    [NonSerialized]
    public Transform Player;

    private Coroutine _observing;
    private Queue<Vector2> _path;
    private Pathfinding _pathfinding;
    private Vector2 _pathDestination;
    private bool _pathDestValid;
    private Vector2 _prevPos;
    private void Start()
    {
        _seePlayer = false;
        _rb = GetComponent<Rigidbody2D>();
        _spawn = transform.position;
        Player = GameObject.FindWithTag("Player").transform;
        state = EnemyState.Patrol;
        _destination = _spawn;
        _path = new Queue<Vector2>();
        _pathDestination = _spawn;
        _prevPos = _spawn;
        _pathfinding = Pathfinding.GetPathfinding(_spawn);
        StartCoroutine(OnDestinationArrived(false));
    }

    private void FixedUpdate()
    {
        if (InputHandler.DisableInput) return;
        if (state == EnemyState.Follow)
        {
            //AssignNewPath();
            var position = Player.position;
            Moveto(position);
            _destination = position;
            _pathDestValid = false;
            //Debug.Log("Move to player");
        }
        if (state == EnemyState.Patrol)
        {
            if (_path.Count == 0)
            {
                if(_observing == null) _observing = StartCoroutine(OnDestinationArrived(true));
                _pathDestValid = false;
            }
            else if (Vector2.Distance(_rb.position, _pathDestination) < .1f || !_pathDestValid)
            {
                _pathDestination = _path.Dequeue();
                _pathDestValid = true;
                //Moveto(_pathDestination);
            }
            else if (_pathDestValid) Moveto(_pathDestination);
        }

        _prevPos = _rb.position;
    }

    private void Moveto(Vector2 destination)
    {
        var velocity = state != EnemyState.Follow ? speed : followSpeed;
        var position = _rb.position;
        var movement = (destination - position).normalized;
        _rb.MovePosition(position + movement * (velocity * Time.deltaTime));
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
        yield return null;
        SetDestination();
        _observing = null;
    }

    private void SetDestination()
    {
        switch (mode)
        {
            case EnemyMode.Patrol:
            {
                _waypointIndex++;
                if (_waypointIndex == waypoints.Length) _waypointIndex = 0;
                _destination = waypoints[_waypointIndex].position;
                break;
            }
            case EnemyMode.Idle:
                _destination = _spawn + Random.insideUnitCircle * maxDefendRadius;
                while (!_pathfinding.PathExist(_rb.position, _destination))
                {
                    _destination = _spawn + Random.insideUnitCircle * maxDefendRadius;
                }
                break;
            case EnemyMode.Defend:
                _destination = _spawn;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        AssignNewPath();
    }

    private void AssignNewPath()
    {
        Vector2Int pathStart = _pathfinding.FindNearestWalkable(_rb.position);
        _path.Clear();
        // Debug.Log(_pathfinding.PathExist(_rb.position,_destination));
        var path = _pathfinding.GetPath(pathStart, Vector2Int.RoundToInt(_destination));
        Debug.Log(path.Count);
        state = EnemyState.Patrol;
        if(path == null) Debug.LogError("PAth null");
        // if (mode == EnemyMode.Defend && path.Count <= 1) return;
        foreach (var dest in path)
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
        _destination = Player.position;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        Debug.Log("Player escaped");
        _seePlayer = false;
        state = EnemyState.Patrol;
        //if (mode == EnemyMode.Idle || mode == EnemyMode.Defend) _destination = _spawn;
        //else _destination = waypoints[_waypointIndex].position;
        //SetDestination();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) return;
        _seePlayer = true;
        if (Vector2.Distance(_rb.position, Player.position) < attackRange)
        {
            state = EnemyState.Attack;
            _rb.velocity = Vector2.zero;
        }
        else
        {
            state = EnemyState.Follow;
        }
    }

    private void OnDrawGizmosSelected()
    {
        switch (state)
        {
            case EnemyState.Patrol:
                Gizmos.color = Color.green;
                break;
            case EnemyState.Observe:
                Gizmos.color = Color.magenta;
                break;
            case EnemyState.Follow:
                Gizmos.color = Color.yellow;
                break;
            case EnemyState.Attack:
                Gizmos.color = Color.red;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        Gizmos.DrawWireSphere(transform.position, .5f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, _destination);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, _pathDestination);
        var p = _path.ToArray();
        for (int i = 0; i < _path.Count-1; i++)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(p[i], p[i+1]);
        }
    }
}