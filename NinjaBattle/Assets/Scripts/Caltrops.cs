using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Caltrops : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> caltrops = new List<SpriteRenderer>();

    private void Start()
    {
        caltrops = caltrops.OrderBy(caltrop => Random.value).ToList();
        StartCoroutine(AppearCaltrops());
    }

    private IEnumerator AppearCaltrops()
    {
        for (int i = 0; i < caltrops.Count; i++)
        {
            caltrops[i].gameObject.SetActive(true);
            yield return new WaitForSeconds(0.01f);
        }

        yield break;
    }
}
