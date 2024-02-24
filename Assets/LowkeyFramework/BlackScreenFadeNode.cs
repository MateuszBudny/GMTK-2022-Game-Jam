using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System;


namespace NodeCanvas.Tasks.Actions
{
    [Name("Black Screen Fade")]
    [Description("Fades in or out the black screen.")]
    [Category("Camera")]
    public class BlackScreenFadeNode : ActionTask
    {
        public BBParameter<FadeType> fadeType;
        public BBParameter<float> fadeDuration = 2f;
        public BBParameter<bool> forceStartFromBeginning;
        public BBParameter<bool> waitForFadeEnd;

        protected override string info => $"{fadeType}" +
            $"\nDuration: {fadeDuration}" +
            $"{(forceStartFromBeginning.value ? "\nForcing Start From The Beginning" : "")}" +
            $"{(waitForFadeEnd.value ? "\nWaiting For The Fade To End" : "")}";

        //This is called once each time the task is enabled.
        //Call EndAction() to mark the action as finished, either in success or failure.
        //EndAction can be called from anywhere.
        protected override void OnExecute()
        {
            Action onFadeFinished = null;
            if(waitForFadeEnd.value)
            {
                onFadeFinished = () => EndAction(true);
            }

            if(fadeType.value == FadeType.FadeIn)
            {
                BlackScreen.Instance.FadeIn(fadeDuration.value, forceStartFromBeginning.value, onFadeFinished);
            }
            else
            {
                BlackScreen.Instance.FadeOut(fadeDuration.value, forceStartFromBeginning.value, onFadeFinished);
            }

            if(!waitForFadeEnd.value)
            {
                EndAction(true);
            }
        }

        public enum FadeType
        {
            FadeIn,
            FadeOut,
        }
    }
}