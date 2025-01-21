using System.Linq;
using UnityEngine;

public class Victory : MonoBehaviour
{
    [Header("Victory Settings")]
    public string tagToCheck = "Green";
    public GameObject victoryScreen;
    public GameObject explosionEffect;

    [Header("Main Ball Settings")]
    public GameObject mainBall;
    public Rigidbody2D mainBallrb;

    [Header("Camera Settings")]
    public Camera mainCamera;
    public float cameraZoomFactor = 3f;
    public float cameraZoomSpeed = 1f;

    [Header("Gameplay Settings")]
    public float slowMotionFactor = 0.2f;
    public float maxDistance = 5f;

    [Header("References")]
    public LevelGeneration levelGeneration;
    public PuggleAgent puggleAgent;

    private float originalCameraSize;
    private Vector3 cameraOriginalPosition;
    private bool victoryAchieved = false;

    void Start()
    {
        if (victoryScreen != null)
        {
            victoryScreen.SetActive(false);
        }

        if (mainCamera != null)
        {
            originalCameraSize = mainCamera.orthographicSize;
            cameraOriginalPosition = mainCamera.transform.position;
        }
    }

   

    void Update()
    {
        CheckVictoryCondition();  
    }

    private void FixedUpdate()
    {
     
    }

    public void UpdateMainBall(GameObject newMainBall)
    {
        mainBall = newMainBall;
    }

    private void CheckVictoryCondition()
    {
        var objectsWithTag = levelGeneration.marbles.Where(x => x != null && x.CompareTag(tagToCheck)).ToArray();

        if (objectsWithTag.Length == 1)
        {
            HandleSingleRemainingObject(objectsWithTag[0]);

        }
        else if (objectsWithTag.Length == 0)
        {
            TriggerVictory();

        }
        else
        {
            ResetTimeAndCamera();
        }
    }

    private void HandleSingleRemainingObject(GameObject lastGreenBall)
    {
        float distance = Vector3.Distance(mainBall.transform.position, lastGreenBall.transform.position);

        if (distance < maxDistance)
        {
            mainBallrb.linearVelocity /= 20.0f;
        ZoomCamera(cameraZoomFactor, mainBall.transform.position);
        }
        else
        {
            ResetTimeAndCamera();
        }
    }

    private void TriggerVictory()
    {
        victoryAchieved = true;

        if (victoryScreen != null)
        {
            victoryScreen.SetActive(true);
        }

        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, mainBall.transform.position, Quaternion.identity);
        }

        ResetTimeAndCamera();

        //puggleAgent?.AddReward(400f);
        //puggleAgent?.EndEpisode();

        levelGeneration?.Init();
    }

    private void ResetTimeAndCamera()
    {
        Time.timeScale = 1f;
        ZoomCamera(originalCameraSize, cameraOriginalPosition);
    }

    private void ZoomCamera(float targetSize, Vector3 targetPosition)
    {
        if (mainCamera != null)
        {
            mainCamera.transform.position = new Vector3(
                targetPosition.x,
                targetPosition.y,
                mainCamera.transform.position.z
            );
            mainCamera.orthographicSize = Mathf.Lerp(
                mainCamera.orthographicSize,
                targetSize,
                cameraZoomSpeed * Time.unscaledDeltaTime
            );
        }
    }


}
