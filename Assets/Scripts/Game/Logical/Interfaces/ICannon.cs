using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICannon
{
    void Init(int Atk, int Count, float interval);

    int ATK { get; set; }

    int Count { get; set; }

    float Interval { get; set; }

    void Fire();

    void Charge(int chargeCount);
}
