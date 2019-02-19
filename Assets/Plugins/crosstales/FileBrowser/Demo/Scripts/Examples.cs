using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Crosstales.FB.Demo{
    /// <summary>Examples for all methods.</summary>
    [HelpURL("https://www.crosstales.com/media/data/assets/FileBrowser/api/class_crosstales_1_1_f_b_1_1_demo_1_1_examples.html")]

    [System.Serializable]
    public class ProjectInformation{
        public string path;
        public GameObject projectImformation_gmo;
        public Text projectName_text;
        public Text projectPath_text;
        public Text projectDescription_text;
        public Image CarInputImage;
        public Image MapInputImage;
        public Image SettingImage;
    }
    public class Examples : MonoBehaviour{
        #region Variables
        public GameObject ScrollView;

        public Button OpenFilesBtn;
        public Button OpenFoldersBtn;

        public Text Error;
        public Sprite OK_image;
        public Sprite NO_image;
        public ProjectInformation projectInformation;

        #endregion


        #region Public methods

        public void Start(){

            //Util.Config.DEBUG = true;

            if (OpenFilesBtn != null)
                OpenFilesBtn.interactable = FileBrowser.canOpenMultipleFiles;

            if (OpenFoldersBtn != null)
                OpenFoldersBtn.interactable = FileBrowser.canOpenMultipleFolders;
        }

        #endregion


        #region Public methods

        public void OpenSingleFolder(){
            //string path = FileBrowser.OpenSingleFolder("Open folder", "c:");
            string path = FileBrowser.OpenSingleFolder();

            LoadProjectInformation(path);
        }

        #endregion

        private void LoadProjectInformation(string folderPath){
            projectInformation.path = folderPath;
            projectInformation.projectName_text.text = Path.GetFileName(folderPath);
            projectInformation.projectPath_text.text = folderPath;
            projectInformation.projectDescription_text.text = "Project Description";
            List<string> filesPath = new List<string>(Directory.GetFiles(folderPath));
            projectInformation.CarInputImage.sprite = NO_image;
            projectInformation.MapInputImage.sprite = NO_image;
            projectInformation.SettingImage.sprite = NO_image;
            foreach (string filePath in filesPath) {
                if (Path.GetFileName(filePath) == "CarInput.txt") projectInformation.CarInputImage.sprite = OK_image;
                if (Path.GetFileName(filePath) == "MapInput.txt") projectInformation.MapInputImage.sprite = OK_image;
                if (Path.GetFileName(filePath) == "Setting.txt") projectInformation.SettingImage.sprite = OK_image;
            }
        }
    }
}
// © 2017-2019 crosstales LLC (https://www.crosstales.com)