using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField]
    RectTransform CursorTransform;

    [SerializeField]
    GameObject Stamp;

    [SerializeField]
    Recipe tempRecipe;

    const int size = 30;
    const int horizontalLimit = 20;
    const int verticalLimit = 10;

   BlockType[,] hasStamp = new BlockType[horizontalLimit, verticalLimit]; //bool에 리스트가 삽입된거임? 이걸 enum으로 어케 바꾸지? //헐 string 됨 //초기값을 Blank로 두고 싶은데

    
    float timeGap = 0.2f;
    float elapsedTime = 0;

    //private void Start()
    //{
    //    for (int i = 0; i < horizontalLimit; i++)
    //    {
    //        for (int j = 0; j < verticalLimit; j++)
    //        {
    //            hasStamp[i, j] = BlockType.Blank; //이거 enum BlockIndex[0] 이라고 쓰면 왜 안되지?
    //        }

    //    }
    //    RecipeCheck(0,0);
    //}

    void MoveCursor(KeyCode keycode, Vector3 offset)
    {
        if (Input.GetKey(keycode))
        {
            if (Input.GetKeyDown(keycode)) //키가 한 번 눌리면
            {
                elapsedTime = 0;
                CursorTransform.Translate(offset, Space.Self); //생각해보니까 이거 무슨뜻이지?
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
    void MakeStamp()
    {
        int column = (int)CursorTransform.localPosition.x / size;
        int row = (int)CursorTransform.localPosition.y / size;
        if (hasStamp[column, row]==BlockType.Blank) 
        {
            GameObject stamp = GameObject.Instantiate(Stamp, CursorTransform.parent);
            stamp.SetActive(true);
            stamp.GetComponent<RectTransform>().localPosition = CursorTransform.localPosition;
            hasStamp[column, row] = BlockType.Filled;
            RecipeCheck();
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
            MakeStamp();
        }
    }

    void RecipeCheck()
    {
        //string CurrentLocation = hasStamp[column, row];
        //string CurrentRecipe = (string)tempRecipe.Blocks[(column + (row * 2) % 4)]; 이거 외않돼
        for (int column = 0 ; column < 2; column++)
        {
            for(int row=0; row < 2; row++)
            {
                if(hasStamp[column, row] != tempRecipe.Blocks[(column + (row * 2) % 4)])
                {
                    Debug.Log("틀렸습니다");
                    return;
                }
            }
        }
        Debug.Log("사과입니다");

    }
}
