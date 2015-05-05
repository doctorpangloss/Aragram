using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {
	
	public GameObject mainWindow;

	bool sideWindowOpen;
	public float tweenTime = 0.2f;

	public AudioSource[] fartSounds;
	int fartIndex;
	int noCount;

	public GameObject loadBarParent;
	public UISprite loadingBarSprite;
	public int loadingBarXDimension = 500;
	public int loadingBarYDimension = 20;
	public float loadingBarSpeed = 0.35f;
	float loadingBarTimer;
	public tk2dSprite loadingSprite;
	public UIScrollBar scrollBar;

	public UIScrollView scrollView;

	public UISprite popupOverlaySprite;
	public GameObject popupParentObject;
	public GameObject yesButton;
	public GameObject noButton;

	bool popupActive;
	public GameObject PictureObjectPrefab;
	public float pictureObjectBaseSize = 820;
	public string photoFolder = "Content";

	public PictureData[] pictureData;
	float currentPositionIndex;

	[System.Serializable]
	public class PictureData{
		public string pictureName;
		public string pictureCaption;
	}

	public PictureObject[] pictureObjects;

	public GameObject kissPopupParent;
	bool kissActive;

	int noClicks;

	public ParticleSystem fireworks1;
	public ParticleSystem fireworks2;
	public ParticleSystem fireworks3;

	int fireworkTapCounter;

	void Start(){
		popupOverlaySprite.alpha = 0;
		popupParentObject.transform.localScale = Vector3.zero;
		popupParentObject.SetActive(false);

		Init();
	}

	void Init(){
		pictureObjects = new PictureObject[pictureData.Length];
		float currentPosition = 0;
		for (int i = 0; i < pictureData.Length; i++){
			float objectHeight = pictureObjectBaseSize;
			GameObject obj = (GameObject)GameObject.Instantiate(PictureObjectPrefab);
			obj.transform.parent = scrollView.transform;
			obj.transform.localScale = Vector3.one;
			pictureObjects[i] = obj.GetComponent<PictureObject>();

			Texture2D picTexture = (Texture2D)Resources.Load(photoFolder+"/"+pictureData[i].pictureName);
			pictureObjects[i].pictureTexture.mainTexture = picTexture;
			float ratio = ((float)picTexture.height)/picTexture.width;

			if (ratio == 1){
				pictureObjects[i].pictureTexture.SetDimensions(641,641);
				pictureObjects[i].lowerHalf.localPosition = new Vector3(0, -320, 0);
			}else{
				float picHeight = 641*ratio;
				pictureObjects[i].pictureTexture.SetDimensions(641,Mathf.FloorToInt(picHeight));
				pictureObjects[i].lowerHalf.localPosition = Vector3.zero;
				pictureObjects[i].lowerHalf.localPosition = new Vector3(0, (-320+(640-picHeight)), 0);
				objectHeight = pictureObjectBaseSize - (640-picHeight);
			}
			pictureObjects[i].likesLabel.text = Random.Range(12, 87) + " likes";
			pictureObjects[i].captionLabel.text = pictureData[i].pictureCaption;

			//pictureObjects[i].transform.localPosition = new Vector3(0, i * pictureObjectBaseSize * -1, 0);
			pictureObjects[i].transform.localPosition = new Vector3(0, currentPosition, 0);
			currentPosition -= objectHeight;
		}
		//loadBarParent.transform.localPosition = new Vector3(0, ((pictureData.Length-1)*pictureObjectBaseSize*-1)-640, 0);
		loadBarParent.transform.localPosition = new Vector3(0, currentPosition+150, 0);
		scrollView.transform.localPosition = new Vector3(0,119,0);

		SetButtonSize ();
	}

	void Update(){
		LoadingBar();

		if (kissActive){
			if (Input.GetMouseButtonDown(0)){
				fireworkTapCounter += 1;
			}else if (Input.touchCount > 0){
				if (Input.touches[0].phase == TouchPhase.Began){
					fireworkTapCounter += 1;
				}
			}

			if (fireworkTapCounter >= 5){
				Application.LoadLevel(Application.loadedLevel);
			}
		}
	}

	void LoadingBar(){
		if (scrollBar.value >= 0.99f && !sideWindowOpen){
			loadingBarTimer += Time.deltaTime * loadingBarSpeed;
		}
		loadingBarSprite.SetDimensions(Mathf.CeilToInt(Mathf.Lerp(0, loadingBarXDimension, loadingBarTimer)), loadingBarYDimension);

		if (!popupActive && loadingBarTimer >= 1){
			StartPopup();
			popupActive = true;
		}
	}

	void StartPopup(){
		loadingSprite.gameObject.SetActive(false);
		loadingBarSprite.gameObject.SetActive(false);
		StartCoroutine(PopupRoutine());
	}

	IEnumerator PopupRoutine(){
		iTween.ValueTo(this.gameObject, iTween.Hash("from", 0, "to", 0.85f, "time", 0.5f, "easetype", iTween.EaseType.linear, "onupdate", "DarkOverlayAlpha"));
		yield return new WaitForSeconds(0.45f);
		popupParentObject.SetActive(true);
		iTween.ScaleTo(popupParentObject, iTween.Hash("scale", Vector3.one, "time", 0.5f, "easetype", iTween.EaseType.easeOutBack, "islocal", true));
		yesButton.transform.localScale = Vector3.zero;
		noButton.transform.localScale = Vector3.zero;
		yield return new WaitForSeconds(0.4f);
		iTween.ScaleTo(noButton, iTween.Hash("scale", Vector3.one, "time", 0.5f, "easetype", iTween.EaseType.easeOutBack, "islocal", true));
		//iTween.ScaleTo(yesButton, iTween.Hash("scale", Vector3.one, "time", 0.5f, "easetype", iTween.EaseType.easeOutBack, "islocal", true));
	}

	public void OpenSidePanel(){
		if (popupActive)
			return;

		if (!sideWindowOpen){
			iTween.MoveTo(mainWindow, iTween.Hash("x", 360, "islocal", true, "time", tweenTime, "easetype", iTween.EaseType.easeOutQuad));
			sideWindowOpen = true;
		}else{
			iTween.MoveTo(mainWindow, iTween.Hash("x", 0, "islocal", true, "time", tweenTime, "easetype", iTween.EaseType.easeOutQuad));
			sideWindowOpen = false;
		}
	}

	public void NoButton(){
		PlayFartSound(fartIndex);
		fartIndex += 1;
		fartIndex = fartIndex >= fartSounds.Length ? 0 : fartIndex;
		noClicks += 1;
		SetButtonSize ();
	}

	public void YesButton(){
		//kissPopupParent.SetActive(true);
		//kissPopupParent.transform.localScale = Vector3.zero;
		//iTween.ScaleTo(kissPopupParent, iTween.Hash("scale", Vector3.one, "time", 0.5f, "easetype", iTween.EaseType.easeOutBack, "islocal", true));
		StartCoroutine(FireworkRoutine());
	}

	IEnumerator FireworkRoutine(){
		iTween.ValueTo(this.gameObject, iTween.Hash("from", popupOverlaySprite.color.a, "to", 1, "time", 0.5f, "easetype", iTween.EaseType.linear, "onupdate", "DarkOverlayAlpha"));
		popupParentObject.SetActive(false);
		kissActive = true;
		fireworks1.Play();
		yield return new WaitForSeconds(3);
		fireworks1.Stop();
		fireworks2.Play();
		yield return new WaitForSeconds(10);
		fireworks1.Play ();
		fireworks3.Play();
	}

	void SetButtonSize(){
		var noButtonSize = 1 - (0.1f * noClicks);
		var yesButtonSize = 0.15f * noClicks;
		//noButton.transform.localScale = new Vector3 (noButtonSize, noButtonSize, 1);
		yesButton.transform.localScale = new Vector3 (yesButtonSize, yesButtonSize, 1);
	}

	void PlayFartSound (int index){
		fartSounds[index].Play();
	}

	void DarkOverlayAlpha(float alpha){
		popupOverlaySprite.alpha = alpha;
	}
}
