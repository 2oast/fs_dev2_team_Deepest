using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class enemyAI : MonoBehaviour, IDamage, IGrab, ITeleport
{
    [SerializeField] int maxHp;
    [SerializeField] int HP;
    [SerializeField] Renderer model;
    [SerializeField] Transform meshTrans;
    [SerializeField] Animator anim;
    [SerializeField] Material stunMat;
    [SerializeField] Material redMat;
    List<Material> mats;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip spitSound;
    [SerializeField] AudioClip hurtSound;

    public bool isStunned;
    public bool isGrabbed;
    bool isHit;

    Color colorOrig;

    

    Vector3 scaleOrig;

    NavMeshAgent agent;

    [SerializeField] Transform floatTextTrans;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scaleOrig = transform.localScale;
        colorOrig = model.material.color;
        agent = GetComponent<NavMeshAgent>();

        mats = new List<Material>(model.materials);
    }

    // Update is called once per frame
    void Update()
    {
      
    }


    public void takeDamage(int amount)
    {

        if(HP <= maxHp / 2)
        {
            mats.Add(stunMat);
            model.materials = mats.ToArray();
            isStunned = true;
        }
        

        HP -= amount;
        agent.SetDestination(GameManager.instance.player.transform.position);
        audioSource.PlayOneShot(hurtSound, .5f);
        if (HP <= 0)
        {
            StartCoroutine(SquashStretch(
        new Vector3(1.2f, 0.7f, 1.2f),
         0.08f,
         0.12f
         ));
            audioSource.pitch = Random.Range(.5f, 1);
            audioSource.PlayOneShot(hurtSound);
            StartCoroutine(FadeOut(1));
        }
        else
        {
            StartCoroutine(SquashStretch(
        new Vector3(1.2f, 0.7f, 1.2f),
         0.08f,
         0.12f
         ));
            GameObject floatText = Instantiate(UImanager.instance.floatingText, floatTextTrans);
            TextMesh text = floatText.GetComponent<TextMesh>();
            text.text = amount.ToString();
            floatTextTrans.transform.LookAt(GameManager.instance.player.transform.position);
            audioSource.pitch = Random.Range(.5f, 1);
            audioSource.PlayOneShot(hurtSound);
            Destroy(floatText, 1f);
            StartCoroutine(flashRed(.5F));
            StartCoroutine(GameManager.instance.HitStop(.05f));
        }
    }

    IEnumerator flashRed(float duration)
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        model.material.color = colorOrig;
        
    }

    public void Teleport()
    {
    }

    public void Grab(MagicController magicController)
    {
        isGrabbed = true;

        if (agent != null)
            agent.enabled = false;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        magicController.objectGrabbed = gameObject;
    }

    IEnumerator FadeOut(float duration)
    {
        isStunned = false;
        model.material = redMat;

        Material mat = model.material;

        Color startColor = mat.color;
        float startAlpha = startColor.a;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;

            Color c = startColor;
            c.a = Mathf.Lerp(startAlpha, 0f, t);
            mat.color = c;

            yield return null;
        }

        Destroy(gameObject);
    }

    public IEnumerator SquashStretch(
    Vector3 squashScale,
    float squashTime,
    float returnTime
)
    {
        Transform t = meshTrans; // NOT the root if it has physics
        Vector3 originalScale = t.localScale;

        float timer = 0f;
        while (timer < squashTime)
        {
            timer += Time.deltaTime;
            t.localScale = Vector3.Lerp(originalScale, squashScale, timer / squashTime);
            yield return null;
        }

        timer = 0f;
        while (timer < returnTime)
        {
            timer += Time.deltaTime;
            t.localScale = Vector3.Lerp(squashScale, originalScale, timer / returnTime);
            yield return null;
        }
    }

}