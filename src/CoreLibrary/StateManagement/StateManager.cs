namespace CoreLibrary.StateManagement
{
    public class StateManager<T>
    {
        private T _savedState;

        public IRestorable<T> ManagedEntity { get; }

        public StateManager(IRestorable<T> managedEntity)
        {
            ManagedEntity = managedEntity;
        }

        public void Restore()
        {
            ManagedEntity.Restore(_savedState);
        }

        public void Save()
        {
            _savedState = ManagedEntity.Save();
        }
    }
}