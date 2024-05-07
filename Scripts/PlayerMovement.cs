using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityOSC;
using UnityEditor;
using Unity.VisualScripting;

public class PlayerMovement : MonoBehaviour

{
    [SerializeField]
    private float speed;
    [SerializeField]
    private float jumpSpeed;

    private Rigidbody2D rb2d;
    float beat = 1.2f;

    void Start()
    {
        Application.runInBackground = true;


        OSCHandler.Instance.Init();
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/sequence_metro", 1);
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/sequence_float", beat);
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/sequence_vol", 0.1f);
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/stop_DSP", 1);
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/footstep", 0);
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/footstep_vol", 0);
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/jump", 0);
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/jump_vol", 0);
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");

        rb2d.velocity = new Vector2(horizontal * speed, rb2d.velocity.y);

        if (Input.GetKey(KeyCode.Space))
        {
            rb2d.velocity = new Vector2(rb2d.velocity.x, jumpSpeed);
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/jump", 180);
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/jump_vol", 0.2f);
        }

        if(Input.GetKeyUp(KeyCode.Space))
        {
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/jump_vol", 0);
        }

        

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        {
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/footstep", 3);
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/footstep_vol", 0.5f);
        }

        if(Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D))
        {
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/footstep", 0);
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/footstep_vol", 0);
        }
    }
}

[InitializeOnLoad]
public static class PlayModeStateListener
{
    static PlayModeStateListener()
    {
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/stop_DSP", 0);
            Debug.Log("Exiting play mode");
        }
    }
}
