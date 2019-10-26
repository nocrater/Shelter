using System;
using UnityEngine;
using System.Collections;
using NPBehave;
using Action = NPBehave.Action;
using Random = UnityEngine.Random;

public class Animal : MonoBehaviour, Interactive
{
    [Header("States")] [SerializeField] private float timeToMove = 4f;
    [SerializeField] private float timeToStand = 4f;

    [Header("Moving vars")] [SerializeField]
    private float speed = 16;

    [Space] [SerializeField] private float speedMultiplierX = 5;
    [SerializeField] private float speedMultiplierY = 3;

    [Space] [SerializeField] private Vector2 allowedDistanceFromSpawnPoint = new Vector3(7f, 7f);


    private Vector3 spawnPoint;
    private Vector3 movingRect = Vector3.zero;

    private Rigidbody2D rb;

    protected bool isMoving = false;
    private bool isRunAway = false;

    private bool isFacingRight = false;

    private Root behaviorTree;

    public virtual void OnClick()
    {
        throw new NotImplementedException();
    }

    protected enum Direction
    {
        Left = 0,
        Bottom,
        Right,
        Top
    }

    protected Direction direction = Direction.Left;

    protected void Start()
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
                        new DecideForInterval(() => DecideStanding(), timeToStand),
                        new RandomSelector(
                            new DecideForInterval(() => DecideMovingOnX(), timeToMove, 1f),
                            new DecideForInterval(() => DecideMovingOnY(), timeToMove, 1f)
                        )
                    )
                )
            )
        );
        behaviorTree.Start();

#if UNITY_EDITOR
        Debugger debugger = (Debugger) this.gameObject.AddComponent(typeof(Debugger));
        debugger.BehaviorTree = behaviorTree;
#endif
    }

    private void DecideStanding()
    {
        isMoving = false;
    }

    private void DecideMovingOnX()
    {
        float x = Random.Range(-speedMultiplierX, speedMultiplierX);

        if (Math.Abs(x) < 0.01)
            return;

        if (transform.position.x > spawnPoint.x + allowedDistanceFromSpawnPoint.x)
            x = -Mathf.Abs(x);
        if (transform.position.x < spawnPoint.x - allowedDistanceFromSpawnPoint.x)
            x = Mathf.Abs(x);

        movingRect = new Vector3(x, 0, 0);

        direction = x > 0 ? Direction.Right : Direction.Left;
        //Flip();

        isMoving = true;
    }

    private void DecideMovingOnY()
    {
        float y = Random.Range(-speedMultiplierY, speedMultiplierY);

        if (Math.Abs(y) < 0.01)
            return;

        if (transform.position.y > spawnPoint.y + allowedDistanceFromSpawnPoint.y)
            y = -Mathf.Abs(y);
        if (transform.position.y < spawnPoint.y - allowedDistanceFromSpawnPoint.y)
            y = Mathf.Abs(y);

        movingRect = new Vector3(0, y, 0);

        direction = y > 0 ? Direction.Top : Direction.Bottom;

        isMoving = true;
    }

    private void FixedUpdate()
    {
        if (isMoving)
            rb.MovePosition(transform.position + Time.fixedDeltaTime * speed * 0.01f * movingRect);
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
        if (Math.Abs(movingRect.x) > 0.01)
            movingRect.x = Random.Range(0, 2) == 0 ? speedMultiplierX : -speedMultiplierX;
        else
            movingRect.y = Random.Range(0, 2) == 0 ? speedMultiplierY : -speedMultiplierY;
        //Flip();
    }

    private void Flip()
    {
        if ((!isFacingRight && (movingRect.x < 0)) || (isFacingRight && (movingRect.x > 0)))
            return;

        isFacingRight = !isFacingRight;

        Vector3 theScale = transform.localScale;
        theScale.x = isFacingRight ? -1 : 1;
        transform.localScale = theScale;
    }
}