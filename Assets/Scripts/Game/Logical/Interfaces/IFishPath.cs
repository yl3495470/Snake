using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFishPath
{
    void Init(Vector3 pStartPos , float pSpeed);


    float Speed { get; set; }

    //通过设定算法，从而得出每一次的位置变化
    Vector3 GetPos(float Time);
}
