using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;

    private Vector3 offset;

    // Start�� ù ������ ������Ʈ ������ ȣ��˴ϴ�.
    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    // Update�� �����Ӹ��� �� ���� ȣ��˴ϴ�.
    void LateUpdate()
    {
        transform.position = player.transform.position + offset;
    }
}