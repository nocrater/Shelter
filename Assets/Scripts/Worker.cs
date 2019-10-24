using UnityEngine;

public class Worker : MonoBehaviour, Interactive
{
    private Rigidbody2D rb;
    
    public void OnClick()
    {
        Debug.Log("Mmmm");
    }
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
}
