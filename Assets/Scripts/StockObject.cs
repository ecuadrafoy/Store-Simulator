
using UnityEngine;
using UnityEngine.Rendering;

public class StockObject : MonoBehaviour
{
    public StockInfo stockInfo;
    public float moveSpeed;
    public bool isPlaced;
    public Rigidbody rigidBody;
    public Collider collider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stockInfo = StockInfoController.instance.GetInfo(stockInfo.name);
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlaced == true)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, Vector3.zero, moveSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, moveSpeed * Time.deltaTime);
        }

    }

    public void Pickup()
    {
        rigidBody.isKinematic = true;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        isPlaced = false;
        collider.enabled = false;
    }
    public void Makeplaced()
    {
        rigidBody.isKinematic = true;
        isPlaced = true;
        collider.enabled = false;

    }
    public void Release()
    {
        rigidBody.isKinematic = false;
        collider.enabled = true;

    }
    public void PlaceInBox()
    {
        rigidBody.isKinematic = true;
        collider.enabled = false;
    }
}
