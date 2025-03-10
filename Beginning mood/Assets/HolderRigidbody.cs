using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HolderRigidbody {
    private RigidbodyConstraints _constraints;
    private float mass;
    private float drag;
    private float angularDrag;
    private bool useGravity;
    public HolderRigidbody(Rigidbody copy) {
        _constraints = copy.constraints;
        mass = copy.mass;
        drag = copy.drag;
        angularDrag = copy.angularDrag;
        useGravity = copy.useGravity;
    }

    public void Apply(Rigidbody target) {
        target.constraints = _constraints;
        target.mass = mass;
        target.drag = drag;
        target.angularDrag = angularDrag;
        target.useGravity = useGravity;
    }
}
