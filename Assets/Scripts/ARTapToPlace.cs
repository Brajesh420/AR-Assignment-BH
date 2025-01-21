using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using System.IO;

public class ARTapToCaptureAndSave : MonoBehaviour
{
    public GameObject cubePrefab; 
    public RawImage displayImage; 
    private ARRaycastManager raycastManager;

    private int captureFrameCount = -1; 

    void Start()
    {
        raycastManager = FindObjectOfType<ARRaycastManager>();

        
        if (displayImage != null)
        {
            displayImage.gameObject.SetActive(false);
        }
    }

    void Update()
    {
       
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            PlaceCubeAtTouch();
            captureFrameCount = 10; 
        }

       
        if (captureFrameCount > 0)
        {
            captureFrameCount--;

           
            if (captureFrameCount == 0)
            {
                CaptureScreenImage();
            }
        }
    }

    void PlaceCubeAtTouch()
    {
        Vector2 touchPosition = Input.GetTouch(0).position;
        List<ARRaycastHit> hits = new List<ARRaycastHit>();

       
        if (raycastManager.Raycast(touchPosition, hits, TrackableType.PlaneWithinPolygon))
        {
            Pose hitPose = hits[0].pose;

           
            GameObject cube = Instantiate(cubePrefab, hitPose.position, hitPose.rotation);

      
            Renderer cubeRenderer = cube.GetComponent<Renderer>();
            if (cubeRenderer != null)
            {
                cubeRenderer.material.color = Random.ColorHSV();
            }
        }
    }

    void CaptureScreenImage()
    {
       
        RenderTexture renderTexture = new RenderTexture(Screen.width, Screen.height, 24);
        ScreenCapture.CaptureScreenshotIntoRenderTexture(renderTexture);

        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGBA32, false);
        RenderTexture.active = renderTexture;
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();
        RenderTexture.active = null;

      
        SaveImageToDevice(texture);

    
        ShowImageInUI(texture);

        Debug.Log(" captured successfully!");
    }

    void SaveImageToDevice(Texture2D texture)
    {
      
        string path = Path.Combine(Application.persistentDataPath, "CapturedImage.png");

      
        byte[] imageData = texture.EncodeToPNG();

        File.WriteAllBytes(path, imageData);

        Debug.Log($"Image saved to: {path}");
    }

    void ShowImageInUI(Texture2D texture)
    {
        if (displayImage != null)
        {
           
            displayImage.texture = texture;

 
            displayImage.gameObject.SetActive(true);

          
            StartCoroutine(HideImageAfterSeconds(2));
        }
    }

    IEnumerator HideImageAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        if (displayImage != null)
        {
         
            displayImage.gameObject.SetActive(false);
        }
    }
}
