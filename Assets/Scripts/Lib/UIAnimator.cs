using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIAnimator : MonoBehaviour
{
	public delegate void AllFinishedHandler();
	public event AllFinishedHandler onAllFinished;

	GameObject target;
	List<UIAnimation> animations = new List<UIAnimation>();

	Queue<UIAnimation> q = new Queue<UIAnimation>();
	bool enabled = false;

	void Start()
	{

	}

	void UpdateWithDelta(float delta)
	{
		q.Clear();
		foreach (var a in animations)
		{
			q.Enqueue(a);
			a.tempTime = delta;
		}
		animations.Clear();

		while (q.Count > 0)
		{
			UIAnimation anime = q.Dequeue();
			if (anime.tempTime >= anime.remainTime)
			{
				float extraTime = anime.tempTime - anime.remainTime;
				anime.nowTime = anime.duration;
				if (anime.onAnimate != null)
					anime.onAnimate(1.0f);
				if (anime.onFinish != null)
					anime.onFinish();
				foreach (var newAnime in anime.next)
				{
					newAnime.Reset();
					newAnime.tempTime = extraTime;
					if (newAnime.onStart != null)
						newAnime.onStart();
					q.Enqueue(newAnime);
				}
			}
			else
			{
				anime.nowTime += anime.tempTime;
				if (anime.onAnimate != null)
					anime.onAnimate(anime.easeFunc(0.0f, 1.0f, anime.nowTime / anime.duration));
				animations.Add(anime);
			}
		}

		if (animations.Count == 0)
		{
			Finish();
		}
	}

	void Update()
	{
		if (!enabled)
			return;
		UpdateWithDelta(Time.deltaTime);
	}

	/// <summary>
	/// This would immediately finish all animation by given a extremely large time delta. 
	/// </summary>
	public void FinishImmediate()
	{
		UpdateWithDelta(float.MaxValue);
	}

	void Finish()
	{
		if (onAllFinished != null)
		{
			onAllFinished();
		}
		GameObject.Destroy(this);
	}

	/// <summary>
	/// This would create a UIAnimator component for target game object and start given animation automatically.
	/// When every thing get finished this component will destroy itself automatically.
	/// </summary>
	/// <param name="go">Target game object to attach on</param>
	/// <param name="animation">Initial animation to be played. Can be null, and this could trigger onAllFinished immediately.</param>
	/// <param name="onAllFinished">Callback when all animations are finished(including next of animation).</param>
	/// <returns>UIAnimator component</returns>
	public static UIAnimator Begin(GameObject go, UIAnimation animation, AllFinishedHandler onAllFinished = null)
	{
		UIAnimator animator = go.AddComponent<UIAnimator>();
		animator.target = go;
		animator.AddAnimation(animation);
		animator.enabled = true;
		animator.onAllFinished += onAllFinished;
		return animator;
	}

	public void AddAnimation(UIAnimation animation)
	{
		if (animation != null)
		{
			animation.Reset();
			animations.Add(animation);
			if (animation.onStart != null)
				animation.onStart();
		}
	}

	/// <summary>
	/// Immediately finish all animations on target object. This action will finish the rest part animations!
	/// </summary>
	/// <param name="go"></param>
	public static void FinishAllOnObject(GameObject go)
	{
		foreach (var animator in go.GetComponents<UIAnimator>())
		{
			animator.FinishImmediate();
		}

	}

	/// <summary>
	/// Immediately stop all animations on target object. This action will not finish the rest animations!
	/// </summary>
	/// <param name="go"></param>
	public static void StopAllOnObject(GameObject go)
	{
		foreach (var animator in go.GetComponents<UIAnimator>())
		{
			GameObject.Destroy(animator);
		}

	}
}

public class UIAnimation
{
	public List<UIAnimation> next = new List<UIAnimation>();
	public delegate void AnimateHandler(float progress);
	public delegate void OnStartHandler();
	public delegate void OnFinishHandler();

	public AnimateHandler onAnimate;
	public OnStartHandler onStart;
	public OnFinishHandler onFinish;
	public float duration;
	public float nowTime;
	/// <summary>
	/// this is used by UIAnimator in Update for time calculating. DO NOT USE IT.
	/// </summary>
	public float tempTime;

