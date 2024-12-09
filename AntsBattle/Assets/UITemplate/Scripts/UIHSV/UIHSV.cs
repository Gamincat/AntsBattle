using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Graphic))]
public class UIHSV : MonoBehaviour {

	Graphic img;
	Graphic Img{
		get{
			if (img == null)
				img = this.GetComponent<Graphic> ();
			return img;
		}
	}

	Material Mat{
		get{ 
			if (Img.material == null || !Img.material.HasProperty("_Hue")
				|| !Img.material.HasProperty("_Sat")
				|| !Img.material.HasProperty("_Val")
				|| !Img.material.HasProperty("_Color")) {
				Img.material = new Material (Shader.Find ("Custom/HsvUI"));
			}
			return Img.material;
		}
	}

	[Range(0f, 360f)]
	public float hue = 0;
	[Range(-1f, 1f)]
	public float satulation = 0;
	[Range(0f, 2f)]
	public float value = 1f;
	[Range(0f, 1f)]
	public float alpha = 1f;

	private Color color;

	void Update(){
		UpdateHue ();
		UpdateSatulation();
		UpdateValue();
		UpdateAlpha();
	}

	void UpdateHue(){
		Mat.SetFloat ("_Hue", hue);
	}

	void UpdateSatulation()
	{
		Mat.SetFloat("_Sat",satulation);
	}
	void UpdateValue()
	{
		Mat.SetFloat("_Val", value);
	}
	void UpdateAlpha()
	{
		color = Mat.GetColor("_Color");
		color = new Color(color.r, color.g, color.b, alpha);
		Mat.SetColor("_Color", color);
	}

	// 色を変える
	public void ChangeHue(float val = 25){
		hue += val;
		hue = hue % 360f;
	}
	public void ChangeSatulation(float val = 25)
	{
		satulation += val;
		satulation = satulation % 100;
	}
	public void ChangeValue(float val = 25)
	{
		value += val;
		value = value % 100;
	}
	public void ChangeAlpha(float val = 25)
	{
		alpha += val;
		alpha = alpha % 100;
	}
}
