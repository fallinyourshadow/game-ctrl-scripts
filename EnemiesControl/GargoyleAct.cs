using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GargoyleAct : MonoBehaviour
{
    // Start is called before the first frame update
    public int initDirection = -1;//1 或 -1
    public float startAngle = 180f;//开始位置
    public float turnSpeed = 20f;//允许的最大角速度20度每帧
    public float maxDeltaAngle = 5f;//最大转速5度每帧
    public float waitTime = 2f;//等待时间2秒
    public float changePoint = 30f;//开始加速或减速时的偏移量
    private float acceleration;//加速度由公式 maxDeltaAngle / (2 * changePoint / maxDeltaAngle)得到
    private float angle;
    public float m_Timer;//计时器
    public float angleTemp;//保存瞬时角速度
    private float endAngle;//结束的角度，在该脚本中等于两倍的startAngle
    private Quaternion m_Rotation;
    private Rigidbody m_Rigidbody;
    private Vector3 m_Direction;
    private struct State//保存角速度状态
    {
        public bool isSpeedDown;
        public bool isSpeedUp;
        public bool isWaiting;
    };
    State state;
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        initData();
    }
    // Update is called once per frame
    void Update()
    {
        StateCheck();//更新状态
        GetAngleTemp();//获得瞬时角速度
        UpDateAngle(angleTemp);//更新朝向角度
        GetRotation();//获得旋转量
    }
    private void OnAnimatorMove()
    {
        m_Rigidbody.MoveRotation (m_Rotation);//旋转
    }
    private void StateCheck()
    {
        state.isWaiting = Mathf.Approximately(angle % startAngle, 0);//时候处于等待状态
        state.isSpeedUp = angle <= startAngle + changePoint;//是否处于加速状态
        state.isSpeedDown = angle >= endAngle - changePoint;//是否处于减速状态
        //ps:速度为负时加速即这里的减速
    }
    private void GetAngleTemp()
    {
        if (state.isWaiting)
        {
            state.isWaiting = WaitMinute(waitTime);
        }
        if (!state.isWaiting)
        {
            if (state.isSpeedUp)
            {
                raiseDeltaAngle(acceleration);
            }
            else if (state.isSpeedDown)
            {
                reduceDeltaAngle(acceleration);
            }
        }
    }
    private void GetRotation()
    {
        float sin = Mathf.Sin(Mathf.PI * angle / 180f);
        float cos = Mathf.Cos(Mathf.PI * angle / 180f);
        m_Direction.Set(initDirection * sin, 0f, initDirection * cos);//获得当前朝向
        m_Direction = Vector3.RotateTowards(transform.forward, m_Direction, turnSpeed * Time.deltaTime, 0f);//获得目标朝向
        m_Rotation = Quaternion.LookRotation(m_Direction);//获得两个向量间的旋转量
    }
    private void initData()
    {
        m_Rotation = Quaternion.identity;
        m_Direction.Set(1f, 0f, 1f);
        angle = startAngle;
        angleTemp = 0f;
        m_Timer = 0f;
        acceleration = maxDeltaAngle / (2 * changePoint / maxDeltaAngle); //由加速度公式算出角加速度
        endAngle = 2 * startAngle;
    }
    private void UpDateAngle(float angleTemp)
    {
        angle += angleTemp;
    }
    private bool WaitMinute(float waitTime)//等待中时返回true，等待结束返回false
    {
        m_Timer += Time.deltaTime;
        if(m_Timer < waitTime)
        {
            angleTemp = 0f;
            return true;
        }
        else
        {
            m_Timer = 0f;
            return false;
        }
    }
    private void reduceDeltaAngle(float acceleration)//减速
    {
        angleTemp -= acceleration;
    }
    private void raiseDeltaAngle(float acceleration)//加速
    {
        angleTemp += acceleration;
    }
}
