using System;
using System.Collections;
using System.Collections.Generic;
using HighlightPlus;
using UnityEngine;

[Serializable]
public class ObjectSelectionHelper<T> where T : Component {
    public T curObject;
    public HighlightProfile currentProfile;
    HighlightEffect GetHighlight(T obj) {
        var highlight = obj.GetComponent<HighlightEffect>();
        if (highlight == null) {
            highlight = obj.gameObject.AddComponent<HighlightEffect>();
        }

        return highlight;
    }

    public bool Select(T obj, HighlightProfile profile) {
        bool objectChanged = curObject != obj;
        if (objectChanged) {
            Deselect();
        }
        
        bool profileChanged = profile != currentProfile;
        if (obj != null && (profileChanged || objectChanged)) {
            var highlight = GetHighlight(obj);
            highlight.ProfileLoad(profile);
            highlight.highlighted = true;
            curObject = obj;
            currentProfile = profile;
        }

        return objectChanged || profileChanged;
    }

    public void Deselect() {
        if (curObject != null) {
            var highlight = GetHighlight(curObject);
            highlight.highlighted = false;
            curObject = null;
            currentProfile = null;
        }
    }
}
