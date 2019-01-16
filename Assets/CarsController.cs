using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarAttributes{
    public int index;
    public int prefab_number;
    public GameObject gmo;
    private float _x, _y, _rotation;
    public float x { get { return _x; } set { _x = value; gmo.transform.position = new Vector3(value, 1.39f, gmo.transform.position.z); } }
    public float y { get { return _y; } set { _y = value; gmo.transform.position = new Vector3(gmo.transform.position.x, 1.39f, value); } }
    public float rotation { get { return _rotation; } set { _rotation = value; gmo.transform.localRotation = Quaternion.Euler(new Vector3(0, 90 - value, 0)); } }
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
    public TextAsset cars_textAsset;
    public GameObject[] car_prefabs;
    public Transform car_folder;
    public CarAttributes[] carsAttributes;

	// Use this for initialization
	void Start () {
        InitCars();
	}

    float time = 0.0f;
    private void Update(){
        time += Time.deltaTime;
	}

    void InitCars(){
        string[] tempString = cars_textAsset.text.Split('#');
        carsAttributes = new CarAttributes[tempString.Length];

        for (int i = 1; i < tempString.Length; i++){
            carsAttributes[i] = new CarAttributes(tempString[i]);
            StartCoroutine(CarSimulator(i));
        }
    }

    IEnumerator CarSimulator(int index){
        carsAttributes[index].gmo = Instantiate(car_prefabs[carsAttributes[index].prefab_number], car_folder);
        carsAttributes[index].gmo.transform.localScale = new Vector3(2, 2, 2);
        carsAttributes[index].x = carsAttributes[index].status[0].x;
        carsAttributes[index].y = carsAttributes[index].status[0].y;
        carsAttributes[index].rotation = carsAttributes[index].status[0].rotation;
        for (int i = 1; i < carsAttributes[index].status.Count;i++){
            float timeFrame = carsAttributes[index].status[i].time - carsAttributes[index].status[i - 1].time;
            float xFrame = carsAttributes[index].status[i].x - carsAttributes[index].status[i - 1].x;
            float yFrame = carsAttributes[index].status[i].y - carsAttributes[index].status[i - 1].y;
            float rotationFrame = carsAttributes[index].status[i].rotation - carsAttributes[index].status[i - 1].rotation;
            while(time < carsAttributes[index].status[i].time){
                carsAttributes[index].x = carsAttributes[index].status[i - 1].x + xFrame * (time - carsAttributes[index].status[i - 1].time) / timeFrame;
                carsAttributes[index].y = carsAttributes[index].status[i - 1].y + yFrame * (time - carsAttributes[index].status[i - 1].time) / timeFrame;
                carsAttributes[index].rotation = carsAttributes[index].status[i-1].rotation + rotationFrame * (time - carsAttributes[index].status[i - 1].time) / timeFrame;
                yield return null;
            }
        }
        yield return null;
    }
}
