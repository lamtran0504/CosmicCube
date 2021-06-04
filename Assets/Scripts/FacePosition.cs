using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FacePosition {

    public int x { get; set; }
    public int y { get; set; }
    public Direction direction { get; set; }

    public FacePosition(int x, int y, Direction direction) {
        this.x = x;
        this.y = y;
        this.direction = direction;
    }

    public FacePosition(int hashCode) {
        direction = new Direction(hashCode % 6);
        hashCode /= 6;
        y = hashCode % 10;
        x = hashCode / 10;
    }

    public static bool operator == (FacePosition pos0, FacePosition pos1) {
        return pos0.x == pos1.x & pos0.y == pos1.y & pos0.direction == pos1.direction;
    }

    public static bool operator !=(FacePosition pos0, FacePosition pos1) {
        return pos0.x != pos1.x || pos0.y != pos1.y || pos0.direction != pos1.direction;
    }

    public override bool Equals(object obj) {
        if (obj == null || !(obj is FacePosition))
            return false;
        return x == ((FacePosition)obj).x & y == ((FacePosition)obj).y & direction == ((FacePosition)obj).direction;
    }

    public override int GetHashCode() {
        return (x * 10 + y) * 6 + direction.value;
    }

    public override string ToString() {
        return x.ToString() + ", " + y.ToString() + ", " + direction.ToString();
    }
}
