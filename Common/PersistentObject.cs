namespace Temnenkov.SJB.Common
{
    public abstract class PersistentObject
    {
        public abstract void Save(IDatabase db);
    }
}
