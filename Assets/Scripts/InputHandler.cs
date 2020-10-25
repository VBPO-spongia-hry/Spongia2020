using UnityEngine;

public static class InputHandler
{
    public static bool DisableInput;
    public static Vector2 GetMovement()
    {
        return new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical")) * Time.deltaTime;
    }
}
