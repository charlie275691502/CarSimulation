using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

public enum ScenarioType{
    Highway, Intersection, Roundabout
}

public enum DisplayType{
    ProjectInformation, MapEditor
}

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
        public GameObject CreateButton_gmo;
        public GameObject SimulateButton_gmo;
    }
    [System.Serializable]
    public class MapEditor{
        public GameObject mapEditor_gmo;
        private ScenarioType _scenarioType;
        public ScenarioType scenarioType{ get { return _scenarioType; } set { _scenarioType = value; ChangeScenario(value); } }
        public Dropdown scenarioTypeDropdown;
        public HighwayDefineUI highwayDefineUI;
        public IntersectionDefineUI intersectionDefineUI;
        public RoundaboutDefineUI roundaboutDefineUI;

        void ChangeScenario(ScenarioType type){
            highwayDefineUI.highway_gmo.SetActive(type == ScenarioType.Highway);
            intersectionDefineUI.intersection_gmo.SetActive(type == ScenarioType.Intersection);
            roundaboutDefineUI.roundabout_gmo.SetActive(type == ScenarioType.Roundabout);
        }

        public string GenerateInputText(){
            string ret = "";
            switch(scenarioType){
                case ScenarioType.Highway:
                    ret += "0 0";
                    int N = (int.Parse(highwayDefineUI.highwayLength_input.text) - 1) / 25 + 1;
                    for (int i = 1; i < N; i++) ret += "\n0 " + i.ToString();
                    break;
                default:
                    ret += "0 0";
                    int east = (int.Parse(intersectionDefineUI.eastIntersectionLength_input.text) - 1) / 25 + 1;
                    int south = (int.Parse(intersectionDefineUI.southIntersectionLength_input.text) - 1) / 25 + 1;
                    int west = (int.Parse(intersectionDefineUI.westIntersectionLength_input.text) - 1) / 25 + 1;
                    int north = (int.Parse(intersectionDefineUI.northIntersectionLength_input.text) - 1) / 25 + 1;
                    for (int i = 1; i <= north; i++) ret += "\n0 " + i.ToString();
                    for (int i = 1; i <= east; i++) ret += "\n" + i.ToString() + " 0";
                    for (int i = 1; i <= south; i++) ret += "\n0 -" + i.ToString();
                    for (int i = 1; i <= west; i++) ret += "\n-" + i.ToString() + " 0";
                    break;
                    
            }
            return ret;
        }
    }
    [System.Serializable]
    public class HighwayDefineUI{
        public GameObject highway_gmo;
        public InputField highwayLength_input;
        public InputField trafficLane_input;
    }
    [System.Serializable]
    public class IntersectionDefineUI{
        public GameObject intersection_gmo;
        public InputField eastIntersectionLength_input;
        public InputField southIntersectionLength_input;
        public InputField westIntersectionLength_input;
        public InputField northIntersectionLength_input;
        public InputField trafficLane_input;
    }
    [System.Serializable]
    public class RoundaboutDefineUI{
        public GameObject roundabout_gmo;
    }
    public class Examples : MonoBehaviour{
        #region Variables
        public Button OpenFilesBtn;
        public Button OpenFoldersBtn;

        public Text Error;
        public Sprite OK_image;
        public Sprite NO_image;

        private DisplayType _displayType;
        public DisplayType displayType{ get { return _displayType; } set { _displayType = value; ChangeDisplayType(value); }}
        public ProjectInformation projectInformation;
        public MapEditor mapEditor;

        #endregion


        #region Init methods

        public void Start(){
            mapEditor.scenarioType = ScenarioType.Highway;
            //Util.Config.DEBUG = true;

            if (OpenFilesBtn != null)
                OpenFilesBtn.interactable = FileBrowser.canOpenMultipleFiles;

            if (OpenFoldersBtn != null)
                OpenFoldersBtn.interactable = FileBrowser.canOpenMultipleFolders;
        }

        #endregion

        public void OpenSingleFolder(){
            //string path = FileBrowser.OpenSingleFolder("Open folder", "c:");
            string path = FileBrowser.OpenSingleFolder();

            LoadProjectInformation(path);
        }

        void ChangeDisplayType(DisplayType type){
            projectInformation.projectImformation_gmo.SetActive(type == DisplayType.ProjectInformation);
            mapEditor.mapEditor_gmo.SetActive(type == DisplayType.MapEditor);
        }

        void LoadProjectInformation(string folderPath){
            projectInformation.path = folderPath;
            projectInformation.projectImformation_gmo.SetActive(true);
            projectInformation.projectName_text.text = Path.GetFileName(folderPath);
            projectInformation.projectPath_text.text = folderPath;
            projectInformation.projectDescription_text.text = "Project Description";
            List<string> filesPath = new List<string>(Directory.GetFiles(folderPath));
            bool existCarInput = filesPath.Exists(filePath=>Path.GetFileName(filePath) == "CarInput.txt");
            bool existMapInput = filesPath.Exists(filePath=>Path.GetFileName(filePath) == "MapInput.txt");
            bool existSetting = filesPath.Exists(filePath => Path.GetFileName(filePath) == "Setting.txt");
            projectInformation.CarInputImage.sprite = existCarInput ? OK_image : NO_image;
            projectInformation.MapInputImage.sprite = existMapInput ? OK_image : NO_image;
            projectInformation.SettingImage.sprite = existSetting ? OK_image : NO_image;
            projectInformation.CreateButton_gmo.SetActive(!existMapInput);
            projectInformation.SimulateButton_gmo.SetActive(existCarInput && existMapInput && existSetting);
            displayType = DisplayType.ProjectInformation;
        }

        public void LoadMapEditor(){
            displayType = DisplayType.MapEditor;
        }

        public void BackToProjectInformation(){
            displayType = DisplayType.ProjectInformation;
        }

        public void CreateMapInputFile(){
            StreamWriter sw = new StreamWriter(Path.Combine(projectInformation.path, "MapInput.txt"));
            sw.Write(mapEditor.GenerateInputText());
            sw.Close();
            displayType = DisplayType.ProjectInformation;
            LoadProjectInformation(projectInformation.path);
        }

        public void StartSimulate(){
            PlayerPrefs.SetString("FolderPath", projectInformation.path);
            Application.LoadLevel("SimulationScene");
        }

        public void ScenarioTypeDropdownValueChanged(){
            mapEditor.scenarioType = (ScenarioType)mapEditor.scenarioTypeDropdown.value;
        }
    }
}
// © 2017-2019 crosstales LLC (https://www.crosstales.com)