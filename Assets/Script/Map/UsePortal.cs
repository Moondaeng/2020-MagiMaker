using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsePortal : MonoBehaviour
{
    public Vector3 position;
    GameObject player;

    private void OnCollisionEnter(Collision coll)
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (coll.collider.tag == "Player")
        {
            Transform ParentTransform = coll.transform;
            while (true)
            {
                if (ParentTransform.parent == null)
                    break;
                else
                    ParentTransform = ParentTransform.parent;
            }

            Debug.Log("Position = " + ParentTransform.position);

            CGlobal.roomCount++;
            switch(this.tag)
            {             
                case "PORTAL":                                     
                    ParentTransform.position = new Vector3(0, 1, CConstants.ROOM_DISTANCE_Z * CGlobal.roomCount - CConstants.PORTAL_DISTANCE_Z);
                    break;

                case "RIGHT_PORTAL":                                   
                    ParentTransform.position = new Vector3(CConstants.PORTAL_DISTANCE_X, 1, CConstants.ROOM_DISTANCE_Z * CGlobal.roomCount - CConstants.PORTAL_DISTANCE_Z);
                    break;

                case "LEFT_PORTAL":
                    ParentTransform.position = new Vector3(-CConstants.PORTAL_DISTANCE_X, 1, CConstants.ROOM_DISTANCE_Z * CGlobal.roomCount - CConstants.PORTAL_DISTANCE_Z);
                    break;

            }

            CGlobal.usePortal = true; //포탈 사용 시 다음 맵 생성
            CGlobal.isClear = false; //포탈을 사용해서 새로운 방으로 왔으므로 방은 클리어되지 않은 상태
        }
    }
}
