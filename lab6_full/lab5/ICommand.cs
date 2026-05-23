namespace lab1
{
    /// <summary>
    /// Command pattern interface for undo/redo operations
    /// </summary>
    public interface ICommand
    {
        void Execute();
        void Undo();
        string Description { get; }
    }
}