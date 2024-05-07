using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphicSetter : MonoBehaviour
{
    public void QualitySetting(int _lv)
    {
        int currentLv = QualitySettings.GetQualityLevel();
        if(currentLv != _lv)
            QualitySettings.SetQualityLevel(_lv);
    }
}
