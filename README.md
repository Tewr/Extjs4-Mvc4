Extjs4-Mvc4
===========

Html helper lib for integrating [Extjs4] with [Asp.net MVC4]. Pretty much the same idea as the commercial lib [Ext.net], though this code totally unrelated.

[Extjs4]: http://docs.sencha.com/ext-js/4-1/
[Asp.net MVC4]: http://www.asp.net/mvc/mvc4
[Ext.net]: http://www.ext.net/

Editable Grid
-------------

Creates a [Ext.grid.Panel] with [CellEditing plugin] from a model instance property, based on [this example].
The idea is to keep the implementation DRY, so rather than re-modelleing your .net models in javascript to
create the grid, a grid configuration is created by inspecting the model at runtime. The generated configuration 
includes column configurations with type-based editors. The grid configuration is not loaded with ajax but 
directly from script in markup, saving us a roundtrip.

Like things usually are in ASP.net MVC, column names can be specified with the [DisplayAttribute], or if unspecified they
will be deducted from the model property name. 

Theres not yet a practical way of submitting the form data back to the server once modified 
(if you want to use this, you need to build that yourself).

[Ext.grid.Panel]: http://docs.sencha.com/ext-js/4-1/#!/api/Ext.grid.Panel
[CellEditing plugin]: http://docs.sencha.com/ext-js/4-1/#!/api/Ext.grid.plugin.CellEditing
[DisplayAttribute]: http://msdn.microsoft.com/en-us/library/system.componentmodel.dataannotations.displayattribute.aspx
[this example]: http://dev.sencha.com/deploy/ext-4.1.0-gpl/examples/grid/cell-editing.html

### Sample usage

#### Model
```cs
    public class HomeModel
    {
        public IEnumerable<PersonModel> Persons { get; set; }
    }

    public class PersonModel
    {
        public int Id { get; set; }

        [Display(Name="The Person name")]
        public string Name { get; set; }

        public string OccupationId { get; set; }

        public IEnumerable<SelectListItem> Occupations { get; set; }

        [Display(Name = "Date of birth")]
        public DateTime BirthDate { get; set; }

        [Display(Name = "Ammassed fortune")]
        [DisplayFormat(DataFormatString = "{0} $")]
        public decimal Fortune { get; set; }

        public int Nationalities { get; set; }
    }
```

#### View
```html
@using Tewr.ExtJsMvc
@using Tewr.ExtJsMvc.EditableGrid
@model Tewr.ExtJsMvc.Examples.Models.HomeModel
<div id="personsGrid"></div>
<script type="text/javascript" src="~/Scripts/Tewr.ExtJs-Mvc.grid.EditableGrid.js"></script>
@Html.EditableGrid(
    "personsGrid",
    x => x.Persons, 
    new GridOptions { width = "700", height = "400"},
    g => g
        .Text(m => m.Name, new ColumnOptions { Width = 120 })
        .Combo(m => m.Occupations, m => m.OccupationId, new ComboColumnOptions { Header = "Occupation", Width = 100 })
        .Combo(m => m.Occupations, new ComboColumnOptions { Header = "Second Occupation", Width = 120 })
        .Date(m => m.BirthDate, new ColumnOptions { Width = 100})
        .Number(m => m.Fortune, new ColumnOptions { Width = 100 })
        .Number(m => m.Nationalities)
    )
```


The same(ish) example can be found in full in [ExtJsMvc4.Examples] project.
[ExtJsMvc4.Examples]: https://github.com/Tewr/Extjs4-Mvc4/tree/master/Tewr.ExtJs-Mvc/Tewr.ExtJs-Mvc.Examples
