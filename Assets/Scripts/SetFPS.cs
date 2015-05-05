using UnityEngine;
using System.Collections;

public class SetFPS : MonoBehaviour {

	public int fps = 60;

	// Use this for initialization
	void Start () {
		Application.targetFrameRate = fps;
	}
}
