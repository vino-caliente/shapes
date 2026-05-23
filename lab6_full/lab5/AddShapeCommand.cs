namespace lab1
{
    /// <summary>
    /// Command for adding a shape (supports undo)
    /// </summary>
    public class AddShapeCommand : ICommand
    {
        private ShapeList shapeList;
        private Shape shape;

        public string Description => $"Added {shape.GetType().Name}";

        public AddShapeCommand(ShapeList list, Shape newShape)
        {
            shapeList = list;
            shape = newShape;
        }

        public void Execute()
        {
            shapeList.Add(shape);
        }

        public void Undo()
        {
            shapeList.Remove(shape);
        }
    }
}