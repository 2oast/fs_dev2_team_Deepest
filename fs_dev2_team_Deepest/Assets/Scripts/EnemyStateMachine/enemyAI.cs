using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class enemyAI : MonoBehaviour, IDamage, IGrab, ITeleport, IThrow
{
    [SerializeField] int maxHp;
    [SerializeField] int HP;
    [SerializeField] int throwDamage = 15;

    [SerializeField] BoxCollider throwCollider;
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
    bool isThrown = false;
    bool isHit;

    Color colorOrig;

    Rigidbody rb;

    Vector3 scaleOrig;

    NavMeshAgent agent;

    [SerializeField] Transform floatTextTrans;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scaleOrig = transform.localScale;
        colorOrig = model.material.color;
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        mats = new List<Material>(model.materials);
        throwCollider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {
      
    }


    public void takeDamage(int amount)
    {
        PlayerAnimatorManager.instance.PlayTargetAnimation(anim, "enemySquash", .5f);

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
            audioSource.pitch = Random.Range(.5f, 1);
            audioSource.PlayOneShot(hurtSound);
            GameObject floatText = Instantiate(UImanager.instance.floatingText, floatTextTrans);
            TextMesh text = floatText.GetComponent<TextMesh>();
            text.text = amount.ToString();
            Destroy(floatText, 1f);
            StartCoroutine(GameManager.instance.HitStop(.03f));
            StartCoroutine(FadeOut(1));
        }
        else
        {
            GameObject floatText = Instantiate(UImanager.instance.floatingText, floatTextTrans);
            TextMesh text = floatText.GetComponent<TextMesh>();
            text.text = amount.ToString();
            floatTextTrans.transform.LookAt(GameManager.instance.player.transform.position);
            audioSource.pitch = Random.Range(.5f, 1);
            audioSource.PlayOneShot(hurtSound);
            Destroy(floatText, 1f);
            StartCoroutine(flashRed(.5F));
            StartCoroutine(GameManager.instance.HitStop(.03f));
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
        magicController.throwObject = GetComponent<IThrow>();
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

    IEnumerator Knockback(float duration, float force)
    {
        rb.isKinematic = false;
        agent.enabled = false;
        Vector3 direction = (transform.position - GameManager.instance.player.transform.position).normalized;
        direction.y = 0;
        rb.AddForce(direction * force, ForceMode.Impulse);

        yield return new WaitForSeconds(duration);

        rb.isKinematic = true;
        agent.enabled = true;
        
    }

    public void Throw(MagicController magicController)
    {
        magicController.objectGrabbed.transform.SetParent(null);

        magicController.objectGrabbed = null;
        magicController.isTelegrabbing = false;

        rb.AddForce(Camera.main.transform.forward * magicController.throwForce, ForceMode.Impulse);

        magicController.audSource.PlayOneShot(magicController.throwClip);
        GameObject effect = Instantiate(magicController.throwPref, magicController.teleGrabLocation);
        Destroy(effect, 3);
        magicController.teleGrabPref.SetActive(false);
        throwCollider.enabled = true;
        isThrown = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(isThrown)
        {
            IDamage dmg = other.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(throwDamage);
                StartCoroutine(FadeOut(2));
            }

            StartCoroutine(FadeOut(3));
        }
       

    }
}