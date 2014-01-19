// LICENSE: Public Domain

using System;
using UnityEngine;

// A wrapper class for viewing GameObject hierarchy in the debugger.
// To use, add a watch for "UnityObjectTree.Wrap(whatever)".
class UnityObjectTree
{
    public GameObject _self;
    public Component[] components;

    public override string ToString()
    {
        return _self.ToString();
    }

    UnityObjectTree(GameObject obj, UnityObjectTree parent = null) {
        this._self = obj;
        this.components = obj.GetComponents<Component>();
        this.z_parent = parent;
    }

    public static UnityObjectTree Wrap(GameObject obj) {
        return new UnityObjectTree(obj);
    }
    public static UnityObjectTree Wrap(Component obj) {
        return new UnityObjectTree(obj.gameObject);
    }

    private UnityObjectTree[] z_children;
    public UnityObjectTree[] children
    {
        get {
            if (z_children == null && _self.transform)
            {
                z_children = new UnityObjectTree[_self.transform.childCount];
                for (int i = 0; i < _self.transform.childCount; i++)
                    z_children[i] = new UnityObjectTree(_self.transform.GetChild(i).gameObject, this);
            }
            return z_children;
        }
    }

    private UnityObjectTree z_parent;
    public UnityObjectTree parent
    {
        get {
            if (z_parent == null && _self.transform && _self.transform.parent)
                z_parent = new UnityObjectTree(_self.transform.parent.gameObject);
            return z_parent;
        }
    }
}
