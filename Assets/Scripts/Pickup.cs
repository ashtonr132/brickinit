using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (PlayerControls.currenthealth == PlayerControls.maxhealth && transform.name.Contains("Health"))
        {
            Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), GameObject.Find("Player").GetComponent<Collider2D>(), true);
        }
        else
        {
            Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(), GameObject.Find("Player").GetComponent<Collider2D>(), false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Contains("Player")) {
            switch (gameObject.name)
            {
                case "Coin":
                    AudioSource.PlayClipAtPoint(Resources.Load("Sound Effects/Coin Pickup") as AudioClip, Camera.main.transform.position, 1);

                    PlayerControls.points += 5;
                    Destroy(gameObject);
                    break;
                case "MaxHealth":

                    if (PlayerControls.currenthealth < PlayerControls.maxhealth)
                    {
                        AudioSource.PlayClipAtPoint(Resources.Load("Sound Effects/Health Pickup") as AudioClip, Camera.main.transform.position, 1);

                        PlayerControls.currenthealth = PlayerControls.maxhealth;
                        Destroy(gameObject);
                    }
                    break;
                case "Health":

                    if (PlayerControls.currenthealth < PlayerControls.maxhealth)
                    {
                        AudioSource.PlayClipAtPoint(Resources.Load("Sound Effects/Health Pickup") as AudioClip, Camera.main.transform.position, 1);

                        PlayerControls.currenthealth++;
                        Destroy(gameObject);
                    }
                    break;
            } }
    }
    
}
