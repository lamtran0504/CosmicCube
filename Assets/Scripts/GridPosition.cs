using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GridPosition {

    public int x { get; set; }
    public int y { get; set; }

    public static GridPosition undefined { get { return new GridPosition(-1, -1); } }

    public GridPosition(int x, int y) {
        this.x = x;
        this.y = y;
    }

    public static bool operator ==(GridPosition gp0, GridPosition gp1) {
        return gp0.x == gp1.x & gp0.y == gp1.y;
    }

    public static bool operator !=(GridPosition gp0, GridPosition gp1) {
        return gp0.x != gp1.x || gp0.y != gp1.y;
    }

    public override string ToString() {
        return "(" + x + ", " + y + ")";
    }

    public override bool Equals(object obj) {
        if (obj == null || !(obj is GridPosition))
            return false;
        return this == (GridPosition)obj;
    }

    public override int GetHashCode() {
        return x * 10 + y;
    }
}
