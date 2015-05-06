using UnityEngine;
using System.Collections;

public interface ICUIPlayer {
    void CheckIcon();
    void IsComeback();
    void IsHasQuit();
    void ShowEffect();
    void ShowTinhDiem();
    void StartRemainingTime(float remainingTime);
    void StartTime(float total);
    void UpdateInfo();
}
