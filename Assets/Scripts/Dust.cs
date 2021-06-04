using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dust : MonoBehaviour {

    public void Disappear(float delay) {
        Vector3 v = new Vector3(3, 1, 2);
        Vector3 d = new Vector3(Utilities.rand.Next(10), Utilities.rand.Next(10), Utilities.rand.Next(20));
        d *= 0.01f;
        v = v / 3.6f + d;
        StartCoroutine(IDisappear(v, 0.015f, delay));
    }

    public void Appear(float delay) {
        Vector3 v = new Vector3(3, 1, 2);
        Vector3 d = new Vector3(Utilities.rand.Next(10), Utilities.rand.Next(10), Utilities.rand.Next(20));
        d *= 0.01f;
        v = v / 3.6f + d;
        StartCoroutine(IAppear(transform.position, v, 0.015f, delay));
    }

	private IEnumerator IDisappear(Vector3 direction, float speed, float delay) {
        yield return new WaitForSeconds(delay);
        float scale = 0.1f;
        while (scale > 0) {
            scale -= 0.0015f;
            transform.localScale = new Vector3(scale, scale * 5, scale);
            transform.position += direction * speed;
            speed *= 1.02f;
            yield return new WaitForFixedUpdate();
        }
        Destroy(gameObject);
        Destroy(this);
        yield return null;
    }

    private IEnumerator IAppear(Vector3 finalPosition, Vector3 direction, float speed, float time) {
        yield return new WaitForSeconds(time);
        float scale = 0;
        float spd = speed;
        Vector3 pos = finalPosition;
        while (scale < 0.1f) {
            scale += 0.0015f;
            pos += direction * spd;
            spd *= 1.02f;
        }

        scale = 0;
        transform.position = pos;
        while (scale < 0.1f) {
            scale += 0.0015f;
            transform.localScale = new Vector3(scale, scale * 5, scale);
            transform.position += -direction * speed;
            speed *= 1.02f;
            yield return new WaitForFixedUpdate();
        }
        transform.localScale = new Vector3(0.1f, 0.5f, 0.1f);
        transform.position = finalPosition;
        yield return null;
    }
}
