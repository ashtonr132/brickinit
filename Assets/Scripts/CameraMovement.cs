using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraMovement : MonoBehaviour {

    [SerializeField]
    internal GameObject player;
    internal float cameraoffset = 0.35f, cameraspeed = 4;
	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update()
    {
        if (player != null && !SceneManager.GetActiveScene().name.Contains("Boss Fight"))
        {
            if (Vector3.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.transform.position.x, player.transform.position.y + cameraoffset)) > 0.01f)
            {
                transform.position = Vector3.Slerp(transform.position, new Vector3(player.transform.position.x + cameraoffset, player.transform.position.y + cameraoffset, transform.position.z), Time.deltaTime * cameraspeed);
            }
        }
    }
}
