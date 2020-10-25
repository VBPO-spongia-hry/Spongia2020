using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float playerSpeed;
    private Rigidbody2D _rb;
    private Transform _camera;
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (Camera.main != null) _camera = Camera.main.transform;
        _camera.position = transform.position;
    }

    private void Update()
    {
        if(InputHandler.DisableInput) return;
        var velocity = InputHandler.GetMovement() * playerSpeed;
        _rb.MovePosition(_rb.position + velocity);
       
    }

    private void LateUpdate()
    {
        var position = _rb.position;
        _camera.position = Vector3.Lerp(_camera.position, new Vector3(position.x, position.y,-2), Time.deltaTime);
    }
}
