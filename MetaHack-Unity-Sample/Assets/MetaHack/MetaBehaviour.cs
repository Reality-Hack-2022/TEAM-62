
using System;
using Newtonsoft.Json.Linq;
using UnityEngine;

public abstract class MetaBehaviour : MonoBehaviour {
    protected void Start() {
        MetaHack.Instance.OnReady += OnReady;
        MetaHack.Instance.OnQuit += OnQuit;
    }

    protected abstract void OnReady(int userId);
    protected abstract void OnQuit(int userId);

    protected void On(string name, Action<JObject, int> callback)
        => MetaHack.Instance.On(name, callback);
    
    protected void Send(string data, int? userId = null)
        => MetaHack.Instance.Send(data, userId);
}