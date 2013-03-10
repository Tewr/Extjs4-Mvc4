namespace Tewr.ExtJsMvc.EditableGrid
{
    public abstract class ColumnSpecification<TRowModelType>
    {
        protected ColumnSpecification(ColumnConfig config)
        {
            ColumnConfig = config;
        }

        public string RawValueFromModel(TRowModelType model)
        {
            return GetValueFromModel(model).ToString();
        }

        public virtual ColumnConfig ColumnConfig { get; private set; }

        public string Id 
        {
            get 
            {
                return ColumnConfig.dataIndex;
            }    
        }

        public abstract object GetValueFromModel(TRowModelType model);
    }
}