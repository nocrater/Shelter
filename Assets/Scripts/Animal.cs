using UnityEngine;
using System.Collections;

public class Animal : MonoBehaviour
{
    private Vector3 movingRect;
    
    private Rigidbody2D rb;

    private bool isMoving = false;

    private bool isFacingRight = false;

    public float timeToMove = 4f;
    public float timeToStand = 4f;

    public float speed = 16;

    public float minX = -5;
    public float maxX = 5;
    public float minY = -3;
    public float maxY = 3;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        StartCoroutine("CoUpdate");
    }

    IEnumerator CoUpdate()
    {
        yield return new WaitForSeconds(Random.Range(0, 3f));

        while (true) 
        {
            if (Random.Range(0, 2) == 0)
            {
                float x = Random.Range(minX, maxX);
                movingRect = new Vector3(transform.position.x + x, transform.position.y, 0);
                Flip(x);
            }
            else
            {
                movingRect = new Vector3(transform.position.x, transform.position.y + Random.Range(minY, maxY), 0);
            }

            isMoving = true;
            yield return new WaitForSeconds(Random.Range(timeToMove - 1, timeToMove));
            isMoving = false;

            yield return new WaitForSeconds(Random.Range(timeToStand - 1, timeToStand));
        }

        
    }

    void FixedUpdate() 
    {
        if (isMoving)
        {
            rb.MovePosition(Vector2.Lerp(transform.position, movingRect, speed * 0.01f * Time.fixedDeltaTime));
        }
    }

    void Flip(float x)
    {
        if ((isFacingRight && x < 0) || (!isFacingRight && x > 0))
        {
            isFacingRight = !isFacingRight;

            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;          
        }
    }
}
