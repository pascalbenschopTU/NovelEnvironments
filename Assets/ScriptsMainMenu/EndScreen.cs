using System;
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
            ResultsDistanceWalked.text = $"{distance} m";
        }

        private void SetLandmarksFound(int value, int max = -1)
        {
            ResultsLandmarksFound.text = max >= 0 ? $"{value} / {max}" : $"{value}";
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


                // GameObject player = GameObject.Find("Player");
                // int participant_id = ExperimentMetaData.ParticipantNumber;
                // int environment_id = (int)ExperimentMetaData.Environments[ExperimentMetaData.Index].EnvironmentType;
                // object count = player.GetComponent<SqliteLogging>().getCountPictureByUserInEnvironment(participant_id, environment_id);

                // ResultsPicturesTakenObject.SetActive(true);
                ResultsPicturesTaken.text = $"{value}";
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
                if (!ResultsPicturesTakenObject.activeSelf)
                {
                    var currentPos = ResultsObjectsPickedUpObject.transform.localPosition;
                    ResultsObjectsPickedUpObject.transform.localPosition =
                        new Vector3(currentPos.x, currentPos.y + 100, 0);
                }
                else
                {
                    var currentPos = ResultsObjectsPickedUpObject.transform.localPosition;
                    ResultsObjectsPickedUpObject.transform.localPosition = new Vector3(currentPos.x, -245, 0);
                }
                ResultsObjectsPickedUp.text = max >= 0? $"{value} / {max}": $"{value}";
            }
            else
            {
                ResultsObjectsPickedUpObject.SetActive(false);
            }
        }
    }
}
