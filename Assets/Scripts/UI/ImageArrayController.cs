using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class ImageArrayController : MonoBehaviour
{
    public Image imageHolder;
    public bool canLoop;
    public bool resetIndexOnEnabled;
    public Sprite[] images;

    [Header("Buttons")]
    public Button nextButton;
    public Button prevButton;

    private int index;
    private AudioManager audioManager;

    private void Awake()
    {
        index = 0;
        UpdateImage();
    }

    private void Start()
    {
        audioManager = AudioManager.Instance;
    }

    private void OnEnable()
    {
        if (resetIndexOnEnabled)
        {
            index = 0;
            UpdateImage();
        }
    }

    public void OnNextImagePressed(InputAction.CallbackContext context)
    {
        if (context.performed)
            NextImage();
    }

    public void NextImage()
    {
        if (ModifyIndex(1))
            audioManager.Play("Select");
        else
            audioManager.Play("SelectOut");
        UpdateImage();
        nextButton.Select();
    }

    public void OnPrevImagePressed(InputAction.CallbackContext context)
    {
        if (context.performed)
            PrevImage();
    }

    public void PrevImage()
    {
        if (ModifyIndex(-1))
            audioManager.Play("Select");
        else
            audioManager.Play("SelectOut");
        UpdateImage();
        prevButton.Select();
    }

    private void UpdateImage()
    {
        imageHolder.sprite = images[index];
    }

    private bool ModifyIndex(int value)
    {
        index += value;
        if (index >= images.Length || index < 0)
        {
            if (canLoop)
            {
                index = (index % images.Length + images.Length) % images.Length;
                return true;
            }
            else
            {
                index = (index < 0) ? 0 : images.Length - 1;
                return false;
            }
        }
        return true;
    }
}
