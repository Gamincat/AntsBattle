using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventArea : MonoBehaviour
{
    [SerializeField] public UnityEvent OnEnter;
    [SerializeField] public UnityEvent OnStay;
    [SerializeField] public UnityEvent OnExit;


    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("EventArea");
    }


    private void OnTriggerEnter(Collider other)
    {
        OnEnter?.Invoke();
    }


    private void OnTriggerStay(Collider other)
    {
        OnStay?.Invoke();
    }


    private void OnTriggerExit(Collider other)
    {
        OnExit?.Invoke();
    }


    private void OnCollisionEnter(Collision other)
    {
        OnEnter?.Invoke();
    }


    private void OnCollisionStay(Collision other)
    {
        OnStay?.Invoke();
    }


    private void OnCollisionExit(Collision other)
    {
        OnExit?.Invoke();
    }
}