	public enum EaseType {
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic,
        EaseInQuart,
        EaseOutQuart,
        EaseInOutQuart,
        EaseInQuint,
        EaseOutQuint,
        EaseInOutQuint,
        EaseOutInQuint,
        EaseInSine,
        EaseOutSine,
        EaseInOutSine,
        EaseInExpo,
        EaseOutExpo,
        EaseInOutExpo,
        EaseInCirc,
        EaseOutCirc,
        EaseInOutCirc,
        Linear,
        Spring,
        EaseInBounce,
        EaseOutBounce,
        EaseInOutBounce,
        EaseInBack,
        EaseOutBack,
        EaseInOutBack,
        EaseInElastic,
        EaseOutElastic,
        EaseInOutElastic,
        Punch
	};

	public EaseType easeType = EaseType.Linear;

	public delegate float EaseFunctionHanlder(float start, float end, float value);
	public EaseFunctionHanlder easeFunc = UIAnimationEaseFunc.linear;

	public float remainTime { get { return duration - nowTime; } }

	public UIAnimation(float duration, AnimateHandler onAnimate, 
		EaseType easeType = EaseType.Linear, UIAnimation next = null)
	{
		this.duration = duration;
		this.onAnimate = onAnimate;
		AddNext(next);
		SetEaseFunction(easeType);
		Reset();
	}

	void SetEaseFunction(EaseType easeType)
	{
		switch (easeType)
		{
			case EaseType.EaseInQuart:
				easeFunc = UIAnimationEaseFunc.easeInQuart;
				break;
			case EaseType.EaseOutQuart:
				easeFunc = UIAnimationEaseFunc.easeOutQuart;
				break;
			case EaseType.EaseInOutQuart:
				easeFunc = UIAnimationEaseFunc.easeInOutQuart;
				break;
			case EaseType.EaseInBack:
				easeFunc = UIAnimationEaseFunc.easeInBack;
				break;
			case EaseType.EaseOutBack:
				easeFunc = UIAnimationEaseFunc.easeOutBack;
				break;
			case EaseType.EaseInOutBack:
				easeFunc = UIAnimationEaseFunc.easeInOutBack;
				break;
			case EaseType.EaseOutElastic:
				easeFunc = UIAnimationEaseFunc.easeOutElastic;
				break;
            case EaseType.EaseInCubic:
                easeFunc = UIAnimationEaseFunc.easeInCubic;
                break;
            case EaseType.EaseOutCubic:
                easeFunc = UIAnimationEaseFunc.easeOutCubic;
                break;
            case EaseType.EaseInOutCubic:
                easeFunc = UIAnimationEaseFunc.easeInOutCubic;
                break;
            case EaseType.Punch:
                easeFunc = UIAnimationEaseFunc.punch;
                break;
            case EaseType.EaseInQuad:
                easeFunc = UIAnimationEaseFunc.easeInQuad;
                break;
            case EaseType.EaseOutQuad:
                easeFunc = UIAnimationEaseFunc.easeOutQuad;
                break;
            case EaseType.EaseInOutQuad:
                easeFunc = UIAnimationEaseFunc.easeInOutQuad;
                break;
            case EaseType.EaseInQuint: easeFunc = UIAnimationEaseFunc.easeInQuint; break;
            case EaseType.EaseOutQuint: easeFunc = UIAnimationEaseFunc.easeOutQuint; break;
            case EaseType.EaseInOutQuint: easeFunc = UIAnimationEaseFunc.easeInOutQuint; break;
            case EaseType.EaseOutInQuint: easeFunc = UIAnimationEaseFunc.easeOutInQuint; break;
            case EaseType.EaseInSine: easeFunc = UIAnimationEaseFunc.easeInSine; break;
            case EaseType.EaseOutSine: easeFunc = UIAnimationEaseFunc.easeOutSine; break;
            case EaseType.EaseInOutSine: easeFunc = UIAnimationEaseFunc.easeInOutSine; break;
            case EaseType.EaseInExpo: easeFunc = UIAnimationEaseFunc.easeInExpo; break;
            case EaseType.EaseOutExpo: easeFunc = UIAnimationEaseFunc.easeOutExpo; break;
            case EaseType.EaseInOutExpo: easeFunc = UIAnimationEaseFunc.easeInOutExpo; break;
            case EaseType.EaseInCirc: easeFunc = UIAnimationEaseFunc.easeInCirc; break;
            case EaseType.EaseOutCirc: easeFunc = UIAnimationEaseFunc.easeOutCirc; break;
            case EaseType.EaseInOutCirc: easeFunc = UIAnimationEaseFunc.easeInOutCirc; break;
            case EaseType.Linear: easeFunc = UIAnimationEaseFunc.linear; break;
            case EaseType.Spring: easeFunc = UIAnimationEaseFunc.spring; break;
            case EaseType.EaseInBounce: easeFunc = UIAnimationEaseFunc.easeInBounce; break;
            case EaseType.EaseOutBounce: easeFunc = UIAnimationEaseFunc.easeOutBounce; break;
            case EaseType.EaseInOutBounce: easeFunc = UIAnimationEaseFunc.easeInOutBounce; break;
            case EaseType.EaseInElastic: easeFunc = UIAnimationEaseFunc.easeInElastic; break;
            case EaseType.EaseInOutElastic: easeFunc = UIAnimationEaseFunc.easeInOutElastic; break;
			default:
				easeFunc = UIAnimationEaseFunc.linear;
				break;
		}
	}

