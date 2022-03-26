using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.WebRTC;
using UnityEngine;
using WebSocketSharp;

[Serializable]
public class IceMsg {
  public int to; // userId
}

[Serializable]
public class IceCandidateMsg : IceMsg {
  public RTCIceCandidate candidate;
}

[Serializable]
public class RTCSessionDescriptionJson {
  public RTCSessionDescriptionJson(RTCSessionDescription descr) {
    type = RTCSdpTypeToString(descr.type);
    sdp = descr.sdp;
  }
  public string type;
  public string sdp;

  string RTCSdpTypeToString(RTCSdpType type) {
    switch (type) {
      case RTCSdpType.Offer: return "offer";
      case RTCSdpType.Pranswer: return "pranswer";
      case RTCSdpType.Answer: return "answer";
      case RTCSdpType.Rollback: return "rollback";
      default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
    }
  }
}

[Serializable]
public class IceHostMsg : IceMsg {
  public RTCSessionDescriptionJson hostDescription;
}

[Serializable]
public class IceRemoteMsg : IceMsg {
  public RTCSessionDescriptionJson remoteDescription;
}

public abstract class NetworkBackend : MonoBehaviour {
  public bool LogEnabled = true;
  protected void Log(object message) {
    if (!LogEnabled) return;
    Debug.Log(message);
  }
  
  public Action<int> OnClose;
  public Action<string, int> OnMessage;
  public Action<int> OnError;
  public Action<int> OnOpen;
  public Action<int> OnReady;
  
  public abstract void Init(string host);
  public abstract void Send(string data, int? toid = null);
  public abstract void On(string name, Action<JObject, int> callback);
  public abstract void Close();
}

/*public class WebSocketBackend : NetworkBackend {
  
  WebSocket _webSocket;

  public EventHandler<CloseEventArgs> OnClose;
  public EventHandler<MessageEventArgs> OnMessage;
  public EventHandler<ErrorEventArgs> OnError;
  public EventHandler OnOpen;
  
  public override void Init(string host) {
    _webSocket = new WebSocket($"wss://{host}/ws");
    _webSocket.OnClose += OnClose;
    _webSocket.OnMessage += OnMessage;
    _webSocket.OnError += OnError;
    _webSocket.OnOpen += OnOpen;
    _webSocket.Connect();
  }

  public override void Send(string data) {
    if (_webSocket != null && _webSocket.ReadyState == WebSocketState.Open) {
      _webSocket.Send(data);
    }
  }

  public override void Close() {
    if (_webSocket != null) {
      _webSocket.Close();
      _webSocket = null;
    }
  }
}*/

/*
  Star configuration *-*, but there is only one connection between each peers because its bidirectionnal
  For each new peers that connect to the network, all existing peers connect to it
  This code works with only one only one data channel per peers
*/
