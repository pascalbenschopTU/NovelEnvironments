using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;

namespace ScriptsMainMenu
{
    public class EndScreen : MonoBehaviour
    {
        [SerializeField]
        private GameObject ResultsPicturesTakenObject;
        [SerializeField]
        private GameObject ResultsObjectsPickedUpObject;
        [SerializeField]
        private TextMeshProUGUI ResultsGameTime;
        [SerializeField]
        private TextMeshProUGUI ResultsDistanceWalked;
        [SerializeField]
        private TextMeshProUGUI ResultsLandmarksFound;
        [SerializeField]
        private TextMeshProUGUI ResultsPicturesTaken;
        [SerializeField]
        private TextMeshProUGUI ResultsObjectsPickedUp;

        public void LoadEndScreen()
        {
            // this method needs data from the run and the run configuration which is stored in the ExperimentMetaData
            // TODO get all the data from the run
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            ExperimentMetaData.EndTime = DateTime.Now;
            
            var directoryPath = Path.Join(Application.dataPath, $"ExperimentLogs_{ExperimentMetaData.ParticipantNumber}");
            directoryPath = Path.Join(directoryPath, $"{ExperimentMetaData.EndTime:dd-MM-yyyy_hh-mm-ss}");
            
            CsvUtils.SavePositionalDataToCsv(Recorder.recording, directoryPath);
            CsvUtils.SaveTaskDataToCsv(Recorder.tasks, directoryPath);

            var gameTime = CalculateGameTime(ExperimentMetaData.StartTime, ExperimentMetaData.EndTime);
            var distance = CalculateDistanceWalked(directoryPath);
            var numLandmarks = CalculateLandmarksFound(directoryPath);
            var picturesTaken = CalculatePicturesTaken(directoryPath);
            var objectsPickedUp = CalculateObjectsPickedUp(directoryPath);
            var roamingEntropy = CalculateRoamingEntropy(directoryPath);
            CsvUtils.SaveExperimentData(directoryPath, new ExperimentData
            {
                DistanceWalked = distance,
                LandmarksFound = numLandmarks,
                LandmarksFoundMax = -1,
                PicturesTaken = picturesTaken,
                GameTime = gameTime.ToString(),
                ObjectsPickedUp = objectsPickedUp,
                ObjectsPickedUpMax = -1,
                RoamingEntropy = roamingEntropy 
            });
        }

        public void OnContinue()
        {
            ExperimentMetaData.ExperimentFinished = false;
        }

        private void OnEnable()
        {
            if(ExperimentMetaData.ExperimentFinished)
            {
                LoadEndScreen();
            }
        }
        private TimeSpan CalculateGameTime(DateTime start, DateTime finish)
        {
            var gameTime = finish - start;
            ResultsGameTime.text = $"{gameTime.Minutes} m : {gameTime.Seconds} s";
            return gameTime;
        }
        private float CalculateDistanceWalked(string directoryName)
        {
            var distance = 0f;
            Dictionary<int, List<PositionalData>> positionalData = CsvUtils.LoadPositionalDataFromCsv(directoryName);
            if (positionalData != null && positionalData.Count > 0)
            {
                distance = (int)Aggregate.CalculateDistance(positionalData);
            }

            ResultsDistanceWalked.text = $"{distance} m";
            return distance;
        }
        private int CalculateLandmarksFound(string directoryName)
        {
            if (ExperimentMetaData.Environments == null)
            {
                ResultsPicturesTakenObject.SetActive(false);
                return 0;
            }

            var count = 0;

            Dictionary<int, List<TaskData>> tasks = CsvUtils.LoadTaskDataFromCsv(directoryName);
            if (tasks != null && tasks.Count > 0)
            {
                count = tasks.SelectMany(pair => pair.Value).Where(taskdata => taskdata.task == "Landmark").Count();
            }

            ResultsLandmarksFound.text = $"{count}";
            return count;
        }
        private int CalculatePicturesTaken(string directoryName)
        {
            if (ExperimentMetaData.Environments == null)
            {
                ResultsPicturesTakenObject.SetActive(false);
                return 0;
            }
            var taskActive = ExperimentMetaData.Environments.Any(env => env.CameraTask);

            if (!taskActive)
            {
                ResultsPicturesTakenObject.SetActive(false);
                return 0;
            }

            ResultsPicturesTakenObject.SetActive(true);
            var count = 0;

            Dictionary<int, List<TaskData>> tasks = CsvUtils.LoadTaskDataFromCsv(directoryName);
            if (tasks != null && tasks.Count > 0)
            {
                count = tasks.SelectMany(pair => pair.Value).Count(taskData => taskData.task == "Picture");
            }

            ResultsPicturesTaken.text = $"{count}";
            return count;
        }
        private int CalculateObjectsPickedUp(string directoryName)
        {
            if (ExperimentMetaData.Environments == null)
            {
                ResultsObjectsPickedUpObject.SetActive(false);
                return 0;
            }
            var taskActive = ExperimentMetaData.Environments.Any(env => env.PickupTask);

            if (!taskActive)
            {
                ResultsObjectsPickedUpObject.SetActive(false);
                return 0;
            }
            ResultsObjectsPickedUpObject.SetActive(true);

            var count = 0;

            Dictionary<int, List<TaskData>> tasks = CsvUtils.LoadTaskDataFromCsv(directoryName);
            if (tasks != null && tasks.Count > 0)
            {
                count = tasks.SelectMany(pair => pair.Value).Count(taskData => taskData.task == "Gathering");
            }

            ResultsObjectsPickedUp.text = $"{count}";
            return count;
        }
        private int CalculateRoamingEntropy(string directoryName)
        {
            return -1;
        }
    }
}
