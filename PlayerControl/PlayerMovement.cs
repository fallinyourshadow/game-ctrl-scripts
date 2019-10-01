using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    
    public float turnSpeed = 20f;//角速度为20每秒，用于计算每帧的转角变化
    Vector3 m_Movement;//声明Vector3对象，它代表一个三维的向量，用记录玩家期望的朝向
    Quaternion m_Rotation = Quaternion.identity;//声明一个四元数对象m_Rotation，并初始化为一个代表无旋转的值,用于记录旋转

    Animator m_Animator;//声明一个Animator对象，为Animator组件所用
    Rigidbody m_Rigidbody;//声明一个Rigidbody对象，为Rigidbody组件所用

    // Start is called before the first frame update
    void Start()
    {
        m_Animator = GetComponent<Animator>();//关联Animator组件，<>括号内是类型参数用于查询Animator组件
        m_Rigidbody = GetComponent<Rigidbody>();//关联Animator组件
    }
    // 每帧调用一次
    void FixedUpdate()
    {
        /*获得水平轴和垂直轴的矢量值*/
        float horizontal = Input.GetAxis("Horizontal");//获得水平轴的值 AD
        float vertical = Input.GetAxis("Vertical");//获得垂直轴的值 WS
        /*合成目标向量*/
        m_Movement.Set(horizontal, 0f, vertical);//设置移动向量的值，该向量由三个坐标轴的值构成，竖直值轴的值应该为0，0f代表该数为浮点数类型，因为John不会向上或向下移动
        m_Movement.Normalize();//规范化该向量，使其在任何方向的大小一致

        /*是否移动的bool值*/
        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);//如果水平轴的值近似为0则该值为false
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);//如果垂直轴的值近似为0则该值为false
        bool isWalking = hasHorizontalInput || hasVerticalInput;//是否行走，很明显这里是或的关系，true行走，false空闲
        m_Animator.SetBool("IsWalking",isWalking);//这里将isWalking的布尔值传入先前的IsWalking参数

        /*获得旋转所需的四元数值*/
        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement, turnSpeed * Time.deltaTime, 0f);//得到朝向
        //@1 transform.forward 是个访问Transform组件的捷径，获得当前的朝向
        //@2 目标朝向
        //@3 每帧的变化角度 d_angle = turnSpeed * time.deltaTime,time.deltaTime是每帧之间的间隔时间
        //@4 
        m_Rotation = Quaternion.LookRotation(desiredForward);//得到的旋转
    }
    // 将之前所得到的m_Movement和m_Rotation应用于root motion
    void OnAnimatorMove()
    {
        /*实现移动*/
        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement * m_Animator.deltaPosition.magnitude);
        /*实现转向*/
        m_Rigidbody.MoveRotation(m_Rotation);
    }
}
