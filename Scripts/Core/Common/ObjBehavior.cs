using UnityEngine;
using System.Collections;
using SGameEngine;
using System.Collections.Generic;
using SGameSetting;

public class ObjBehavior : MonoBehaviour {
    public GameObject debug_obj_ = null;
	//Obj owner_;
    Dictionary<string, float> once_fx = new Dictionary<string, float>();

    /*
    public void delete_self()
    {
        if (debug_obj_ != null)
        {
            GameObject.Destroy(debug_obj_);
            debug_obj_ = null;
        }
    }

	public void set_owner(Obj _owner)
	{
		owner_ = _owner;
	}

	public Obj get_owner()
	{
		return owner_;
	}

    public void  set_debug_obj_info(float x, float y, float z, float angle)
    {
        if (debug_obj_ == null)
        {
            debug_obj_ = new GameObject( "debug_server_pos" );
        }
        Vector3 pos = new Vector3(x, y, z);
        pos.y = SceneManager.Instance.get_ground_height(pos);
        debug_obj_.transform.position = pos;
        debug_obj_.transform.eulerAngles = new Vector3(0, angle, 0);
    }
	
    // Check
    public void NewEvent(AnimationEvent _anim_event)
    {
        SGameEngine.SGameLog.log_error(string.Format("Anim Event No Rec: {0} model: {1}, ani: {2}", _anim_event.stringParameter, owner_.get_avatar_id(), owner_.get_cur_ani_name()));
    }

	public void ani(AnimationEvent _anim_event )
	{
		on_anim_event(_anim_event);
	}

	void AnimNotify(AnimationEvent _anim_event)
	{
		on_anim_event(_anim_event);
	}

    public void effect(AnimationEvent _anim_event)
    {
        Spirit spt = get_owner() as Spirit;
        if (spt != null && spt.skill_sound_COM != null)
        {
            spt.skill_sound_COM.play_effects(_anim_event.stringParameter);
        }
    }

    //AnimationEvent.floatParameter 内,只能执行一次的特效
    // 用于循环动作 触发特效的情况
    public void once_effect(AnimationEvent _anim_event)
    {
        
        Spirit spt = get_owner() as Spirit;
        if (spt != null && spt.skill_sound_COM != null)
        {
            string fx_str = _anim_event.stringParameter;
            if (!once_fx.ContainsKey(fx_str) || once_fx[fx_str] + _anim_event.floatParameter < Time.time)
            {
                spt.skill_sound_COM.play_effects(_anim_event.stringParameter);

                once_fx[fx_str] = Time.time;
            }
        }
    }

    public void color_corr(AnimationEvent _anim_event)
    {
        on_color_correction_event(_anim_event);
    }

    public void change_tex(AnimationEvent _anim_event)
    {
        TextureChanger tc = GetComponent<TextureChanger>();
        if (tc != null)
        {
            tc.Change(tc.tex_spawn);
        }
    }

    public void effect_loop(AnimationEvent _anim_event)
    {
        ((Spirit)get_owner()).skill_sound_COM.play_effects(_anim_event.stringParameter, Skill.POINT_EFFECT_LOOP);
    }

    public void play_music(string _id)
    {
        int music_id = 0;
        if (int.TryParse(_id, out music_id) && music_id > 0)
        {
            AudioManager.Instance.play_music_by_id(music_id);
        }
    }

    private void player_audio_base( string _id_str )
    {
        int audio_id = 0;
        if (int.TryParse(_id_str, out audio_id) && audio_id > 0)
        {
            AnimatorTimeLineMgr.Instance.set_cinema_audio_id(audio_id);
            AudioManager.Instance.play_audio_by_id(audio_id);
        }
    }

    public void play_audio( string _id_list_str )
    {
        if (string.IsNullOrEmpty(_id_list_str))
        {
            return;
        }
        string[] split_job_audio = _id_list_str.Split(',');

        if (split_job_audio.Length == 1)
        {
            player_audio_base(split_job_audio[0]);
            return;
        }
        
        SPlayer hero = ObjManager.Instance.get_hero();
        if (hero == null)
        {
            return;
        }
        int index = hero.get_job_id() - Const.JOB_ID_BEGIN;

        if (index >= split_job_audio.Length)
        {
            SGameLog.log_error(string.Format("[ObjBehavior](play_audio) not found cinema audio by job: {0}, current audio string: {1} ", hero.get_job_id(), _id_list_str));
            return;
        }
        else
        {
            player_audio_base(split_job_audio[index]);
        }
    }

    public void fx(string fx_str)
    {
        int fx_id = 0;
        // 这个fx_id 不一定是fx_obj 还可能是声音,提示文本等等.
        if (int.TryParse(fx_str, out fx_id) && fx_id > 0)
        {
            Obj obj = get_owner() as Obj;
            SfxEffect fx_obj = null;
            if (obj != null && obj.skill_sound_COM != null)
            {
                object eff = obj.skill_sound_COM.play_effect(fx_id);
                if (CommonTool.is_sfx(eff))
                {
                    fx_obj = eff as SfxEffect;
                }
            }
            else
            {
                fx_obj = SfxEffect.play_obj_sfx((Obj)get_owner(), fx_id, gameObject);
            }
            
            if (fx_obj != null && sfx_info.data[fx_id].loop == 1)
            {
                if (loop_fx.ContainsKey(fx_id))
                {
                    SfxEffect old_fx = loop_fx[fx_id];
                    if (old_fx != null)
                    {
                        old_fx.finish();
                    }
                }
                loop_fx[fx_id] = fx_obj;
            }
        }
		else
		{
			SGameLog.log_error(string.Format("fx {0} is invalid {1}, current cinema id is  {2}, this error may be caused by a cinema", fx_str, gameObject.name, AnimatorTimeLineMgr.current_cinema != null ? AnimatorTimeLineMgr.current_cinema.cinema_id_ : 0));
		}
    }

    public void stop_fx(string fx_str)
    {
        int fx_id = 0;
        if (int.TryParse(fx_str, out fx_id) && fx_id > 0)
        {
            if (loop_fx.ContainsKey(fx_id))
            {
                loop_fx[fx_id].finish();
                loop_fx.Remove(fx_id);
            }
        }
    }

    public void stop_loop_fx()
    {
        foreach (int k in loop_fx.Keys)
        {
            loop_fx[k].finish();
        }
        loop_fx.Clear();
    }

    public void hide_monster(string goName)
    {
        if (string.IsNullOrEmpty(goName))
        {
            gameObject.SetActive(false);
            return;
        }

        GameObject go = AnimatorTimeLineMgr.Instance.get_cinema_go(goName);
        if (go != null)
        {
            go.SetActive(false);
        }
    }

    public void say(string _word_str)
    {
        if (string.IsNullOrEmpty(_word_str))
        {
            CinemaUI.instance.ClearSubtitle();
        }
        else
        {
            int chat_id = 0;
            if (int.TryParse(_word_str, out chat_id) && CinemaChatInfo.data.ContainsKey(chat_id))
            {
                _word_str = CinemaChatInfo.data[chat_id].Content;
            }

			if (CinemaUI.instance != null)
			{
	            CinemaUI.instance.SetSubtitle(_word_str);
			}
        }
    }

    public void color_correction(string _id_str)
    {
        //兼容之前的做法，不填就是默认播放63号黑屏变色
        if (string.IsNullOrEmpty(_id_str))
        {
            ColorCorrectionMgr.Instance.start_color_correction(63);
            return;
        }

        int id = 0;
        if (int.TryParse(_id_str, out id))
        {
            ColorCorrectionMgr.Instance.start_color_correction(id);
        }
    }

    public void activate_trigger(string cin_id)
    {
        int cid = 0;
        if (int.TryParse(cin_id, out cid) && cid > 0)
        {
            STrigger trg = ObjManager.Instance.get_obj_by_cinema_index(cid) as STrigger;
            bool flag = AnimatorTimeLineMgr.Instance.IsPlaying;
            if (trg != null)
                trg.next_state(flag);
        }
    }

    public void play_dissolve()
    {
        Obj obj = get_owner();
        if (obj == null)
        {
            return;
        }

        StartCoroutine(obj.do_dissolve_effect_coroutine());
    }

	//highlight
	public void hilight(AnimationEvent _anim_event){

		if(!color.data.ContainsKey(_anim_event.intParameter))
			SGameEngine.SGameLog.log_error(string.Format("color name {0} not defined",_anim_event.intParameter));
		color.DataStruct d=color.data[_anim_event.intParameter];
		Color c=new Color(d.r,d.g,d.b,d.a);
		if (owner_ != null &&  owner_.buff_material_COM != null)
        {
            owner_.buff_material_COM.AddBuffMat(_anim_event.floatParameter, new System.Object[1] { c }, ComponentBuffMat.BUFF_MAT_TYPE.MONSTRT_WARNING, ComponentBuffMat.Priority.WARNING);
        }
	}

	//material _anim point
	public void mat_anim(AnimationEvent _anim_event)
	{
		MaterialAnim materia_anim = GetComponent<MaterialAnim>();
		if(materia_anim == null)
		{
			return;
		}
		materia_anim.play();
	}

	void on_anim_event(AnimationEvent _anim_event)
	{
        if (owner_ == null)
        {
            return;
        }
        if (owner_.get_model() == null)
        {
            return;
        }
		//SGameEngine.SGameLog.log(string.Format("on_anim_event {0} {1} {2} {3}", _anim_event.animationState, _anim_event.stringParameter, _anim_event.time, owner_.get_cur_ani_name()));
        if (is_valid_owner() && owner_.keyframe_mgr_COM != null && _anim_event.stringParameter != null)
        {
            ((Spirit)owner_).keyframe_mgr_COM.on_anim_key_event(owner_.get_cur_ani_hash(), _anim_event.stringParameter);
        }
	}

    bool is_valid_owner()
    {
        if (owner_.is_spirit())
        {
            return true;
        }
        else if(owner_.is_client_obj())
        {
            return true;
        }

        return false;
    }

    void on_color_correction_event(AnimationEvent _anim_event)
    {
        int id = _anim_event.intParameter;
        ColorCorrectionMgr.Instance.start_color_correction(id);
    }

    void OnAnimatorMove()
    {
        if (ObjManager.Instance.is_valid_obj(owner_))
        {
            if (!owner_.is_model_load_succ())
            {
                return;
            }

            if (owner_ is SDropItem || owner_ is PickPoint)
            {
                return;
            }
            else if (owner_ is SMonster)
            {
                SMonster mst = (SMonster)owner_;
                if (!mst.can_corr_pos_to_server())
                {
                    return;
                }                
            }

            if(owner_.state_COM.is_state(StateDefine.STATE_SKILLING))
            {
                StateBase state = owner_.state_COM.get_desc(StateDefine.STATE_SKILLING).handle;
                StateSkilling state_skill = (StateSkilling)state;
                if (state_skill.get_anim_move())
                {
                    Animator animator = GetComponent<Animator>();
                    state_skill.set_anim_move_delta(animator.deltaPosition);
                    state_skill.set_anim_angle_delta(animator.deltaRotation.eulerAngles.y);
                }
            }
        }
    }

    public void reset_owner_scale()
    {
        if (owner_ != null)
            owner_.set_scale(owner_.get_init_scale());
    }
    */
#if UNITY_EDITOR
    void OnDrawGizmos()
    {	
        //if (owner_ != null && owner_.is_monster())
        {
            return;
        }

		if (debug_obj_ != null)
		{
			Gizmos.color = Color.red;
			float x = debug_obj_.transform.position.x;
			float y = debug_obj_.transform.position.y;
			float z = debug_obj_.transform.position.z;
			Gizmos.DrawRay(new Ray(new Vector3(x, y, z), debug_obj_.transform.forward));
		}
    }
#endif

}
