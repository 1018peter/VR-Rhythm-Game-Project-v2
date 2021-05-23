using UnityEngine;
using UnityEngine.InputSystem;


public class HandAnimationControl : MonoBehaviour
{
    [SerializeField] Animator handAnimator;
    [SerializeField] InputActionReference gripAction;
    [SerializeField] InputActionReference pinchAction;

    private void OnEnable()
    {
        gripAction.action.performed += GripAnimation;
        pinchAction.action.performed += PinchAnimation;
    }

    private void PinchAnimation(InputAction.CallbackContext obj)
    {
        handAnimator.SetFloat("Trigger", obj.ReadValue<float>());
    }

    private void GripAnimation(InputAction.CallbackContext obj)
    {
        handAnimator.SetFloat("Grip", obj.ReadValue<float>());
    }
}