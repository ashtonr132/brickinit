using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigBehav : MonoBehaviour {
    [SerializeField]
    GameObject Player;
    private int maxhealth = 6, currenthealth, damage = 1, lasthealth;
    private float speed = 0.65f, attackspeed = 1f, wanderrange = 0.75f;
    private Vector2 SpawnLocation;
    private Rigidbody2D rb;
    private bool attackedrecently = false, ishealing = false, facingright = true, goingright = true, charging = false, attacking = false;
    private Animator animator;
    [SerializeField]
    AudioClip[] deathsounds;
    private enum State
    {
        Idle,
        Charging,
        Attacking,
        Fleeing
    }
    private State currentstate;
	// Use this for initialization
	void Start ()
    {
        currentstate = State.Idle;
        currenthealth = maxhealth;
        SpawnLocation = transform.position;
        rb = GetComponent<Rigidbody2D>();
        lasthealth = maxhealth;
        animator = GetComponent<Animator>();
        animator.enabled = false;
	}

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale != 0)
        {
            if (Mathf.Abs(rb.velocity.x) < 0 && !charging && !attacking && !(animator.enabled && animator.GetCurrentAnimatorStateInfo(0).IsName("Pig_Charge")) && !(animator.enabled && animator.GetCurrentAnimatorStateInfo(0).IsName("Pig_Punch"))) //not walking attacking or charging, or finishing an attack or charge anim
            {
                animator.enabled = false;
            }
            else
            {
                animator.enabled = true;
            }
            if (currenthealth <= 0)
            {
                AudioSource.PlayClipAtPoint(deathsounds[Random.Range(0, deathsounds.Length - 1)], Camera.main.transform.position, 1);

                PlayerControls.points += 25;
                Destroy(gameObject);
            }
            CheckState();
            switch (currentstate)
            {
                case State.Charging:
                    if (!charging)
                    {
                        StopCoroutine(Charge());
                        StartCoroutine(Charge());
                    }
                    break;
                case State.Attacking:
                    if (attacking == false)
                    {
                        StopCoroutine(Attack());
                        StartCoroutine(Attack());
                    }
                    break;
                case State.Fleeing:
                    if (Player.transform.position.x < transform.position.x)
                    {
                        rb.velocity = new Vector2(speed, 0);
                    }
                    else
                    {
                        rb.velocity = new Vector2(-speed, 0);
                    }
                    break;
                default:
                    Wander(SpawnLocation, wanderrange);
                    break;
            }
            GetComponent<SpriteRenderer>().flipX = rb.velocity.x >= 0 ? false : true;
            if ((currentstate == State.Charging || currentstate == State.Attacking) && Player.transform.position.x < transform.position.x)
            {
                GetComponent<SpriteRenderer>().flipX = true;
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

    private void CheckState()
    {
        if (currentstate != State.Fleeing)
        {
            if (currenthealth == 1 && Vector2.Distance(transform.position, Player.transform.position) < 1f)
            {
                currentstate = State.Fleeing;
            }
            else if (Vector2.Distance(transform.position, Player.transform.position) < 0.225f && Mathf.Abs(transform.position.y - Player.transform.position.y) <= 0.15f && currentstate != State.Charging && currentstate != State.Fleeing) //stood next to me
            {
                currentstate = State.Attacking;
            }
            else if (Vector2.Distance(transform.position, Player.transform.position) < 1f && Mathf.Abs(transform.position.y - Player.transform.position.y) <= 0.15f) //nearby
            {
                currentstate = State.Charging;
            }
            else
            {
                currentstate = State.Idle;
            }
        }
        else
        {
            if (!ishealing)
            {
                StartCoroutine(Healing());
            }
            if (Mathf.Abs(transform.position.x - Player.transform.position.x) > 1.5)
            {
                SpawnLocation = transform.position;
                currentstate = State.Idle;
            }
        }
    }
    private void Wander(Vector2 spawnlocation, float range)
    {
        if (transform.position.x > spawnlocation.x + range)
        {
            goingright = false;
        }
        else if(transform.position.x < spawnlocation.x - range)
        {
            goingright = true;
        }
        if (goingright)
        {
            rb.velocity = new Vector2(speed, 0);
        }
        else
        {
            rb.velocity = new Vector2(-speed, 0);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.Contains("Brick"))
        {
            StopCoroutine(AttackedCD());
            StartCoroutine(AttackedCD());
            currenthealth -= 2;
            AudioSource.PlayClipAtPoint(Resources.Load("Sound Effects/Enemy Hit") as AudioClip, Camera.main.transform.position, 1);

            if (currentstate != State.Charging && currentstate != State.Attacking && Mathf.Abs(transform.position.x - Player.transform.position.x) >= 1f)
            {
                currentstate = State.Fleeing;
            }
        }
        if (collision.gameObject.name.Contains("Player"))
        {
            if (currentstate == State.Charging)
            {
                PlayerControls.DamagePlayer(2);
                currentstate = State.Attacking;
            }
        }
    }
    private IEnumerator Healing()
    {
        ishealing = true;
        while (currenthealth < maxhealth)
        {
            if ((currentstate == State.Fleeing || currentstate == State.Idle) && attackedrecently == false && currenthealth < maxhealth)
            {
                yield return new WaitForSeconds(1.5f);
            }
            yield return null;
        }
        ishealing = false;
    }
    internal IEnumerator AttackedCD()
    {
        attackedrecently = true;
        yield return new WaitForSeconds(4);
        attackedrecently = true;
    }
    private IEnumerator Attack()
    {
        attacking = true;
        animator.enabled = true;
        animator.SetTrigger("Punch");
        yield return new WaitForSeconds(1.15f + Random.Range(-0.15f, 0.15f));
        if (Mathf.Abs(Player.transform.position.x - transform.position.x)  < 0.2f && Mathf.Abs(transform.position.y - Player.transform.position.y) <= 0.15f)
        {
            PlayerControls.DamagePlayer(1);
        }
        yield return new WaitForSeconds(0.25f + Random.Range(-0.05f, 0.05f));
        attacking = false;
    }
    private IEnumerator Charge()
    {
        charging = true;
        yield return new WaitForSeconds(0.75f + Random.Range(-0.15f, 0.15f));
        animator.enabled = true;
        animator.SetTrigger("Charge");
        while (currentstate == State.Charging)
        {
            rb.velocity = new Vector2(Player.transform.position.x - transform.position.x, 0).normalized * speed * Random.Range(1.80f, 2.15f);
            yield return null;
        }
        charging = false;
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
