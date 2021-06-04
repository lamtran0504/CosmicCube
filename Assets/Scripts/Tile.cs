using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour {

    private Dust[,,] dustParticles;
    private int a, b, c;
    public int x { get; set; }
    public int y { get; set; }
    [SerializeField]
    private Material tileMaterial, pressedMaterial;

    private bool mouseDown;

    public void Form(float delay) {
        GetComponent<MeshRenderer>().enabled = false;

        dustParticles = new Dust[10, 2, 10];
        a = 10;
        b = 2;
        c = 10;
        for (int i = 0; i < a; i++) {
            for (int j = 0; j < b; j++) {
                for (int k = 0; k < c; k++) {
                    Vector3 pos = new Vector3(x * LevelManager.TILE_SIZE - 0.45f + 0.1f * i, -0.05f + 0.1f * j, y * LevelManager.TILE_SIZE - 0.45f + 0.1f * k);
                    GameObject dust = Instantiate(AnimationManager.Instance.dustPrefab, pos + LevelManager.Instance.MapStart, Quaternion.identity);
                    dust.transform.SetParent(transform);
                    dust.transform.localScale = Vector3.zero;
                    dustParticles[i, j, k] = dust.GetComponent<Dust>();
                }
            }
        }
        name = "DustToTile";

        StartCoroutine("IForm", delay);
    }

    private IEnumerator IForm(float delay) {
        yield return new WaitForSeconds(delay);
        float t = 0.21f;
        for (int ac = 0; ac <= a + c - 2; ac++) {
            for (int a = Mathf.Max(ac - c + 1, 0); a < Mathf.Min(this.a, ac + 1); a++) {
                for (int b = 0; b < this.b; b++) {
                    dustParticles[a, b, ac - a].Appear(Utilities.rand.Next(10) * t * 3 / 10);
                }
            }
            t -= 0.01f;
            yield return new WaitForSeconds(t);
        }
        while (transform.childCount > 0)
            yield return new WaitForEndOfFrame();
        Destroy(gameObject);
        yield return null;
    }

    public void Disintegrate(float delay) {
        GetComponent<MeshRenderer>().enabled = false;

        dustParticles = new Dust[10, 2, 10];
        a = 10;
        b = 2;
        c = 10;
        for (int i = 0; i < a; i++) {
            for (int j = 0; j < b; j++) {
                for (int k = 0; k < c; k++) {
                    Vector3 pos = new Vector3(x * LevelManager.TILE_SIZE - 0.45f + 0.1f * i, -0.05f + 0.1f * j, y * LevelManager.TILE_SIZE - 0.45f + 0.1f * k);
                    GameObject dust = Instantiate(AnimationManager.Instance.dustPrefab, pos + LevelManager.Instance.MapStart, Quaternion.identity);
                    dust.transform.SetParent(transform);
                    dustParticles[i, j, k] = dust.GetComponent<Dust>();
                }
            }
        }
        name = "TileFromDust";

        StartCoroutine("IDisintegrate", delay);
    }

    private IEnumerator IDisintegrate(float delay) {
        yield return new WaitForSeconds(delay);
        float t = 0.21f;
        for (int ac = a + c - 2; ac >= 0; ac--) {
            for (int a = Mathf.Max(ac - c + 1, 0); a < Mathf.Min(this.a, ac + 1); a++) {
                for (int b = 0; b < this.b; b++) {
                    dustParticles[a, b, ac - a].Disappear(Utilities.rand.Next(10) * t * 3 / 10);
                }
            }
            t -= 0.01f;
            yield return new WaitForSeconds(t);
        }
        while (transform.childCount > 0)
            yield return new WaitForEndOfFrame();
        //Destroy(gameObject);
        yield return null;
    }

    public void LightOn() {
        GetComponent<MeshRenderer>().material = pressedMaterial;
    }

    public void LightOff() {
        GetComponent<MeshRenderer>().material = tileMaterial;
    }

}
