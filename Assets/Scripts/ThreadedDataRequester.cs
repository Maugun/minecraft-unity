using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;

public class ThreadedDataRequester : MonoBehaviour {

    private static ThreadedDataRequester _instance;
    private Queue<ThreadInfo> _dataQueue = new Queue<ThreadInfo>();

    private int _updateNb = 0;

    void Awake() {
        _instance = this;
    }

    public static void RequestData(Func<object> generateData, Action<object> callback) {
        ThreadStart threadStart = delegate {
            _instance.DataThread(generateData, callback);
        };

        new Thread(threadStart).Start();
    }

    void DataThread(Func<object> generateData, Action<object> callback) {
        object data = generateData();
        lock (_dataQueue) {
            _dataQueue.Enqueue(new ThreadInfo(callback, data));
        }
    }


    void Update() {
        if (_dataQueue.Count > 0 && _updateNb % 3 == 0) {
            //for (int i = 0; i < _dataQueue.Count; i++) {
            ThreadInfo threadInfo = _dataQueue.Dequeue();
            threadInfo.callback(threadInfo.parameter);
            //}
            _updateNb = 0;
        }
        _updateNb++;
    }

    struct ThreadInfo {
        public readonly Action<object> callback;
        public readonly object parameter;

        public ThreadInfo(Action<object> callback, object parameter) {
            this.callback = callback;
            this.parameter = parameter;
        }

    }
}
