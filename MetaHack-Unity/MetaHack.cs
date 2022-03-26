using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using UnityEngine;

// https://docs.unity3d.com/Packages/com.unity.webrtc@2.3/manual/index.html
[RequireComponent(typeof(NetworkBackend))]
public class MetaHack : MonoBehaviour {

    static MetaHack _instance;
    public static MetaHack Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<MetaHack>();
                if (_instance == null) {
                    GameObject go = new GameObject("MetaHack");
                    _instance = go.AddComponent<MetaHack>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instance;
        }
    }
    
    NetworkBackend _network;
    [SerializeField] string _signalingServer = "phone-tracker.glitch.me";
    [SerializeField] bool _enableDebugLogs = false;
    
    void Awake() {
        if (_instance == null) {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        } else if (_instance != this) {
            Debug.LogError("Multiple MetaHack instances detected...");
            //Destroy(gameObject);
        }

        _network = GetComponent<NetworkBackend>();
        if (_network == null) {
            _network = gameObject.AddComponent<WebRTCBackend>();
        }

        _network.LogEnabled = _enableDebugLogs;

        _network.OnReady += userId => OnReady?.Invoke(userId);
        _network.OnClose += userId => OnQuit?.Invoke(userId);
        
        _network.Init(_signalingServer);
    }

    public Action<int> OnReady; // remote user connected and ready to receive messages
    public Action<int> OnQuit; // remote user disconnected
    
    // send data to a user or broadcast it 
    public void Send(string data, int? userId = null) => _network.Send(data, userId);
    
    // if data is sent as json, callback is fired when json.evt == name
    public void On(string name, Action<JObject, int> callback) => _network.On(name, callback);

    void OnDestroy() {
        _network.Close();
    }
}