	public void AddNext(UIAnimation next)
	{
		if (next != null)
			this.next.Add(next);
	}

	public void Reset()
	{
		nowTime = 0.0f;
		tempTime = 0.0f;
	}

	public static UIAnimation TweenAlpha(UIWidget widget, float duration, float from, float to, EaseType easeType = EaseType.Linear, OnStartHandler onStart = null, OnFinishHandler onFinish = null)
	{
		UIAnimation anime = new UIAnimation(duration, (p) => { widget.alpha = Mathf.Lerp(from, to, p); }, easeType);
		if (onStart != null)
			anime.onStart += onStart;
		if (onFinish != null)
			anime.onFinish += onFinish;
		return anime;
	}

	public static UIAnimation TweenAlpha(UIPanel widget, float duration, float from, float to, EaseType easeType = EaseType.Linear, OnStartHandler onStart = null, OnFinishHandler onFinish = null)
	{
		UIAnimation anime = new UIAnimation(duration, (p) => { widget.alpha = Mathf.Lerp(from, to, p); }, easeType);
		if (onStart != null)
			anime.onStart += onStart;
		if (onFinish != null)
			anime.onFinish += onFinish;
		return anime;
	}

	public static UIAnimation TweenPosition(GameObject widget, float duration, Vector3 from, Vector3 to, EaseType easeType = EaseType.Linear, OnStartHandler onStart = null, OnFinishHandler onFinish = null)
	{
		UIAnimation anime = new UIAnimation(duration, (p) => { widget.transform.localPosition = Vector3.Lerp(from, to, p); }, easeType);
		if (onStart != null)
			anime.onStart += onStart;
		if (onFinish != null)
			anime.onFinish += onFinish;
		return anime;
	}


}

/// <summary>
/// Many ease function for UIAnimation.
/// </summary>
public class UIAnimationEaseFunc
{
	#region Ease Curves (from iTween)

    public static float linear(float start, float end, float value)
    {
        return Mathf.Lerp(start, end, value);
    }

    public static float clerp(float start, float end, float value)
    {
        float min = 0.0f;
        float max = 360.0f;
        float half = Mathf.Abs((max - min) / 2.0f);
        float retval = 0.0f;
        float diff = 0.0f;
        if ((end - start) < -half)
        {
            diff = ((max - start) + end) * value;
            retval = start + diff;
        }
        else if ((end - start) > half)
        {
            diff = -((max - end) + start) * value;
            retval = start + diff;
        }
        else retval = start + (end - start) * value;
        return retval;
    }

    public static float spring(float start, float end, float value)
    {
        value = Mathf.Clamp01(value);
        value = (Mathf.Sin(value * Mathf.PI * (0.2f + 2.5f * value * value * value)) * Mathf.Pow(1f - value, 2.2f) + value) * (1f + (1.2f * (1f - value)));
        return start + (end - start) * value;
    }

    public static float easeInQuad(float start, float end, float value)
    {
        end -= start;
        return end * value * value + start;
    }

    public static float easeOutQuad(float start, float end, float value)
    {
        end -= start;
        return -end * value * (value - 2) + start;
    }

    public static float easeInOutQuad(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end / 2 * value * value + start;
        value--;
        return -end / 2 * (value * (value - 2) - 1) + start;
    }

    public static float easeInCubic(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value + start;
    }

