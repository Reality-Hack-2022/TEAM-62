using System;
using UnityEngine;
using System.Collections;

// Work like a static class, you don't need to add this component into your scene (this is handled automatically)
public class MainThread : MonoBehaviour {
    // can't use the generic one with action because its not thread safe and the ConcurrencyQueue is not available in this c# version for unity
    static Queue _actions = new Queue();

    /*[RuntimeInitializeOnLoadMethod]
    static void Init() {
        MainThread mainThread = FindObjectOfType<MainThread>();
        if (mainThread == null) { // create the mainThread if it don't already exist and hide it from the hierarchy
            GameObject mainThreadObj = Instantiate(new GameObject("MainThread"));
            mainThreadObj.AddComponent<MainThread>();
            mainThreadObj.hideFlags = HideFlags.HideInHierarchy;
        } else { // ensure the mainThread is active and enabled
            mainThread.gameObject.SetActive(true);
            mainThread.enabled = true;
        }
    }*/
  
    void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    void Start() {
        StartCoroutine(CallInMainThread());
    }

    public static void Call(Action action) {
        lock (_actions.SyncRoot) {
            _actions.Enqueue(action);
        }
    }

    IEnumerator CallInMainThread() {
        while (true) {
            yield return new WaitUntil(() => {
                lock (_actions.SyncRoot) {
                    return _actions.Count > 0;
                }
            });
            lock (_actions.SyncRoot) {
                while (_actions.Count > 0)
                    ((Action)_actions.Dequeue()).Invoke();
            }
        }
    }
}