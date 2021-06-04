using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CubePosition {

    public int x { get; set; }
    public int y { get; set; }
    public Rotation rotation { get; set; }

    public CubePosition(int x, int y, Rotation rotation) {
        this.x = x;
        this.y = y;
        this.rotation = rotation;
    }

    public CubePosition(int hashCode) {
        rotation = new Rotation(hashCode % 36);
        y = ((hashCode - hashCode % 36) / 36) % 10;
        x = (hashCode - hashCode % 36 - y * 36) / 360;
    }

    public static bool operator %(CubePosition pos0, CubePosition pos1) {
        return pos0.x == pos1.x & pos0.y == pos1.y & pos0.rotation.Vector[0] == pos1.rotation.Vector[0];
    }

    public static bool operator == (CubePosition pos0, CubePosition pos1) {
        return pos0.x == pos1.x & pos0.y == pos1.y & pos0.rotation == pos1.rotation;
    }

    public static bool operator !=(CubePosition pos0, CubePosition pos1) {
        return pos0.x != pos1.x || pos0.y != pos1.y || pos0.rotation != pos1.rotation;
    }

    public override bool Equals(object obj) {
        if (obj == null || !(obj is CubePosition))
            return false;
        return x == ((CubePosition)obj).x & y == ((CubePosition)obj).y & rotation == ((CubePosition)obj).rotation;
    }

    public override int GetHashCode() {
        return (x * 10 + y) * 36 + rotation.GetHashCode();
    }

    public override string ToString() {
        return x.ToString() + ", " + y.ToString() + ", " + rotation.ToString();
    }
}
