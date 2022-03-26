using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class MetaLayer : MetaBehaviour {

    [SerializeField] GameObject _prefab;
    Dictionary<int, Dictionary<string, GameObject>> _remoteObjects = new Dictionary<int, Dictionary<string, GameObject>>();
    
    void Start() {
        base.Start();
        On("position", OnPosition);
    }
    
    protected override void OnReady(int userId) {}

    protected override void OnQuit(int userId) {
        if (!_remoteObjects.ContainsKey(userId)) return;
        foreach (GameObject go in _remoteObjects[userId].Values) {
            Destroy(go);
        }
        _remoteObjects[userId].Clear();
    }
    
    void OnPosition(JObject obj, int userId) {
        if (!_remoteObjects.ContainsKey(userId)) {
            _remoteObjects.Add(userId, new Dictionary<string, GameObject>());
        }

        string id = obj["id"].Value<string>();
        if (!_remoteObjects[userId].ContainsKey(id)) {
            _remoteObjects[userId].Add(id,Instantiate(_prefab));
            _remoteObjects[userId][id].name = userId + "-" + id;
        }

        _remoteObjects[userId][id].transform.position = new Vector3(
            obj["position"]["x"].Value<float>(),
            obj["position"]["y"].Value<float>(),
            obj["position"]["z"].Value<float>()
        );
    }
}
