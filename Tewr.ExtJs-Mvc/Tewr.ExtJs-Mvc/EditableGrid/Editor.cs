using System.Collections.Generic;

namespace Tewr.ExtJsMvc.EditableGrid
{
    public class Editor
    {
        public IEnumerable<IEnumerable<string>> store { get; set; }

        public bool? allowBlank { get; set; }

        public string xtype { get; set; }

    }
}