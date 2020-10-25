using UnityEngine;

public static class InputHandler
{
    private static bool disabled;
    public static bool DisableInput
    {
        get => disabled;
        set
        {
            disabled = value;
            foreach (var rb in Object.FindObjectsOfType<Rigidbody2D>())
            {
                rb.isKinematic = value;
                if (value) rb.velocity = Vector2.zero;
            }
        }
    }

    public static Vector2 GetMovement()
    {
        return new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical")) * Time.deltaTime;
    }
}
