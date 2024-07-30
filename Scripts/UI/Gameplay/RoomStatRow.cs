using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomStatRow : MonoBehaviour
{
    [Header("Room Selection")]
    [SerializeField] protected List<int> roomList;
    [SerializeField] protected int currentRoomIndex;
    [SerializeField] protected RoomManager roomManager; //where you'll get stats from

    [Header("Text")]
    [SerializeField] protected Text roomName;
    [SerializeField] protected Text timeText;
    [SerializeField] protected Text damageTaken;
    [SerializeField] protected Text fallsText;
    [SerializeField] protected Text sigPtsText;
    [SerializeField] protected Text sigMovesText;
    [SerializeField] protected Text potionsText;

    [Header("Arrows")]
    [SerializeField] protected Button leftArrow;
    [SerializeField] protected Button rightArrow;
    [SerializeField] protected Color filledColor; //if there's another page for the arrow
    [SerializeField] protected Color emptyColor; //if there's no other page for the arrow
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //DrawRow();
    }

    //public void StepLeft()
    //{
    //    if (currentRoomIndex <= 0)
    //        return;

    //    currentRoomIndex--;
    //    DrawRow();
    //}

    //public void StepRight()
    //{
    //    if (currentRoomIndex >= roomList.Count - 1)
    //        return;

    //    currentRoomIndex++;
    //    DrawRow();
    //}

    //public void DrawRow()
    //{
    //    if (currentRoomIndex < 0 || currentRoomIndex > roomList.Count - 1)
    //        return;

    //    int myRoom = roomList[currentRoomIndex];

    //    roomName.text = "Room " + myRoom;
    //    timeText.text = roomManager.GetRoomTime(myRoom).ToString("0") + "s";
    //    damageTaken.text = roomManager.GetDamageTaken(myRoom).ToString("0");
    //    fallsText.text = roomManager.GetFalls(myRoom).ToString("0");
    //    potionsText.text = roomManager.GetPotionsUsed(myRoom).ToString("0");
    //    sigPtsText.text = roomManager.GetSignaturePointsGained(myRoom).ToString("0");
    //    sigMovesText.text = roomManager.GetSignatureMovesUsed(myRoom).ToString("0");

    //    if (leftArrow != null)
    //    {
    //        if (currentRoomIndex == 0)
    //        {
    //            ButtonColorManipulation leftArrowManip = leftArrow.GetComponent<ButtonColorManipulation>();
    //            if (leftArrowManip != null)
    //                leftArrowManip.ActivateColorManipulation(false);

    //            Image leftArrowImage = leftArrow.GetComponent<Image>();
    //            if (leftArrowImage != null)
    //                leftArrow.GetComponent<Image>().color = emptyColor;

    //            leftArrow.gameObject.GetComponent<Button>().enabled = false;
    //        }
    //        else
    //        {
    //            leftArrow.gameObject.GetComponent<Button>().enabled = true;

    //            ButtonColorManipulation leftArrowManip = leftArrow.GetComponent<ButtonColorManipulation>();
    //            if (leftArrowManip != null)
    //                leftArrowManip.ActivateColorManipulation(true);

    //            Image leftArrowImage = leftArrow.GetComponent<Image>();
    //            if (leftArrowImage != null)
    //                leftArrowImage.color = Color.white;
    //        }
    //    }

    //    if (rightArrow != null)
    //    {
    //        if (currentRoomIndex == roomList.Count - 1)
    //        {
    //            ButtonColorManipulation rightArrowManip = rightArrow.GetComponent<ButtonColorManipulation>();
    //            if (rightArrowManip != null)
    //                rightArrowManip.ActivateColorManipulation(false);

    //            Image rightArrowImage = rightArrow.GetComponent<Image>();
    //            if (rightArrowImage != null)
    //                rightArrowImage.color = emptyColor;

    //            rightArrow.gameObject.GetComponent<Button>().enabled = false;
    //        }
    //        else
    //        {
    //            rightArrow.gameObject.GetComponent<Button>().enabled = true;

    //            ButtonColorManipulation rightArrowManip = rightArrow.GetComponent<ButtonColorManipulation>();
    //            if (rightArrowManip != null)
    //                rightArrowManip.ActivateColorManipulation(true);

    //            Image rightArrowImage = rightArrow.GetComponent<Image>();
    //            if (rightArrowImage != null)
    //                rightArrowImage.color = Color.white;
    //        }
    //    }
    //}
}
