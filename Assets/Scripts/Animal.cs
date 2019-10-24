﻿using System;
using UnityEngine;
using System.Collections;
using NPBehave;
using Action = NPBehave.Action;
using Random = UnityEngine.Random;

public class Animal : MonoBehaviour, Interactive
{
    [Header("States")]

    [SerializeField] private float timeToMove = 4f;
    [SerializeField] private float timeToStand = 4f;

    [Header("Moving vars")]
    
    [SerializeField] private float speed = 16;
    
    [Space]
    
    [SerializeField] private float speedMultiplierX = 5;
    [SerializeField] private float speedMultiplierY = 3;

    [Space]
    
    [SerializeField] private Vector2 allowedDistanceFromSpawnPoint = new Vector3(7f, 7f);
    
    
    private Vector3 spawnPoint;
    private Vector3 speedRect = Vector3.zero;
    
    private Rigidbody2D rb;

    private bool isMoving = false;
    private bool isRunAway = false;

    private bool isFacingRight = false;

    private Root behaviorTree;

    public virtual void OnClick()
    {
        throw new NotImplementedException();
    }
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spawnPoint = new Vector3(transform.position.x, transform.position.y);
        
        behaviorTree = new Root(
            new Selector(
                new BlackboardCondition("runAway", Operator.IS_EQUAL, true, Stops.IMMEDIATE_RESTART,
                    new Sequence(
                        new Action(() => RunAway()),
                        new WaitUntilStopped()
                    )
                    ),
                
                new Repeater(
                    new Sequence(
                        new Action(() => DecideStanding()),
                        new Wait(timeToStand, 0.5f),
                
                        new RandomSelector(
                            new Action(() => DecideMovingOnX()),
                            new Action(() => DecideMovingOnY())
                        ),
                        new Wait(timeToMove, 1f)
                    )
                    )
            )
        );
        behaviorTree.Start();
        
//        #if UNITY_EDITOR
//                Debugger debugger = (Debugger)this.gameObject.AddComponent(typeof(Debugger));
//                debugger.BehaviorTree = behaviorTree;
//        #endif
    }

    private void DecideStanding()
    {
        isMoving = false;
    }
    
    private void DecideMovingOnX()
    {
        float x = Random.Range(-speedMultiplierX, speedMultiplierX);

        if (transform.position.x > spawnPoint.x + allowedDistanceFromSpawnPoint.x)
            x = -Mathf.Abs(x);

        if (transform.position.x < spawnPoint.x - allowedDistanceFromSpawnPoint.x)
            x = Mathf.Abs(x);

        speedRect = new Vector3(x, 0, 0);
        Flip();
        
        isMoving = true;
    }
    
    private void DecideMovingOnY()
    {
        float y = Random.Range(-speedMultiplierY, speedMultiplierY);
        if (transform.position.y > spawnPoint.y + allowedDistanceFromSpawnPoint.y)
            y = -Mathf.Abs(y);

        if (transform.position.y < spawnPoint.y - allowedDistanceFromSpawnPoint.y)
            y = Mathf.Abs(y);

        speedRect = new Vector3(0, y, 0);
        
        isMoving = true;
    }

    private void FixedUpdate() 
    {
        if (isMoving)
        {
            rb.MovePosition(transform.position + Time.fixedDeltaTime * speed * 0.01f * speedRect);
        }
    }
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Animal") && !isRunAway && isMoving)
            StartCoroutine(ActivateRunAway());
    }

    IEnumerator ActivateRunAway()
    {
        isRunAway = true;
        behaviorTree.Blackboard["runAway"] = true;
        yield return new WaitForSeconds(3f);
        behaviorTree.Blackboard["runAway"] = false;
        isRunAway = false;
    }

    private void RunAway()
    {
        if (Math.Abs(speedRect.x) > 0.01)
            speedRect.x = Random.Range(0, 2) == 0 ? speedMultiplierX : -speedMultiplierX;
        else
            speedRect.y = Random.Range(0, 2) == 0 ? speedMultiplierY : -speedMultiplierY;
        Flip();
    }

    private void Flip()
    {
        if ((!isFacingRight && (speedRect.x < 0)) || (isFacingRight && (speedRect.x > 0))) 
            return;
        
        isFacingRight = !isFacingRight;

        Vector3 theScale = transform.localScale;
        theScale.x = isFacingRight ? -1 : 1;
        transform.localScale = theScale;
    }
}
