using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BonusView : MonoBehaviour
{
    private Image m_Image = null;
    private Material auxMat = null;
    private void OnEnable()
    {
        PrepareStartup();
        InitiateGlowAnimation();
    }

    private void PrepareStartup()
    {   
        m_Image ??= this.GetComponent<Image>();

        if (auxMat == null)
        {   
            auxMat ??= new Material(m_Image.material);
            m_Image.material = auxMat;
        }
    }


    private void InitiateGlowAnimation()
    {
        float t = auxMat.GetFloat("_GlowIntensity");
        auxMat.SetFloat("_GlowIntensity", 0.0f);
        DOTween.To(() => t, (float value) => t = value, 1.0f, 5f).SetEase(Ease.InOutQuad).SetDelay(Random.Range(4f, 10f)).SetId(this).SetLoops(-1 , LoopType.Yoyo).OnUpdate(() => 
        {
            auxMat.SetFloat("_GlowIntensity", t);
        });
    }

    private void DisableGlowAnimation()
    { 
        if(DOTween.IsTweening(this))
            DOTween.Kill(this);
    }


    private void OnDisable()
    {
        DisableGlowAnimation();
    }

}
