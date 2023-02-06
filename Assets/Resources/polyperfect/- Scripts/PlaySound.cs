using UnityEngine;

namespace PolyPerfect
{
  public class PlaySound : MonoBehaviour
  {
    [SerializeField]
    private AudioClip animalSound;
    [SerializeField]
    private AudioClip walking;
    [SerializeField]
    private AudioClip eating;
    [SerializeField]
    private AudioClip running;
    [SerializeField]
    private AudioClip attacking;
    [SerializeField]
    private AudioClip death;
    [SerializeField]
    private AudioClip sleeping;

    private float CalculateDistanceToPlayer()
        {
            GameObject player = GameObject.Find("Player");
            float distance = Vector3.Distance(player.transform.position, transform.position);
            float intensity = Mathf.Clamp(1.0f - (distance / 100.0f), 0.0f, 1.0f);

            return intensity;
        }

    public void AnimalSound()
    {
      if (animalSound)
      {
                transform.GetComponent<AudioSource>().clip = animalSound;
                transform.GetComponent<AudioSource>().volume = CalculateDistanceToPlayer();
                transform.GetComponent<AudioSource>().Play();
      }
    }

    public void Walking()
    {
        if (walking)
        {
                transform.GetComponent<AudioSource>().clip = walking;
                transform.GetComponent<AudioSource>().volume = CalculateDistanceToPlayer();
                transform.GetComponent<AudioSource>().Play();
        }
    }

    public void Eating()
    {
      if (eating)
      {
                transform.GetComponent<AudioSource>().clip = eating;
                transform.GetComponent<AudioSource>().volume = CalculateDistanceToPlayer();
                transform.GetComponent<AudioSource>().Play();
      }
    }

    public void Running()
    {
      if (running)
      {
                transform.GetComponent<AudioSource>().clip = running;
                transform.GetComponent<AudioSource>().volume = CalculateDistanceToPlayer();
                transform.GetComponent<AudioSource>().Play();
      }
    }

    public void Attacking()
    {
      if (attacking)
      {
                transform.GetComponent<AudioSource>().clip = attacking;
                transform.GetComponent<AudioSource>().volume = CalculateDistanceToPlayer();
                transform.GetComponent<AudioSource>().Play();
      }
    }

    public void Death()
    {
      if (death)
      {
                transform.GetComponent<AudioSource>().clip = death;
                transform.GetComponent<AudioSource>().volume = CalculateDistanceToPlayer();
                transform.GetComponent<AudioSource>().Play();
      }
    }

    public void Sleeping()
    {
      if (sleeping)
      {
                transform.GetComponent<AudioSource>().clip = sleeping;
                transform.GetComponent<AudioSource>().volume = CalculateDistanceToPlayer();
                transform.GetComponent<AudioSource>().Play();
      }
    }
  }
}