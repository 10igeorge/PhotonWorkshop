using UnityEngine;
using System.Collections;

public class DestroyOnAnimComplete:MonoBehaviour {
    public void AnimationCompleted() {
        Destroy(gameObject);
    }
}
