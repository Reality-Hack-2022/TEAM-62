using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using UnityEngine;

public class MetaCube : MetaBehaviour {
    void Start() {
        base.Start();
        On("test", OnTest);
    }

    protected override void OnReady(int userId) {
        Debug.Log($"OnReady {userId}");
        Send("{\"evt\":\"test\", \"str\":\"hello\"}", userId);
    }

    protected override void OnQuit(int userId) {
        Debug.Log($"OnQuit {userId}");
        gameObject.GetComponent<Renderer>().material.color = Color.white;
    }

    //[On("test")]
    void OnTest(JObject obj, int userId) {
        Debug.Log($"receive hello from {userId}");
        gameObject.GetComponent<Renderer>().material.color = Color.red;
    }
}
