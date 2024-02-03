using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDoubleJumpState : PlayerBaseState, IRootState
{
    public PlayerDoubleJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        InitializeSubState();
        HandleJump();

        Ctx.DoubleJump = false;
    }

    public override void UpdateState()
    {
        HandleGravity();
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Ctx.Animator.SetBool(Ctx.IsJumpingHash, false);
        if (Ctx.IsJumpPressed)
        {
            Ctx.RequireNewJumpPress = true;
        }
        //Ctx.CurrentJumpResetRoutine = Ctx.StartCoroutine(IJumpResetRoutine());
        if (Ctx.JumpCount == 2)
        {
            Ctx.JumpCount = 0;
            Ctx.Animator.SetInteger(Ctx.JumpCountHash, Ctx.JumpCount);
        }
    }

    public override void InitializeSubState()
    {
        if (!Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SetSubState(Factory.Idle());
        }
        else if (Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SetSubState(Factory.Walk());
        }
        else
        {
            SetSubState(Factory.Run());
        }
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.CharacterController.isGrounded)
        {
            SwitchState(Factory.Grounded());
        }
        else if (Ctx.IsAttackPressed && !Ctx.RequireNewAttackPress)
        {
            SwitchState(Factory.Attack());
        }
    }

    IEnumerator IJumpResetRoutine()
    {
        yield return new WaitForSeconds(0.5f);
        Ctx.JumpCount = 0;
    }

    void HandleJump()
    {
        /*if (Ctx.JumpCount < 3 && Ctx.CurrentJumpResetRoutine != null)
        {
            Ctx.StopCoroutine(Ctx.CurrentJumpResetRoutine);
        }*/
        Ctx.Animator.SetBool(Ctx.IsJumpingHash, true);
        Ctx.IsJumping = true;
        Ctx.JumpCount = 2;
        Ctx.Animator.SetInteger(Ctx.JumpCountHash, Ctx.JumpCount);
        Ctx.CurrentMovementY = Ctx.InitialJumpVelocities[Ctx.JumpCount];
        Ctx.AppliedMovementY = Ctx.InitialJumpVelocities[Ctx.JumpCount];

        Ctx.AudioManager.Play("Jump");
    }

    public void HandleGravity()
    {
        bool isFalling = Ctx.CurrentMovementY <= 0.0f || !Ctx.IsJumpPressed;
        float fallMultiplier = 2.0f;

        if (isFalling)
        {
            float previousYVelocity = Ctx.CurrentMovementY;
            Ctx.CurrentMovementY = Ctx.CurrentMovementY + (Ctx.JumpGravities[Ctx.JumpCount] * fallMultiplier * Time.deltaTime);
            Ctx.AppliedMovementY = Mathf.Max((previousYVelocity + Ctx.CurrentMovementY) * 0.5f, -20.0f);
        }
        else
        {
            float previousYVelocity = Ctx.CurrentMovementY;
            Ctx.CurrentMovementY = Ctx.CurrentMovementY + (Ctx.JumpGravities[Ctx.JumpCount] * Time.deltaTime);
            Ctx.AppliedMovementY = (previousYVelocity + Ctx.CurrentMovementY) * 0.5f;
        }
    }
}
