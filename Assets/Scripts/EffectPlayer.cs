using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectPlayer : MonoBehaviour
{
    [SerializeField] GameObject visualEffect;
    [SerializeField] AudioSource soundEffect;
    [SerializeField] float duration;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PlayEffect());
    }

    IEnumerator PlayEffect()
    {
        if(soundEffect != null)
        {
            soundEffect.Play();
        }
        
        yield return new WaitForSeconds(duration);
        Destroy(this.gameObject);
        yield break;
    }
}
