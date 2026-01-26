using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class BossStats : MonoBehaviour, IDamage
{
    [SerializeField] int maxHP;
    [SerializeField] int health;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip spitSound;
    [SerializeField] AudioClip hurtSound;

    [SerializeField] Transform floatTextTrans;

    Color colorOrig;

    [SerializeField] Image bossHealthBar;

    public Material redMat;

    public Renderer model;

    private void Start()
    {
        colorOrig = model.material.color;
    }

    public void takeDamage(int amount)
    {
       
        health -= amount;
        audioSource.PlayOneShot(hurtSound, .5f);
        if (health <= 0)
        {
            audioSource.pitch = Random.Range(.5f, 1);
            audioSource.PlayOneShot(hurtSound);
            GameObject floatText = Instantiate(UImanager.instance.floatingText, floatTextTrans);
            TextMesh text = floatText.GetComponent<TextMesh>();
            text.text = amount.ToString();
            Destroy(floatText, 1f);
            StartCoroutine(GameManager.instance.HitStop(.1f));
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
            StartCoroutine(GameManager.instance.HitStop(.05f));
        }
    }

    IEnumerator FadeOut(float duration)
    {
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

    IEnumerator flashRed(float duration)
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.05f);
        model.material.color = colorOrig;

    }

    private void Update()
    {
        bossHealthBar.fillAmount = (float)health / maxHP;

        if(health <= 0)
        {
            GameManager.instance.menuActive = GameManager.instance.menuWin;
            GameManager.instance.StatePause();
            GameManager.instance.menuWin.SetActive(true);
        }
    }


    
}
