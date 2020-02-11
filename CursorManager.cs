using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField]
    RectTransform CursorTransform;

    [SerializeField]
    GameObject Stamp;

    const int size = 30;
    const int horizontalLimit = 20;
    const int verticalLimit = 10;

    bool[,] hasStamp = new bool[horizontalLimit , verticalLimit ];

    float timeGap = 0.2f;
    float elapsedTime = 0;

    void MoveCursor(KeyCode keycode, Vector3 offset)
    {
        if (Input.GetKey(keycode))
        {
            if (Input.GetKeyDown(keycode)) //키가 한 번 눌리면
            {
                elapsedTime = 0;
                CursorTransform.Translate(offset, Space.Self); //커서를 이동
            }
            else
            {
                elapsedTime += Time.deltaTime; //델타타임(프레임 사이의 시간?)을 누적더하기
                if (timeGap <= elapsedTime) //타임 갭을 넘어서면
                {
                    CursorTransform.Translate(offset, Space.Self); //커서를 한번 더 이동하고
                    elapsedTime = 0; //초기화
                }
            }
        }
    }
    void MakeStamp(GameObject stamp)
    {
        int column = (int)CursorTransform.localPosition.x / size;
        int row = (int)CursorTransform.localPosition.y / size;
        if (!hasStamp[column, row])
        {
            GameObject STAMP = GameObject.Instantiate(stamp, CursorTransform.parent); //Stamp의 타입이 먼지 모르겠음
            STAMP.SetActive(true);
            STAMP.GetComponent<RectTransform>().localPosition = CursorTransform.localPosition;
            hasStamp[column, row] = true;
        }

    }

    void Update()
    {
        if (size <= CursorTransform.localPosition.x)
        {
            MoveCursor(KeyCode.LeftArrow, new Vector3(-size, 0, 0));
        }

        if (CursorTransform.localPosition.x < (size * (horizontalLimit-1)))                
        {
            MoveCursor(KeyCode.RightArrow, new Vector3(size, 0, 0));
        }

        if (CursorTransform.localPosition.y < (size * (verticalLimit-1)))
        {
            MoveCursor(KeyCode.UpArrow, new Vector3(0, size, 0));
        }

        if ((size <= CursorTransform.localPosition.y))
        {
            MoveCursor(KeyCode.DownArrow, new Vector3(0, -size, 0));
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            MakeStamp(Stamp);
        }
    }
}
