/*
 * 序列帧动画控制器
 * by leon
 * */
using UnityEngine;
using System.Collections;

[AddComponentMenu("Effects/sequence images")]
public class SequenceFrameAnimCtr : MonoBehaviour 
{
	public byte grid_x;
	public byte grid_y;
	public byte fps = 2;
	public byte start_index = 0;
	public byte end_index;
	public Transform cam;
	public float uvOffsetX = 0;
	public float uvOffsetY = 0;
	

	private Material mesh_mat_;
	private byte total_grid_;
	private float till_x_;
	private float till_y_;
	private float intervals_;
	private float timer_;
	private byte current_index_;

	// Use this for initialization
	protected void Start ()
	{
		get_material();
		total_grid_ = (byte)(grid_x * grid_y);
		till_x_ = 1f/grid_x;
		till_y_ = 1f/grid_y;
		intervals_ = 1f/fps;
		timer_ = 0;
		if(end_index == 0)
		{
			end_index = (byte)(total_grid_ - 1);
		}
		current_index_ = start_index;
		if(cam != null)
		{
			transform.LookAt(cam);
		}
		next_frame();
	}
	
	// Update is called once per frame
	protected void Update () 
	{
		timer_ += Time.deltaTime;
		if(timer_ > intervals_)
		{
			next_frame();
			if(cam != null)
			{
				transform.LookAt(cam);
			}
			timer_ = 0;
		}
	}

	private void next_frame()
	{
		if (mesh_mat_ == null)
		{
			get_material();
			if(mesh_mat_ == null)
			{
				return;
			}
		}
		byte row = (byte)(grid_y - current_index_ / grid_x - 1);
		byte column = (byte)(current_index_ % grid_x);
		mesh_mat_.mainTextureScale = new Vector2(till_x_, till_y_);
		mesh_mat_.mainTextureOffset = new Vector2(column*till_x_ + uvOffsetX, row*till_y_ + uvOffsetY);
		current_index_++;
		if(current_index_ > end_index)
		{
			current_index_ = start_index;
		}
	}

	private void get_material()
	{
		mesh_mat_ =  GetComponent<Renderer>().material;
	}
}
