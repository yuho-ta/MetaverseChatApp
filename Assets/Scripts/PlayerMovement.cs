using System.Collections; 
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovements : MonoBehaviour
{
    public Animator animator; 
    public float acceleration = 1.0f; 
    public float deceleration = 0.5f; 
    public float maxSpeed = 1f; 
    public float moveSpeed = 2f; 

    public Transform cameraTransform; 
    public Vector3 cameraOffset = new Vector3(0, -0.5f, 1);

    public Canvas canvas; 

    private float speedX = 0f; 
    private float speedY = 0f; 
    public Button DanceButton;
    public Button ClapButton;
    public Button WaveButton;
    void Start(){
        DanceButton.onClick.AddListener(PlayDance);
        ClapButton.onClick.AddListener(PlayClap);
        WaveButton.onClick.AddListener(PlayWave);
    }
    void Update()
    {
        if (canvas != null && canvas.gameObject.activeSelf)
        {
            animator.SetFloat("MoveX", 0f);
            animator.SetFloat("MoveY", 0f);
            return;
        }

        float targetSpeedX = 0f;
        float targetSpeedY = 0f;

        if (Input.GetKey(KeyCode.UpArrow))
        {
            targetSpeedY = 1f;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            targetSpeedY = -1f;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            targetSpeedX = -1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            targetSpeedX = 1f;
        }

        speedX = Mathf.MoveTowards(speedX, targetSpeedX * maxSpeed, (targetSpeedX == 0 ? deceleration : acceleration) * Time.deltaTime);

        speedY = Mathf.MoveTowards(speedY, targetSpeedY * maxSpeed, (targetSpeedY == 0 ? deceleration : acceleration) * Time.deltaTime);

        animator.SetFloat("MoveX", speedX);
        animator.SetFloat("MoveY", speedY);

        Vector3 movement = new Vector3(speedX, 0, speedY) * moveSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World);

        if (cameraTransform != null)
        {
            cameraTransform.position = transform.position + cameraOffset;
        }
    }
    public void PlayDance()
    {
        ResetAllAnimations();
        animator.SetTrigger("Dance01");
    }

    public void PlayWave()
    {
        ResetAllAnimations();
        animator.SetTrigger("Wave");
    }

    public void PlayClap()
    {
        ResetAllAnimations();
        animator.SetTrigger("Clap");
    }

    private void ResetAllAnimations()
    {
        animator.ResetTrigger("Dance01");
        animator.ResetTrigger("Wave");
        animator.ResetTrigger("Clap");
    }
}
