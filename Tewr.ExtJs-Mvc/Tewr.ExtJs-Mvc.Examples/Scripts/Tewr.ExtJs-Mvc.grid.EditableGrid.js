Ext.onReady(function () {

    Ext.Loader.setConfig({
        enabled: true
    });

    Ext.Loader.setPath('Ext.ux', 'http://extjs.cachefly.net/ext-4.1.1-gpl/examples/ux/');

    Ext.require([
        'Ext.selection.CellModel',
        'Ext.grid.*',
        'Ext.data.*',
        'Ext.util.*',
        'Ext.state.*',
        'Ext.form.*',
        'Ext.ux.CheckColumn'
    ]);



    Ext.define('Tewr.ExtJsMvc.grid.EditableGrid', {
        statics: {
            create: function (configuration) {
                /* configuration definition:  
                { targetElement : 'domId', 
                modelDefinition: {model def object}, 
                columnConfig: {array of column config objects}, 
                data : {inital data for the grid, array of model objects}
                */
                var me = Tewr.ExtJsMvc.grid.EditableGrid;

                if (typeof configuration == 'string') {
                    configuration = JSON.decode(configuration);
                }

                me.addRendering(configuration);

                var gridModelName = configuration.targetElement + "-GridModel";

                Ext.define(gridModelName, {
                    extend: 'Ext.data.Model',
                    fields: configuration.modelDefinition
                });

                var store = Ext.create('Ext.data.Store', {
                    // destroy the store if the grid is destroyed
                    autoDestroy: true,
                    model: gridModelName,
                    proxy: {
                        type: 'memory'
                    },
                    data: configuration.data,
                    sorters: [{
                        property: 'Name',
                        direction: 'ASC'
                    }]
                });

                var cellEditing = Ext.create('Ext.grid.plugin.CellEditing', {
                    clicksToEdit: 1
                });

                Ext.create('Ext.grid.Panel', {
                    store: store,
                    columns: configuration.columnConfig,
                    renderTo: configuration.targetElement,
                    frame: true,
                    width: configuration.gridOptions.width,
                    height: configuration.gridOptions.height,
                    plugins: [cellEditing]
                });
            },

            addRendering: function (configuration) {
                for (var i = 0; i < configuration.columnConfig.length; i++) {
                    var colConfig = configuration.columnConfig[i];

                    if (colConfig.addComboRendering) {
                        var store = colConfig.editor.store;
                        colConfig.renderer = (function (fmt) {
                            return function(value) {
                                var storeMember = store[value];

                                if (storeMember) {
                                    var val = storeMember[1];
                                    if (fmt) {
                                        val = Ext.String.format(fmt, val);
                                    }

                                    return val;
                                }

                                return colConfig.noValueSelectedValue;
                            };
                        })(colConfig.customStringFormat);
                    } else if (colConfig.customStringFormat) {

                        colConfig.renderer = (function (fmt) {
                            return function (value) {
                                return Ext.String.format(fmt, value);
                            };
                        })(colConfig.customStringFormat);
                    }
                }
            }
        }
    });
});