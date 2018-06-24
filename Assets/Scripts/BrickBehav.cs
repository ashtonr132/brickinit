using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickBehav : MonoBehaviour {

	// Use this for initialization
	void Start () {
        AudioSource.PlayClipAtPoint(Resources.Load("Sound Effects/Rock Throw") as AudioClip, Camera.main.transform.position, 1);
    }

    // Update is called once per frame
    void Update () {
		
	}
    private void OnCollisionEnter2D(Collision2D collision)
    {
        AudioSource.PlayClipAtPoint(Resources.Load("Sound Effects/Brick Hit") as AudioClip, Camera.main.transform.position, 1);
        GetComponent<ParticleSystem>().Play();
        Destroy(GetComponent<Rigidbody2D>());
        Destroy(GetComponent<Collider2D>());
        Destroy(GetComponent<SpriteRenderer>());
        Destroy(gameObject, 5);
    }
}
