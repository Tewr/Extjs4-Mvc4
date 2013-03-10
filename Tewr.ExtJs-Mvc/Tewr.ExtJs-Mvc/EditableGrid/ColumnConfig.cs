namespace Tewr.ExtJsMvc.EditableGrid
{
    public class ColumnConfig
    {
        

        public ColumnConfig(ColumnOptions columnOptions = null)
        {
            if (columnOptions == null)
            {
                return;
            }

            width = columnOptions.Width.HasValue ? columnOptions.Width.ToString() : "auto";
            header = columnOptions.Header;
            dataIndex = columnOptions.Id;
        }

        #region properties used by ExtJs Framework 

        public string width { get; set; }

        public string dataIndex { get; set; }

        public string header { get; set; }

        public string xtype { get; set; }

        public string fieldType { get; set; }

        public Editor editor { get; set; }

        public string format { get; set; }

        #endregion

        #region custom properties

        public bool addComboRendering { get; set; }

        public string noValueSelectedValue { get; set; }

        public string customStringFormat { get; set; }

        #endregion
    }
}