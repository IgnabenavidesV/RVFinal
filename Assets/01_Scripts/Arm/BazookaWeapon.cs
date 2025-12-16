using UnityEngine;
using UnityEngine.InputSystem;

public class BazookaWeapon : MonoBehaviour
{
    public Transform firePoint;
    public GameObject projectilePrefab;
    public float fireCooldown = 0.6f;

    public InputActionProperty fireAction;
    public InputActionProperty holsterAction;

    private float lastFireTime = -999f;

    // ? Solo se permite disparar cuando el WeaponManager lo arma
    private bool isEquipped = false;

    public void SetEquipped(bool equipped)
    {
        isEquipped = equipped;

        // Solo escuchar inputs si está equipada
        if (equipped) EnableInput();
        else DisableInput();
    }

    private void OnEnable()
    {
        // Si se activa por cualquier razón sin equipar, NO escuchar
        if (isEquipped) EnableInput();
    }

    private void OnDisable()
    {
        DisableInput();
    }

    private void EnableInput()
    {
        if (fireAction.action != null)
        {
            fireAction.action.Enable();
            fireAction.action.performed -= OnFire;
            fireAction.action.performed += OnFire;
        }

        if (holsterAction.action != null)
        {
            holsterAction.action.Enable();
            holsterAction.action.performed -= OnHolster;
            holsterAction.action.performed += OnHolster;
        }
    }

    private void DisableInput()
    {
        if (fireAction.action != null)
        {
            fireAction.action.performed -= OnFire;
            fireAction.action.Disable();
        }

        if (holsterAction.action != null)
        {
            holsterAction.action.performed -= OnHolster;
            holsterAction.action.Disable();
        }
    }

    private void OnHolster(InputAction.CallbackContext ctx)
    {
        if (!isEquipped) return;
        if (WeaponManager.Instance != null)
            WeaponManager.Instance.UnequipWeapon();
    }

    private void OnFire(InputAction.CallbackContext ctx)
    {
        if (!isEquipped) return;

        Debug.Log("?? DISPARO");

        if (Time.time - lastFireTime < fireCooldown) return;
        lastFireTime = Time.time;

        if (firePoint == null || projectilePrefab == null) return;

        Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
    }
}
