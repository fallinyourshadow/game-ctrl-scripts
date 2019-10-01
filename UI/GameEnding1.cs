using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEnding1 : MonoBehaviour
{
    public AudioSource exitAudio;//
    public AudioSource caughtAudio;//关联音频
    public float fadeDuration = 1f;//渐变发生持续时间
    public GameObject player;//用于添加John对象
    bool m_IsPlayerAtExit;//是否逃脱游戏
    bool m_IsPlayerCaught;//是否被抓住
    bool m_HasAudioPlayed;//是否播放音频
    float m_Timer;//记录持续时间
    public CanvasGroup exitBackroundImageCanvasGroup;//用于关联CanvasGroup组件
    public CanvasGroup caughtBackroundImageCanvasGroup;//用于关联CanvasGroup组件
    public float displayImageDuration = 1f;
    void OnTriggerEnter(Collider other)//触发器
    {
         if(other.gameObject == player)//如果触发者是John
        {
            m_IsPlayerAtExit = true;//游戏结束
        }
    }
    void Update()//帧更新
    {
        if(m_IsPlayerAtExit)//如果触发了逃脱条件
        {
            EndLevel(exitBackroundImageCanvasGroup, false, exitAudio);//逐渐显示逃脱成功结束图片播放成功的音频，并退出游戏
        }
        else if(m_IsPlayerCaught)//如果被抓住
        {
            EndLevel(caughtBackroundImageCanvasGroup, true, caughtAudio);//组件显示逃脱失败图片播放失败的音频,并且重新开始游戏
        }
    }
    void EndLevel (CanvasGroup imageCanvasGroup, bool doRestart, AudioSource audioSource)//根据第一个参数处理结束画面
    {
        if(!m_HasAudioPlayed)//如果没有音频在播放，防止重复
        {
            audioSource.Play();//根据传入的AudioSource播放音频
            m_HasAudioPlayed = true;//正在播放音乐
        }
        m_Timer += Time.deltaTime;
        if(doRestart)//是否重新开始游戏
        {
            caughtBackroundImageCanvasGroup.alpha = m_Timer / fadeDuration;//在一秒类组件显示失败图片
            if (m_Timer > displayImageDuration + fadeDuration)//持续2秒
            {
                SceneManager.LoadScene(0);//重新加载你的第一个场景
            }
        }
        else
        {
            exitBackroundImageCanvasGroup.alpha = m_Timer / fadeDuration;//在一秒内逐渐显示成功图片
            if(m_Timer > displayImageDuration + fadeDuration)//持续2秒
            {
                Application.Quit();//退出
            }
        }
    }
    public void CaughPlayer ()
    {
        m_IsPlayerCaught = true;
    }
}