    public static float easeOutCubic(float start, float end, float value)
    {
        value--;
        end -= start;
        return end * (value * value * value + 1) + start;
    }

    public static float easeInOutCubic(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end / 2 * value * value * value + start;
        value -= 2;
        return end / 2 * (value * value * value + 2) + start;
    }

    public static float easeInQuart(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value * value + start;
    }

    public static float easeOutQuart(float start, float end, float value)
    {
        value--;
        end -= start;
        return -end * (value * value * value * value - 1) + start;
    }

    public static float easeInOutQuart(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end / 2 * value * value * value * value + start;
        value -= 2;
        return -end / 2 * (value * value * value * value - 2) + start;
    }

    public static float easeInQuint(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value * value * value + start;
    }

    public static float easeOutInQuint(float start, float end, float value)
    {
        value /= .5f;
        if (value <= 1)
            return easeOutQuint(start, (start + end) * .5f, value);
        else
            return easeInQuint((start + end) * .5f, end, value - 1);
    }

    public static float easeOutQuint(float start, float end, float value)
    {
        value--;
        end -= start;
        return end * (value * value * value * value * value + 1) + start;
    }

    public static float easeInOutQuint(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end / 2 * value * value * value * value * value + start;
        value -= 2;
        return end / 2 * (value * value * value * value * value + 2) + start;
    }

    public static float easeInSine(float start, float end, float value)
    {
        end -= start;
        return -end * Mathf.Cos(value / 1 * (Mathf.PI / 2)) + end + start;
    }

    public static float easeOutSine(float start, float end, float value)
    {
        end -= start;
        return end * Mathf.Sin(value / 1 * (Mathf.PI / 2)) + start;
    }

    public static float easeInOutSine(float start, float end, float value)
    {
        end -= start;
        return -end / 2 * (Mathf.Cos(Mathf.PI * value / 1) - 1) + start;
    }

    public static float easeInExpo(float start, float end, float value)
    {
        end -= start;
        return end * Mathf.Pow(2, 10 * (value / 1 - 1)) + start;
    }

    public static float easeOutExpo(float start, float end, float value)
    {
        end -= start;
        return end * (-Mathf.Pow(2, -10 * value / 1) + 1) + start;
    }

    public static float easeInOutExpo(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return end / 2 * Mathf.Pow(2, 10 * (value - 1)) + start;
        value--;
        return end / 2 * (-Mathf.Pow(2, -10 * value) + 2) + start;
    }

    public static float easeInCirc(float start, float end, float value)
    {
        end -= start;
        return -end * (Mathf.Sqrt(1 - value * value) - 1) + start;
    }

    public static float easeOutCirc(float start, float end, float value)
    {
        value--;
        end -= start;
        return end * Mathf.Sqrt(1 - value * value) + start;
    }

    public static float easeInOutCirc(float start, float end, float value)
    {
        value /= .5f;
        end -= start;
        if (value < 1) return -end / 2 * (Mathf.Sqrt(1 - value * value) - 1) + start;
        value -= 2;
        return end / 2 * (Mathf.Sqrt(1 - value * value) + 1) + start;
    }

    public static float easeInBounce(float start, float end, float value)
    {
        end -= start;
        float d = 1f;
        return end - easeOutBounce(0, end, d - value) + start;
    }

    public static float easeOutBounce(float start, float end, float value)
    {
        value /= 1f;
        end -= start;
        if (value < (1 / 2.75f))
        {
            return end * (7.5625f * value * value) + start;
        }
        else if (value < (2 / 2.75f))
        {
            value -= (1.5f / 2.75f);
            return end * (7.5625f * (value) * value + .75f) + start;
        }
        else if (value < (2.5 / 2.75))
        {
            value -= (2.25f / 2.75f);
            return end * (7.5625f * (value) * value + .9375f) + start;
        }
        else
        {
            value -= (2.625f / 2.75f);
            return end * (7.5625f * (value) * value + .984375f) + start;
        }
    }
    
    public static float easeInOutBounce(float start, float end, float value)
    {
        end -= start;
        float d = 1f;
        if (value < d / 2) return easeInBounce(0, end, value * 2) * 0.5f + start;
        else return easeOutBounce(0, end, value * 2 - d) * 0.5f + end * 0.5f + start;
    }

    public static float easeInBack(float start, float end, float value)
    {
        end -= start;
        value /= 1;
        float s = 1.70158f;
        return end * (value) * value * ((s + 1) * value - s) + start;
    }

