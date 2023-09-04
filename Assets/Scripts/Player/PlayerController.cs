using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    CharacterController cc;
    Animator anim;
    GameObject model;
    Vector3 curMoveInput;
    Vector3 moveDir;

    public float moveSpeed = 1.0f;
    public float gravity = 9.81f;
    public float instantaniousJumpVel = 10f;

    bool isJumping;
    float curJumpTime;
    float jumpTimeDuration = 0.11f;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            model = GameObject.FindGameObjectWithTag("PlayerModel");
            anim = model.GetComponent<Animator>();

            cc = GetComponent<CharacterController>();


            if (!cc) throw new UnassignedReferenceException("CC not set " + name);


            throw new NullReferenceException();

        }

        catch (UnassignedReferenceException e)
        {
            Debug.Log(e.Message);
        }
        catch (NullReferenceException e)
        {
            Debug.Log(e.Message);
        }
        finally
        {
            Debug.Log("This code always runs");
        }


        GameManager.Instance.input.Keyboard.Move.performed += ctx => Move(ctx);
        GameManager.Instance.input.Keyboard.Move.canceled += ctx => Move(ctx);

        GameManager.Instance.input.Keyboard.Jump.performed += ctx => JumpPressed(ctx);
        GameManager.Instance.input.Keyboard.Jump.canceled += ctx => JumpReleased(ctx);
    }

    private void JumpPressed(InputAction.CallbackContext ctx)
    {
        if (cc.isGrounded)
        {
            isJumping = true;
            curJumpTime = Time.time;
        }
    }

    private void JumpReleased(InputAction.CallbackContext ctx)
    {
        isJumping = false;
    }

    private void Move(InputAction.CallbackContext ctx)
    {
        if (ctx.canceled)
        {
            curMoveInput = Vector3.zero;
            moveDir = Vector3.zero;
            return;
        }

        Vector2 move = ctx.action.ReadValue<Vector2>();
        //move.Normalize();

        anim.SetFloat("hValue", move.x);
        anim.SetFloat("vValue", move.y);

        moveDir = new Vector3(move.x, 0, move.y).normalized;
        curMoveInput = moveDir * moveSpeed * Time.deltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > (curJumpTime + jumpTimeDuration))
            isJumping = false;


        if (isJumping)
        {
            curMoveInput.y += instantaniousJumpVel * Time.deltaTime;
        }

        if (!cc.isGrounded)
            curMoveInput.y += -gravity * Time.deltaTime;

        cc.Move(curMoveInput);
    }
}