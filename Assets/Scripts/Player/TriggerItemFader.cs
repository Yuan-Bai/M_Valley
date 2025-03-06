using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerItemFader : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<ItemFader>(out var itemFader))
        {
            itemFader.FadeIn();
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<ItemFader>(out var itemFader))
        {
            itemFader.FadeOut();
        }
    }
}
