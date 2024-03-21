using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CoinPickUp : MonoBehaviour
{
    [SerializeField] AudioClip coinPickupSFX;
    [SerializeField] int pointsPerPickUp = 100;

    bool wasCollected = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && !wasCollected)
        {
            FindObjectOfType<GameSession>().AddToScore(pointsPerPickUp);
            AudioSource.PlayClipAtPoint(coinPickupSFX, Camera.main.transform.position);
            Destroy(gameObject);
            gameObject.SetActive(false);
            wasCollected = true;
        }
    }
}
