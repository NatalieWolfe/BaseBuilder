
﻿using UnityEngine;
using System.Collections.Generic;

public class PrefabPool {
    private GameObject prefab;
    private Transform parent;
    private Stack<GameObject> allocated;

    public PrefabPool(GameObject prefab) {
        this.prefab = prefab;
        parent = prefab.transform.root;
        allocated = new Stack<GameObject>();
    }

    public void SetParent(Transform parent) {
        this.parent = parent;
    }

    public void PrimePool(int count) {
        while (allocated.Count < count) {
            Release(Instantiate());
        }
    }

    public GameObject Acquire() {
        GameObject obj;
        if (allocated.Count > 0) {
            obj = allocated.Pop();
        }
        else {
            obj = Instantiate();
        }
        obj.SetActive(true);
        return obj;
    }

    public GameObject Acquire(Vector3 pos) {
        GameObject obj = Acquire();
        obj.transform.position = pos;
        return obj;
    }

    public void Release(GameObject obj) {
        obj.SetActive(false);
        allocated.Push(obj);
    }

    private GameObject Instantiate() {
        GameObject obj = (GameObject)Object.Instantiate(prefab);
        obj.transform.parent = parent;
        return obj;
    }
}
