﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameView : SingletonMonoBehavior<gameView> {
    private void Start() {
        Cursor.visible = false;
    }
}
