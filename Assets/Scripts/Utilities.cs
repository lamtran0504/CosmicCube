using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities {

    public static System.Random rand = new System.Random();

    public static float CUBE_SIZE = 1f, TILE_SIZE = 1.1f, TILE_H = .2f;

    public static Direction GetRotatedVector(Direction vectorDirection, Direction movementDirection) {
        if (movementDirection == Direction.forward) {
            if (vectorDirection == Direction.up) return Direction.forward;
            else if (vectorDirection == Direction.forward) return Direction.down;
            else if (vectorDirection == Direction.down) return Direction.back;
            else if (vectorDirection == Direction.back) return Direction.up;
            else return vectorDirection;
        }
        else if (movementDirection == Direction.back) {
            if (vectorDirection == Direction.up) return Direction.back;
            else if (vectorDirection == Direction.back) return Direction.down;
            else if (vectorDirection == Direction.down) return Direction.forward;
            else if (vectorDirection == Direction.forward) return Direction.up;
            else return vectorDirection;
        }
        else if (movementDirection == Direction.left) {
            if (vectorDirection == Direction.up) return Direction.left;
            else if (vectorDirection == Direction.left) return Direction.down;
            else if (vectorDirection == Direction.down) return Direction.right;
            else if (vectorDirection == Direction.right) return Direction.up;
            else return vectorDirection;
        }
        else if (movementDirection == Direction.right) {
            if (vectorDirection == Direction.up) return Direction.right;
            else if (vectorDirection == Direction.right) return Direction.down;
            else if (vectorDirection == Direction.down) return Direction.left;
            else if (vectorDirection == Direction.left) return Direction.up;
            else return vectorDirection;
        }
        else return Direction.undefined;
    }
}
