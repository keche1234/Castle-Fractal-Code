using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProtoRoomManager02 : RoomManager
{
    [SerializeField] protected TextMeshProUGUI timeText;
    [SerializeField] protected string timeTextColor;
    protected bool finalFloor = false;
    protected float roomTime = 0;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new void Update()
    {
        if (!current.RoomCleared())
        {
            roomTime += Time.deltaTime;
            int minutes = (int)(roomTime / 60);
            int seconds = (int)(roomTime % 60);
            timeText.text = "<mark=#" + timeTextColor + ">Time\n" + minutes + ":" + $"{seconds:D2}";
        }
    }

    public override void Step()
    {
        //if (level < 6)
        //{
        //    base.Step();
        //    player.TakeDamage(((int)player.GetCurrentHealth()) - 30);
        //    switch (level)
        //    {
        //        case 1:
        //            //Add appropriate powerup
        //            PickupCW sword = Instantiate(pickupPrefab, new Vector3(-0.5f, 0.75f, 0), Quaternion.Euler(0, 0, 0));
        //            sword.Initialize(0, 9, 0, 0, null, null);
        //            spawnManager.SetWaveInfo(0, 2);
        //            break;
        //        case 2:
        //            //Remove weapon
        //            player.RemoveCustomWeapon();

        //            //Add appropriate powerup
        //            PickupCW crossbow = Instantiate(pickupPrefab, new Vector3(-0.5f, 0.75f, 0), Quaternion.Euler(0, 0, 0));
        //            crossbow.Initialize(3, 3, 0, 0, null, null);
        //            spawnManager.SetWaveInfo(0, 2);
        //            break;
        //        case 3:
        //            //Break weapon
        //            player.RemoveCustomWeapon();

        //            //Add appropriate powerup
        //            PickupCW spear = Instantiate(pickupPrefab, new Vector3(-0.5f, 0.75f, 0), Quaternion.Euler(0, 0, 0));
        //            spear.Initialize(2, 6, 0, 0, null, null);
        //            spawnManager.SetWaveInfo(0, 2);
        //            break;
        //        case 4:
        //            //Remove weapon
        //            player.RemoveCustomWeapon();

        //            //Add appropriate powerup
        //            PickupCW axe = Instantiate(pickupPrefab, new Vector3(-0.5f, 0.75f, 0), Quaternion.Euler(0, 0, 0));
        //            axe.Initialize(1, 12, 0, 0, null, null);
        //            spawnManager.SetWaveInfo(0, 2);
        //            break;
        //        case 5:
        //            //Remove weapon
        //            player.RemoveCustomWeapon();

        //            //Add appropriate powerup
        //            PickupCW tome = Instantiate(pickupPrefab, new Vector3(-0.5f, 0.75f, 0), Quaternion.Euler(0, 0, 0));
        //            tome.Initialize(4, 15, 0, 0, null, null);
        //            spawnManager.SetWaveInfo(0, 2);
        //            break;
        //        case 6:
        //            finalFloor = true;
        //            player.RemoveCustomWeapon();
        //            PickupCW[] pickups = new PickupCW[5];
        //            for (int i = 0; i < pickups.Length; i++)
        //                pickups[i] = Instantiate(pickupPrefab, new Vector3(-4 + (2 * i), 0.75f, -2), Quaternion.Euler(0, 0, 0));

        //            pickups[0].Initialize(0, 9, 20, 20, null, null);
        //            pickups[1].Initialize(1, 12, 25, 25, null, null);
        //            pickups[2].Initialize(2, 6, 15, 15, null, null);
        //            pickups[3].Initialize(3, 3, 30, 30, null, null);
        //            pickups[4].Initialize(4, 15, 10, 10, null, null);
        //            spawnManager.SetWaveInfo(0, 5);
        //            break;
        //        default:
        //            break;
        //    }
        //}
    }

    public bool IsFinalFloor()
    {
        return finalFloor;
    }
}
