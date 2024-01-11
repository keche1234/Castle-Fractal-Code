using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Splitting : MonoBehaviour
{
    [Header("Basic Properties")]
    [SerializeField] protected Projectile splitPrefab;
    [SerializeField] protected int num;
    [SerializeField] protected float spread;
    [SerializeField] protected float size; //transform
    [SerializeField] protected float power;
    protected bool splitActivated = false;

    [Header("Hitbox Properties")]
    [SerializeField] protected Character source;
    [SerializeField] protected string targetTag;

    [Header("Recursive Properties")]
    [SerializeField] protected bool recursive;
    [SerializeField] protected int splitsLeft;

    [SerializeField] public SizePattern amount;
    [SerializeField] protected int amountDelta;
    protected LinkedListNode<int> customAmount;

    [SerializeField] public SizePattern degrees;
    [SerializeField] protected float degreesDelta;
    protected LinkedListNode<float> customDegrees;

    [SerializeField] public SizePattern scale;
    [SerializeField] protected float scaleDelta;
    protected LinkedListNode<float> customScale;

    [SerializeField] public SizePattern pow;
    [SerializeField] protected float powDelta;
    protected LinkedListNode<float> customPow;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    /*
     * Source and Target Tag
     */
    public void SetSource(Character c)
    {
        source = c;
        transform.parent = source.GetRoomManager().GetCurrent().transform;
    }

    public void SetTargetTag(string t)
    {
        targetTag = t;
    }

    /*
     * Num/Amount Functions
     */
    public void SetNum(int n)
    {
        num = n;
    }

    public void SetAmountDelta(int sd)
    {
        amountDelta = sd;
    }

    public void SetCustomAmount(LinkedListNode<int> ca)
    {
        customAmount = ca;
        if (ca != null)
            splitsLeft = 0;
    }

    /*
     * Spread/Degrees Functions
     */
    public void SetSpread(float s)
    {
        spread = s;
    }

    public void SetDegreesDelta(float dd)
    {
        degreesDelta = dd;
    }

    public void SetCustomDegrees(LinkedListNode<float> cd)
    {
        customDegrees = cd;
        if (cd != null)
            splitsLeft = 0;
    }

    /*
     * Size/Scale Functions
     */
    public void SetSize(float s)
    {
        size = s;
        transform.localScale = new Vector3(size, size, size);
    }

    public void SetScaleDelta(float sd)
    {
        scaleDelta = sd;
    }

    public void SetCustomScale(LinkedListNode<float> cs)
    {
        customScale = cs;
        if (cs != null)
            splitsLeft = 0;
    }

    /*
     * Pow/Power Functions
     */
    public void SetPower(float p)
    {
        power = p;
        if (gameObject.GetComponent<Explosive>() != null)
        {
            gameObject.GetComponent<Explosive>().SetExplosionMod(power);
        }
        {
            gameObject.GetComponent<Projectile>().SetDamageMod(power);
        }
    }

    public void SetPowDelta(float pd)
    {
        powDelta = pd;
    }

    public void SetCustomPow(LinkedListNode<float> cp)
    {
        customPow = cp;
        if (cp != null)
            splitsLeft = 0;
    }

    public void SetSplitsLeft(int sl)
    {
        splitsLeft = sl;
    }

    public void OnTriggerEnter(Collider collider)
    {
        if (enabled)
        {
            if (collider.gameObject.CompareTag("Wall") && !splitActivated)
            {
                if (splitsLeft > 0)
                {
                    splitActivated = true;
                    for (int i = 0; i < num; i++)
                    {
                        //Basic setup
                        Projectile piece = (Projectile)Instantiate(splitPrefab, transform.position, transform.rotation);
                        Vector3 norm = transform.position - collider.ClosestPoint(transform.position);

                        if (Mathf.Abs(norm.z) < 0.05f) //left or right wall
                            piece.gameObject.transform.rotation = Quaternion.LookRotation(new Vector3(-transform.forward.x, 0, transform.forward.z));
                        else //top or bottom wall
                            piece.gameObject.transform.rotation = Quaternion.LookRotation(new Vector3(transform.forward.x, 0, -transform.forward.z));

                        if (num > 1)
                            piece.transform.Rotate(0, (-spread / 2) + (i * spread / (num - 1)), 0);

                        piece.transform.Translate(-transform.forward, Space.World);

                        //Splitting properties
                        if (piece.gameObject.GetComponent<Splitting>() != null && recursive)
                        {
                            Splitting pieceSplit = piece.GetComponent<Splitting>();
                            pieceSplit.SetSplitsLeft(splitsLeft - 1);
                            //Set the piece's num
                            switch (amount)
                            {
                                case SizePattern.Linear:
                                    pieceSplit.SetNum(num + amountDelta);
                                    pieceSplit.SetAmountDelta(amountDelta);
                                    break;
                                case SizePattern.Geometric:
                                    pieceSplit.SetNum(num * amountDelta);
                                    pieceSplit.SetAmountDelta(amountDelta);
                                    break;
                                default:
                                    pieceSplit.SetNum(customAmount.Value);
                                    pieceSplit.SetCustomAmount(customAmount.Next);
                                    break;
                            }

                            //Set the piece's spread
                            switch (degrees)
                            {
                                case SizePattern.Linear:
                                    pieceSplit.SetSpread(spread + degreesDelta);
                                    pieceSplit.SetDegreesDelta(degreesDelta);
                                    break;
                                case SizePattern.Geometric:
                                    pieceSplit.SetSpread(num * degreesDelta);
                                    pieceSplit.SetDegreesDelta(degreesDelta);
                                    break;
                                default:
                                    pieceSplit.SetSpread(customDegrees.Value);
                                    pieceSplit.SetCustomDegrees(customDegrees.Next);
                                    break;
                            }

                            //Set the piece's scale
                            switch (scale)
                            {
                                case SizePattern.Linear:
                                    pieceSplit.SetSize(size + scaleDelta);
                                    pieceSplit.SetScaleDelta(scaleDelta);
                                    break;
                                case SizePattern.Geometric:
                                    pieceSplit.SetSize(size * scaleDelta);
                                    pieceSplit.SetScaleDelta(scaleDelta);
                                    break;
                                default:
                                    pieceSplit.SetSize(customScale.Value);
                                    pieceSplit.SetCustomScale(customScale.Next);
                                    break;
                            }

                            //Set the piece's pow
                            switch (pow)
                            {
                                case SizePattern.Linear:
                                    pieceSplit.SetPower(power + powDelta);
                                    pieceSplit.SetPowDelta(powDelta);
                                    break;
                                case SizePattern.Geometric:
                                    pieceSplit.SetPower(power * powDelta);
                                    pieceSplit.SetPowDelta(powDelta);
                                    break;
                                default:
                                    pieceSplit.SetPower(customPow.Value);
                                    pieceSplit.SetCustomPow(customPow.Next);
                                    break;
                            }

                            if (piece.GetComponent<Explosive>() != null)
                            {
                                ((Explosive)piece).SetSource(source);
                                ((Explosive)piece).SetTargetTag(targetTag);
                            }
                            else
                            {
                                piece.SetSource(source);
                                piece.SetTargetTag(targetTag);
                            }

                            pieceSplit.amount = amount;
                            pieceSplit.degrees = degrees;
                            pieceSplit.scale = scale;
                            pieceSplit.pow = pow;

                            pieceSplit.SetSource(source);
                            pieceSplit.SetTargetTag(targetTag);
                        }
                    }
                }
                //Destroy(gameObject);
            }
        }
    }

    public enum SizePattern
    {
        Linear,
        Geometric,
        Custom
    }
}
