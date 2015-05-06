using UnityEngine;
using System.Collections;
using Electrotank.Electroserver5.Api;

public interface IGamePlay {
    void OnStart();
    void OnUpdate();
    void OnFinish();
    void AddCommandEsObject(PluginMessageEvent e, string command, string action, EsObject eso);
    void AddCommandEsObject(string command, string action, EsObject eso);
    void OnDispose();
}
