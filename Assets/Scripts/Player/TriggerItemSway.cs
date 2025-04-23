using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerItemSway : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<ItemSway>(out var itemSway))
        {
            if (transform.position.x > itemSway.transform.position.x)
            {
                itemSway.SwayLeft();
            }
            else
            {
                itemSway.SwayRight();
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<ItemSway>(out var itemSway))
        {
            if (transform.position.x > itemSway.transform.position.x)
            {
                itemSway.SwayLeft();
            }
            else
            {
                itemSway.SwayRight();
            }
        }
    }
}
