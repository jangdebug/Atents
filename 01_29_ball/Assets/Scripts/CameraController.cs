using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;

    private Vector3 offset;

    // Start는 첫 프레임 업데이트 이전에 호출됩니다.
    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    // Update는 프레임마다 한 번씩 호출됩니다.
    void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}