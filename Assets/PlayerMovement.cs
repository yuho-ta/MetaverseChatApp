using System.Collections; 
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
    public Animator animator; 
    public float acceleration = 1.0f; 
    public float deceleration = 0.5f; 
    public float maxSpeed = 1f; 
    public float moveSpeed = 2f; 

    public Transform cameraTransform; // カメラのTransform
    public Vector3 cameraOffset = new Vector3(0, -0.5f, 1); // カメラのオフセット

    public Canvas canvas; // Canvasオブジェクト

    private float speedX = 0f; 
    private float speedY = 0f; 

    void Update()
    {
        // Canvasがアクティブな場合は動かない
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
            targetSpeedY = -1f;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            targetSpeedY = 1f;
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            targetSpeedX = 1f;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            targetSpeedX = -1f;
        }

        // 水平方向の速度を徐々に変化させる
        speedX = Mathf.MoveTowards(speedX, targetSpeedX * maxSpeed, (targetSpeedX == 0 ? deceleration : acceleration) * Time.deltaTime);

        // 垂直方向の速度を徐々に変化させる
        speedY = Mathf.MoveTowards(speedY, targetSpeedY * maxSpeed, (targetSpeedY == 0 ? deceleration : acceleration) * Time.deltaTime);

        // Animatorパラメータの更新
        animator.SetFloat("MoveX", speedX);
        animator.SetFloat("MoveY", speedY);

        // キャラクターの移動
        Vector3 movement = new Vector3(speedX, 0, speedY) * moveSpeed * Time.deltaTime;
        transform.Translate(movement, Space.World);

        // カメラをプレイヤーに追従させる
        if (cameraTransform != null)
        {
            cameraTransform.position = transform.position + cameraOffset;
        }
    }
}
