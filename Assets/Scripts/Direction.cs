using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Direction {

    public int value { get; private set; }

    public Direction(int value) {
        if (value > 6) this.value = 6;
        else this.value = value;
    }

    public static Direction down { get { return new Direction(0); } }
    public static Direction back { get { return new Direction(1); } }
    public static Direction left { get { return new Direction(2); } }
    public static Direction up { get { return new Direction(3); } }
    public static Direction forward { get { return new Direction(4); } }
    public static Direction right { get { return new Direction(5); } }
    public static Direction undefined { get { return new Direction(6); } }

    public Direction GetRotatedVector(Direction movementDirection) {
        if (movementDirection == forward) {
            if (this == up) return forward;
            else if (this == forward) return down;
            else if (this == down) return back;
            else if (this == back) return up;
            else return this;
        }
        else if (movementDirection == back) {
            if (this == up) return back;
            else if (this == back) return down;
            else if (this == down) return forward;
            else if (this == forward) return up;
            else return this;
        }
        else if (movementDirection == left) {
            if (this == up) return left;
            else if (this == left) return down;
            else if (this == down) return right;
            else if (this == right) return up;
            else return this;
        }
        else if (movementDirection == right) {
            if (this == up) return right;
            else if (this == right) return down;
            else if (this == down) return left;
            else if (this == left) return up;
            else return this;
        }
        else return undefined;
    }

    public override string ToString() {
        switch (value) {
            case 0: return "down";
            case 1: return "back";
            case 2: return "left";
            case 3: return "up";
            case 4: return "forward";
            case 5: return "right";
            default: return "undefined";
        }
    }

    public override bool Equals(object obj) {
        if (obj == null || !(obj is Direction))
            return false;
        return value == ((Direction)obj).value;
    }

    public override int GetHashCode() {
        return value;
    }

    public static bool operator ==(Direction vector0, Direction vector1) {
        return vector0.value == vector1.value;
    }

    public static bool operator !=(Direction vector0, Direction vector1) {
        return vector0.value != vector1.value;
    }

    public static Direction operator -(Direction vector) {
        return new Direction((vector.value + 3) % 6);
    }

    public static Direction operator +(Direction originalVector, Direction movementVector) {
        if (movementVector == forward) {
            if (originalVector == up) return forward;
            else if (originalVector == forward) return down;
            else if (originalVector == down) return back;
            else if (originalVector == back) return up;
            else return originalVector;
        }
        else if (movementVector == back) {
            if (originalVector == up) return back;
            else if (originalVector == back) return down;
            else if (originalVector == down) return forward;
            else if (originalVector == forward) return up;
            else return originalVector;
        }
        else if (movementVector == left) {
            if (originalVector == up) return left;
            else if (originalVector == left) return down;
            else if (originalVector == down) return right;
            else if (originalVector == right) return up;
            else return originalVector;
        }
        else if (movementVector == right) {
            if (originalVector == up) return right;
            else if (originalVector == right) return down;
            else if (originalVector == down) return left;
            else if (originalVector == left) return up;
            else return originalVector;
        }
        else return undefined;
    }

}
