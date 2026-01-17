using UnityEngine;
[System.Serializable]
public class StockInfo
{
    public string name;
    public enum StockType
    {
        Cereal,
        bigDrink,
        chipsTube,
        fruit,
        fruitLarge
    }
    public StockType typeofStock;
    public float price;
    public StockObject stockObject;
}

