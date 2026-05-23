using System.Collections.Generic;

namespace lab1
{
    /// <summary>
    /// Manages commands for undo/redo functionality
    /// </summary>
    public class CommandManager
    {
        private Stack<ICommand> undoStack = new Stack<ICommand>();
        private Stack<ICommand> redoStack = new Stack<ICommand>();

        public void ExecuteCommand(ICommand command)
        {
            command.Execute();
            undoStack.Push(command);
            redoStack.Clear();
        }

        public bool CanUndo() => undoStack.Count > 0;

        public void Undo()
        {
            if (CanUndo())
            {
                ICommand command = undoStack.Pop();
                command.Undo();
                redoStack.Push(command);
            }
        }

        public bool CanRedo() => redoStack.Count > 0;

        public void Redo()
        {
            if (CanRedo())
            {
                ICommand command = redoStack.Pop();
                command.Execute();
                undoStack.Push(command);
            }
        }

        public string GetLastCommandDescription()
        {
            return undoStack.Count > 0 ? undoStack.Peek().Description : "No actions";
        }
    }
}