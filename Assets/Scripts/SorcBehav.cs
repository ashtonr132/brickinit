 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SorcBehav : MonoBehaviour
{

    private float health, maxhealth = 35, waittime =1;
    private Animator a;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    [SerializeField]
    GameObject HealthBar, skull, rock;
    [SerializeField]
    Transform t;
    

    // Use this for initialization
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        health = maxhealth;
        a = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(Do(Random.value * 100));
    }
    private void Update()
    {
        if (Time.timeScale != 0)
        {
            if (!GetComponent<CapsuleCollider2D>().enabled && (transform.position.x < 2.4f || transform.position.x > 2.4f))
            {
                GetComponent<CapsuleCollider2D>().enabled = true;
            }
            if (health < 0)
            {
                AudioSource.PlayClipAtPoint(Resources.Load("Sound Effects/FanFare") as AudioClip, Camera.main.transform.position, 1);
                transform.GetChild(1).gameObject.SetActive(true);
                transform.GetChild(1).SetParent(null);
                Destroy(gameObject);
            }
        }
    }
    private IEnumerator Do(float rand)
   {
        sr.color = Color.yellow;
        yield return new WaitForSeconds(2);
        if (HealthBar.GetComponent<Slider>().value > 0.5f)
        {
            sr.color = Color.white;
        }
        else
        {
            a.speed = 2f;
            waittime = 0.5f;
            sr.color = new Color(255, 0, 255, 1);
        }
        if (rand < 25)
        {
            GetComponent<CapsuleCollider2D>().enabled = false;
            a.SetTrigger("SlideF");
            yield return new WaitForSeconds(2.8f* waittime);
            if (transform.position.x >= 2.4f)
            {
                rb.AddForce(-Vector2.right * 8, ForceMode2D.Impulse);
                sr.flipX = false;
            }
            else
            {
                sr.flipX = true;
                rb.AddForce(Vector2.right * 8, ForceMode2D.Impulse);

            }
            a.SetTrigger("SlideB");
            yield return new WaitForSeconds(2.8f* waittime); GetComponent<CapsuleCollider2D>().enabled = true;

            StartCoroutine(Do(Random.value * 100));
        }
        else if(rand < 50)
        {
            a.SetTrigger("Lightning");
            yield return new WaitForSeconds(2f* waittime);
            if (Mathf.Abs(GameObject.Find("Player").transform.position.x - transform.position.x) < 2)
            {
                PlayerControls.DamagePlayer(3);
            }
            yield return new WaitForSeconds(0.3f* waittime);
            StartCoroutine(Do(Random.value * 100));
        }
        else if (rand < 75)
        {
            a.SetTrigger("Summon");
            GameObject skully;
            for (int i = Random.Range(3, 6); i > 0; i--)
            {
                skully = Instantiate(skull, t.position, Quaternion.identity); yield return new WaitForSeconds(0.3f);
                skully.GetComponent<SkullBehav>().speed = 0.3f;
                yield return new WaitForSeconds(0.1f*waittime);
            }
            StartCoroutine(Do(Random.value * 100));
        }
        else
        {
            a.SetTrigger("Sky");
            yield return new WaitForSeconds(1.15f*waittime);
            for (int i = Random.Range(12, 19); i > 0; i--)
            {
                GameObject rocky;
                rocky = Instantiate(rock, new Vector2(GameObject.Find("Rockspawn").transform.position.x + Random.Range(-3f, 3f), 2.5f), Quaternion.identity);
                rocky.transform.localScale *= 3;
            }
            StartCoroutine(Do(Random.value * 100));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Contains("Brick") && sr.color == Color.yellow)
        {
            AudioSource.PlayClipAtPoint(Resources.Load("Sound Effects/Enemy Hit") as AudioClip, Camera.main.transform.position, 1);

            health--;
            HealthBar.GetComponent<Slider>().value = 1 / maxhealth * health;
        }
    }
}
