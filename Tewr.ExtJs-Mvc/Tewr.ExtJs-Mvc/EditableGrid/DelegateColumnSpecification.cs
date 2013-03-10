namespace Tewr.ExtJsMvc.EditableGrid
{
    public delegate object ValueFactory<T>(T rowModel);

    public class DelegateColumnSpecification<TRowModelType> : ColumnSpecification<TRowModelType>
    {
        private readonly ValueFactory<TRowModelType> _valueFactory;

        public DelegateColumnSpecification(ValueFactory<TRowModelType> valueFactory, ColumnConfig config) : base(config)
        {
            _valueFactory = valueFactory;
        }

        public override object GetValueFromModel(TRowModelType model)
        {
            return _valueFactory(model).ToString();
        }
    }
}