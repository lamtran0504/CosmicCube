using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Rotation {

    public Direction[] Vector { get; private set; }

    public int color;

    public static Rotation Start { get { return new Rotation(0, Direction.down, Direction.back); } }

    public Rotation(int color, Direction vector0, Direction vector1) {    
        this.color = color;
        if (color < 0 || color > 5) {
            Debug.Log("Rotation color can only be integers in the range of 0 and 5.");
            Vector = new Direction[6] { Direction.undefined, Direction.undefined, Direction.undefined, Direction.undefined, Direction.undefined, Direction.undefined };
            return;
        }
        int value0 = vector0.value;
        int value1 = vector1.value;
        int value2;
        if (value0 % 3 == value1 % 3) {
            Vector = new Direction[6] { Direction.undefined, Direction.undefined, Direction.undefined, Direction.undefined, Direction.undefined, Direction.undefined };
            return;
        }
        int a = value0 % 3;
        int b = value1 % 3;
        bool boo = true;
        if (a != value0) boo = !boo;
        if (b != value1) boo = !boo;
        if ((a == 1 & b == 0) || (a == 2 & b == 1) || (a == 0 & b == 2)) boo = !boo;
        value2 = 3 - a - b;
        if (!boo)
            value2 += 3;
        Direction vector2 = new Direction(value2);
        Vector = new Direction[6] { vector0, vector1, vector2, -vector0, -vector1, -vector2 };
    }

    public Rotation(int color, int value0, int value1) {
        this.color = color;
        if (color < 0 || color > 5) {
            Debug.Log("Rotation color can only be integers in the range of 0 and 5.");
            Vector = new Direction[6] { Direction.undefined, Direction.undefined, Direction.undefined, Direction.undefined, Direction.undefined, Direction.undefined };
            return;
        }
        int value2;
        if (value0 % 3 == value1 % 3) {
            Vector = new Direction[6] { Direction.undefined, Direction.undefined, Direction.undefined, Direction.undefined, Direction.undefined, Direction.undefined };
            return;
        }
        int a = value0 % 3;
        int b = value1 % 3;
        bool boo = true;
        if (a != value0) boo = !boo;
        if (b != value1) boo = !boo;
        if (a > b || (a == 0 & b == 2)) boo = !boo;
        value2 = 3 - a - b;
        if (!boo)
            value2 += 3;
        Direction vector0 = new Direction(value0);
        Direction vector1 = new Direction(value1);
        Direction vector2 = new Direction(value2);
        Vector = new Direction[6] { vector0, vector1, vector2, -vector0, -vector1, -vector2 };
    }

    public Rotation(int hashCode) {
        int value1 = hashCode % 6;
        int value0 = (hashCode - value1) / 6;
        color = 0;
        int value2;
        if (value0 % 3 == value1 % 3)
            Vector = new Direction[6] { Direction.undefined, Direction.undefined, Direction.undefined, Direction.undefined, Direction.undefined, Direction.undefined };
        int a = value0 % 3;
        int b = value1 % 3;
        bool boo = true;
        if (a != value0) boo = !boo;
        if (b != value1) boo = !boo;
        if (a > b || (a == 0 & b == 2)) boo = !boo;
        value2 = 3 - a - b;
        if (!boo)
            value2 += 3;
        Direction vector0 = new Direction(value0);
        Direction vector1 = new Direction(value1);
        Direction vector2 = new Direction(value2);
        Vector = new Direction[6] { vector0, vector1, vector2, -vector0, -vector1, -vector2 };
    }

    public void PrintAll() {
        Debug.Log("(" + color + ", " + Vector[0] + ", " + Vector[1] + ", " + Vector[2] + ", " + Vector[3] + ", " + Vector[4] + ", " + Vector[5] + ")");
    }

    public Rotation GetNext(Direction movementDirection) {
        Direction vector0 = Vector[0].GetRotatedVector(movementDirection);
        Direction vector1 = Vector[1].GetRotatedVector(movementDirection);
        return new Rotation(color, vector0, vector1);
    }

    public Direction[] GetAbsoluteColor() {
        switch (color) {
            case 0: return new Direction[2] { Vector[0], Vector[1] };
            case 1: return new Direction[2] { Vector[2], Vector[0] };
            case 2: return new Direction[2] { Vector[4], Vector[5] };
            case 3: return new Direction[2] { Vector[3], Vector[4] };
            case 4: return new Direction[2] { Vector[2], Vector[3] };
            case 5: return new Direction[2] { Vector[1], Vector[5] };
            default: return new Direction[2] { Direction.undefined, Direction.undefined };
        }
    }

    public static bool operator ==(Rotation rot0, Rotation rot1) {
        return rot0.GetAbsoluteColor()[0] == rot1.GetAbsoluteColor()[0] & rot0.GetAbsoluteColor()[1] == rot1.GetAbsoluteColor()[1];
    }

    public static bool operator !=(Rotation rot0, Rotation rot1) {
        return rot0.GetAbsoluteColor()[0] != rot1.GetAbsoluteColor()[0] || rot0.GetAbsoluteColor()[1] != rot1.GetAbsoluteColor()[1];
    }

    public override bool Equals(object obj) {
        if (obj == null || !(obj is Rotation))
            return false;
        return GetAbsoluteColor()[0] == ((Rotation)obj).GetAbsoluteColor()[0] & GetAbsoluteColor()[1] == ((Rotation)obj).GetAbsoluteColor()[1];
    }

    public override int GetHashCode() {
        return GetAbsoluteColor()[0].value * 6 + GetAbsoluteColor()[1].value;
    }

    public override string ToString() {
        return "(" + color + ", " + Vector[0].ToString() + ", " + Vector[1].ToString() + ")";
    }
}