﻿using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public float m_speed = 1.0f;

    [Header("SelectKey")]
    public KeyCode m_key1 = KeyCode.Alpha1;
    public KeyCode m_key2 = KeyCode.Alpha2;
    public KeyCode m_key3 = KeyCode.Alpha3;
    public KeyCode m_key4 = KeyCode.Alpha4;

    [Header("ChangeCameraKey")]
    public KeyCode m_keyF5 = KeyCode.F5;

    [Header("HealKey")]
    public KeyCode m_keyF = KeyCode.F;

    [Header("GetKey")]
    public KeyCode m_keyE = KeyCode.E;

    [Header("JumpKey")]
    public KeyCode m_keySpace = KeyCode.Space;

    [HideInInspector]
    private CharacterController m_cc;

    private float m_movementY = 0.0f;//점프구현을 위해 Y방향은 따로 
    private Vector3 m_movement;//캐릭터 Transform 기준 움직이려는 방향
    private Vector3 m_movePosition;//World Transform 기준 움직이게 하려는 방향

    string InputH = "Horizontal";
    string InputV = "Vertical";

    private float m_gravity = -9.8f;

    bool[] m_numKeyDown = new bool[4];
    bool m_jumped = false;
    bool m_f5 = false;

    private void Start()
    {
        m_cc = GetComponent<CharacterController>();
    }

    //캐릭터의 이동거리를 리턴
    public float GetMagnitude()
    {
        return m_movement.magnitude;
    }

    //캐릭터가 이동하고 있는 방향을 리턴
    public float GetDirection()
    {
        return GetMagnitude() > 0 ? Mathf.Atan2(m_movePosition.x, m_movePosition.z) * Mathf.Rad2Deg : 0;
    }


    //점프 체크
    public bool IsJumped()
    {
        if (m_jumped)
        {
            m_jumped = false;
            return true;
        }

        return false;
    }

    public bool IsF5()
    {
        m_f5 = Input.GetKeyDown(m_keyF5);

        if (m_f5)
        {
            m_f5 = false;

            return true;
        }

        return false;
    }

    //캐릭터와 땅과의 거리를 리턴
    public float GetGroundDistance()
    {
        RaycastHit hit;
        float dis = 0.0f;
        Ray ray = new Ray(transform.position, Vector3.down);//캐릭터 위치에서 y -1방향으로 레이 생성

        if (Physics.Raycast(ray.origin, ray.direction, out hit, 10))
            dis = ray.origin.y - hit.point.y;

        return dis;
    }

    //캐릭터가 땅에 닿아있는지 확인
    public bool IsGrounded()
    {
        return m_cc.isGrounded;
    }

    //캐릭터를 회전
    public void Rotate(Quaternion angle)
    {
        transform.rotation = angle;
    }

    //캐릭터가 이동하고 있는 방향을 리턴
    public int GetNumKey()
    {
        m_numKeyDown[0] = Input.GetKeyDown(m_key1);
        m_numKeyDown[1] = Input.GetKeyDown(m_key2);
        m_numKeyDown[2] = Input.GetKeyDown(m_key3);
        m_numKeyDown[3] = Input.GetKeyDown(m_key4);

        for(int i = 0; i< 4; i++)
        {
            if (m_numKeyDown[i])
            {
                return i + 1;
            }
        }

        return 0;
    }

    //캐릭터의 움직임(앞뒤좌우)을 업데이트함
    public void UpdateInput()
    {
        m_movement.x = Input.GetAxis(InputH) * m_speed;
        m_movement.z = Input.GetAxis(InputV) * m_speed;
        m_movementY += IsGrounded() ? 0 : (m_gravity * Time.deltaTime);//땅에 안닿았을 경우, 중력 받기

        m_jumped = m_cc.isGrounded ? Input.GetKeyDown(m_keySpace) : false;//점프키를 눌렀을 때 점프
        if (m_jumped)
        {
            m_movementY = 3.0f;//점프력 4
        }

        m_movePosition.Set(m_movement.x, m_movementY, m_movement.z);
        m_cc.Move(transform.rotation * m_movePosition * Time.deltaTime);
    }

}
