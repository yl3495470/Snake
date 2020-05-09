using UnityEngine;
using System.Collections;

namespace Config
{
    public class ParachuteTpl : ScriptableObject
    {
        public ParachuteList tpl;
    }

    [System.Serializable]
    public class ParachuteList
    {
        //起飞距离;
        public float takeOffDistance;
        //升空速度;
        public float horizontalSpeed;
        //初始速度;
        public float initSpeed;
        //巡航速度;
        public float cruiseSpeed;
        //巡航高度;
        public float cruiseHeight;
        //最大爬升速度;
        public float maxClimbSpeed;
    }
}
