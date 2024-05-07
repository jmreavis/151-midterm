using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrowningTimer : MonoBehaviour
{
    [SerializeField]
    private Slider slider;
    [SerializeField]
    private float maxTime = 5;
    private float timer = 0;
    private bool isUnderwater = false;

    void Start()
    {
        slider.maxValue = maxTime;
        OSCHandler.Instance.SendMessageToClient("pd", "/unity/triangle_vol", 0);
    }

    void Update()
    {
        // slider should follow sonic
        slider.transform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, 1, 0));
        slider.value = maxTime - timer;

        // slider should show up if underwater
        slider.gameObject.SetActive(isUnderwater);
        // timer should count if underwater
        if (isUnderwater)
        {
            timer += Time.deltaTime;
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/test3", 500 - (timer * 100)); //i might need to adjust timer value so metronome val changes quicker?
            if (timer > maxTime)
            {
                // Death
                OSCHandler.Instance.SendMessageToClient("pd", "/unity/sequence_vol", 0);
                OSCHandler.Instance.SendMessageToClient("pd", "/unity/triangle_vol", 820);
                OSCHandler.Instance.SendMessageToClient("pd", "/unity/pitch_vol", .2f);

                gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            isUnderwater = true;
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/test2", 1); //turn on metronome
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/noise_burst", 1);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            isUnderwater = false;
            timer = 0;
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/test2", 0);
            OSCHandler.Instance.SendMessageToClient("pd", "/unity/test3", timer);
        }
    }
}