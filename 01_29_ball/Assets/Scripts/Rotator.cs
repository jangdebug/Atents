using UnityEngine;
using System.Collections;

public class Rotator : MonoBehaviour
{

	// �� �������� �������ϱ� ����
	void Update()
	{
		// �� ��ũ��Ʈ�� ����� ���� ������Ʈ�� X������ 15��,
		// Y������ 30��, Z������ 45�� ȸ���ϰ� deltaTime ���� ���ϸ�
		// �������� �ƴ� �ʸ� �������� ȸ���մϴ�.
		transform.Rotate(new Vector3(15, 30, 45) * Time.deltaTime);
	}
}