using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*改良版，将胶囊碰撞组件替换成了Character controller组件，解决了碰撞后乱动的问题，顺便试试中文编码*/

public class CharacterControl : MonoBehaviour
{

    private struct State //存储状态信息的结构体
    {
        public bool hasLeftInput;
        public bool hasRightInput;
        public bool hasForwardInput;
        public bool hasBackwardInput;
        public bool isCircling;
        public bool isGoStright;
        public bool isTurning;
        public bool isRetreat;
        public bool isWalking;
        public bool isIdel;
    };
    private bool retreatEnable; //是否允许快速转身
    private bool isMoving;
    private float angle;
    private float 移动速度;
    private Vector3 moveDirection;
    private Quaternion m_Rotation;
    private AudioSource audioSource;
    private CharacterController m_Controller;
    private Animator m_Animator;
    private Rigidbody m_Rigidbody;
    /*set control key*/
    public KeyCode 左转键 = KeyCode.LeftArrow;
    public KeyCode 右转键 = KeyCode.RightArrow;
    public KeyCode 前进键 = KeyCode.UpArrow;
    public KeyCode 快速回头键 = KeyCode.DownArrow;
    public float 最大移动速度 = 0.025f; //移动速度
    public float 最大角速度 = 20f; //模型的旋转速度
    public float 静止角速度 = 4f; //每帧最多转4度
    public float 运动角速度 = 4f; //每帧最多转4度
    public float 加速度 = 0.001f; //每帧

    // Start is called before the first frame update
    void Start()
    {
        m_Controller = GetComponent<CharacterController>();
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
        initData();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        State state = StateCheck();//检查按键获得状态
        m_Animator.SetBool("IsWalking", state.isWalking); //播放移动动画
        if(state.isWalking)
        {
            if(!audioSource.isPlaying)
            {
                audioSource.Play();//会更改audioSouce.isPlaying为Ture
            }
        }
        else
        {
            audioSource.Stop();//会更改audioSouce.isPlaying为false
        }
        moveDirection = MoveByThis(state);//获得移动方向
        m_Rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, moveDirection, 最大角速度 * Time.deltaTime, 0f));//获得下一帧的旋转量
    }
    void OnAnimatorMove()
    {
        m_Controller.Move((isMoving? 1f:0f)*moveDirection);//移动
        m_Rigidbody.MoveRotation(m_Rotation);//旋转
    }
    State StateCheck()//检查按键获得状态
    {
        State state = new State();
        state.hasLeftInput = Input.GetKey(左转键);
        state.hasRightInput = Input.GetKey(右转键);
        state.hasForwardInput = Input.GetKey(前进键);
        state.hasBackwardInput = Input.GetKey(快速回头键);
        bool hasVerticalInput = state.hasForwardInput || state.hasBackwardInput && state.hasBackwardInput != state.hasForwardInput;//异或
        bool hasHorizontalInput = state.hasLeftInput || state.hasRightInput && state.hasRightInput != state.hasLeftInput;
        state.isCircling = !hasVerticalInput && hasHorizontalInput;//是否处于原地转圈状态
        state.isGoStright = state.hasForwardInput && !hasHorizontalInput;//是否处于直行状态
        state.isTurning = state.hasForwardInput && hasVerticalInput;//是否处于转向行走状态
        state.isRetreat = state.hasBackwardInput && !hasHorizontalInput;//是否回头
        state.isWalking = state.hasForwardInput || hasHorizontalInput;//是否播放行走动画
        state.isIdel = !hasVerticalInput && !hasHorizontalInput;
        isMoving = state.isGoStright || state.isTurning;
        return state;
    }
    Vector3 MoveByThis(State state)//获得移动方向
    {
        
        Vector3 Direction = Vector3.zero;
        if (Mathf.Approximately(angle % 360f, 0f))//watch dog 防止溢出
        {
            angle = 0f;
        }
        if( 移动速度 < 最大移动速度 && state.hasForwardInput)
        {
            移动速度 += 加速度;
        }//更新速度
        if (state.isCircling) 
        {
            if (state.hasRightInput)
            {
                angle += 静止角速度;
            }
            else if (state.hasLeftInput)
            {
                angle -= 静止角速度;
            }
            移动速度 = 最大移动速度;
            retreatEnable = true;//置为允许快速转身
            Direction.Set(移动速度 * Mathf.Sin(angle * Mathf.PI / 180), 0f, 移动速度 * Mathf.Cos(angle * Mathf.PI / 180));
            return Direction;
        }
        else if (state.isRetreat)//快速转身，只有在retreatEnable为true的情况下才能快速转身
        {
            if (retreatEnable)
            {
                angle += 180f;
                retreatEnable = false;//快速转身置为不允许，防止按住不放，反复转身
                移动速度 = 最大移动速度;
            }
            Direction.Set(移动速度 * Mathf.Sin(angle * Mathf.PI / 180), 0f, 移动速度 * Mathf.Cos(angle * Mathf.PI / 180));
            return Direction;
        }
        else if (state.isTurning)//移动中转向，两帧4度
        {
            if (state.hasRightInput)
            {
                angle += 运动角速度;
            }
            else if (state.hasLeftInput)
            {
                angle -= 运动角速度;
            }
            Direction.Set(移动速度 * Mathf.Sin(angle * Mathf.PI / 180), 0f, 移动速度 * Mathf.Cos(angle * Mathf.PI / 180));
            return Direction;
        }
        else//仅仅按住W或什么都没按的时候
        {
            if(state.isIdel)
            {
                移动速度 = 0f;
            }
             retreatEnable = true;
             return moveDirection;
        }
    }
    void initData()//初始化一些变量
    {
        angle = 0f;
        retreatEnable = true;
        移动速度 = 0f;
        m_Rotation = Quaternion.identity;
        moveDirection = Vector3.zero;
    }
}