//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using TMPro;

//public class HorizontalInputShow : MonoBehaviour
//{
//    TMP_Text text;
//    int horizontalMoveLastFrame = 0;

//    // Start is called before the first frame update
//    void Start()
//    {
//        text = GetComponent<TMP_Text>();
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        text.text = "按键：";
//        if (Input.GetKey(KeyCode.A))
//        {
//            text.text += " A";
//            if (Input.GetKey(KeyCode.D))
//            {
//                text.text += ", D";
//            }
//        }
//        else if (Input.GetKey(KeyCode.D))
//        {
//            text.text += " D";
//        }

//        if (Input.GetKeyDown(KeyCode.A))
//        {
//            horizontalMoveLastFrame = -1;
//        }
//        if (Input.GetKeyUp(KeyCode.A))
//        {
//            if (horizontalMoveLastFrame == -1)
//            {
//                if (!Input.GetKey(KeyCode.D))
//                    horizontalMoveLastFrame = 0;
//                else
//                    horizontalMoveLastFrame = 1;
//            }
//        }
//        if (Input.GetKeyDown(KeyCode.D))
//        {
//            horizontalMoveLastFrame = 1;
//        }
//        if (Input.GetKeyUp(KeyCode.D))
//        {
//            if (horizontalMoveLastFrame == 1)
//            {
//                if (!Input.GetKey(KeyCode.A))
//                    horizontalMoveLastFrame = 0;
//                else
//                    horizontalMoveLastFrame = -1;
//            }
//        }

//        text.text += "\n 水平输入：" + horizontalMoveLastFrame;

//    }
//}
