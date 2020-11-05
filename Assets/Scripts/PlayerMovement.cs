using System;
using Environment;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PlayerMovement : MonoBehaviour
{
    public float playerSpeed;
    public Transform flashlight;
    private Rigidbody2D _rb;
    private Transform _camera;
    public GameObject sideSkeleton;
    public GameObject frontSkeleton;
    public GameObject backSkeleton;
    public static IInteractable _interactable;
    public Vector2 Velocity => InputHandler.GetMovement() * playerSpeed;
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        if (Camera.main != null) _camera = Camera.main.transform;
        _camera.position = transform.position;
    }

    private void Update()
    {
        SetFlashlight();
        if(InputHandler.DisableInput) return;
        _rb.velocity = Vector2.zero;
        _rb.MovePosition(_rb.position + Velocity);
        var direction = InputHandler.GetMovement().normalized;
        if (Velocity.magnitude > 0)
        {
            flashlight.rotation = Quaternion.Slerp(flashlight.rotation,
                Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90),
                10 * Time.deltaTime);
            SetDirection(direction);
        }

        if (InputHandler.GetInteract())
        {
            _interactable?.Interact();
        }
    }

    private void SetFlashlight()
    {
        flashlight.gameObject.SetActive(Map.Instance.current.ActiveGlobalLight.intensity < .3f);
        int[] layers;
        if (Map.Instance.current.IsInInterior)
        {
            layers = new[]
            {
                SortingLayer.NameToID("Items"),
                SortingLayer.NameToID("Default"),
                SortingLayer.NameToID("Interior"),
            };
        }
        else
        {
            layers = new[]
            {
                SortingLayer.NameToID("Items"),
                SortingLayer.NameToID("Default"),
                SortingLayer.NameToID("Environment"),
            };
        }

        flashlight.GetComponent<Light2D>().m_ApplyToSortingLayers = layers;
    }

    private void LateUpdate()
    {
        var position = _rb.position;
        _camera.position = Vector3.Lerp(_camera.position, new Vector3(position.x, position.y,-2), playerSpeed * Time.deltaTime);
    }

    public void SetDirection(Vector2 movement)
    {
        var directions = new [] { Vector2.down, Vector2.right, Vector2.left, Vector2.up, };
        var mindelta = float.PositiveInfinity;
        var dir = Vector2.zero;
        frontSkeleton.SetActive(false);
        backSkeleton.SetActive(false);
        sideSkeleton.SetActive(false);
        foreach (var direction in directions)
        {
            if ((movement - direction).magnitude < mindelta)
            {
                mindelta = (movement - direction).magnitude;
                dir = direction;
            }
        }
        
        if (dir == Vector2.down) frontSkeleton.SetActive(true);
        else if (dir == Vector2.up) backSkeleton.SetActive(true);
        else if (dir == Vector2.left)
        {
            //Debug.Log("Left active");
            sideSkeleton.SetActive(true);
            sideSkeleton.transform.localScale = new Vector3(1,1);
        }
        else
        {
            sideSkeleton.SetActive(true);
            sideSkeleton.transform.localScale = new Vector3(-1,1);
        }
    }
}
