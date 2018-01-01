using UnityEngine;
using System.Collections;

[AddComponentMenu("Effects/UVAnim")]
public class UVAnim : MonoBehaviour {
	public float SpeedU=0.1f;
	public float SpeedV=0.1f;
	public int materialIndex = 0;

	private  Material mainMat=null;
	private float OffsetU=0;
	private float OffsetV=0;


	// Use this for initialization
	void Start () {
		if(GetComponent<Renderer>()==null)
			return;
		mainMat=GetComponent<Renderer>().materials[materialIndex];
	}
	
	// Update is called once per frame
	void Update () {

		if(mainMat==null)
			return;
		OffsetU+=Time.deltaTime*SpeedU;
		OffsetV+=Time.deltaTime*SpeedV;

		OffsetU=Mathf.Repeat(OffsetU,1);
		OffsetV=Mathf.Repeat(OffsetV,1);
	
		mainMat.mainTextureOffset=new Vector2(OffsetU,OffsetV);

	}
}
