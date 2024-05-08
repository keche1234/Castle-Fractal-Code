using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageRotate : MonoBehaviour
{
    [SerializeField] protected Text messageText;
    [SerializeField] protected List<string> messageList;
    protected int messageIndex = 0;
    protected Color32 messageTextColor = Color.white;

    protected float fade = 0; // bounces between minFade and maxFade transparency (draws the clamped value)
    protected float minFade = -0.25f;
    protected float maxFade = 4.5f;
    protected float fadeSpeed = 1f; //Multiply by -1 to switch direction

    protected bool running = true;

    // Start is called before the first frame update
    void Start()
    {
        //messageText = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (running && messageList.Count > 1)
        {
            fade += fadeSpeed * Time.deltaTime;

            if (fade <= minFade)
            {
                fadeSpeed = 1f;
                messageIndex = ++messageIndex % messageList.Count;
                messageText.text = messageList[messageIndex];
            }
            else if (fade >= maxFade)
            {
                fadeSpeed = -1f;
            }
        }

        messageText.color = new Color32(messageTextColor.r, messageTextColor.g, messageTextColor.b, (byte)(255 * Mathf.Clamp01(fade)));
    }

    public void SetMessageList(List<string> messages)
    {
        messageList = new List<string>(messages);
        fadeSpeed = -1f;
        fade = maxFade;

        if (messageList.Count > 0)
        {
            messageText.text = messageList[0];
            messageIndex = 0;
        }
        else
            messageIndex = -1;
    }

    public void RunRotation()
    {
        running = true;
    }

    public void StopRotation()
    {
        running = false;
    }
}
