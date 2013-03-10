using System;

namespace Tewr.ExtJsMvc.EditableGrid
{
    public class ModelEventArgs<T> : EventArgs
    {
        public ModelEventArgs(T model)
        {
            Model = model;
        }

        public T Model { get; private set; }
    }
}