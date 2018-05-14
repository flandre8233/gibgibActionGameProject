using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class keyboardDetector {
    public KeyCode targetKey;
    bool _isHold;

    public bool isHold {
        get {
            return _isHold;
        }
    }

    public void update() {
    }
}
