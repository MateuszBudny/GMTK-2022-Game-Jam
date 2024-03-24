public class DontDestroySingleBehaviour<T> : SingleBehaviour<T> where T : DontDestroySingleBehaviour<T>
{
    protected override void OnFirstInstance()
    {
        base.OnFirstInstance();
        DontDestroyOnLoad(this);
    }

    protected override void OnAnotherInstance()
    {
        Destroy(gameObject);
    }
}
