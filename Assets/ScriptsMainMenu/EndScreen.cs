using System;
using System.Collections.Generic;
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
            SetGameTime(ExperimentMetaData.StartTime, ExperimentMetaData.EndTime);
            SetDistanceWalked(0);
            SetLandmarksFound(0, 10);
            SetPicturesTaken(0);
            SetObjectsPickedUp(0, 10);
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

        private void SetGameTime(DateTime start, DateTime finish)
        {
            var gameTime = finish - start;
            ResultsGameTime.text = $"{gameTime.Minutes} m : {gameTime.Seconds} s";
        }

        private void SetDistanceWalked(int distance)
        {
            distance = 0;
            
            Dictionary<int, List<PositionalData>> positionalData = CsvUtils.PositionalDataFromCsv();
            if (positionalData != null && positionalData.Count > 0)
            {
                distance = (int)Aggregate.CalculateDistance(positionalData);
            }

            ResultsDistanceWalked.text = $"{distance} m";
        }

        private void SetLandmarksFound(int value, int max = -1)
        {
            if (ExperimentMetaData.Environments == null)
            {
                ResultsPicturesTakenObject.SetActive(false);
                return;
            }

            var count = 0;

            Dictionary<int, List<TaskData>> tasks = CsvUtils.TaskDataFromCsv();
            if (tasks != null && tasks.Count > 0)
            {
                count = tasks.SelectMany(pair => pair.Value).Where(taskdata => taskdata.task == "Landmark").Count();
            }

            ResultsLandmarksFound.text = $"{count}";
        }

        private void SetPicturesTaken(int value)
        {
            if (ExperimentMetaData.Environments == null)
            {
                ResultsPicturesTakenObject.SetActive(false);
                return;
            }
            var taskActive = ExperimentMetaData.Environments.Any(env => env.CameraTask);

            if (taskActive)
            {
                ResultsPicturesTakenObject.SetActive(true);

                var count = 0;

                Dictionary<int, List<TaskData>> tasks = CsvUtils.TaskDataFromCsv();
                if (tasks != null && tasks.Count > 0)
                {
                    count = tasks.SelectMany(pair => pair.Value).Where(taskdata => taskdata.task == "Picture").Count();
                }

                ResultsPicturesTaken.text = $"{count}";
            }
            else
            {
                ResultsPicturesTakenObject.SetActive(false);
            }
        }
        
        private void SetObjectsPickedUp(int value, int max = -1)
        {
            if (ExperimentMetaData.Environments == null)
            {
                ResultsObjectsPickedUpObject.SetActive(false);
                return;
            }
            var taskActive = ExperimentMetaData.Environments.Any(env => env.PickupTask);

            if (taskActive)
            {
                ResultsObjectsPickedUpObject.SetActive(true);

                var count = 0;

                Dictionary<int, List<TaskData>> tasks = CsvUtils.TaskDataFromCsv();
                if (tasks != null && tasks.Count > 0)
                {
                    count = tasks.SelectMany(pair => pair.Value).Where(taskdata => taskdata.task == "Gathering").Count();
                }

                ResultsObjectsPickedUp.text = $"{count}";
            }
            else
            {
                ResultsObjectsPickedUpObject.SetActive(false);
            }
        }
    }
}
