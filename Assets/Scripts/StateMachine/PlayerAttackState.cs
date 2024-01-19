using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerBaseState, IRootState
{
    public PlayerAttackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        InitializeSubState();
        HandleAttack();
    }

    public override void UpdateState()
    {
        Ctx.AttackTimer += Time.deltaTime;
        HandleGravity();
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        if (Ctx.IsAttackPressed)
        {
            Ctx.RequireNewAttackPress = true;
        }
        Ctx.IsAttacking = false;
        Ctx.Animator.SetBool(Ctx.AttackHash, false);
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
        if (Ctx.CharacterController.isGrounded && Ctx.AttackTimer >= Ctx.TimeToAttack)
        {
            Ctx.AttackTimer = 0;
            Ctx.IsAttacking = false;
            Ctx.AttackArea.SetActive(Ctx.IsAttacking);
            SwitchState(Factory.Grounded());
        }
    }

    void HandleAttack()
    {
        Ctx.IsAttacking = true;
        Ctx.AttackArea.SetActive(Ctx.IsAttacking);
        Ctx.Animator.SetBool(Ctx.AttackHash, true);
        Ctx.AttackParticleSystem.Play();
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