using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSPlayerAnimator : MonoBehaviour
{
        private Animator anim;

        private void Start()
        {
            anim = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            float verticalInput = Input.GetAxis("Vertical");
            bool isMoving = Mathf.Abs(verticalInput) > 0.1f;

            if (anim != null)
            {
                // Set animation parameters based on character state
                anim.SetBool("IsMoving", isMoving);
            }
        }
}
