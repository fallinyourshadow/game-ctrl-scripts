using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Observer : MonoBehaviour
{
    public Transform player;//关联游戏对象John
    public GameEnding1 gameEnding;//用于更改GameEnding类中的私有成员变量
    bool m_IsPlayerInRange = false;//记录John是否进入触发区
    
    void OnTriggerEnter(Collider other)//进入触发区
    {
        if(other.transform == player)//进入对象是否是John
        {
            m_IsPlayerInRange = true;
        }
    }
    void OnTriggerExit(Collider other)//出触发区
    {
        if(other.transform == player)
        {
            m_IsPlayerInRange = false;
        }
    }
    void Update()
    {
        if(m_IsPlayerInRange)//进入了触发区
        {
            Vector3 direction = player.position - transform.position + Vector3.up;//众所周知向量的值等于坐标A减去坐标B，其中Vector3.up相当于(0,1,0)
            Ray ray = new Ray(transform.position, direction);//用Ray方法实例化一个名为ray的Ray类对象，第一个参数
            RaycastHit raycastHit;//
            if(Physics.Raycast(ray, out raycastHit))//如果该光线打到某个对象返回true否则返回false，out参数的值可以通过其他方式更改或设置，而RaycastHit类型的参数就是一个out参数
            {
                if(raycastHit.collider.transform == player)//检测被光线击中的对象是否是John
                {
                    /*这里将要更变之前写的GameEnding脚本，为它添加重启游戏功能*/
                    gameEnding.CaughPlayer();//调用gameEnding实例对象的公共方法CaughPlayer()将GameEnding1类中的bool类型成员m_IsPlayerCaught置为true
                }
            }
        }
    }
}
