using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class ShelfSpaceController : MonoBehaviour
{
    public StockInfo stockInfo;
    //public int amountOnShelf;
    public List<StockObject> objectsOnShelf;
    public List<Transform> bigDrinkPoints;
    public List<Transform> cerealPoints, tubeChipPoints, fruitPoints, largeFruitPoints;
    public TMP_Text shelfLabel;
    public void PlaceStock(StockObject objectToPlace)
    {
        bool preventPlacing = true;
        // if (amountOnShelf == 0)
        if (objectsOnShelf.Count == 0)
        {
            stockInfo = objectToPlace.stockInfo;
            preventPlacing = false;

        }
        else
        {
            if (stockInfo.name == objectToPlace.stockInfo.name)
            {
                preventPlacing = false;
                switch (stockInfo.typeofStock)
                {
                    case StockInfo.StockType.bigDrink:
                        if (objectsOnShelf.Count >= bigDrinkPoints.Count)
                        {
                            preventPlacing = false;
                        }
                        break;

                    case StockInfo.StockType.Cereal:
                        if (objectsOnShelf.Count >= cerealPoints.Count)
                        {
                            preventPlacing = false;
                        }
                        break;
                    case StockInfo.StockType.chipsTube:
                        if (objectsOnShelf.Count >= tubeChipPoints.Count)
                        {
                            preventPlacing = false;
                        }
                        break;
                    case StockInfo.StockType.fruit:
                        if (objectsOnShelf.Count >= fruitPoints.Count)
                        {
                            preventPlacing = false;
                        }
                        break;
                    case StockInfo.StockType.fruitLarge:
                        if (objectsOnShelf.Count >= largeFruitPoints.Count)
                        {
                            preventPlacing = false;
                        }
                        break;
                }
            }
        }
        if (preventPlacing == false)
        {
            //objectToPlace.transform.SetParent(transform);
            objectToPlace.Makeplaced();


            switch (stockInfo.typeofStock)
            {
                case StockInfo.StockType.bigDrink:
                    objectToPlace.transform.SetParent(bigDrinkPoints[objectsOnShelf.Count]);
                    break;

                case StockInfo.StockType.Cereal:
                    objectToPlace.transform.SetParent(cerealPoints[objectsOnShelf.Count]);
                    break;
                case StockInfo.StockType.chipsTube:
                    objectToPlace.transform.SetParent(tubeChipPoints[objectsOnShelf.Count]);

                    break;
                case StockInfo.StockType.fruit:
                    objectToPlace.transform.SetParent(fruitPoints[objectsOnShelf.Count]);
                    break;
                case StockInfo.StockType.fruitLarge:
                    objectToPlace.transform.SetParent(largeFruitPoints[objectsOnShelf.Count]);
                    break;
            }
            //amountOnShelf++;
            objectsOnShelf.Add(objectToPlace);
            shelfLabel.text = "$" + objectsOnShelf[0].stockInfo.price;

        }
    }
    public StockObject GetStock()
    {
        StockObject objectToReturn = null;
        if (objectsOnShelf.Count > 0)
        {
            objectToReturn = objectsOnShelf[objectsOnShelf.Count - 1];
            objectsOnShelf.RemoveAt(objectsOnShelf.Count - 1);
        }
        if (objectsOnShelf.Count == 0)
        {
            shelfLabel.text = "";
        }

        return objectToReturn;
    }

}
