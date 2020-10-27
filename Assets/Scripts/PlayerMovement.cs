using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float playerSpeed;
    public Transform flashlight;
    private Rigidbody2D _rb;
    private Transform _camera;
    public static IInteractable _interactable;

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
        var direction = InputHandler.GetMovement().normalized;
        if (velocity.magnitude > 0)
        {
            flashlight.rotation = Quaternion.Slerp(flashlight.rotation,
                Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90),
                10 * Time.deltaTime);
        }

        if (InputHandler.GetInteract())
        {
            _interactable?.Interact();
        }
    }

    private void LateUpdate()
    {
        var position = _rb.position;
        _camera.position = Vector3.Lerp(_camera.position, new Vector3(position.x, position.y,-2), playerSpeed * Time.deltaTime);
    }
}
