using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CompletedScreen : MonoBehaviour
{
    public static CompletedScreen Instance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}
