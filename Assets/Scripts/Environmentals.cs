using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Environmentals : MonoBehaviour {

    private bool up = true;
    [SerializeField]
    Sprite openchest;
    [SerializeField]
    GameObject coin, health, maxhealth, signtext;
    [SerializeField]
    string signText;
    [SerializeField]
    bool endchest;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Contains("Player"))
        {
            switch (gameObject.name)
            {
                case "Ladder":
                    if (collision.gameObject.transform.position.y < transform.position.y)
                    {
                        up = true;
                    }
                    else
                    {
                        up = false;
                    }
                    if (gameObject.name == "Ladder" && collision.gameObject.name.Contains("Player"))
                    {
                        var velo = collision.gameObject.GetComponent<Rigidbody2D>().velocity;
                        if (up)
                        {
                            velo.y = 4;
                        }
                        else
                        {
                            velo.y = -4;
                        }
                        collision.gameObject.GetComponent<Rigidbody2D>().velocity = velo;
                    }
                    break;
                case "Chest":
                    if (collision.gameObject.name.Contains("Player") && GetComponent<SpriteRenderer>().sprite != openchest)
                    {
                        AudioSource.PlayClipAtPoint(Resources.Load("Sound Effects/Chest Open") as AudioClip, Camera.main.transform.position, 1);
                        GetComponent<SpriteRenderer>().sprite = openchest;
                        var loot = (int)((Random.value * 4) + 1);
                        for (int i = loot; i > 0; i--)
                        {
                            var chance = Random.value * 100;
                            GameObject newdrop;
                            if (chance < 80)
                            {
                                newdrop = Instantiate(coin, transform.position, Quaternion.identity);
                                newdrop.name = "Coin";
                            }
                            else if (chance < 95)
                            {
                                newdrop = Instantiate(health, transform.position, Quaternion.identity);
                                newdrop.name = "Health";
                            }
                            else
                            {
                                newdrop = Instantiate(maxhealth, transform.position, Quaternion.identity);
                                newdrop.name = "MaxHealth";
                            }
                            newdrop.GetComponent<Rigidbody2D>().velocity = new Vector2(Random.Range(-0.25f, 0.25f), Random.Range(0.75f, 1.75f));
                        }
                        if (endchest)
                        {
                            StartCoroutine(EndLevel());
                        }
                    }
                    break;
                case "Sign":
                    signtext.SetActive(true);
                    signtext.GetComponent<Text>().text = signText;
                    StopCoroutine(SignText());
                    StartCoroutine(SignText());
                    break;
            }
        }
    }
    private IEnumerator SignText()
    {
        yield return new WaitForSeconds(5);
        signtext.SetActive(false);
    }
    private IEnumerator EndLevel()
    {
        yield return new WaitForSeconds(5);
        if (SceneManager.GetActiveScene().name == "Tutorial Level")
        {
            SceneManager.LoadScene("Level 01");
        }
        else if (SceneManager.GetActiveScene().name == "Level 01")
        {
            SceneManager.LoadScene("Boss Fight");
        }
        else
        {
            SceneManager.LoadScene("Main Menu");
        }
    }
}
