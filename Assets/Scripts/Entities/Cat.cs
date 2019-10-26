using System;
using UnityEngine;

public class Cat : Animal
{
    private Animator animator;
    private static readonly int DirectionId = Animator.StringToHash("direction");
    private static readonly int IsMoving = Animator.StringToHash("isMoving");

    private Direction lastDirection;
    private bool lastIsMoving;
    

    private new void Start()
    {
        animator = GetComponent<Animator>();
        lastDirection = direction;
        lastIsMoving = isMoving;
        base.Start();
    }

    private void Update()
    {
        if (direction != lastDirection)
        {
            animator.SetInteger(DirectionId, (int) direction);
            lastDirection = direction;
        }

        if (lastIsMoving != isMoving)
        {
            animator.SetBool(IsMoving, isMoving);
            lastIsMoving = isMoving;
        }
    }

    public override void OnClick()
    {
        Debug.Log("Mya");
    }
}
