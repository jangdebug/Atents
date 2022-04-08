using System.Collections;
using System.Collections.Generic;
using UnityEngine;


struct GroundData
{
    public float xPos;
    public float width;
}

public class SpriteScroll : MonoBehaviour
{
    [SerializeField] float scrollSpeed = 5f;

    SpriteRenderer background;
    Vector2 offset = Vector2.zero;


    [SerializeField] SpriteRenderer[] grounds;
    [SerializeField] float bounds = 5f;
    GroundData[] groundDatas;
    float halfWidth = 0;
    float prePosX = 0;



    // Start is called before the first frame update
    private void Start()
    {
        var worldScreenHeight = Camera.main.orthographicSize * 2.0f;
        //y의 양수부분만 orthographicSize  => *2해서 음수부분까지
        var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;
        background = GetComponent<SpriteRenderer>();
        if (background)
        {
            background.drawMode = SpriteDrawMode.Tiled; 
            // tiled    : 게임 내의 리소스를 반복해서 사용
            // sliced   : 게임 내의 리소스의 특정 구간을 지정하고, 지정 부분만 크기 조절가능하게
            var size = background.size;
            size.x = worldScreenWidth;
            background.size = size;
        }

        halfWidth = worldScreenWidth * 0.5f;
        var count = grounds.Length;
        if (1 < count)
        {
            groundDatas = new GroundData[count];
            for (int i = 0; count > i; i++)
            {
                groundDatas[i].width = grounds[i].size.x;
                // 두 번째 지형부터 앞의 지형 위치와 설정 값들을 참조하여 시작 위치를 변경한다.
                if (0 < i)
                {
                    groundDatas[i].xPos = groundDatas[i - 1].xPos + bounds + groundDatas[i].width;
                    grounds[i].transform.position = Vector3.right * groundDatas[i].xPos + Vector3.down;
                }
                else groundDatas[i].xPos = grounds[i].transform.position.x;
            }
            prePosX = groundDatas[count - 1].xPos;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GameMgr.Instance.isDead) return;

        if (background)
        {
            offset.x = Mathf.Repeat(Time.time * scrollSpeed * 0.01f, 1);
            background.material.mainTextureOffset = offset;
        }

        var count = grounds.Length;
        if (1 < count)
        {
            for (int i = 0; count > i; i++)
            {
                groundDatas[i].xPos -= Time.deltaTime * scrollSpeed;
                // 오브젝트가 카메라 영역을 벗어나면 해당 오브젝트를 제일 오른쪽 위치로 옮긴다.
                // 이동 위치는 가장 오른쪽에 위치한 오브젝트의 x좌표 + 자신의 너비 + 각 오브젝트간의 거리.
                if (-halfWidth >= groundDatas[i].xPos)
                {
                    groundDatas[i].xPos = prePosX + bounds + groundDatas[i].width;
                }
                // 지형의 위치가 (x, -1, 0)이기 때문에 Vector3.down을 더한다.
                grounds[i].transform.position = Vector3.right * groundDatas[i].xPos + Vector3.down;
                // 카메라의 영역을 벗어날 경우 가장 오른쪽에 위치한 오브젝트의 위치를 저장하여 둔다.
                // 자신의 앞에 위치했던 지형이 가장 오른쪽에 위치하게 되는 지형과 같다.
                prePosX = groundDatas[i].xPos;
            }
        }

    }
}
