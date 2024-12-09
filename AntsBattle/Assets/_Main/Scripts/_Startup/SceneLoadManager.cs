using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(Canvas))]
public class SceneLoadManager : MonoBehaviour
{
    private static SceneLoadManager _main;

    public static SceneLoadManager Main
    {
        set => _main = value;
        get
        {
            if (_main) return _main;
            _main = FindObjectOfType<SceneLoadManager>(true);
            return _main;
        }
    }


    [SerializeField] private float fadeDuration = 0.5f;
    [SerializeField] private Image fadeImage;


    private void Awake()
    {
        var color = fadeImage.color;
        color.a = 0f;
        fadeImage.color = color;
        fadeImage.enabled = true;
        gameObject.SetActive(false);
    }


    public void Load(string scene)
    {
        if (!enabled) return;
        enabled = false;
        gameObject.SetActive(true);
        var doFade = fadeImage.DOFade(1f, fadeDuration);
        doFade.onComplete = () =>
        {
            if (scene.Length <= 0) scene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(scene);
        };
    }
}