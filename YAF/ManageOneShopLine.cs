using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageOneShopLine : MonoBehaviour
{
    public ManageShopLines theShopContainer;
    public string item;
    public string desc;
    public double price;
    public double euroPrice;

    public TextMesh ourDesc;
    public TextMesh ourPrice;
    public bool isInfinite = false; // A rotated 8 ;)
    public bool isRotated = false;

    public Collider ourCollider;

    public ManageOneShopLine lineIRotate; // If this item is about buying a rotation, which line to rotate. Careful that it should be visible!

    private bool isRotating = false;

    const string rotatedSuffix = "Rot";

    Quaternion fromRotation = Quaternion.Euler(0, 0, 90);
    Quaternion toRotation = Quaternion.Euler(0, 0, 0);

    float speed = 0.2f;
    float initTime = 0;

    bool wasBought = false;
    bool isPlaying = false; // Is the sound playing?

    private AudioSource ourSound = null;
    private AudioReverbFilter ourReverb = null; // Some sound have reverb. We must destroy the reverb before destroying the sound.

    public enum Money
    {
        Regular,
        Actual,
        Hero,
        Diamonds
    }

    public static readonly Dictionary<Money, string> moneyInSave = new Dictionary<Money, string>
     {
         { Money.Regular, "Coins" },
            { Money.Actual, "Euros" },
            { Money.Hero, "Hero" },
            { Money.Diamonds, "Junk" }
     };

    public static readonly Dictionary<Money, string> moneyAbbr = new Dictionary<Money, string>
     {
         { Money.Regular, "Coins" },
            { Money.Actual, "€" },
            { Money.Hero, "SCoins" },
            { Money.Diamonds, "Diamonds" }
     };

    public Money usedMoney;

    Rigidbody ourRigidBody;

    public GameObject boughtPanel;

    public bool WasBought { get => wasBought; }

    private void Awake()
    {
        ourRigidBody = GetComponent<Rigidbody>();
        ourSound = GetComponent<AudioSource>();
        ourReverb = GetComponent<AudioReverbFilter>();

        ourRigidBody.isKinematic = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        ourDesc.text = desc;

        // Resize the description depending on the length
        float lengthText = desc.Length;
        if (lengthText > 20)
        {
            float sizeDesc = 0.05f * (1.1f* 20f/(lengthText));

            ourDesc.characterSize = sizeDesc;
        }

        string abbrvCurrency = moneyAbbr[usedMoney];

        // Show the price correctly
        if (isInfinite)
        {
            ourPrice.text = "8";

            // Check if rotate bought
            bool hasKey = LocalSave.HasBoolKey(item + rotatedSuffix);
            if (hasKey)
            {
                isRotated = LocalSave.GetBool(item + rotatedSuffix);
            }

            if (!isRotated)
            {
                ourPrice.transform.Rotate(fromRotation.eulerAngles);
            }
            else
            {
                ourPrice.text = "8 " + abbrvCurrency;
            }
        }
        else
        {
            if (usedMoney == Money.Actual)
            {
                ourPrice.text = euroPrice + " " + abbrvCurrency;
            }
            else
            {
                string priceAbbr = YAFFacility.AbbreviateNumber(price);

                ourPrice.text = priceAbbr + " " + abbrvCurrency;
            }
        }
        // Check then if bought
        //checkIfBought();
    }

    const int maxBought = 40; // Above that number we destroy instead, so all does not go super-slow.

    internal bool checkIfBought(int nbBought)
    {
        bool hasKeyBought = LocalSave.HasBoolKey(item);

        if (hasKeyBought)
        {
            wasBought = LocalSave.GetBool(item);

            if (wasBought)
            {
                if (nbBought > maxBought)
                {
                    Destroy(gameObject);
                }
                else
                {
                    ourRigidBody = GetComponent<Rigidbody>();

                    ourRigidBody.isKinematic = false;
                    ourCollider.enabled = true;

                    boughtPanel.SetActive(true);

                    if (lineIRotate != null)
                    {
                        // That was about buying a rotation (could have other perks)
                        lineIRotate.rotate();
                    }

                    if (ourReverb != null)
                    {
                        Destroy(ourReverb);
                    }
                    if (ourSound != null)
                    {
                        Destroy(ourSound);
                    }

                    return true;
                }
            }
        }
        return false;
    }

    float lastY;
    float destY;
    bool isTranslated = false;
    float initTimeTr = 0;

    internal void moveTo(float yLine)
    {
        lastY = transform.localPosition.y;
        destY = yLine;

        isTranslated = true;

        //ourCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (isTranslated)
        {
            initTimeTr += Time.deltaTime;

            transform.localPosition = Vector3.Lerp(new Vector3(transform.localPosition.x, lastY, transform.localPosition.z), new Vector3(transform.localPosition.x, destY, transform.localPosition.z), initTimeTr * speed * 4);

            if (Mathf.Abs(transform.localPosition.y - destY) < 0.01f)
            {
                isTranslated = false;

                //ourCollider.enabled = true;
            }
        }
        if (isInfinite)
        {
            if (isRotating)
            {
                initTime += Time.deltaTime;

                ourPrice.transform.rotation = Quaternion.Lerp(fromRotation, toRotation, initTime * speed);
            }
            if (Mathf.Abs(ourPrice.transform.rotation.eulerAngles.z - toRotation.eulerAngles.z) < 0.1f)
            {
                isRotating = false;

                string abbrvCurrency = moneyAbbr[usedMoney];

                ourPrice.text = "8 " + abbrvCurrency;
            }
        }

        if (isPlaying)
        {
            if (ourSound != null && !ourSound.isPlaying)
            {
                if (ourReverb != null)
                { 
                    Destroy(ourReverb, 1.9f);
                }
                Destroy(ourSound, 2);

                isPlaying = false;
            }
        }
    }

    private void FixedUpdate()
    {
        //if (Application.platform != RuntimePlatform.Android)
        //{
        //    Vector3 touchPos = Input.mousePosition;

        //    // Check if the user has clicked
        //    bool aTouch = Input.GetMouseButtonDown(0);

        //    if (aTouch)
        //    {
        //        // Debug.Log( "Moused moved to point " + touchPos );

        //        checkBoard(touchPos);
        //    }
        //}
        //else
        //{
        //    if (Input.touchCount >= 1)
        //    {
        //        Touch firstFinger = Input.GetTouch(0);

        //        if (Input.touchCount == 1)
        //        {
        //            Vector3 touchPos = firstFinger.position;

        //            if (firstFinger.phase != TouchPhase.Moved)
        //            {
        //                checkBoard(touchPos);
        //            }
        //        }
        //    }
        //}
    }

    void checkBoard(Vector3 touchPos)
    {
        Vector3 wp = Camera.main.ScreenToWorldPoint(touchPos);

        Ray ray = Camera.main.ScreenPointToRay(wp);

        RaycastHit raycastHit;
        Debug.Log("Something Hit ???");
        if (Physics.Raycast(ray, out raycastHit))
        {
            Debug.Log("Something Hit");
            if (raycastHit.collider == ourCollider)
            {
                buyIfPossible();
            }
        }
    }

    void OnMouseDown()
    {
        buyIfPossible();
    }

    private void buyIfPossible()
    {
        // Check if there is enough money, and if so, mark the item as bought.
        double nbOfCoins = 0;

        double savePrice = price; // If the price is in euro, save the sorting price here first

        string Key = moneyInSave[usedMoney];

        bool hasKey = LocalSave.HasDoubleKey(Key);
        if (hasKey)
        {
            nbOfCoins = LocalSave.GetDouble(Key);
        }
        else if (usedMoney == Money.Actual)
        {
            // First give some credit :)
            LocalSave.SetDouble(Key, 10000);

            LocalSave.Save();

            nbOfCoins = 10000;
        }

        if (usedMoney == Money.Actual)
        {
            price = euroPrice;
        }
        if (isInfinite)
        {
            if (isRotated)
            {
                price = 8;
            }
            else
            {
                nbOfCoins = 0;
            }
        }
        if (nbOfCoins > price)
        {
            LocalSave.SetBool(item, true);
            nbOfCoins -= price;

            LocalSave.SetDouble(Key, nbOfCoins);

            LocalSave.Save();

            wasBought = true;

            //transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, -2);

            ourRigidBody.isKinematic = false;
            ourCollider.enabled = true;

            boughtPanel.SetActive(true);

            if (lineIRotate != null)
            {
                // That was about buying a rotation (could have other perks)
                lineIRotate.rotate();
            }
            if (ourSound != null)
            {
                ourSound.Play();

                isPlaying = true;
            }
            theShopContainer.oneLineRemoved();
        }
        if (usedMoney == Money.Actual)
        {
            price = savePrice;
        }
    }

    public void rotate()
    {
        isRotating = true;
        isRotated = true;

        LocalSave.SetBool(item + rotatedSuffix, true);
    }
}