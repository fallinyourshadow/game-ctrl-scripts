using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;//NavMeshAgent类所在的命名空间


public class WaypointPatrol : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public Transform[] waypoints;//一个存储Transform类型数据的数组，主要为了存储多个目的地
    int m_CurrentWaypointIndex;//用于检索数组的每个元素
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent.SetDestination(waypoints[0].position);//将该数组的第一个元素设为初始目标
    }
    // Update is called once per frame
    void Update()
    {
        if(navMeshAgent.remainingDistance < navMeshAgent.stoppingDistance)//如果剩余的距离小于先前设置的0.2，则视为到达目标地 
        {
            m_CurrentWaypointIndex = (m_CurrentWaypointIndex + 1) % waypoints.Length;//计数器，m_CurrentWaypointIndex的值始终在0到waypoints.Length之间循环
            navMeshAgent.SetDestination(waypoints[m_CurrentWaypointIndex].position);//切换下一个目标，最终回到初始地点
        }
    }
}
