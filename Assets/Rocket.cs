using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Rocket : MonoBehaviour
{
    public float v;
    public float engineStrength;
    public float monoPropellantCapacity;
    public float monoPropellant;
    public float propellantConsumption;
    public float fuelCapacity;
    public float fuel;
    public float fuelConsumption;
    public float mass;
    public float tiltStrength;
    public float gravity;
    public Text fuelText;
    public Text monoPropellantText;
    public GameObject gameState;
    public GameObject landingPad;
    public GameObject bubble;
    public GameObject arrow;
    private Rigidbody2D rb;
    private Vector2 startPosition;
    private Quaternion startRotation;
    public bool finished;
    public bool outOfBounds;
    private const int mld = 10000000;
    private Rect leftInputField = new Rect(
        0, 0, 
        Screen.width / 4, Screen.height / 3);

    private Rect middleInputField = new Rect(
        Screen.width / 4, 0,
        Screen.width / 4, Screen.height / 3);

    private Rect rightInputField = new Rect(
        Screen.width / 2, 0,
        Screen.width / 2, Screen.height / 3);

    // Use this for initialization
    void Start()
    {
        if (System.IO.File.Exists(Application.persistentDataPath + "/params.json"))
        {
            StreamReader sr = new StreamReader("params.json");
            string s = sr.ReadToEnd();
            sr.Close();
            JsonUtility.FromJsonOverwrite(s, this);
        }
        else
        {
            string json = "{\"engineStrength\":5.0,\"fuelCapacity\":12.0,\"fuelConsumption\":0.05,\"mass\":2.0,\"tiltStrength\":0.2,\"gravity\":0.15,\"monoPropellantCapacity\":5.0,\"propellantConsumption\":0.02}";
            JsonUtility.FromJsonOverwrite(json, this);
        }

        rb = gameObject.GetComponent<Rigidbody2D>();
        updateRigidbody();

        startPosition = transform.position;
        startRotation = transform.rotation;

        bubble.transform.position = new Vector3(mld, mld, mld);

        reset();
    }

    void updateRigidbody()
    {
        rb.gravityScale = gravity;
        rb.mass = mass;
    }

    enum Direction
    {
        COUNTER_CLOCKWISE, CLOCKWISE
    }

    // Update is called once per frame
    void tilt(Direction direction)
    {
        if (monoPropellant > 0)
        {
            float tilt = (direction == Direction.CLOCKWISE ? 1 : -1) * tiltStrength / 10;
            rb.AddForceAtPosition(transform.right * tilt, transform.TransformPoint(0, 2, 0));
            monoPropellant -= propellantConsumption;
            updateMonoPropellantText();
        }
    }

    void updateFuelText()
    {
        fuelText.text = "Fuel: " + (fuel > 0 ? fuel : 0).ToString("F2");
    }

    void updateMonoPropellantText()
    {
        monoPropellantText.text = "Mono Propellant: " + (monoPropellant > 0 ? monoPropellant : 0).ToString("F2");
    }

    void accelerate()
    {
        if (fuel > 0)
        {
            rb.AddRelativeForce(new Vector2(0, engineStrength));
            fuel -= fuelConsumption;
            updateFuelText();
        }
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (finished)
        {
            return;
        }
        if (collision.gameObject.name == "Landing Pad")
        {
            if (collision.relativeVelocity.y < 1 && Mathf.Abs(transform.rotation.z) < 15)
            {
                gameState.GetComponent<GameState>().win();
            }
            else 
            {
                gameState.GetComponent<GameState>().lose();
            }
            finished = true;
            rb.Sleep();
        }
        if (collision.gameObject.name == "Colision Cheater")
        {
            gameState.GetComponent<GameState>().lose();
            finished = true;
            rb.Sleep();
        }
    }

    public void reset()
    {
        transform.position = startPosition;
        transform.rotation = startRotation; 
        fuel = fuelCapacity;
        monoPropellant = monoPropellantCapacity;
        finished = false;
        updateFuelText();
        updateMonoPropellantText();
        rb.WakeUp();
        landingPad.GetComponent<LandingPadScript>().randomize();
    }

    void processInput()
    {
        foreach (Touch i in Input.touches)
        {
            if (leftInputField.Contains(i.position))
            {
                tilt(Direction.COUNTER_CLOCKWISE);
            }
            else if (middleInputField.Contains(i.position))
            {
                tilt(Direction.CLOCKWISE);
            }
            else if (rightInputField.Contains(i.position))
            {
                accelerate();
            }
        }
        if (Input.GetMouseButton(0))
        {
            Vector2 pos = Input.mousePosition;
            if (leftInputField.Contains(pos))
            {
                tilt(Direction.COUNTER_CLOCKWISE);
            }
            else if (middleInputField.Contains(pos))
            {
                tilt(Direction.CLOCKWISE);
            }
            else if (rightInputField.Contains(pos))
            {
                accelerate();
            }
        }
        if (Input.GetKey(KeyCode.W))
        {
            accelerate();
        }
        if (Input.GetKey(KeyCode.A))
        {
            tilt(Direction.COUNTER_CLOCKWISE);
        }
        if (Input.GetKey(KeyCode.D))
        {
            tilt(Direction.CLOCKWISE);
        }
    }

    void Update()
    {
        v = rb.velocity.y;
        if (!finished)
        {
            processInput();
        }

        if (outOfBounds)
        {
            Vector2 pos = Camera.main.WorldToScreenPoint(transform.position);
            Vector2 bub = Camera.main.WorldToScreenPoint(bubble.GetComponent<SpriteRenderer>().bounds.size);
            Vector2 magicNumber = new Vector2(6,8);
            Vector3 newBubblePosition = new Vector3();
            if (pos.x > Screen.width - bub.x / magicNumber.x)
            {
                newBubblePosition.x = Screen.width - bub.x / magicNumber.x;
            }
            else if (pos.x < bub.x / magicNumber.x)
            {
                newBubblePosition.x = bub.x / magicNumber.x;
            }
            else
            {
                newBubblePosition.x = pos.x;
            }
            if (pos.y > Screen.height - bub.y / magicNumber.y)
            {
                newBubblePosition.y = Screen.height - bub.y / magicNumber.y;
            }
            else
            {
                newBubblePosition.y = pos.y;
            }
            newBubblePosition.z = 0;
            bubble.transform.position = Camera.main.ScreenToWorldPoint(newBubblePosition);
            bubble.transform.position = new Vector3(bubble.transform.position.x, bubble.transform.position.y, 0);
            arrow.transform.position = bubble.transform.position;
            
            Vector3 direction = transform.position - arrow.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
            arrow.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    void OnBecameInvisible()
    {
        outOfBounds = true;
    }

    void OnBecameVisible()
    {
        outOfBounds = false;
        bubble.transform.position = new Vector3(mld, mld, mld);
        arrow.transform.position = new Vector3(mld, mld, mld);
    }
}
