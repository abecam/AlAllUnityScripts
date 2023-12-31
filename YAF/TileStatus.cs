using UnityEngine;
using System.Collections;

public class TileStatus : MonoBehaviour {

	public bool isSelected = false;
	public bool isOk = false;
	public bool isVisited = false;

	// Use this for initialization
	void Start () {
		isSelected = false;
		isOk = false;
		isVisited = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public bool IsSelected {
		get {
			return isSelected;
		}
		set {
			isSelected = value;
		}
	}

	public bool IsOk {
		get {
			return isOk;
		}
		set {
			isOk = value;
		}
	}

	public bool IsVisited {
		get {
			return isVisited;
		}
		set {
			isVisited = value;
		}
	}
}
