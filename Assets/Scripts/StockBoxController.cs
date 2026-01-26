using System.Collections.Generic;
using NUnit.Framework.Internal;
using Unity.Mathematics;
using UnityEngine;



public class StockBoxController : MonoBehaviour
{
    public StockInfo stockInfo;
    public List<Transform> bigDrinkPoints;
    public List<Transform> cerealPoints, tubeChipPoints, fruitPoints, largeFruitPoints;
    public List<StockObject> stockInBox;

    public bool testFill;
    public Rigidbody rigidBody;
    public Collider collider;
    public float moveSpeed = 5f;
    private bool isHeld;
    public GameObject flap1, flap2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (testFill == true)
        {
            testFill = false;
            SetupBox(stockInfo);
        }
        if (isHeld == true)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, Vector3.zero, moveSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.identity, moveSpeed * Time.deltaTime);
        }
    }
    public void SetupBox(StockInfo stockType)
    {
        stockInfo = stockType;
        List<Transform> activePoints = new List<Transform>();
        switch (stockInfo.typeofStock)
        {
            case StockInfo.StockType.bigDrink:
                activePoints.AddRange(bigDrinkPoints);
                break;

            case StockInfo.StockType.Cereal:
                activePoints.AddRange(cerealPoints);
                break;
            case StockInfo.StockType.chipsTube:
                activePoints.AddRange(tubeChipPoints);
                break;
            case StockInfo.StockType.fruit:
                activePoints.AddRange(fruitPoints);
                break;
            case StockInfo.StockType.fruitLarge:
                activePoints.AddRange(largeFruitPoints);
                break;
        }
        if (stockInBox.Count == 0)
        {
            for (int i = 0; i < activePoints.Count; i++)
            {
                StockObject stock = Instantiate(stockType.stockObject, activePoints[i]);
                stock.transform.localPosition = Vector3.zero;
                stock.transform.localRotation = Quaternion.identity;
                stockInBox.Add(stock);
                stock.PlaceInBox();
            }
        }
    }
    public void Pickup()
    {
        rigidBody.isKinematic = true;

        collider.enabled = false;
        isHeld = true;
    }
    public void Release()
    {
        rigidBody.isKinematic = false;
        collider.enabled = true;
        isHeld = false;

    }
    public void OpenClose()
    {
        if (flap1.activeSelf == true)
        {
            flap1.SetActive(false);
            flap2.SetActive(false);
        }
        else
        {
            flap1.SetActive(true);
            flap2.SetActive(true);
        }
    }
    public void PlaceStockOnShelf(ShelfSpaceController shelf)
    {
        if (stockInBox.Count > 0)
        {
            StockObject lastStock = stockInBox[stockInBox.Count - 1];
            shelf.PlaceStock(lastStock);
            if (lastStock.isPlaced == true)
            {
                stockInBox.RemoveAt(stockInBox.Count - 1);
            }
        }
        if (flap1.activeSelf == true)
        {
            OpenClose();
        }
    }
}
