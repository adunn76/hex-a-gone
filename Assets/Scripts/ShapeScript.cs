using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShapeScript : MonoBehaviour
{
    // Define what's needed to make the shape
    public class Shape
    {
        public Vector2[] vertices;
        public ushort[] triangles;

        public Shape(Vector2[] verts, ushort[] tris)
        {
            vertices = verts;
            triangles = tris;
        }
    }

    public TMP_Dropdown shapeSelect; // Dropdown for identifying the selected shape
    public Color shapeColor; // Color assigned to the shape
    public int textureSize = 4; // Pixel length and width of the texture

    public SpriteMask mask; // Sprite Mask used for finding objects
    public GameObject danceButton; // Button that allows for dancing
    public Material rainbowMat; // Rainbow material for objects
    public Material blankMat; // Blank material for objects

    public AudioClip changeSound; // Sound for changes
    public AudioClip foundSound; // Sound for finding the penguin

    public Shape[] shapes; // Array of shapes used

    public GameplayScript gameplayScript;   // Reference to the gameplay script

    SpriteRenderer spriteRend; // Sprite Renderer on object
    PolygonCollider2D pCol; // Polygon Collider on object

    public bool shapeDance = false; // Check if the shape is dancing or not
    bool doubleClick = false; // Check for double-click
    bool holdCheck = false; // Check if the mouse has been held down long enough
    bool mouseIsDown = false; // Check if the mouse is currently being held down


    void Start()
    {
        // Grab the necessary components
        spriteRend = GetComponent<SpriteRenderer>();
        pCol = GetComponent<PolygonCollider2D>();

        // Define the values for each shape option
        shapes = new Shape[shapeSelect.options.Count];

        // Hexagon
        shapes[0] = new Shape(
            new Vector2[] {
                new Vector2(-0.5f, -(Mathf.Sqrt(3f)/2f)),
                new Vector2(-1f, 0f),
                new Vector2(-0.5f, (Mathf.Sqrt(3f)/2f)),
                new Vector2(0.5f, (Mathf.Sqrt(3f)/2f)),
                new Vector2(1f, 0f),
                new Vector2(0.5f, -(Mathf.Sqrt(3f)/2f))},
            new ushort[] {
                0, 1, 5,
                1, 2, 5,
                2, 3, 4,
                2, 4, 5});

        // Pentagon
        shapes[1] = new Shape(
            new Vector2[] {
                new Vector2(-(Mathf.Sin(4*Mathf.PI/5)/Mathf.Sin(2*Mathf.PI/5)), -(Mathf.Cos(Mathf.PI/5)/Mathf.Sin(2*Mathf.PI/5))),
                new Vector2(-1f, Mathf.Cos(2*Mathf.PI/5)/Mathf.Sin(2*Mathf.PI/5)),
                new Vector2(0f, 1f/Mathf.Sin(2*Mathf.PI/5)),
                new Vector2(1f, Mathf.Cos(2*Mathf.PI/5)/Mathf.Sin(2*Mathf.PI/5)),
                new Vector2(Mathf.Sin(4*Mathf.PI/5)/Mathf.Sin(2*Mathf.PI/5), -(Mathf.Cos(Mathf.PI/5)/Mathf.Sin(2*Mathf.PI/5)))},
            new ushort[] {
                0, 1, 4,
                1, 2, 4,
                2, 3, 4});

        // Square
        shapes[2] = new Shape(
            new Vector2[] {
                new Vector2(-1f, -1f),
                new Vector2(-1f, 1f),
                new Vector2(1f, 1f),
                new Vector2(1f, -1f)},
            new ushort[] {
                0, 1, 2,
                2, 3, 0});

        // Triangle
        shapes[3] = new Shape(
            new Vector2[] {
                new Vector2(-1, -(Mathf.Sqrt(3f)/2f)),
                new Vector2(0f, (Mathf.Sqrt(3f)/2f)),
                new Vector2(1, -(Mathf.Sqrt(3f)/2f))},
            new ushort[] { 0, 1, 2});

        // Create a shape to start with
        DrawShape();
    }

    public void Dance()
    {
        // Turn dancing effects on or off
        shapeDance = !shapeDance;
        if (shapeDance)
        {
            GetComponent<Animator>().enabled = true;
            danceButton.GetComponent<Image>().material = blankMat;
            danceButton.GetComponentInChildren<TextMeshProUGUI>().text = "Stop";
            danceButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.black;
            spriteRend.material = rainbowMat;
            GetComponent<AudioSource>().Play();

        }
        else
        {
            StopDance();
        }
    }

    public void StopDance()
    {
        // Allows dance to be stopped if game is running
        GetComponent<Animator>().enabled = false;
        danceButton.GetComponent<Image>().material = rainbowMat;
        danceButton.GetComponentInChildren<TextMeshProUGUI>().text = "Dance!";
        danceButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
        transform.localEulerAngles = Vector3.zero;
        transform.localScale = Vector3.one;
        spriteRend.material = blankMat;
        spriteRend.color = shapeColor;
        GetComponent<AudioSource>().Stop();
    }

    IEnumerator CheckDoubleClick()
    {
        // Set a double click active within the time restraints
        if (!doubleClick)
        {
            doubleClick = true;
            yield return new WaitForSeconds(0.2f);
            doubleClick = false;
        }
        yield return null;
    }

    IEnumerator CheckHold()
    {
        // Set a double click active within the time restraints
        if (!holdCheck && !mouseIsDown)
        {
            yield return new WaitForSeconds(0.1f);
            if (mouseIsDown)
            {
                holdCheck = true;
            }
        }
        yield return null;
    }

    void OnMouseDown()
    {
        // Change the shape to a random color if the shape is double clicked and not dancing
        if (doubleClick && !shapeDance && !gameplayScript.gameStart)
        {
            GetComponent<AudioSource>().PlayOneShot(changeSound);
            shapeColor = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
            spriteRend.color = shapeColor;
            doubleClick = false;
        }
        else
        {
            StartCoroutine(CheckDoubleClick());
        }
    }

    private void OnMouseUp()
    {
        mouseIsDown = false;
        holdCheck = false;
    }

    void OnMouseDrag()
    {
        if (holdCheck)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos = new Vector3(mousePos.x, mousePos.y, 0);
            transform.position = mousePos;
        }
        else
        {
            StartCoroutine(CheckHold());
            mouseIsDown = true;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (gameplayScript.timerActive)
        {
            gameplayScript.score++;
            GetComponent<AudioSource>().PlayOneShot(foundSound);
            col.GetComponent<PenguinColorScript>().Hide();
        }
    }

    public void DrawShape()
    {
        // Set the shape color and shape
        shapeColor = Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.5f, 1f);
        Shape newShape = shapes[shapeSelect.value];

        // Create a texture to be utilized for the sprite
        Texture2D texture = new Texture2D(textureSize, textureSize);

        // Create an array to fill the empty pixels white
        List<Color> cols = new List<Color>();
        for (int i = 0; i < (texture.width * texture.height); i++)
        {
            cols.Add(Color.white);
        }
        texture.SetPixels(cols.ToArray());
        texture.Apply();

        // Apply the actual color to the sprite renderer
        spriteRend.color = shapeColor;

        // Create the sprite
        spriteRend.sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), 1f);

        // Retrieve offset values
        Vector2 minVals = Vector2.zero;
        float maxY = 0f;

        foreach (Vector2 vi in newShape.vertices)
        {
            if (vi.x < minVals.x)
                minVals.x = vi.x;
            if (vi.y < minVals.y)
                minVals.y = vi.y;
            if (vi.y >= maxY)
                maxY = vi.y;
        }

        // The offset becomes the min values becuase we want to draw the shape at (0,0) on the texture
        Vector2 offset = minVals;

        /* For all shapes but the square, the height of the shape is shorter than
         * its width. This calculates the offset of that height to properly center
         * the shape within the texture boundaries. */
        if (newShape != shapes[2] && maxY - minVals.y < 2f)
            offset.y -= (2f - (maxY - minVals.y)) / 2f;

        // Special offset for Pentagon's collider points
        Vector2 pentOff = new Vector2(0f, ((1 / Mathf.Sin(2f * Mathf.PI / 5f)) - ((1 + Mathf.Cos(Mathf.PI / 5f)) / (2 * Mathf.Sin(2f * Mathf.PI / 5f)))));

        // Localize the coordinate and set the collider points
        Vector2[] loCords = new Vector2[newShape.vertices.Length];
        Vector2[] colPoints = new Vector2[newShape.vertices.Length];

        for (int i = 0; i < newShape.vertices.Length; i++)
        {
            loCords[i] = (newShape.vertices[i] - offset) * ((float)textureSize / 2f);
            // Check if the object is a pentagon to apply the additional offset
            colPoints[i] = (shapeSelect.value == 1) ? (newShape.vertices[i] - pentOff) * ((float)textureSize / 2f) : newShape.vertices[i] * ((float)textureSize / 2f);
        }

        // Create the shape and assign the points to the collider
        spriteRend.sprite.OverrideGeometry(loCords, newShape.triangles);
        pCol.points = colPoints;

        // Assign the new sprite to the mask
        mask.sprite = spriteRend.sprite;

        // Reset the object's position
        transform.position = Vector3.zero;

        // Sound indicator the object has changed
        GetComponent<AudioSource>().PlayOneShot(changeSound);
    }
}

