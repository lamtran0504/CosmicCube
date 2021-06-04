using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraScript : MonoBehaviour {

    private bool isMovingToLevel;
    private bool IsMovingToLevel {
        get {
            return isMovingToLevel;
        }
        set {
            isMovingToLevel = value;
            if (value) nextLevelPosition = unitOffset * LevelManager.Instance.Rows + LevelManager.Instance.MapStart;
        }
    }
    private const float cameraSpeed = 10f;
    public const float fixedX = 10.3f, fixedY = 11.6f, fixedZ = -4.8f;

    public Vector3 unitOffset = new Vector3(1.2f, 1.8f, -1.3f);

    private float smoothTime;
    private Vector3 velocity = Vector3.zero;
    private Vector3 previousPos = Vector3.zero;
    private Vector3 nextLevelPosition = Vector3.zero;
    private float xChange, yChange, zChange;
    private int cameraMovementFrame;

    void FixedUpdate() {
        if (ButtonManager.Instance.SceneIndex != 3)
            return;
        if (IsMovingToLevel)
            MoveToStage();
	}

    public void EnterGameplay() {
        transform.position = new Vector3(7, 11.6f, -7) + Cube.Instance.gameObject.transform.position;
        transform.rotation = Quaternion.Euler(45, -45, 0);
    }

    public void ToMainMenu() {
        transform.position = new Vector3(4.5f, 4, -9f);
        transform.rotation = Quaternion.Euler(20, -40, 0);
    }

    public void GetFocusOnCube() {
        Vector3 cubePos = Cube.Instance.transform.position;
        transform.position = new Vector3(cubePos.x + 7, cubePos.y + 11, cubePos.z - 7);
    }

    private void MoveToStage() {
        smoothTime = 1;
        transform.position = Vector3.SmoothDamp(transform.position, nextLevelPosition, ref velocity, smoothTime);
        if (Vector3.Distance(transform.position, nextLevelPosition) < 0.1f)
            IsMovingToLevel = false;
    }

    public void EnterNewStage(Vector3 mapStart, bool withoutAnimation = false) {
        if (withoutAnimation) {
            transform.position = unitOffset * LevelManager.Instance.Rows + LevelManager.Instance.MapStart;
            transform.rotation = Quaternion.Euler(45, -45, 0);
            IsMovingToLevel = false;
        }
        else {
            IsMovingToLevel = true;
            yChange = Mathf.Abs(transform.position.y - nextLevelPosition.y);
            smoothTime = yChange / cameraSpeed;
            cameraMovementFrame = 1;
        }
    }

    public void EndGame(Vector3 finalPos) {
        StartCoroutine("IChangePos", finalPos);
        StartCoroutine("IChangeRot");
    }

    private IEnumerator IChangePos(Vector3 finalPos) {
        while (Vector3.Distance(transform.position, finalPos) > 0.1f & !AnimationManager.Instance.SkippingEndgame) {
            transform.position = Vector3.MoveTowards(transform.position, finalPos, cameraSpeed * 0.936f / 70);
            yield return new WaitForFixedUpdate();
        }
        if (!AnimationManager.Instance.SkippingEndgame)
            transform.position = finalPos;
        yield return null;
    }

    private IEnumerator IChangeRot() {
        float a = 0;
        while (a < 45 & !AnimationManager.Instance.SkippingEndgame) {
            a += cameraSpeed * 2 / 70;
            transform.rotation = Quaternion.Euler(45 + a, -45 + a, 0);
            yield return new WaitForFixedUpdate();
        }
        if (!AnimationManager.Instance.SkippingEndgame)
            transform.rotation = Quaternion.Euler(90, 0, 0);
        yield return null;
    }
}
