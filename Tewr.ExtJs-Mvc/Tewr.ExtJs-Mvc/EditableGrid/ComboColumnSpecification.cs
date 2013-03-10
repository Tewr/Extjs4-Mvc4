using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Tewr.ExtJsMvc.EditableGrid
{
    public class ComboColumnSpecification<TRowModelType> : ColumnSpecification<TRowModelType>
    {
        private readonly Func<TRowModelType, IEnumerable<SelectListItem>> _availableOptionsFactory;
        private readonly Func<TRowModelType, string> _selectedValueIdFactory;
        private readonly ColumnConfig _columnConfig;
        private Dictionary<string, string> _valueDictionary;

        public ComboColumnSpecification(
            Func<TRowModelType, IEnumerable<SelectListItem>> availableOptionsFactory,
            ColumnConfig columnConfig)
            : this(availableOptionsFactory, 
                GetSelectedFactory(availableOptionsFactory),
                columnConfig)
        {
        }

        public ComboColumnSpecification(
            Func<TRowModelType, IEnumerable<SelectListItem>> availableOptionsFactory,
            Func<TRowModelType, string> selectedValueIdFactory, 
            ColumnConfig columnConfig)
            : base(columnConfig)
        {
            _availableOptionsFactory = availableOptionsFactory;
            _selectedValueIdFactory = selectedValueIdFactory;
            _columnConfig = columnConfig;
            _columnConfig.addComboRendering = true;
            
        }

        private static Func<TRowModelType, string> GetSelectedFactory(Func<TRowModelType, IEnumerable<SelectListItem>> availableOptions)
        {
            return x =>
            {
                var item = availableOptions(x).FirstOrDefault(si => si.Selected);
                if (item != null)
                {
                    return item.Value;
                }

                return string.Empty;
            };
        }

        public override object GetValueFromModel(TRowModelType model)
        {
            if (_valueDictionary == null)
            {
                _valueDictionary = _availableOptionsFactory(model).ToDictionary(x => x.Value, x => x.Text);
                _columnConfig.editor.store = _valueDictionary.Select(kvp => new [] {kvp.Key, kvp.Value});
            }
            
            var selectedValueId = _selectedValueIdFactory(model);
            if (selectedValueId == string.Empty)
            {
                return string.Empty;
            }

            return selectedValueId;
        }
    }
}