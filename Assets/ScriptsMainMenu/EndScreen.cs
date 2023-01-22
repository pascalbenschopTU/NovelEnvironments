using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ScriptsLogUser;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ScriptsMainMenu
{
    public class EndScreen : MonoBehaviour
    {
        [SerializeField] private GameObject EmptyEndScreen;
        [SerializeField] private GameObject DataEndScreen;
        
        [SerializeField] private GameObject ResultsPicturesTakenObject;
        [SerializeField] private GameObject ResultsObjectsPickedUpObject;
        [SerializeField] private TextMeshProUGUI ResultsGameTime;
        [SerializeField] private TextMeshProUGUI ResultsDistanceWalked;
        [SerializeField] private TextMeshProUGUI ResultsLandmarksFound;
        [SerializeField] private TextMeshProUGUI ResultsPicturesTaken;
        [SerializeField] private TextMeshProUGUI ResultsObjectsPickedUp;
        
        [SerializeField] private GameObject ImageContainerList;
        [SerializeField] private GameObject ImageContainerPrefab;
        [SerializeField] private Image EmptyImagePrefab;

        [SerializeField] private Button ButtonBack;
        [SerializeField] private Button ButtonForward;

        private int pageIndex;
        private int totalPages;
        private List<GameObject> imageContainers;
        private List<Image> imageObjects;

        private RoamingEntropy roamingEntropy;
        
        public void LoadEndScreen()
        {
            // this method needs data from the run and the run configuration which is stored in the ExperimentMetaData
            // TODO get all the data from the run
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            var showData = Convert.ToBoolean(PlayerPrefs.GetInt("EndScreenActiveSetting"));
            ExperimentMetaData.EndTime = DateTime.Now;

            var directoryPath = ExperimentMetaData.LogDirectory;
            CsvUtils.SavePositionalDataToCsv(Recorder.recording, directoryPath);
            CsvUtils.SaveTaskDataToCsv(Recorder.tasks, directoryPath);

            var gameTime = CalculateGameTime(GameTime.TotalGameTime);
            var distance = CalculateDistanceWalked(directoryPath);
            var numLandmarks = CalculateLandmarksFound(directoryPath);
            var picturesTaken = CalculatePicturesTaken(directoryPath);
            var objectsPickedUp = CalculateObjectsPickedUp(directoryPath);
            // var roamingEntropy = this.roamingEntropy.CalculateRoamingEntropy(directoryPath);
            this.roamingEntropy.CalculateRoamingEntropy(directoryPath);
            CsvUtils.SaveExperimentData(directoryPath, new ExperimentData
            {
                DistanceWalked = distance,
                LandmarksFound = numLandmarks,
                PicturesTaken = picturesTaken,
                GameTime = gameTime.ToString(),
                ObjectsPickedUp = objectsPickedUp,
                RoamingEntropy = 0.0f 
            });
            
            // hide or show data
            EmptyEndScreen.SetActive(!showData);
            DataEndScreen.SetActive(showData);
            if (showData)
            {
                ShowPicturesTaken(Path.Join(directoryPath, "pictures"));
            }
        }

        public void OnContinue()
        {
            ExperimentMetaData.ExperimentFinished = false;
        }

        public void PicturesShowNextPage()
        {
            imageContainers[pageIndex].SetActive(false);
            pageIndex++;
            imageContainers[pageIndex].SetActive(true);
            ButtonForward.gameObject.SetActive(pageIndex < totalPages - 1);
            ButtonBack.gameObject.SetActive(true);
        }
        public void PicturesShowPreviousPage()
        {
            imageContainers[pageIndex].SetActive(false);
            pageIndex--;
            imageContainers[pageIndex].SetActive(true);
            ButtonBack.gameObject.SetActive(pageIndex > 0);
            ButtonForward.gameObject.SetActive(true);
        }
        
        private void ShowPicturesTaken(string srcDir)
        {
            ButtonBack.gameObject.SetActive(false);
            ButtonForward.gameObject.SetActive(false);
            if (!Directory.Exists(srcDir) || !ExperimentMetaData.Environments.Any(env => env.CameraTask))
            {
                return;
            }
            var pictures = new List<Texture2D>();
            imageObjects = new List<Image>();
            imageContainers = new List<GameObject>();

            var files = Directory.GetFiles(srcDir);
            foreach (var file in files)
            {
                if (file.Contains(".meta"))
                {
                    continue;
                }
                var bytes = File.ReadAllBytes(file);
                var pic = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
                if (pic.LoadImage(bytes))
                {
                    pictures.Add(pic);
                }
            }

            totalPages = Mathf.RoundToInt(Mathf.Floor(pictures.Count / 4f)) + 1;
            pageIndex = 0;
            for (int i = 0; i < totalPages; i++)
            {
                imageContainers.Add(Instantiate(ImageContainerPrefab, new Vector3(0,0,0), Quaternion.identity, ImageContainerList.transform));
                imageContainers[i].SetActive(false);
                imageContainers[i].transform.localPosition = new Vector3(0, 0, 0);
            }

            for (var i = 0; i < pictures.Count; i++)
            {
                var index = Mathf.RoundToInt(Mathf.Floor(i / 4f));
                var sprite = Sprite.Create(pictures[i], new Rect(0, 0, Screen.width, Screen.height),
                    new Vector2(0.5f, 0.5f), 1f);
                var obj = Instantiate(sprite, new Vector3(0, 0, 0), Quaternion.identity, imageContainers[index].transform);
                var img = Instantiate(EmptyImagePrefab, new Vector3(0, 0, 0), Quaternion.identity, imageContainers[index].transform);
                img.sprite = obj;
                img.transform.localPosition = new Vector3(i % 4 * 340 - 515, 0, 0);
                imageObjects.Add(img);
            }

            if (imageContainers.Count > 1)
            {
                for (var i = 1; i < imageContainers.Count; i++)
                {
                    imageContainers[i].gameObject.SetActive(false);
                }
                ButtonForward.gameObject.SetActive(true);
            }

            imageContainers[0].SetActive(true);
        }

        public void ShowEndScreen()
        {
            if(ExperimentMetaData.ExperimentFinished)
            {
                roamingEntropy = GetComponent<RoamingEntropy>();
                LoadEndScreen();
            }
        }
        private TimeSpan CalculateGameTime(int GameTimeInSeconds)
        {
            TimeSpan totalGameTime = new TimeSpan(GameTimeInSeconds / 3600, GameTimeInSeconds / 60, GameTimeInSeconds % 60);
            ResultsGameTime.text = $"{totalGameTime.Minutes} m : {totalGameTime.Seconds} s";
            return totalGameTime;
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
                count = tasks.SelectMany(pair => pair.Value).Where(taskdata => taskdata.task.Contains("Landmark")).Count();
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
                ResultsPicturesTaken.text = "---";
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
                ResultsObjectsPickedUp.text = "---";
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
    }
}
