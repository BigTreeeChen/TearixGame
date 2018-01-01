using UnityEngine;
using System.Collections;



public class ParticleSystemEx :MonoBehaviour 
{

	public bool EnableCameraCulling=true;//cann not change dymamic
	
	private bool is_stop_outside_ = false;
	private bool is_stop_by_culling_ = false;
	private bool is_status_played_ = true;
	private float start_time_ = 0;
	private float last_check_time_ = -1f;
	private bool is_loop_ = false;
	private bool is_inited_ = false;
	
	public void stop()
	{
		is_stop_outside_=true;
		iternal_stop();
	}

	public void play()
	{
		is_stop_outside_=false;
		if(!is_stop_by_culling_){
			iternal_play();
			if(!is_loop_){
				start_time_=Time.time;
			}
		}
	}

	public void pause()
	{
		if(EnableCameraCulling){
			Debug.LogError("pause effect cann`t use camera culling!");
		}
		else{
			GetComponent<ParticleSystem>().Pause();
		}
	}

	public void before_recycle()
	{
		is_inited_ = false;
	}

	private void init()
	{
		is_inited_ = true;
		is_stop_outside_=false;
		is_stop_by_culling_=false;
		is_status_played_=true;
		start_time_=0;
		last_check_time_=-1f;//very important!
		is_loop_=GetComponent<ParticleSystem>().loop;
		if(!GetComponent<ParticleSystem>().playOnAwake){
			is_stop_outside_=true;
			is_status_played_=false;
		}
		else{
			if(!is_stop_outside_&&!is_loop_){
				start_time_=Time.time;
			}
		}
	}

	protected void Update()
	{
		if(!is_inited_)
		{
			init ();
		}
		if(EnableCameraCulling ){

			float t=Time.time;
			if(t-last_check_time_<0.5f)
				return;
			last_check_time_=t;
			check_seen(t);
		}

	}



	private void check_seen(float _time)
	{
		if(is_stop_outside_)
			return;
		if(!is_loop_&&(_time-start_time_)>GetComponent<ParticleSystem>().duration){
			is_status_played_=false;
			is_stop_outside_=true;
			return;
		}
		Camera cullCamera=Camera.main;

		if(cullCamera==null)
			is_stop_by_culling_=false;
		else{
			Vector3 vp=cullCamera.WorldToViewportPoint(transform.position);
			if(vp.x<1.1&&vp.x>-0.1&&vp.y<1.1&&vp.y>-0.1){
				is_stop_by_culling_=false;

			}
			else{
				is_stop_by_culling_=true;

			}
	
		}

		if(is_stop_by_culling_){
			if(!is_loop_)
				is_stop_outside_=true;//non loop is stopped once out of view
			iternal_stop();

		}
		else if(!is_stop_by_culling_&&!is_stop_outside_&&!is_status_played_){
			iternal_play();
		}
	}

	private void iternal_play()
	{
		is_status_played_=true;
		GetComponent<ParticleSystem>().Play();
	}
	
	
	private void iternal_stop()
	{
		if(is_status_played_)
		{
			is_status_played_=false;
			GetComponent<ParticleSystem>().Stop();

		}
	}


}

