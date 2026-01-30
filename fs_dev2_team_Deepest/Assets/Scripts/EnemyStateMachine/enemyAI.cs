using UnityEngine;
using System.Collections;
using UnityEngine.AI;
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

    void Start()
    {
        scaleOrig = transform.localScale;

        if (model != null)
            colorOrig = model.material.color;

        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        if (model != null)
            mats = new List<Material>(model.materials);
        else
            mats = new List<Material>();

        if (throwCollider == null)
            throwCollider = GetComponent<BoxCollider>();

        HP = maxHp;
    }

    public void takeDamage(int amount)
    {
        if (anim != null && PlayerAnimatorManager.instance != null)
            PlayerAnimatorManager.instance.PlayTargetAnimation(anim, "enemySquash", .5f);

        if (!isStunned && HP <= maxHp / 2)
        {
            if (stunMat != null && model != null && !mats.Contains(stunMat))
            {
                mats.Add(stunMat);
                model.materials = mats.ToArray();
            }
            isStunned = true;
        }

        HP -= amount;

        if (agent != null && agent.enabled && !isStunned && !isGrabbed && !isThrown)
        {
            if (GameManager.instance != null && GameManager.instance.player != null)
                agent.SetDestination(GameManager.instance.player.transform.position);
        }

        if (audioSource != null && hurtSound != null)
            audioSource.PlayOneShot(hurtSound, .5f);

        if (HP <= 0)
        {
            if (audioSource != null && hurtSound != null)
            {
                audioSource.pitch = Random.Range(.5f, 1f);
                audioSource.PlayOneShot(hurtSound);
            }

            ShowFloatText(amount);

            if (GameManager.instance != null)
                GameManager.instance.RequestHitStop(.03f);

            StartCoroutine(FadeOut(1f));
        }
        else
        {
            ShowFloatText(amount);

            if (audioSource != null && hurtSound != null)
            {
                audioSource.pitch = Random.Range(.5f, 1f);
                audioSource.PlayOneShot(hurtSound);
            }

            StartCoroutine(flashRed(.05f));

            if (GameManager.instance != null)
                StartCoroutine(GameManager.instance.HitStop(.03f));
        }
    }

    void ShowFloatText(int amount)
    {
        if (UImanager.instance == null || UImanager.instance.floatingText == null || floatTextTrans == null)
            return;

        GameObject floatText = Instantiate(UImanager.instance.floatingText, floatTextTrans);
        TextMesh text = floatText.GetComponent<TextMesh>();
        if (text != null)
            text.text = amount.ToString();

        Destroy(floatText, 1f);
    }

    IEnumerator flashRed(float duration)
    {
        if (model != null)
        {
            model.material.color = Color.red;
            yield return new WaitForSeconds(duration);
            model.material.color = colorOrig;
        }
        else
        {
            yield return new WaitForSeconds(duration);
        }
    }

    public void Teleport()
    {
    }

    public void Grab(MagicController magicController)
    {
        isGrabbed = true;

        if (agent != null)
            agent.enabled = false;

        if (rb != null)
            rb.isKinematic = false;

        magicController.objectGrabbed = gameObject;
        magicController.throwObject = GetComponent<IThrow>();
    }

    IEnumerator FadeOut(float duration)
    {
        isStunned = false;

        if (model != null && redMat != null)
            model.material = redMat;

        if (model == null)
        {
            Destroy(gameObject);
            yield break;
        }

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

    public void Throw(MagicController magicController)
    {
        if (magicController.objectGrabbed != null)
            magicController.objectGrabbed.transform.SetParent(null);

        magicController.objectGrabbed = null;
        magicController.isTelegrabbing = false;

        if (rb != null)
            rb.AddForce(Camera.main.transform.forward * magicController.throwForce, ForceMode.Impulse);

        if (magicController.audSource != null && magicController.throwClip != null)
            magicController.audSource.PlayOneShot(magicController.throwClip);

        if (magicController.throwPref != null)
        {
            GameObject effect = Instantiate(magicController.throwPref, magicController.teleGrabLocation);
            Destroy(effect, 3f);
        }

        if (magicController.teleGrabPref != null)
            magicController.teleGrabPref.SetActive(false);

        if (throwCollider != null)
            throwCollider.enabled = true;

        isThrown = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isThrown)
            return;

        IDamage dmg = other.GetComponent<IDamage>();
        if (dmg != null)
            dmg.takeDamage(throwDamage);

        StartCoroutine(FadeOut(2f));
    }
}
