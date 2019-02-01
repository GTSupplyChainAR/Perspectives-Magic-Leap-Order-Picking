using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;


public class CameraController : MonoBehaviour {
    public float speed = 0.3f;
   
    void Start () {
        transform.position = new Vector3(-0.85f, -0.2f, 1.5f);
    }

    // Update is called once per frame
    void FixedUpdate () {

        //Fast Move
        //Tweak and no tweak
        // if (Input.GetKeyDown(KeyCode.Joystick1Button1) || Input.GetKeyDown(KeyCode.Joystick2Button1))
        //if (Input.GetKeyDown(KeyCode.Joystick1Button2) || Input.GetKeyDown(KeyCode.Joystick2Button2))

        if (Input.GetKey(KeyCode.Alpha1)){ //Middle
            transform.position = new Vector3(-0.85f, -0.2f, 1.5f);
        } else if (Input.GetKey(KeyCode.Alpha2)) { //Middle Right
            transform.position = new Vector3(-1.6f, -0.2f, 1.5f);
        } else if (Input.GetKey(KeyCode.Alpha3)){ //Lower Middle
            transform.position = new Vector3(-0.85f, 0.1f, 1.5f);
        } else if (Input.GetKey(KeyCode.Alpha4)) { //Lower Right
            transform.position = new Vector3(-1.6f, 0.1f, 1.5f);

        }

        //Tweak
        if (Input.GetKey(KeyCode.LeftArrow)){
            transform.position += Vector3.right  * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow)){
            transform.position += Vector3.left * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.UpArrow)){
            transform.position += Vector3.down * speed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow)){
            transform.position += Vector3.up * speed * Time.deltaTime;
        }
    } 
}
