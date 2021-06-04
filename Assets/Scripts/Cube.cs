using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : Singleton<Cube> {

    public MeshRenderer[] faces;
    public CubePosition position;

    public bool isFalling;
    public bool cannotMove;

    Vector3 worldPosition;
    float velocityY;
    static float g = 9.8f;

    public void Move(Direction direction) {
        if (!cannotMove)
            StartCoroutine("IMove", direction);
    }

    private IEnumerator IMove(Direction direction) {
        GameManager.Instance.MoveCount++;
        Vector3 worldPos = transform.position;
        Vector3 eulers;
        Vector3 rotCenter;
        if (direction == Direction.forward) { position.y++; eulers = Vector3.right; rotCenter = new Vector3(worldPos.x, worldPos.y - .55f, worldPos.z + .55f); }
        else if (direction == Direction.back) { position.y--; eulers = Vector3.left; rotCenter = new Vector3(worldPos.x, worldPos.y - .55f, worldPos.z - .55f); }
        else if (direction == Direction.left) { position.x--; eulers = Vector3.forward; rotCenter = new Vector3(worldPos.x - .55f, worldPos.y - .55f, worldPos.z); }
        else if (direction == Direction.right) { position.x++; eulers = Vector3.back; rotCenter = new Vector3(worldPos.x + .55f, worldPos.y - .55f, worldPos.z); }
        else { eulers = Vector3.zero; rotCenter = new Vector3(); }
        GetComponent<Rigidbody>().useGravity = false;
        cannotMove = true;
        for (int i = 0; i < 9; i++) {
            transform.RotateAround(rotCenter, eulers, 10);
            yield return new WaitForFixedUpdate();
        }
        cannotMove = false;

        GetComponent<Rigidbody>().useGravity = true;

        position.rotation = position.rotation.GetNext(direction);
        Direction[] v = position.rotation.Vector;

        Vector3 pos = LevelManager.Instance.CubeToWorldPos(position.x, position.y);
        transform.position = pos;
        GridPosition gp = new GridPosition(position.x, position.y);
        if (LevelManager.Instance.targetDict.ContainsKey(gp)) {
            for (int i = 0; i < 6; i++) {
                if (LevelManager.Instance.targets[i].Contains(gp)) {
                    if (position.rotation.Vector[i] == Direction.down) {
                        if (LevelManager.Instance.targetDict.Count == 1)
                            GameManager.Instance.CompleteStage();
                        else
                            LevelManager.Instance.RemoveTarget(new GridPosition(position.x, position.y));
                    }
                    break;
                }
            }
        }

        transform.position = pos;
        yield return null;
    }

    public void EnterGameplay() {
        GetComponent<BoxCollider>().enabled = true;
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<Rigidbody>().useGravity = true;
    }

    public void StartNewStage(CubePosition startingPosition) {
        position = startingPosition;
        position.rotation = Rotation.Start;
        transform.rotation = Quaternion.identity;
        cannotMove = false;
    }

    public void ExitGameplay() {
        for (int i = 0; i < 6; i++)
            faces[i].enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<Rigidbody>().useGravity = false;
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.GetComponent<Cube>() != null)
            return;
        AudioManager.Instance.PlayCubeSound();
        if (isFalling) {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            Vector3 pos = LevelManager.Instance.CubeToWorldPos(position.x, position.y);
            transform.position = pos;
            isFalling = false;
        }
        else if (GameManager.Instance.IsInEndGame) {
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            GameManager.Instance.IsInEndGame = false;
        }
    }


}
