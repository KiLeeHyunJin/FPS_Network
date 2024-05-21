using TMPro;
using UnityEngine;

public class TimeDisplay : MonoBehaviour
{
    public TMP_Text timeText;

    void Update()
    {
       
        float time = Time.timeSinceLevelLoad;

        // 시, 분, 초를 계산합니다.
        int hours = (int)(time / 3600);
        int minutes = (int)((time % 3600) / 60);
        int seconds = (int)(time % 60);

        // TMP Text에 시간을 표시합니다.
        timeText.text = string.Format("{0:00}:{1:00}:{2:00}", hours, minutes, seconds);
    }
}
