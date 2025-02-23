using Unity.VisualScripting;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;
    public PlayerController playerController;

    public void Jog()
    {
        animator.SetTrigger("Jog");
        animator.ResetTrigger("idle");
        animator.ResetTrigger("Sprint");
        animator.ResetTrigger("JogBackwards");
        animator.ResetTrigger("JogR");
        animator.ResetTrigger("JogL");
        animator.ResetTrigger("JogFR");
        animator.ResetTrigger("JogFL");
    }

    public void Idle()
    {
        animator.ResetTrigger("Jog");
        animator.SetTrigger("idle");
        animator.ResetTrigger("Sprint");
        animator.ResetTrigger("JogBackwards");
        animator.ResetTrigger("JogR");
        animator.ResetTrigger("JogL");
        animator.ResetTrigger("JogFR");
        animator.ResetTrigger("JogFL");
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        // Reset all triggers before setting new ones
        ResetAllTriggers();

        // Handle WASD input
        if (!playerController.isGrounded)
        {
            animator.SetTrigger("Jump");
        }
        else if ( (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !Input.GetKeyDown(KeyCode.S)) || (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))) // Forward (W)
        {
            animator.SetTrigger("Jog");
        } 
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.D)) // Forward-Right (W + D)
        {
            animator.SetTrigger("JogFR");
        }
        else if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.A)) // Forward-Left (W + A)
        {
            animator.SetTrigger("JogFL");
        } 
        else if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W) ) // Backward (S)
        {
            animator.SetTrigger("JogBackwards");
        }
        else if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D)) // Left (A)
        {
            animator.SetTrigger("JogL");
        }
        else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A)) // Right (D)
        {
            animator.SetTrigger("JogR");
        }
        else
        {
            // If no movement keys are pressed, set Idle
            animator.SetTrigger("Idle");
        }
    }

    // Function to reset all animation triggers
    void ResetAllTriggers()
    {
        animator.ResetTrigger("Idle");
        animator.ResetTrigger("Jog");
        animator.ResetTrigger("Sprint");
        animator.ResetTrigger("JogBackwards");
        animator.ResetTrigger("JogR");
        animator.ResetTrigger("JogL");
        animator.ResetTrigger("JogFR");
        animator.ResetTrigger("JogFL");
    }
}


