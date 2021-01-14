using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenguinColorScript : MonoBehaviour
{
    public List<SpriteRenderer> penColors;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Hide();
        }
    }

    public void Hide()
    {
        GetComponent<CircleCollider2D>().enabled = false;
        StartCoroutine(DelayAndHide());
    }
    IEnumerator DelayAndHide()
    {
        yield return new WaitForSeconds(0.25f);
        // Give the penguin a new color and move its position within the window
        Color newColor = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);

        for (int i = 0; i < penColors.Count; i++)
        {
            penColors[i].color = newColor;
        }

        transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Random.Range(100, Screen.width - 100), Random.Range(100, Screen.height - 100), Camera.main.farClipPlane / 2));

        GetComponent<CircleCollider2D>().enabled = true;

        yield return null;
    }
}
