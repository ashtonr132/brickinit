using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullBehav : MonoBehaviour {

    internal int maxhealth = 1, currenthealth;
    internal float speed = 0.175f;
    GameObject player, pare;
	// Use this for initialization
	void Start ()
    {
        currenthealth = maxhealth;
        player = GameObject.Find("Player");
	}

    // Update is called once per frame
    void Update()
    {
        if (player != null) {
            GetComponent<Rigidbody2D>().velocity = (player.transform.position - transform.position).normalized * speed;
            transform.rotation = Quaternion.LookRotation(player.transform.position, Vector3.up) * Quaternion.Euler(0, 90, 0);
            var ps = GetComponent<ParticleSystem>().shape;
            if (GetComponent<Rigidbody2D>().velocity.x > 0)
            {
                GetComponent<SpriteRenderer>().flipX = true;
                ps.rotation = new Vector3(0, 0, -25);
            }
            else
            {
                GetComponent<SpriteRenderer>().flipX = false;
                ps.rotation = new Vector3(0, 0, 155);
            }
        } }
    private void OnCollisionEnter2D(Collision2D collision)
    {
            if (collision.gameObject.tag == "Player")
            {
                PlayerControls.DamagePlayer(1);
                if (pare != null)
                {
                    pare.GetComponent<SkeleBehav>().spawns--;
                }
                Destroy(gameObject);
            }
            else if (collision.gameObject.name.Contains("Brick"))
            {
            AudioSource.PlayClipAtPoint(Resources.Load("Sound Effects/Enemy Hit") as AudioClip, Camera.main.transform.position, 1);

            if (pare != null)
                {
                    pare.GetComponent<SkeleBehav>().spawns--;
                }
                PlayerControls.points += 1; AudioSource.PlayClipAtPoint(Resources.Load("Sound Effects/SkullDeath") as AudioClip, Camera.main.transform.position, 1);

            Destroy(gameObject);
            }
    }
    internal void SetPar(GameObject par)
    {
        pare = par;
    }
}
