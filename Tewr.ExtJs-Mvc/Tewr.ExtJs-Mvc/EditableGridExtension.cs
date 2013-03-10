using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using Tewr.ExtJsMvc.EditableGrid;

namespace Tewr.ExtJsMvc
{
    public static class EditableGridExtension
    {
        /// <summary>
        /// Renders a extjs grid in-place using the specified model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="propertyExpression"></param>
        /// <param name="options"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static MvcHtmlString EditableGrid<T, T2>(
            this HtmlHelper<T> htmlHelper,
            Expression<Func<T, IEnumerable<T2>>> propertyExpression,
            GridOptions options = null,
            Func<EditableGrid<T, T2>, EditableGrid<T, T2>> columns = null) where T2 : class
        {
            return EditableGrid(htmlHelper, string.Empty, propertyExpression, options, columns);
        }

        /// <summary>
        /// Renders a extjs grid in the element with the specified <see cref="gridElementId"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="gridElementId"></param>
        /// <param name="propertyExpression"></param>
        /// <param name="options"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static MvcHtmlString EditableGrid<T, T2>(
            this HtmlHelper<T> htmlHelper,
            string gridElementId,
            Expression<Func<T, IEnumerable<T2>>> propertyExpression,
            GridOptions options = null,
            Func<EditableGrid<T, T2>, EditableGrid<T, T2>> columns = null) where T2 : class
        {
            var subModel = propertyExpression.Compile()(htmlHelper.ViewData.Model);

            if (subModel == null)
            {
                var prop = propertyExpression.Body.ToString();
                throw new ArgumentException(string.Format("The expression \"{0}\" returned null", prop));
            }

            var editableGrid = new EditableGrid<T, T2>(gridElementId, subModel, options);

            if (columns != null)
            {
                columns(editableGrid);
            }

            return MvcHtmlString.Create(editableGrid.Render());
        }


    }
}