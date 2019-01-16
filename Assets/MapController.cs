using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour {
    public List<Vector2Int> streets;
    public GameObject straight;
    public GameObject turn;
    public GameObject TCross;
    public GameObject XCross;
    public TextAsset map_textAsset;
    public Transform map_folder;

	// Use this for initialization
	void Start () {
        InitStreets();
	}
	
    void InitStreets(){
        string[] tempString = map_textAsset.text.Split('\n');
        streets = new List<Vector2Int>();

        foreach(string s in tempString){
            string[] tmp = s.Split(' ');
            streets.Add(new Vector2Int(int.Parse(tmp[0]), int.Parse(tmp[1])));
        }

        foreach(Vector2Int street in streets){
            bool hasUp = streets.Exists(s => s.x == street.x && s.y == street.y + 1);
            bool hasDown = streets.Exists(s => s.x == street.x && s.y == street.y - 1);
            bool hasRight = streets.Exists(s => s.x == street.x + 1 && s.y == street.y);
            bool hasLeft = streets.Exists(s => s.x == street.x - 1 && s.y == street.y);

            int neighbor_amount = (hasUp ? 1 : 0) + (hasDown ? 1 : 0) + (hasRight ? 1 : 0) + (hasLeft ? 1 : 0);
            if (neighbor_amount == 1){
                if (hasUp || hasDown)
                    InstantiateStreet(1, 0, street.x, street.y);
                else if (hasRight || hasLeft)
                    InstantiateStreet(1, 90.0f, street.x, street.y);
            } else if (neighbor_amount == 2){
                if (hasUp && hasLeft)
                    InstantiateStreet(2, 0, street.x, street.y);
                else if (hasDown && hasLeft)
                    InstantiateStreet(2, -90.0f, street.x, street.y);
                else if (hasDown && hasRight)
                    InstantiateStreet(2, -180.0f, street.x, street.y);
                else if (hasUp && hasRight)
                    InstantiateStreet(2, -270.0f, street.x, street.y);
                else if (hasUp && hasDown)
                    InstantiateStreet(1, 0, street.x, street.y);
                else if (hasLeft && hasRight)
                    InstantiateStreet(1, 90.0f, street.x, street.y);
            } else if (neighbor_amount == 3){
                if (!hasRight)
                    InstantiateStreet(3, 0, street.x, street.y);
                if (!hasUp)
                    InstantiateStreet(3, -90.0f, street.x, street.y);
                if (!hasLeft)
                    InstantiateStreet(3, -180.0f, street.x, street.y);
                if (!hasDown)
                    InstantiateStreet(3, -270.0f, street.x, street.y);
            } else if (neighbor_amount == 4){
                InstantiateStreet(4, 0, street.x, street.y);
            }
        }
    }

    void InstantiateStreet(int type, float angle, int x, int y){
        GameObject gmo = Instantiate((type == 1) ? straight :
                                     (type == 2) ? turn :
                                     (type == 3) ? TCross : XCross, map_folder);
        gmo.transform.localPosition = new Vector3(x * 25, y * -25, 0);
        gmo.transform.localScale = new Vector3(0.8333333f, 0.8333333f, 0.8333333f);
        gmo.transform.localRotation = Quaternion.Euler(0, 0, angle);
    }
}
