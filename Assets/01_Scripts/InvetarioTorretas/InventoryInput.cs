using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryInput : MonoBehaviour
{
    public VRInventory inventory;
    public InputActionProperty joystick;

    private float cooldown = 0.3f;
    private float timer = 0f;

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0f) return;

        Vector2 input = joystick.action.ReadValue<Vector2>();

        if (input.x > 0.7f)
        {
            inventory.SelectNext();
            timer = cooldown;
        }
        else if (input.x < -0.7f)
        {
            inventory.SelectPrevious();
            timer = cooldown;
        }
    }
}