    public static float easeOutBack(float start, float end, float value)
    {
        float s = 1.70158f;
        end -= start;
        value = (value / 1) - 1;
        return end * ((value) * value * ((s + 1) * value + s) + 1) + start;
    }

    public static float easeInOutBack(float start, float end, float value)
    {
        float s = 1.70158f;
        end -= start;
        value /= .5f;
        if ((value) < 1)
        {
            s *= (1.525f);
            return end / 2 * (value * value * (((s) + 1) * value - s)) + start;
        }
        value -= 2;
        s *= (1.525f);
        return end / 2 * ((value) * value * (((s) + 1) * value + s) + 2) + start;
    }
    
    public static float easeInElastic(float start, float end, float value)
    {
        end -= start;

        float d = 1f;
        float p = d * .3f;
        float s = 0;
        float a = 0;

        if (value == 0) return start;

        if ((value /= d) == 1) return start + end;

        if (a == 0f || a < Mathf.Abs(end))
        {
            a = end;
            s = p / 4;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
        }

        return -(a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
    }
    
    public static float easeOutElastic(float start, float end, float value)
    {
        /* GFX47 MOD END */
        //Thank you to rafael.marteleto for fixing this as a port over from Pedro's UnityTween
        end -= start;

        float d = 1f;
        float p = d * .3f;
        float s = 0;
        float a = 0;

        if (value == 0) return start;

        if ((value /= d) == 1) return start + end;

        if (a == 0f || a < Mathf.Abs(end))
        {
            a = end;
            s = p / 4;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
        }

        return (a * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) + end + start);
    }

    public static float easeInOutElastic(float start, float end, float value)
    {
        end -= start;

        float d = 1f;
        float p = d * .3f;
        float s = 0;
        float a = 0;

        if (value == 0) return start;

        if ((value /= d / 2) == 2) return start + end;

        if (a == 0f || a < Mathf.Abs(end))
        {
            a = end;
            s = p / 4;
        }
        else
        {
            s = p / (2 * Mathf.PI) * Mathf.Asin(end / a);
        }

        if (value < 1) return -0.5f * (a * Mathf.Pow(2, 10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p)) + start;
        return a * Mathf.Pow(2, -10 * (value -= 1)) * Mathf.Sin((value * d - s) * (2 * Mathf.PI) / p) * 0.5f + end + start;
    }

    public static float punch(float start, float end, float value)
    {
        float amplitude = (value - start) / (end - start);
        float s = 9;
        if (value == 0)
        {
            return 0;
        }
        if (value == 1)
        {
            return 0;
        }
        float period = 1 * 0.3f;
        s = period / (2 * Mathf.PI) * Mathf.Asin(0);
        return (amplitude * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * 1 - s) * (2 * Mathf.PI) / period));
    }

	#endregion
}

/// <summary>
/// This is used to build animation chain easily.
/// </summary>
public class UIAnimationBuilder : List<UIAnimation>
{
	/// <summary>
	/// Peek the first anime.
	/// </summary>
	public UIAnimation First { get {
		if (Count == 0)
			return null;
		return this[0];
	} }

	/// <summary>
	/// Peek the last anime.
	/// </summary>
	public UIAnimation Last
	{
		get
		{
			if (Count == 0)
				return null;
			return this[Count - 1];
		}
	}

	/// <summary>
	/// This will make a chain of all animations in this builder. The first anime is index 0, next is index 1 and so on.
	/// It will return itself.
	/// </summary>
	public UIAnimationBuilder MakeChain()
	{
		for (int i = 0; i < Count - 1; i++)
			this[i].AddNext(this[i+1]);
		return this;
	}

	/// <summary>
	/// Add anime to builder
	/// </summary>
	/// <param name="a1"></param>
	/// <param name="a2"></param>
	/// <returns></returns>
	public static UIAnimationBuilder operator +(UIAnimationBuilder a1, UIAnimation a2)
	{
		a1.Add(a2);
		return a1;
	}

	/// <summary>
	/// Add a collection of animes to builder
	/// </summary>
	/// <param name="a1"></param>
	/// <param name="a2"></param>
	/// <returns></returns>
	public static UIAnimationBuilder operator +(UIAnimationBuilder a1, IEnumerable<UIAnimation> a2)
	{
		a1.AddRange(a2);
		return a1;
	}

}