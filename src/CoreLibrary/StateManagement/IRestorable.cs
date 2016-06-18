namespace CoreLibrary.StateManagement
{
    /// <summary>
    /// Allows for state management 
    /// </summary>
    public interface IRestorable<T>
    {
        /// <summary>
        /// Restore previously saved state 
        /// </summary>
        void Restore(T savedState);

        /// <summary>
        /// Save state 
        /// </summary>
        T Save();
    }
}