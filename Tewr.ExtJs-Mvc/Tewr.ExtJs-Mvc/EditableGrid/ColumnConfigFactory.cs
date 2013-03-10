using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Tewr.ExtJsMvc.EditableGrid
{
    public class ColumnConfigFactory<TRowModel> where TRowModel : class
    {
        private IDictionary<string, ModelMetadata> _rowModelMetaDataByPropertyName;

        public ColumnSpecification<TRowModel> Number<T2>(Expression<Func<TRowModel, T2>> property, ColumnOptions columnOptions = null)
        {
            var modelMetaData = GetModelMetaData(property);
            var columnConfig = InitCommonOptions(modelMetaData, columnOptions);
            columnConfig.xtype = columnConfig.xtype ?? ExtJsColumnXTypes.NumberBoxClass;
            columnConfig.format = GetFormat(modelMetaData, null);

            if (columnOptions == null || !columnOptions.IsReadonly)
            {
                columnConfig.editor = new Editor
                                          {
                                              allowBlank = false,
                                              xtype = ExtJsEditorXTypes.NumberBoxClass
                                          };
            }

            var valueFactory = property.Compile();

            return new DelegateColumnSpecification<TRowModel>(x => valueFactory(x), columnConfig);
        }

        public ColumnSpecification<TRowModel> Text(Expression<Func<TRowModel, string>> property, ColumnOptions columnOptions = null)
        {
            var modelMetaData = GetModelMetaData(property);
            var columnConfig = InitCommonOptions(modelMetaData, columnOptions);
            columnConfig.fieldType = ExtJsFieldTypes.Text;
            columnConfig.xtype = columnConfig.xtype ?? ExtJsColumnXTypes.TextBoxClass;
            if (columnOptions == null || !columnOptions.IsReadonly)
            {
                columnConfig.editor = new Editor
                                          {
                                              allowBlank = false,
                                              xtype = ExtJsEditorXTypes.TextBoxClass
                                          };
            }
            var valueFactory = property.Compile();

            return new DelegateColumnSpecification<TRowModel>(x => valueFactory(x), columnConfig);
        }

        public ColumnSpecification<TRowModel> Date(Expression<Func<TRowModel, DateTime>> property, ColumnOptions columnOptions = null)
        {
            var modelMetaData = GetModelMetaData(property);
            var columnConfig = InitCommonOptions(modelMetaData, columnOptions);
            columnConfig.xtype = columnConfig.xtype ?? ExtJsColumnXTypes.DateBoxClass;
            columnConfig.fieldType = ExtJsFieldTypes.Date;
            columnConfig.format = Utils.PhpStyleDateFormat;
            

            var valueFactory = property.Compile();

            if (columnOptions == null || !columnOptions.IsReadonly)
            {
                columnConfig.editor = new Editor
                {
                    allowBlank = false,
                    xtype = ExtJsEditorXTypes.DatePickerClass,
                    
                };
            }

            return new DelegateColumnSpecification<TRowModel>(x => valueFactory(x), columnConfig);
        }

        private string GetFormat(ModelMetadata modelMetaData, string defaultFormat)
        {
            if (!string.IsNullOrEmpty(modelMetaData.DisplayFormatString))
            {
                return modelMetaData.DisplayFormatString;
            }

            return defaultFormat;
        }

        public ColumnSpecification<TRowModel> CheckBox(Expression<Func<TRowModel, bool>> property, ColumnOptions columnOptions = null)
        {
            var modelMetaData = GetModelMetaData(property);
            var columnConfig = InitCommonOptions(modelMetaData, columnOptions);
            columnConfig.xtype = columnConfig.xtype ?? ExtJsColumnXTypes.CheckBoxClass;
            var valueFactory = property.Compile();

            return new DelegateColumnSpecification<TRowModel>(x => valueFactory(x), columnConfig);
        }

        public ColumnSpecification<TRowModel> Combo(
            Expression<Func<TRowModel, IEnumerable<SelectListItem>>> availableOptions,
            ComboColumnOptions columnOptions = null)
        {
            var modelMetaData = GetModelMetaData(availableOptions);
            var availableOptionsFactory = availableOptions.Compile();
            Func<TRowModel, string> valueFactory = m => GetSelectedFromCollection(m, columnOptions, availableOptionsFactory);

            return Combo(availableOptionsFactory, columnOptions, valueFactory, modelMetaData);
        }

        private static string GetSelectedFromCollection(TRowModel m, ComboColumnOptions optionsFactory, Func<TRowModel, IEnumerable<SelectListItem>> availableOptionsFactory)
        {
            SelectListItem selectListItem = availableOptionsFactory(m).First(si => si.Selected);
            if (selectListItem.Value == null)
            {
                return GetNoValueSelectedValue(optionsFactory);
            }

            return selectListItem.Value;
        }

        public ColumnSpecification<TRowModel> Combo(
            Func<TRowModel, IEnumerable<SelectListItem>> availableOptions, 
            Expression<Func<TRowModel, string>> selectedField,
            ComboColumnOptions columnOptions = null)
        {
            var modelMetaData = GetModelMetaData(selectedField);
            var valueFactory = selectedField.Compile();

            return Combo(availableOptions, columnOptions, valueFactory, modelMetaData);
        }

        private static ColumnSpecification<TRowModel> Combo(
            Func<TRowModel, IEnumerable<SelectListItem>> availableOptions, 
            ComboColumnOptions columnOptions,
            Func<TRowModel, string> valueFactory, 
            ModelMetadata modelMetaData)
        {
            var columnConfig = InitCommonOptions(modelMetaData, columnOptions);

            columnConfig.xtype = columnConfig.xtype ?? ExtJsColumnXTypes.ComboBoxClass;
            columnConfig.fieldType = ExtJsFieldTypes.Text;

            if (columnOptions == null || !columnOptions.IsReadonly)
            {
                columnConfig.editor = new Editor
                                          {
                                              xtype = ExtJsEditorXTypes.ComboBoxClass,
                                              allowBlank = false,
                                          };
            }

            columnConfig.noValueSelectedValue = GetNoValueSelectedValue(columnOptions);

            var columnSpecification = new ComboColumnSpecification<TRowModel>(availableOptions, valueFactory, columnConfig);

            return columnSpecification;
        }

        private static string GetNoValueSelectedValue(ComboColumnOptions columnOptions)
        {
            return columnOptions == null
                       ? string.Empty
                       : columnOptions.NoValueSelectedValue;
        }

        private IDictionary<string, ModelMetadata> PropertyDictionary {
            get 
            {
                if (_rowModelMetaDataByPropertyName == null)
                {
                    var rowModelMetaData = ModelMetadataProviders.Current.GetMetadataForType(
                        () => (TRowModel)null, 
                        typeof(TRowModel));

                    _rowModelMetaDataByPropertyName = rowModelMetaData.Properties.ToDictionary(x => x.PropertyName);
                }

                return _rowModelMetaDataByPropertyName;
            }
        }

        private static ColumnConfig InitCommonOptions(ModelMetadata propertyMeta, ColumnOptions columnOptions)
        {
            var config = new ColumnConfig(columnOptions);

            InitCommonOptions(config, propertyMeta);

            return config;
        }

        private static void InitCommonOptions(ColumnConfig config, ModelMetadata propertyMeta)
        {
            config.header = config.header ?? propertyMeta.DisplayName ?? propertyMeta.PropertyName;
            config.dataIndex = config.dataIndex ?? propertyMeta.PropertyName;
            config.customStringFormat = propertyMeta.DisplayFormatString;
        }

        private ModelMetadata GetModelMetaData<TReturn>(Expression<Func<TRowModel, TReturn>> columnExpression) 
        {
            var propertyPart = (columnExpression.Body as MemberExpression);
            if (propertyPart == null || propertyPart.Member.MemberType != System.Reflection.MemberTypes.Property) {
                throw new ArgumentException(string.Format("columnExpression {0} does not expose a property.", columnExpression.Body), "columnExpression");
            }

            var propertyName = propertyPart.Member.Name;
            ModelMetadata propertyMeta;
            if (!PropertyDictionary.TryGetValue(propertyName, out propertyMeta)) {
                throw new ArgumentException(string.Format("The property {0} is not a public member of {1}.", propertyName, typeof(TRowModel)), "columnExpression");
            }

            return propertyMeta;
        }
    }
}