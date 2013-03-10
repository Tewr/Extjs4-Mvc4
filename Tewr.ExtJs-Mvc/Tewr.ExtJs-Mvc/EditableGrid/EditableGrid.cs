using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Tewr.ExtJsMvc.EditableGrid
{
    public class EditableGrid<TContainer, TModel> where TModel : class
    {
        private readonly GridOptions options;

        private readonly List<ColumnSpecification<TModel>> _columnSpecifications;

        private readonly ColumnConfigFactory<TModel> _columnSpecificationsFactory;

        private readonly IEnumerable<TModel> _model;

        private readonly string _targetElement;

        public EditableGrid(string gridElementId,
            IEnumerable<TModel> model,
            GridOptions options)
        {
            _model = model;
            this.options = options;
            _targetElement = gridElementId;
            _columnSpecificationsFactory = new ColumnConfigFactory<TModel>();
            _columnSpecifications = new List<ColumnSpecification<TModel>>();
        }

        private IEnumerable<Dictionary<string, string>> GetData() {
            var data = new List<Dictionary<string, string>>();
            foreach (var rowModel in this._model)
            {
                var dataRow = new Dictionary<string, string>();

                foreach (var colConfig in _columnSpecifications)
                {
                    var key = colConfig.Id;
                    if (dataRow.ContainsKey(key))
                    {
                        throw new InvalidOperationException(
                            string.Format("Key {0} appears several times. Use ColumnOptions.Id to specify a unique Id for the column if the same property is used several times.", key));
                    }

                    dataRow[key] = colConfig.RawValueFromModel(rowModel);
                }

                data.Add(dataRow);
            }

            return data;
        }

        private IEnumerable<Dictionary<string, string>> GetModelDefinition()
        {
            var modelDefinition = new List<Dictionary<string, string>>();
            foreach (var columnSpecification in _columnSpecifications)
            { 
                var fieldDefinition = new Dictionary<string, string>();

                fieldDefinition["name"] = columnSpecification.Id;
                string fieldType = columnSpecification.ColumnConfig.fieldType;
                fieldDefinition["type"] = fieldType;
                if (fieldType == ExtJsFieldTypes.Date)
                {
                    fieldDefinition["dateFormat"] = Utils.PhpStyleDateFormat;
                }

                modelDefinition.Add(fieldDefinition);
            }
            return modelDefinition;
        }

        

        public string Render()
        {
            var columnConfig = _columnSpecifications.Select(x => x.ColumnConfig).ToArray();

            /* configuration definition:  
                    { targetElement : 'domId', 
                      modelDefinition: {model def object}, 
                      columnConfig: {array of column config objects}, 
                      gridOptions: {global grid config options}
                      data : {inital data for the grid, array of model objects}
            */

            var modelDefinition = GetModelDefinition();
            var data = GetData();
            var gridOptions = options;
            var targetElement = _targetElement;
            var autoTarget = string.Empty;

            if (string.IsNullOrEmpty(targetElement))
            {
                targetElement = Guid.NewGuid().ToString("N");
                var targetTagBuilder = new TagBuilder("div");
                targetTagBuilder.Attributes.Add("id", targetElement);
                autoTarget = targetTagBuilder.ToString();
            }

            var jsArg = new 
            {
                targetElement,
                modelDefinition,
                columnConfig,
                gridOptions,
                data
            };

            var json = JsonConvert.SerializeObject(jsArg, Formatting.Indented);

            var scriptTagbuilder = new TagBuilder("script");
            scriptTagbuilder.Attributes.Add("type", "text/javascript");
            var funcCall = string.Format("Tewr.ExtJsMvc.grid.EditableGrid.create({0});", json);
            scriptTagbuilder.InnerHtml = string.Format("Ext.onReady(function() {{{0}}});", funcCall);
            var scriptTag = scriptTagbuilder.ToString();

            return autoTarget + scriptTag;
        }

        public override string ToString()
        {
            return Render();
        }

        public MvcHtmlString MvcString()
        {
            return MvcHtmlString.Create(Render());
        }

        public EditableGrid<TContainer, TModel> Number(
            Expression<Func<TModel, double>> property, ColumnOptions columnOptions = null)
        {
            return Number<double>(property, columnOptions);
        }

        public EditableGrid<TContainer, TModel> Number(
            Expression<Func<TModel, float>> property, ColumnOptions columnOptions = null)
        {
            return Number<float>(property, columnOptions);
        }

        public EditableGrid<TContainer, TModel> Number(
            Expression<Func<TModel, decimal>> property, ColumnOptions columnOptions = null)
        {
            return Number<decimal>(property, columnOptions);
        }

        public EditableGrid<TContainer, TModel> Number(
            Expression<Func<TModel, int>> property, ColumnOptions columnOptions = null)
        {
            return Number<int>(property, columnOptions);
        }

        private EditableGrid<TContainer, TModel> Number<T2>(
            Expression<Func<TModel, T2>> property, ColumnOptions columnOptions = null)
        {
            _columnSpecifications.Add(_columnSpecificationsFactory.Number(property, columnOptions));
            return this;
        }

        public EditableGrid<TContainer, TModel> Text(Expression<Func<TModel, string>> property, ColumnOptions columnOptions = null)
        {
            _columnSpecifications.Add(_columnSpecificationsFactory.Text(property, columnOptions));
            return this;
        }

        public EditableGrid<TContainer, TModel> Date(Expression<Func<TModel, DateTime>> property, ColumnOptions columnOptions = null)
        {
            _columnSpecifications.Add(_columnSpecificationsFactory.Date(property, columnOptions));
            return this;
        }

        public EditableGrid<TContainer, TModel> CheckBox(Expression<Func<TModel, bool>> property, ColumnOptions columnOptions = null)
        {
            this._columnSpecifications.Add(this._columnSpecificationsFactory.CheckBox(property, columnOptions));
            return this;
        }

        public EditableGrid<TContainer, TModel> Combo(
            Func<TModel, IEnumerable<SelectListItem>> availableOptions,
            Expression<Func<TModel, string>> selectedField,
            ComboColumnOptions columnOptions = null)
        {
            this._columnSpecifications.Add(this._columnSpecificationsFactory.Combo(availableOptions, selectedField, columnOptions));
            return this;
        }

        public EditableGrid<TContainer, TModel> Combo(
            Expression<Func<TModel, IEnumerable<SelectListItem>>> availableOptions,
            ComboColumnOptions columnOptions = null)
        {
            this._columnSpecifications.Add(this._columnSpecificationsFactory.Combo(availableOptions, columnOptions));
            return this;
        }
    }
}