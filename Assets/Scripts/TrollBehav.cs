using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrollBehav : MonoBehaviour {

    internal int maxhealth = 8, currenthealth, lasthealth;
    internal Vector2 spawnlocation;
    [SerializeField]
    GameObject Rock, player;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private Animator a;
    internal bool attacking = false, attackcd = false, deflecting = false;
    [SerializeField]
    AudioClip[] deathsounds;

    internal enum State
    {
        Idle,
        Deflect,
        Attack
    }
    internal State currentstate;

	// Use this for initialization
	void Start () {
        a = GetComponent<Animator>();
        a.enabled = false;
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        spawnlocation = transform.position;
        currenthealth = maxhealth;
        currentstate = State.Idle;
        lasthealth = currenthealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale != 0)
        {
            if (Mathf.Abs(rb.velocity.x) < 0.05f && !(a.enabled && a.GetCurrentAnimatorStateInfo(0).IsName("Troll Throw")))
            {
                a.enabled = false;
            }
            else if (Mathf.Abs(rb.velocity.x) > 0.05f || !(a.enabled && a.GetCurrentAnimatorStateInfo(0).IsName("Troll Throw")))
            {
                a.enabled = true;
            }
            if (currenthealth <= 0)
            {
                AudioSource.PlayClipAtPoint(deathsounds[Random.Range(0, deathsounds.Length - 1)], Camera.main.transform.position, 1);

                PlayerControls.points += 35;
                Destroy(gameObject);
            }
            CheckState();
            switch (currentstate)
            {
                case State.Deflect:
                    if (!deflecting && !attacking)
                    {
                        StartCoroutine(Deflect());
                    }
                    break;
                case State.Attack:
                    if (!deflecting && !attacking && !attackcd)
                    {
                        StartCoroutine(Attack());
                    }
                    break;
                default:
                    if (Random.value * 100 > 98)
                    {
                        rb.velocity = new Vector2(transform.position.x - (spawnlocation.x + Random.Range(-0.6f, 0.6f)), 0) * 0.25f;
                    }
                    break;
            }
            if (attacking || deflecting)
            {
                if (player.transform.position.x < transform.position.x)
                {
                    sr.flipX = true;
                }
                else
                {
                    sr.flipX = false;
                }
            }
            else if (rb.velocity.x < 0)
            {
                sr.flipX = true;
            }
            else if (rb.velocity.x > 0)
            {
                sr.flipX = false;
            }
            if (lasthealth != currenthealth)
            {
                StopCoroutine(DisplayHPBar());
                foreach (Transform t in transform.GetChild(0))
                {
                    t.gameObject.SetActive(false);
                }
                StartCoroutine(DisplayHPBar());
            }
            lasthealth = currenthealth;
        }
    }
    internal void CheckState()
    {
        if (player != null)
        {
            if (!attacking && !attackcd && PlayerControls.LastBrick != null && PlayerControls.LastBrick.GetComponent<Rigidbody2D>() != null && !PlayerControls.firebuffered && Vector2.Distance(player.transform.position, transform.position) < 1.5f)
            {
                currentstate = State.Deflect;
            }
            else if (Vector2.Distance(player.transform.position, transform.position) < 1.25f)
            {
                currentstate = State.Attack;
            }
            else
            {
                currentstate = State.Idle;
            }
        }
    }
    internal IEnumerator Deflect()
    {
        deflecting = true;
        a.SetTrigger("Throw");
        a.enabled = true;
        yield return new WaitForSeconds(Random.Range(0.5f, 0.1f));
        if (PlayerControls.LastBrick != null && PlayerControls.LastBrick.GetComponent<Rigidbody2D>() != null)
        {
            GameObject newRock = Instantiate(Rock, transform.position + transform.up/12, Quaternion.identity);
            Physics2D.IgnoreCollision(newRock.GetComponent<BoxCollider2D>(), GetComponent<BoxCollider2D>());
            newRock.GetComponent<Rigidbody2D>().velocity = (PlayerControls.LastBrick.transform.position - newRock.transform.position) * 2 + (Vector2.Distance(PlayerControls.LastBrick.transform.position, newRock.transform.position) * Vector3.up / 2);
            newRock.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-1080f, 1080f);
        }
        yield return new WaitForSeconds(0.1f + Random.Range(0.5f, 0.1f));
        deflecting = false;
    }
    internal IEnumerator Attack()
    {
        attackcd = true;
        attacking = true;
        a.SetTrigger("Throw");
        a.enabled = true;
        AudioSource.PlayClipAtPoint(Resources.Load("Sound Effects/Rock Throw") as AudioClip, Camera.main.transform.position, 1);
        yield return new WaitForSeconds(0.6f + Random.Range(0.2f, 0.4f));
        GameObject newRock = Instantiate(Rock, transform.position + transform.up/12, Quaternion.identity);
        Physics2D.IgnoreCollision(newRock.GetComponent<BoxCollider2D>(), GetComponent<BoxCollider2D>());
        newRock.GetComponent<Rigidbody2D>().velocity = 0.75f *((player.transform.position + player.transform.up/5f - newRock.transform.position).normalized * (Vector2.Distance(player.transform.position, transform.position)) * 5) + (Vector2.Distance(player.transform.position, transform.position) * Vector3.up/2);
        newRock.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-1080f, 1080f);
        yield return new WaitForSeconds(0.4f + Random.Range(0.2f, 0.4f));
        attacking = false;
        yield return new WaitForSeconds(0.25f + Random.Range(0, 0.15f));
        attackcd = false;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Contains("Brick"))
        {
            AudioSource.PlayClipAtPoint(Resources.Load("Sound Effects/Enemy Hit") as AudioClip, Camera.main.transform.position, 1);

            currenthealth -= 2;
        }
    }
    internal IEnumerator DisplayHPBar()
    {
        for (int i = 0; i < currenthealth; i++)
        {
            transform.GetChild(0).GetChild(i).gameObject.SetActive(true);
        }
        yield return new WaitForSeconds(5);
        foreach (Transform t in transform.GetChild(0))
        {
            t.gameObject.SetActive(false);
        }
    }
}
