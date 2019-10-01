using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2 : MonoBehaviour
{
    uint frameCount; //记录帧数累加BUF
    int angle; //储存当前的人物角度
    float sin;
    float cos;
    struct preState //存储状态信息的结构体
    {
        public Vector3 preVector; 
        public bool isCircling;
        public bool isGoStright;
        public bool isTurning;
        public bool isRetreat;
        public bool isWalking;
        public bool isIdel;
    };
    bool isMoving; //是否移动判断条件 ，只有在按下 w 的情况下才可能发生移动
    bool retreatEnable; //是否允许快速转身
    preState state; //用于保存状态信息的结构体变量
    public float turnSpeed = 20f;//角速度为20每秒，用于计算每帧的转角变化
    Vector3 m_Movement1;//声明Vector3对象，它代表一个三维的向量，用记录玩家期望的朝向
    Quaternion m_Rotation = Quaternion.identity;//声明一个四元数对象m_Rotation，并初始化为一个代表无旋转的值,用于记录旋转
    Vector3 desiredForward;
    Animator m_Animator;//声明一个Animator对象，为Animator组件所用
    Rigidbody m_Rigidbody;//声明一个Rigidbody对象，为Rigidbody组件所用
    void Start()
    {
        initData();//初始化一些变量
        m_Animator = GetComponent<Animator>();//关联Animator组件，<>括号内是类型参数用于查询Animator组件
        m_Rigidbody = GetComponent<Rigidbody>();//关联Animator组件
    }
    // 每帧调用一次
    void FixedUpdate()
    {
        moveMode_angDriction();//沿水平任意方向移动
    }
    // 将之前所得到的m_Movement和m_Rotation应用于root motion
    void OnAnimatorMove()
    {
        /*实现移动*/
        m_Rigidbody.MovePosition(m_Rigidbody.position + m_Movement1 * (isMoving ? 1f : 0f) * m_Animator.deltaPosition.magnitude);//当且仅当isMoving为true时移动
        /*实现转向*/
        m_Rigidbody.MoveRotation(m_Rotation);
    }
    bool IsMoving(float h, float v)//是否沿向量方向移动
    {
        bool hasHorizontalInput = !Mathf.Approximately(h, 0f);//如果水平轴的值近似为0则该值为false
        bool hasVerticalInput = !Mathf.Approximately(v, 0f);//如果垂直轴的值近似为0则该值为false
        state.isCircling = !hasVerticalInput && hasHorizontalInput;//是否处于原地转圈状态
        state.isGoStright = v > 0f && !hasHorizontalInput;//是否处于直行状态
        state.isTurning = v > 0f && hasVerticalInput;//是否处于转向行走状态
        state.isRetreat = v < 0f && !hasHorizontalInput;//是否回头
        state.isWalking = v > 0f || hasHorizontalInput;//是否播放行走动画
        state.isIdel = !hasVerticalInput && !hasHorizontalInput;
        return state.isGoStright || state.isTurning;
    }
    void initData()
    {
        retreatEnable = true;
        state.preVector.Set(0f, 0f, 1f); //初始朝向为该方向，所以初始化为该值
        angle = 0; //初始的人物角度
        sin = 0f; //angle = 0 sin 0 = 0；
        cos = 1f; //angle = 0 cos 0 = 1；
        frameCount = 0;
    }
    void moveMode_angDriction()
    {
        float vertical = Input.GetAxis("Vertical");//WS
        float horizontal = Input.GetAxis("Horizontal"); //AD
        isMoving = IsMoving(horizontal, vertical); //识别按键是否移动
        m_Animator.SetBool("IsWalking", state.isWalking);//这里将isWalking的布尔值传入先前的IsWalking参数
        if (state.isCircling) //原地转向，每帧转角为 5度
        {
            frameCount++;
            if (frameCount % 1 == 0)//进入条件1帧后改变一次
            {
                if (horizontal > 0)
                {
                    angle += 5;
                }
                else if (horizontal < 0)
                {
                    angle -= 5;
                }
                sin = Mathf.Sin(angle * Mathf.PI / 180);
                cos = Mathf.Cos(angle * Mathf.PI / 180);
                frameCount = 0;//使用完置0
            }
            m_Movement1.Set(1f * sin, 0f, 1f * cos);
            state.preVector = m_Movement1; //保存该向量，
            retreatEnable = true;//置为允许快速转身
        }
        else if (state.isRetreat && retreatEnable)//快速转身，只有在retreatEnable为true的情况下才能快速转身，按住S 10帧后转身
        {
            frameCount++;
            if (frameCount % 10 == 0)//进入条件60帧后改变一次
            {
                angle += 180;
                sin = Mathf.Sin(angle * Mathf.PI / 180);
                cos = Mathf.Cos(angle * Mathf.PI / 180);
                frameCount = 0;
                retreatEnable = false;//快速转身置为不允许，防止按住不放，反复转身
            }
            m_Movement1.Set(1f * sin, 0f, 1f * cos);
            state.preVector = m_Movement1;
        }
        else if (state.isTurning)//移动中转向，两帧转4度
        {
            frameCount++;
            if (frameCount % 2 == 0)//进入条件2帧后改变一次
            {
                if (horizontal > 0)
                {
                    angle += 4;
                }
                else if (horizontal < 0)
                {
                    angle -= 4;
                }
                sin = Mathf.Sin(angle * Mathf.PI / 180);
                cos = Mathf.Cos(angle * Mathf.PI / 180);
                frameCount = 0;
            }
            m_Movement1.Set(1f * sin, 0f, 1f * cos);
            state.preVector = m_Movement1;
            retreatEnable = true;
        }
        else//仅仅按住W或什么都没按的时候
        {
            frameCount = 0;
            m_Movement1 = state.preVector;
            if(vertical == 0f)
                retreatEnable = true; 
        }
        m_Movement1.Normalize();//规范化该向量，使其在任何方向的大小一致
        /*获得旋转所需的四元数值*/
        Vector3 desiredForward = Vector3.RotateTowards(transform.forward, m_Movement1, turnSpeed * Time.deltaTime, 0f);//得到朝向
        m_Rotation = Quaternion.LookRotation(desiredForward);//得到的旋转
        if (angle % 360 == 0)//watch dog 防止溢出
        {
            angle = 0;
        }
    }
}
/*written by 尽虹*/