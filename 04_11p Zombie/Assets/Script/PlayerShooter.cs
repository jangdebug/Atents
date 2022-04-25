using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] private Gun gun;
    private Animator anim;

    private PlayerInput playerInput;


    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        if (playerInput && gun)
        {
            if (playerInput.fire) gun.Fire(); // �Ѿ� �߻�.
            if (playerInput.reload && gun.Reload() && anim) anim.SetTrigger("Reload"); 
            // ������ ���� Ȯ�� ��, ������ �ִϸ��̼� ���.
        }
        if (gun && UIMgr.Instance) UIMgr.Instance.UpdateAmmoText(gun.magAmmo, gun.ammoRemain);
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (gun && anim)
        {
            // �ش� ������Ʈ(Gun)�� pivot�� �ش� �ִϸ��̼�(upper body)�� ������ �Ȳ�ġ ��ġ�� �̵�.
            gun.Pivot = anim.GetIKHintPosition(AvatarIKHint.RightElbow);
            // �޼��� position, rotation�� �ش� ������Ʈ(Gun)�� ���� ������ ��ġ�� �����.
            anim.SetIKPosition(AvatarIKGoal.LeftHand, gun.LeftHandMountPos);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, gun.LeftHandMountRo);
            // ����ġ(weight)�� �߰��Ͽ� ��ġ, ȸ���� �̼����� �Ѵ�.
            anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, gun.LeftHandPosWeight);
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, gun.LeftHandRoWeight);
            // �������� position, rotation�� �ش� ������Ʈ(Gun)�� ������ ������ ��ġ�� �����.
            anim.SetIKPosition(AvatarIKGoal.RightHand, gun.RightHandMountPos);
            anim.SetIKRotation(AvatarIKGoal.RightHand, gun.RightHandMountRo);
            // ����ġ(weight)�� �߰��Ͽ� ��ġ, ȸ���� �̼����� �Ѵ�.
            anim.SetIKPositionWeight(AvatarIKGoal.RightHand, gun.RightHandPosWeight);
            anim.SetIKRotationWeight(AvatarIKGoal.RightHand, gun.RightHandRoWeight);
        }
    }


    public void AddAmmo(int value)
    {
        if (gun) gun.AddAmmo(value);
    }

}
