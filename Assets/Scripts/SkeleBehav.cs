using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeleBehav : MonoBehaviour {

    [SerializeField]
    internal GameObject player, skull;
    internal bool attacking = false, defending = false, attackcd = false;
    internal int maxhealth = 6, currenthealth, spawns = 0, lasthealth;
    [SerializeField]
    Sprite[] sprites;
    private Animator animator;
    private SpriteRenderer spriterenderer;
    private Rigidbody2D rb;
    [SerializeField]
    AudioClip[] deathsounds;

    Vector2 spawnlocation;
    private enum State
    {
        Idle,
        Attacking,
        Defending
    }
    private State currentstate;
	// Use this for initialization
	void Start ()
    {
        rb = GetComponent<Rigidbody2D>();
        spriterenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        animator.enabled = false;
        currentstate = State.Idle;
        currenthealth = maxhealth;
        spawnlocation = transform.position;
        lasthealth = currenthealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale != 0)
        {
            if (Mathf.Abs(rb.velocity.x) < 0.05f && !(animator.enabled && animator.GetCurrentAnimatorStateInfo(0).IsName("Skele_Punch")))
            {
                animator.enabled = false;
            }
            else if (Mathf.Abs(rb.velocity.x) > 0.05f || (animator.enabled && animator.GetCurrentAnimatorStateInfo(0).IsName("Skele_Punch")))
            {
                animator.enabled = true;
            }
            if (currenthealth <= 0)
            {
                PlayerControls.points += 30;
                AudioSource.PlayClipAtPoint(deathsounds[Random.Range(0, deathsounds.Length - 1)], Camera.main.transform.position, 1);
                Destroy(gameObject);
            }
            CheckState();
            switch (currentstate)
            {
                case State.Attacking:
                    if (!attacking && !defending && !attackcd)
                    {
                        StartCoroutine(Attacking());
                    }
                    break;
                case State.Defending:
                    if (!attacking && !defending)
                    {
                        StartCoroutine(Defending());
                    }
                    break;
                default:
                    if (Random.value * 100 > 99)
                    {
                        rb.velocity = new Vector2(transform.position.x - (spawnlocation.x + Random.Range(-0.55f, 0.55f)), 0);
                    }
                    break;
            }
            if (attacking || defending)
            {
                if (player.transform.position.x < transform.position.x)
                {
                    GetComponent<SpriteRenderer>().flipX = true;
                }
                else
                {
                    GetComponent<SpriteRenderer>().flipX = false;
                }

            }
            else if (rb.velocity.x < 0)
            {
                GetComponent<SpriteRenderer>().flipX = true;
            }
            else if (rb.velocity.x > 0)
            {
                GetComponent<SpriteRenderer>().flipX = false;
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
    private IEnumerator Attacking()
    {
        attacking = true;
        animator.enabled = true;
        animator.SetTrigger("Punch");
        yield return new WaitForSeconds(1f);
        var dir = transform.right;
        if(player.transform.position.x < transform.position.x)
        {
            dir = -dir;
        }
        spawns++;
        GameObject skully = Instantiate(skull, transform.position + (dir *0.1f), Quaternion.identity); AudioSource.PlayClipAtPoint(Resources.Load("Sound Effects/Skele Summon") as AudioClip, Camera.main.transform.position, 1);

        Physics2D.IgnoreCollision(skully.GetComponent<BoxCollider2D>(), GetComponent<BoxCollider2D>());
        skully.GetComponent<SkullBehav>().SetPar(gameObject);
        attacking = false;
        attackcd = true;
        yield return new WaitForSeconds(0.25f);
        attackcd = false;
    }
    private IEnumerator Defending()
    {
        defending = true;
        animator.enabled = false;
        spriterenderer.sprite = sprites[1];
        yield return new WaitForSeconds(0.75f + Random.Range(0f, 0.25f));
        spriterenderer.sprite = sprites[0];
        defending = false;
    }
    private void CheckState()
    {
        if (Vector2.Distance(player.transform.position, transform.position) < 1.75f && spawns < 4 && !attackcd)
        {
            currentstate = State.Attacking;
        }
        else if (!attacking && !PlayerControls.firebuffered)
        {
            currentstate = State.Defending;
        }
        else if(!attacking && !defending)
        {
            currentstate = State.Idle;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Contains("Brick") && !defending)
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
