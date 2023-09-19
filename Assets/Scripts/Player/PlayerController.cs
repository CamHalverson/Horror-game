using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    CharacterController cc;
    Animator anim;
    GameObject model;
    Vector3 curMoveInput;
    Vector3 moveDir;

    public Camera playerCamera;
    public float lookspeed = 2f;
    public float lookXLimit = 45f;
    float rotationX = 0;

    public float moveSpeed = 1.0f;
    public float gravity = 9.81f;
    public float instantaniousJumpVel = 10f;

    bool isJumping;
    float curJumpTime;
    float jumpTimeDuration = 0.11f;

    public bool canMove = true;

    bool isPunching;
    bool isKicking;

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

        GameManager.Instance.input.Keyboard.Attack1.performed += ctx => Attack1(ctx);
        GameManager.Instance.input.Keyboard.Attack1.canceled += ctx => Attack1(ctx);

        GameManager.Instance.input.Keyboard.Attack2.performed += ctx => Attack2(ctx);
        GameManager.Instance.input.Keyboard.Attack2.canceled += ctx => Attack2(ctx);
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

    private void Attack1(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            isPunching = true;
            anim.SetFloat("Punching", 1.0f);
        }
        else if(ctx.canceled)
        {
            isPunching = false;
            anim.SetFloat("Punching", 0.0f);
        }
       
    }

    private void Attack2(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            isKicking = true;
            anim.SetFloat("Kicking", 1.0f);
        }
        else if (ctx.canceled)
        {
            isKicking = false;
            anim.SetFloat("Kicking", 0.0f);
        }
        
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

        #region
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        #endregion

        #region 

        cc.Move(moveDir * Time.deltaTime);

        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookspeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookspeed, 0);
        }

        #endregion
    }


}
