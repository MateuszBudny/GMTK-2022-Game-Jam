using NodeCanvas.Framework;


namespace NodeCanvas.Tasks.Actions
{
    public class SetEndingAsDoneNode : ActionTask
    {
        public EndingSO endingToSetAsDone;

        protected override string info => $"Set Ending As Done\n{(endingToSetAsDone != null ? endingToSetAsDone.name : "<none>")}";

        //This is called once each time the task is enabled.
        //Call EndAction() to mark the action as finished, either in success or failure.
        //EndAction can be called from anywhere.
        protected override void OnExecute()
        {
            endingToSetAsDone.SetEndingAsDone();
            EndAction(true);
        }
    }
}