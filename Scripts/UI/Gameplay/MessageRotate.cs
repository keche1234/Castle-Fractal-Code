using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessageRotate : MonoBehaviour
{
    [SerializeField] protected Text messageText;
    [SerializeField] protected Color32 messageTextColor = Color.white;
    [SerializeField] protected List<string> messageList;
    protected int messageIndex = 0;
    
    [Header("Fading")]
    protected float fade = 1; // bounces between minFade and maxFade transparency (draws the clamped value)
    [Range(-10f, 0)]
    [SerializeField] protected float minFade = -0.25f;
    [Range(0, 10f)]
    [SerializeField] protected float maxFade = 4.5f;
    [Range(1, 10f)]
    [SerializeField] protected float baseFadeSpeed = 1f; //Multiply by -1 to switch direction
    protected float fadeSpeed;

    protected bool running = false;

    // Start is called before the first frame update
    void Start()
    {
        fadeSpeed = baseFadeSpeed;
        //messageText = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (running)
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
        //fade = startFade;
    }

    public void StopRotation()
    {
        running = false;
    }
}
