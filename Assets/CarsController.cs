using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class CarAttributes{
    public int index;
    public int prefab_number;
    public GameObject gmo;
    public GameObject informationPanel_gmo;
    private float _x, _y, _rotation;
    private int _speed;
    public float x { get { return _x; } set { _x = value; gmo.transform.position = new Vector3(value, gmo.transform.position.y, gmo.transform.position.z); } }
    public float y { get { return _y; } set { _y = value; gmo.transform.position = new Vector3(gmo.transform.position.x, gmo.transform.position.y, value); } }
    public float rotation { get { return _rotation; } set { _rotation = value; gmo.transform.localRotation = Quaternion.Euler(new Vector3(0, 90 - value, 0)); } }
    public int speed{ get { return _speed; } set { _speed = value; informationPanel_gmo.transform.Find("Constants").Find("Speed").Find("value").GetComponent<Text>().text = value.ToString();}}
    public List<Status> status;

    public CarAttributes(string tempString){
        string[] tmp = tempString.Split('\n');
        index = int.Parse(tmp[0].Split(' ')[0]);
        prefab_number = int.Parse(tmp[0].Split(' ')[1]);

        status = new List<Status>();
        for (int i = 1;i<tmp.Length-1;i++){
            string[] s = tmp[i].Split('\t');
            status.Add(new Status(s[0], s[1], s[2], s[3]));
        }
    }
}

public class Status{
    public float time;
    public float x;
    public float y;
    public float rotation;

    public Status(){}

    public Status(string __time, string __x, string __y, string __rotation){
        time = float.Parse(__time);
        x = float.Parse(__x);
        y = float.Parse(__y);
        rotation = float.Parse(__rotation);
    }
}

public class CarsController : MonoBehaviour {
    public GameObject[] car_prefabs;
    public Transform car_folder;
    public CarAttributes[] carsAttributes;

    public Camera mainCamera;
    public GameObject carStatusPrefab;
    public Transform carStatusFolder;
    public bool isPause;
    public Text pauseText;
    public Slider timeSlider;
    public Dropdown cameraSelectingDropDown;

	// Use this for initialization
    void Start () {
        isPause = true;
        InitCars();
        maxTime = GetMaxTime();
        time = 0.0f;
	}

    public void ClickPauseButton(){
        if(isPause){
            isPause = false;
            pauseText.text = "||";
        } else {
            isPause = true;
            pauseText.text = "►";
        }
    }

    public void ChangeDropDown(int x){
        for (int i = 1; i < carsAttributes.Length; i++) carsAttributes[i].informationPanel_gmo.SetActive(false);
        int value = cameraSelectingDropDown.value;
        if(value == 0){
            mainCamera.transform.parent = null;
            mainCamera.transform.localPosition = new Vector3(0, 28.2f, -45.6f);
            mainCamera.transform.localRotation = Quaternion.Euler(45, 0, 0);
        } else {
            carsAttributes[value].informationPanel_gmo.SetActive(true);
            mainCamera.transform.parent = carsAttributes[value].gmo.transform;
            mainCamera.transform.localPosition = new Vector3(0, 5, -5);
            mainCamera.transform.localRotation = Quaternion.Euler(45, 0, 0);
        }
    }

    public void ChangeTimeValue(){
        if (Mathf.Abs(timeSlider.value - time / maxTime) < 0.02f) return;
        time = timeSlider.value * maxTime;
    }

    float maxTime;
    private float _time;
    float time{ get { return _time; }
        set {
            _time = value;
            timeSlider.value = value / maxTime;
        }
    }
    private void Update(){
        if(!isPause) time += Time.deltaTime;
	}

    void InitCars(){
        StreamReader sr = new StreamReader(Path.Combine(PlayerPrefs.GetString("FolderPath"), "CarInput.txt"));
        string carInput = sr.ReadToEnd();
        sr.Close();
        string[] tempString = carInput.Split('#');
        carsAttributes = new CarAttributes[tempString.Length];

        for (int i = 1; i < tempString.Length; i++){
            cameraSelectingDropDown.AddOptions(new List<string>(1){"Car" + i.ToString()});
            carsAttributes[i] = new CarAttributes(tempString[i]);
            StartCoroutine(CarSimulator(i));
        }
    }

    float GetMaxTime(){
        float ret = 0.0f;
        for (int i = 1; i < carsAttributes.Length; i++){
            foreach(Status status in carsAttributes[i].status){
                ret = Mathf.Max(ret, status.time);
            }
        }
        return ret;
    }

    IEnumerator CarSimulator(int index){
        carsAttributes[index].gmo = Instantiate(car_prefabs[carsAttributes[index].prefab_number], car_folder);
        carsAttributes[index].gmo.transform.localScale = new Vector3(2, 2, 2);
        float floating = GetFloatingByCarType(carsAttributes[index].prefab_number);
        carsAttributes[index].gmo.transform.localPosition = new Vector3(0, floating, 0);
        carsAttributes[index].informationPanel_gmo = carsAttributes[index].gmo.transform.Find("InformationPanel").gameObject;
        //Instantiate(carStatusPrefab, carsAttributes[index].gmo.transform).transform.localPosition = new Vector3(-0.2f, 2.52f, 1.3f);
        carsAttributes[index].x = carsAttributes[index].status[0].x;
        carsAttributes[index].y = carsAttributes[index].status[0].y;
        carsAttributes[index].rotation = carsAttributes[index].status[0].rotation;
        while (isPause) yield return null;
        A:
        for (int i = 1; i < carsAttributes[index].status.Count;i++){
            while (time < carsAttributes[index].status[i-1].time && i > 0) i--;
            float timeFrame = carsAttributes[index].status[i].time - carsAttributes[index].status[i - 1].time;
            float xFrame = carsAttributes[index].status[i].x - carsAttributes[index].status[i - 1].x;
            float yFrame = carsAttributes[index].status[i].y - carsAttributes[index].status[i - 1].y;
            float rotationFrame = carsAttributes[index].status[i].rotation - carsAttributes[index].status[i - 1].rotation;
            carsAttributes[index].speed = (int)(Mathf.Sqrt(xFrame * xFrame + yFrame * yFrame) / timeFrame * 3.6f); // 3600 / 1000
            while(i > 0 && carsAttributes[index].status[i - 1].time < time && time < carsAttributes[index].status[i].time){
                carsAttributes[index].x = carsAttributes[index].status[i - 1].x + xFrame * (time - carsAttributes[index].status[i - 1].time) / timeFrame;
                carsAttributes[index].y = carsAttributes[index].status[i - 1].y + yFrame * (time - carsAttributes[index].status[i - 1].time) / timeFrame;
                carsAttributes[index].rotation = carsAttributes[index].status[i-1].rotation + rotationFrame * (time - carsAttributes[index].status[i - 1].time) / timeFrame;
                yield return null;
            }
        }
        while (carsAttributes[index].status[carsAttributes[index].status.Count - 1].time < time) yield return null;
        goto A;
    }

    float GetFloatingByCarType(int index){
        switch(index){
            case 0: return 1.39f;
            case 1: return 1.39f;
            case 2: return 0.81f;
            case 3: return 0.81f;
            case 4: return 0.81f;
            case 5: return 0.81f;
            case 6: return 0.81f;
            case 7: return 0.81f;
            default: break;
        }
        return 0;
    }
}
