namespace Core.MVC
{
    public abstract class Controller:Nofitier
    {
        public virtual void Init()
        {
            RegisterEventHandle();
        }

        public abstract void RegisterEventHandle();
    }
}
