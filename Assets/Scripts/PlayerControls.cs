using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerControls : MonoBehaviour
{
    private float acceleration = 0.25f, maxspeed = 1f, jumppower = 0.1f, jumpbuffer = 0.1f, waittime, firecd = 1, projspeed = 3;
    private Rigidbody2D rb;
    private Collider2D col;
    private bool isGrounded = false, facingRight = true, jumpbuffered = true;
    internal static bool invincible = false, firebuffered = true, go = false;
    internal SpriteRenderer spriterenderer;
    private RaycastHit2D hit;
    private int lasthealth;
    internal static int currenthealth, points, lives, maxhealth = 10;
    private string waitid;
    internal static GameObject LastBrick;
    private Vector2 spawnpos;
    private Animator animator;
    [SerializeField]
    internal Sprite[] jump;    
    [SerializeField]
    GameObject Brick, PlayerLives, Points, ExitBut, GameOverHolder;
    [SerializeField]
    AudioClip[] deathsounds;


    static PlayerControls instance;
    private void Awake()
    {
        instance = this;
    }
    // Use this for initialization
    void Start()
    {
        lives = 3;
        points = 1000;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        spriterenderer = GetComponent<SpriteRenderer>();
        currenthealth = maxhealth;
        lasthealth = currenthealth;
        StartCoroutine(Point());
        spawnpos = transform.position;
        Points.GetComponent<Text>().text = points.ToString();
        animator = GetComponent<Animator>();
        animator.enabled = false;
        firebuffered = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!go)
        {
            Points.GetComponent<Text>().text = points.ToString();
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (ExitBut.activeSelf)
                {
                    Time.timeScale = Time.timeScale == 0 ? 1 : 0;
                    AudioSource.PlayClipAtPoint(Resources.Load("Sound Effects/Unpause") as AudioClip, Camera.main.transform.position, 1);
                    ExitBut.SetActive(false);
                }
                else
                {
                    AudioSource.PlayClipAtPoint(Resources.Load("Sound Effects/Pause") as AudioClip, Camera.main.transform.position, 1);
                    Time.timeScale = Time.timeScale == 0 ? 1 : 0;
                    ExitBut.SetActive(true);
                }

            }
            if (Time.timeScale != 0)
            {
                if ((isGrounded && Mathf.Abs(rb.velocity.x) > 0.05f) || (animator.enabled && animator.GetCurrentAnimatorStateInfo(0).IsName("CH_Throw")))
                {
                    animator.enabled = true;
                }
                else if (!isGrounded || (isGrounded && Mathf.Abs(rb.velocity.x) < 0.05f))
                {
                    animator.enabled = false;
                }
                if (PlayerLives.activeSelf)
                {
                    PlayerLives.transform.position = transform.GetChild(1).position;
                }
                if (currenthealth <= 0)
                {
                    points -= 100;
                    if (lives > 1)
                    {
                        AudioSource.PlayClipAtPoint(deathsounds[Random.Range(0, deathsounds.Length - 1)], Camera.main.transform.position, 1);
                        lives--;
                        currenthealth = maxhealth;
                        transform.position = spawnpos;
                        PlayerLives.GetComponent<Text>().text = lives.ToString();
                        StartCoroutine(Buffer("invincible", 0.5f));
                    }
                    else
                    {
                        StartCoroutine(GameOver());
                    }
                }
                if (!isGrounded && jumpbuffered) //has the player jumped recently?
                {
                    hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - col.bounds.extents.y * 1.25f), transform.up, col.bounds.extents.y);
                    if (hit.transform.gameObject != null && hit.transform.gameObject.CompareTag("Floor")) //floor is beneath player
                    {
                        isGrounded = true;
                    }
                }
                if (Input.GetKey(KeyCode.A))
                {
                    facingRight = false;
                    rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x - acceleration, -maxspeed, maxspeed), rb.velocity.y);
                }
                else if (Input.GetKey(KeyCode.D))
                {
                    facingRight = true;
                    rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x + acceleration, -maxspeed, maxspeed), rb.velocity.y);
                }
                if (Input.GetKeyDown(KeyCode.Space) && isGrounded && jumpbuffered) //player wants to jump, is grounded and hasnt just jumped (buffer stops jumping during jump takeoff)
                {
                    AudioSource.PlayClipAtPoint(Resources.Load("Sound Effects/Player Jump") as AudioClip, Camera.main.transform.position, 1);

                    rb.AddForce(new Vector2(Mathf.Clamp(rb.velocity.x, -0.5f, 0.5f), 1) * (Mathf.Sqrt(2 * Physics2D.gravity.magnitude * jumppower)) * rb.mass, ForceMode2D.Impulse); // jump up but also in the x direction travelling clamped at half max speed
                    rb.AddForce(new Vector2(Mathf.Clamp(rb.velocity.x, -0.5f, 0.5f), 1) * (Mathf.Sqrt(2 * Physics2D.gravity.magnitude * jumppower)) * rb.mass, ForceMode2D.Impulse); // jump up but also in the x direction travelling clamped at half max speed
                    isGrounded = false;
                    spriterenderer.sprite = jump[1]; //jump launch sprite
                    StartCoroutine(Buffer("jump", jumpbuffer));
                    StartCoroutine(Buffer("jumpanim", 0.05f));

                }
                else if (Input.GetKeyUp(KeyCode.Space) && !isGrounded) //shorten jump if space key not held
                {

                    if (Vector2.Dot(rb.velocity, Vector2.up) > 0)
                    {
                        rb.AddForce(-Vector2.up * Physics2D.gravity.magnitude * 5 * rb.mass);
                    }
                }
                if (Input.GetKeyDown(KeyCode.Mouse0) && firebuffered)
                {
                    animator.enabled = true;
                    animator.SetTrigger("Throw");
                    StartCoroutine(Buffer("fire", firecd));
                    GameObject newbrick = Instantiate(Brick, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
                    LastBrick = newbrick;
                    Physics2D.IgnoreCollision(newbrick.GetComponent<BoxCollider2D>(), col);
                    newbrick.GetComponent<Rigidbody2D>().velocity = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - newbrick.transform.position) * projspeed;
                    newbrick.GetComponent<Rigidbody2D>().angularVelocity = Random.Range(-1080f, 1080f);
                }
                spriterenderer.flipX = facingRight ? false : true;
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
    }
    internal IEnumerator GameOver()
    {
        go = true; GameOverHolder.SetActive(true);
        if (gameObject != null)
        {
            Destroy(GetComponent<SpriteRenderer>());
            Destroy(GetComponent<Animator>());
        }
        yield return new WaitForSeconds(5);
        currenthealth = maxhealth; points = 1000; lives = 3; maxhealth = 10;
        invincible = false; firebuffered = true; go = false;
        SceneManager.LoadScene("Main Menu");
    }
    internal IEnumerator DisplayHPBar()
    {
        if (!go)
        {
            for (int i = 0; i < currenthealth; i++)
            {
                transform.GetChild(0).GetChild(i).gameObject.SetActive(true);
            }
            PlayerLives.SetActive(true);
            yield return new WaitForSeconds(5);
            foreach (Transform t in transform.GetChild(0))
            {
                t.gameObject.SetActive(false);
            }
            PlayerLives.SetActive(false);
        }
    }
    internal IEnumerator Buffer(string waitid, float waittime)
    {
        if (!go)
        {
            if (string.Equals(waitid, "jump"))//start jump anim but dont let jump again right away
            {
                jumpbuffered = false;
                yield return new WaitForSeconds(waittime);
                jumpbuffered = true;
            }
            else if (string.Equals(waitid, "fire"))//start fire 
            {
                firebuffered = false;
                yield return new WaitForSeconds(waittime);
                firebuffered = true;
            }
            else if (string.Equals(waitid, "invincible"))//start invincible
            {
                invincible = true;
                yield return new WaitForSeconds(waittime);
                invincible = false;
            }
            else if (string.Equals(waitid, "jumpanim"))
            {
                yield return new WaitForSeconds(waittime);
                while (rb.velocity.y > 0) //player is moving up during jump
                {
                    spriterenderer.sprite = jump[2];
                    yield return null;
                }
                while (rb.velocity.y < 0) //player is moving down in jump
                {
                    spriterenderer.sprite = jump[3];
                    yield return null;
                }
                spriterenderer.sprite = jump[4];
                yield return new WaitForSeconds(waittime);
                spriterenderer.sprite = jump[0];
            }
        }
    }
    internal static void DamagePlayer(int damage)
    {
        if (!go)
        {
            if (!invincible)
            {
                AudioSource.PlayClipAtPoint(Resources.Load("Sound Effects/Player Damage") as AudioClip, Camera.main.transform.position, 1);
                currenthealth -= damage;
                instance.StartCoroutine(instance.Buffer("invincible", 0.5f));
            }
        }
    }
    private IEnumerator Point()
    {
        while (gameObject != null && go == false)
        {
            yield return new WaitForSeconds(1);
            points--;
            yield return null;
        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!go)
        {
            if (collision.gameObject.name.Contains("Spike"))
            {
                DamagePlayer(1);
                rb.velocity += (Vector2)(transform.position - collision.gameObject.transform.position).normalized / 3;
            }
        }
    }
}
