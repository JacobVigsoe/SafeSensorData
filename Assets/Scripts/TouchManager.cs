using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine.UI;

public class TouchManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private InputAction touchPressAction;

    private bool experimentRunning = false;
    private List<string> accelerometerData;
    private string directoryPath;

    private float Maxtime = 10f;

    private float timer = 0;
    public TMP_Text TextTime; 

    private Coroutine experimentCoroutine;

    public TMP_Text Displaytext;
    public TMP_Text Displaytext2;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        touchPressAction = playerInput.actions["TouchPress"];
        directoryPath = Application.persistentDataPath + "/ExperimentData";
        Directory.CreateDirectory(directoryPath); // Create directory if it doesn't exist
    }

    private void OnEnable()
    {
        touchPressAction.performed += ToggleExperiment;
    }

    private void OnDisable()
    {
        touchPressAction.performed -= ToggleExperiment;
    }

    private void ToggleExperiment(InputAction.CallbackContext context)
    {
        if (!experimentRunning)
        {
            Displaytext2.text = "Throw your phone";
            Displaytext.text = "Stop";
            StartExperiment();
        }
        else
        {
            timer = 0;
            Displaytext2.text = "";
            Displaytext.text = "Start";
            StopExperiment();
        }
    }

    private void StartExperiment()
    {
        Debug.Log("Experiment started.");

        experimentRunning = true;
        accelerometerData = new List<string>();
        experimentCoroutine = StartCoroutine(RecordData());
    }

    private void StopExperiment()
    {
        Debug.Log("Experiment stopped.");

        experimentRunning = false;
        if (experimentCoroutine != null)
        {
            StopCoroutine(experimentCoroutine);
        }

        SaveDataToFile();
    }

    private IEnumerator RecordData()
    {
        float CurrentTime = 0f;
        while (CurrentTime < Maxtime)
        {
            // Access accelerometer data
            Vector3 acceleration = Input.acceleration;

            // Store accelerometer data as a string
            string dataEntry = $"{Time.time},{acceleration.x},{acceleration.y},{acceleration.z}";
            accelerometerData.Add(dataEntry);

            CurrentTime += Time.deltaTime;
            timer += Time.deltaTime;

            TextTime.text = "Time: " + timer.ToString("F2");
            yield return null; // Wait for the next frame
        }
        timer = 0;
        Displaytext2.text = "";
        Displaytext.text = "Start";
        StopExperiment();
    }

    private void SaveDataToFile()
    {
        
        string fileName = "Experiment_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
        string filePath = Path.Combine(directoryPath, fileName);

        
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (string data in accelerometerData)
            {
                writer.WriteLine(data);
            }
        }
    }
}
