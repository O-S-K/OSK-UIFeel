using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace OSK
{
    [System.Serializable]
    public class TweenSettings
    {
        public Transform root;
        public bool playOnEnable = true;
        public bool setAutoKill = true;

        [Min(0)] public float delay = 0f;
        [Min(0)] public float duration = 2f;
        [Min(-1)] public int loopcount = 0;

        [ShowIf(nameof(loopcount), -1)] 
        public LoopType loopType = LoopType.Restart;

        public TypeAnimation typeAnim = TypeAnimation.Ease;

        [ShowIf(nameof(typeAnim), TypeAnimation.Ease)]
        public Ease ease = Ease.Linear;

        [ShowIf(nameof(typeAnim), TypeAnimation.Curve)]
        public AnimationCurve curve;

        public UpdateType updateType = UpdateType.Normal;
        public bool useUnscaledTime = false;
        public UnityEvent eventCompleted;
    }

    public abstract class DoTweenBaseProvider : MonoBehaviour, IDoTweenProviderBehaviours
    {
        //[HideInInspector]
        public TweenSettings settings;
        
        [HideInInspector] public Object target;
        public Transform RootTransform => settings.root ? settings.root : transform;
        public RectTransform RootRectTransform => settings.root as RectTransform;

        public Tweener tweener;
        public Tweener Tweener => tweener;

        public bool IsPlaying => null != tweener && tweener.IsPlaying();

        public virtual void OnEnable()
        {
            if (settings.playOnEnable) Play();
        }

        public virtual void OnDisable() => Stop();
        public virtual void OnDestroy() => Stop();
        
        public abstract object GetStartValue();
        public abstract object GetEndValue();

        public virtual void InitFromMG(bool playOnEnable, bool setAutoKill, UpdateType updateType, bool useUnscaledTime)
        {
            settings.playOnEnable = playOnEnable;
            settings.setAutoKill = setAutoKill;
            settings.updateType = updateType;
            settings.useUnscaledTime = useUnscaledTime;
        }

        public virtual void ProgressTween()
        {
            tweener.SetDelay(settings.delay)
                .SetAutoKill(settings.setAutoKill)
                .SetLoops(settings.loopcount, settings.loopType)
                .SetUpdate(settings.updateType, settings.useUnscaledTime)
                .SetTarget(target)
                .OnComplete(() => settings.eventCompleted?.Invoke());


            if (settings.typeAnim == TypeAnimation.Ease)
                tweener.SetEase(settings.ease);
            else
                tweener.SetEase(settings.curve);
        }

        public virtual void Play()
        {
            tweener?.Kill();
            tweener = null;
            if (!target)
                if (tweener != null)
                    target = (UnityEngine.Object)tweener.target;
            ProgressTween();
        } 
        
        public float GetDuration() => settings.duration;

        public void Preview(float time)
        {
            if (tweener == null) return;
            tweener.Goto(time);
        }

        public void PlayBackwards()
        {
            tweener?.PlayBackwards();
        }

        public virtual void Rewind() => tweener?.Rewind();
        public virtual void Backward() => tweener?.PlayBackwards();

        public virtual void Stop()
        {
            tweener?.Kill();
            tweener = null;
            target = null;
        }

        public virtual void Resume() => tweener?.Play();
        public virtual void Pause() => tweener?.Pause();
        public virtual void Kill() => tweener?.Kill();
        public virtual void Restart() => tweener?.Restart();
    }
}