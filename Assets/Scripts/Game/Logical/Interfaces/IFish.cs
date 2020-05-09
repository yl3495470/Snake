using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFish
{
    void Init(int Hp, IFishPath fishPath);

    int HP { get; set; }

    int FishPath { get; set; }

    void UpdateFish();

    void Demaged(int damagePoint);

    void Dead();
}
