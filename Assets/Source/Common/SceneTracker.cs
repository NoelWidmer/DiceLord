using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISceneTracker
{
    void SetLastScene(int lastSceneIdx);
    int GetLastScene();
}

public class SceneTracker : Singleton<SceneTracker, ISceneTracker>, ISceneTracker
{
    private int _lastSceneIdx;

    public int GetLastScene() => _lastSceneIdx;
    public void SetLastScene(int lastSceneIdx) => _lastSceneIdx = lastSceneIdx;
}
