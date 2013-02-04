using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Gather.Core;
using Gather.Core.Infrastructure;

namespace Gather.Web.Framework.UI
{
    public static class HtmlExtensions
    {

        public static MvcHtmlString BooleanDropDownList(this HtmlHelper htmlHelper, string name)
        {
            return BooleanDropDownList(htmlHelper, name, null, null, null, null);
        }

        public static MvcHtmlString BooleanDropDownList(this HtmlHelper htmlHelper, string name, string emptyText)
        {
            return BooleanDropDownList(htmlHelper, name, emptyText, null, null, null);
        }

        public static MvcHtmlString BooleanDropDownList(this HtmlHelper htmlHelper, string name, string emptyText, object htmlAttributes)
        {
            return BooleanDropDownList(htmlHelper, name, emptyText, null, null, htmlAttributes);
        }

        public static MvcHtmlString BooleanDropDownList(this HtmlHelper htmlHelper, string name, string trueText, string falseText)
        {
            return BooleanDropDownList(htmlHelper, name, null, trueText, falseText, null);
        }

        public static MvcHtmlString BooleanDropDownList(this HtmlHelper htmlHelper, string name, string trueText, string falseText, object htmlAttributes)
        {
            return BooleanDropDownList(htmlHelper, name, null, trueText, falseText, htmlAttributes);
        }

        public static MvcHtmlString BooleanDropDownList(this HtmlHelper htmlHelper, string name, string emptyText, string trueText, string falseText)
        {
            return BooleanDropDownList(htmlHelper, name, emptyText, trueText, falseText, null);
        }

        public static MvcHtmlString BooleanDropDownList(this HtmlHelper htmlHelper, string name, string emptyText, string trueText, string falseText, object htmlAttributes)
        {
            IList<SelectListItem> items;

            if (emptyText == null)
            {
                items = new List<SelectListItem>
                {
                    new SelectListItem { Text = trueText ?? "Yes", Value = "True" },
                    new SelectListItem { Text = falseText ?? "No", Value = "False" }
                };
            }
            else
            {
                items = new List<SelectListItem>
                {
                    new SelectListItem { Text = emptyText, Value = ""},
                    new SelectListItem { Text = trueText ?? "Yes", Value = "True" },
                    new SelectListItem { Text = falseText ?? "No", Value = "False" }
                };
            }

            return htmlHelper.DropDownList(name, items, htmlAttributes);
        }

        public static MvcHtmlString BooleanDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return BooleanDropDownListFor(htmlHelper, expression, null, null, null, null);
        }

        public static MvcHtmlString BooleanDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string emptyText)
        {
            return BooleanDropDownListFor(htmlHelper, expression, emptyText, null, null, null);
        }

        public static MvcHtmlString BooleanDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, object htmlAttributes)
        {
            return BooleanDropDownListFor(htmlHelper, expression, null, null, null, htmlAttributes);
        }

        public static MvcHtmlString BooleanDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string emptyText, object htmlAttributes)
        {
            return BooleanDropDownListFor(htmlHelper, expression, emptyText, null, null, htmlAttributes);
        }

        public static MvcHtmlString BooleanDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string trueText, string falseText)
        {
            return BooleanDropDownListFor(htmlHelper, expression, null, trueText, falseText, null);
        }

        public static MvcHtmlString BooleanDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string trueText, string falseText, object htmlAttributes)
        {
            return BooleanDropDownListFor(htmlHelper, expression, null, trueText, falseText, htmlAttributes);
        }

        public static MvcHtmlString BooleanDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string emptyText, string trueText, string falseText)
        {
            return BooleanDropDownListFor(htmlHelper, expression, emptyText, trueText, falseText, null);
        }

        public static MvcHtmlString BooleanDropDownListFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string emptyText, string trueText, string falseText, object htmlAttributes)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
            bool? value = null;

            if (metadata != null && metadata.Model != null)
            {
                if (metadata.Model is bool)
                    value = (bool)metadata.Model;
                else if (metadata.Model.GetType() == typeof(bool?))
                    value = (bool?)metadata.Model;
            }

            IList<SelectListItem> items;

            if(emptyText == null)
            {
                items = new List<SelectListItem>
                {
                    new SelectListItem { Text = trueText ?? "Yes", Value = "True", Selected = (value.HasValue && value.Value) },
                    new SelectListItem { Text = falseText ?? "No", Value = "False", Selected = (value.HasValue && !value.Value) }
                };
            }
            else
            {
                items = new List<SelectListItem>
                {
                    new SelectListItem { Text = emptyText, Value = "" },
                    new SelectListItem { Text = trueText ?? "Yes", Value = "True", Selected = (value.HasValue && value.Value) },
                    new SelectListItem { Text = falseText ?? "No", Value = "False", Selected = (value.HasValue && !value.Value) }
                };
            }

            return htmlHelper.DropDownListFor(expression, items, htmlAttributes);
        }

        public static MvcHtmlString BreakRuled(this HtmlHelper html, string text)
        {
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            return webHelper.BreakRuled(text);
        }

        public static string DateTimeFormat(this HtmlHelper html, string dateTimeFormat, DateTime dateTime)
        {
            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            return webHelper.DateTimeFormat(dateTimeFormat, dateTime);
        }

        public static MvcHtmlString DescriptionFor<TModel, TValue>(this HtmlHelper<TModel> self, Expression<Func<TModel, TValue>> expression)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, self.ViewData);
            var description = metadata.Description;

            return MvcHtmlString.Create(string.Format(@"<em class=""hint-text"">{0}</em>", description));
        }

        public static string FieldIdFor<T, TResult>(this HtmlHelper<T> html, Expression<Func<T, TResult>> expression)
        {
            var id = html.ViewData.TemplateInfo.GetFullHtmlFieldId(ExpressionHelper.GetExpressionText(expression));
            return id.Replace('[', '_').Replace(']', '_');
        }

        public static MvcHtmlString GatherLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression)
        {
            return GatherLabelFor(html, expression, null, null);
        }
      
        public static MvcHtmlString GatherLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, string labelText)
        {
            return GatherLabelFor(html, expression, null, labelText);
        }

        public static MvcHtmlString GatherLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes)
        {
            return GatherLabelFor(html, expression, new RouteValueDictionary(htmlAttributes), null);
        }

        public static MvcHtmlString GatherLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object htmlAttributes, string labelText)
        {
            return GatherLabelFor(html, expression, new RouteValueDictionary(htmlAttributes), labelText);
        }

        public static MvcHtmlString GatherLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes)
        {
            return GatherLabelFor(html, expression, new RouteValueDictionary(htmlAttributes), null);
        }

        public static MvcHtmlString GatherLabelFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, IDictionary<string, object> htmlAttributes, string labelText)
        {
            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            bool hasError = false;
            string labelInnerText =  labelText ?? metadata.DisplayName ?? metadata.PropertyName ?? htmlFieldName.Split('.').Last();
            labelInnerText += "";

            if (string.IsNullOrEmpty(labelInnerText))
                return MvcHtmlString.Empty;

            if (html.ViewData.ModelState.ContainsKey(htmlFieldName) && html.ViewData.ModelState[htmlFieldName].Errors.Count > 0)
                hasError = true;

            if (htmlAttributes != null)
            {
                if (hasError)
                {
                    if (htmlAttributes.ContainsKey("class"))
                        htmlAttributes["class"] = htmlAttributes["class"] + " error";
                    else
                        htmlAttributes.Add("class", "error");
                }
            }

            if (metadata.IsRequired)
            {
                var span = new TagBuilder("span");
                span.AddCssClass("required");
                span.SetInnerText("*");
                labelInnerText += " " + span.ToString(TagRenderMode.Normal);
            }

            var tag = new TagBuilder("label");
            tag.MergeAttributes(htmlAttributes);
            tag.Attributes.Add("for", html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(htmlFieldName));
            tag.InnerHtml = labelInnerText;

            return MvcHtmlString.Create(tag.ToString(TagRenderMode.Normal));
        }

        public static MvcHtmlString MenuItem(this HtmlHelper html, string linkText, string routeName)
        {
            var route = (Route)RouteTable.Routes[routeName];
            string routeController = route.Defaults["controller"].ToString();
            string routeAction = route.Defaults["action"].ToString();
            string routeId = route.Defaults.ContainsKey("id") ? route.Defaults["id"].ToString():"";
            string currentController = html.ViewContext.RouteData.GetRequiredString("controller");
            string currentAction = html.ViewContext.RouteData.GetRequiredString("action");
            string currentId = html.ViewContext.RouteData.Values.ContainsKey("id") ? html.ViewContext.RouteData.Values["id"].ToString() : "";

            string classes = "main";
            bool overrideRoute = !string.IsNullOrEmpty(html.ViewBag.CurrentMenuRoute);

            if ((overrideRoute && routeName == html.ViewBag.CurrentMenuRoute) ||
                (routeController == currentController && routeAction == currentAction && currentId == routeId && !overrideRoute))
                classes += " active";

            return html.RouteLink(linkText, routeName, null, new {@class = classes});
        }

        public static string NumberToWords(this int number)
        {
            if (number == 0)
                return "zero";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if ((number/1000000) > 0)
            {
                words += NumberToWords(number/1000000) + " million ";
                number %= 1000000;
            }

            if ((number/1000) > 0)
            {
                words += NumberToWords(number/1000) + " thousand ";
                number %= 1000;
            }

            if ((number/100) > 0)
            {
                words += NumberToWords(number/100) + " hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                if (words != "")
                    words += "and ";

                var unitsMap = new[] {"zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen"};
                var tensMap = new[] {"zero", "ten", "twenty", "thirty", "forty", "fifty", "sixty", "seventy", "eighty", "ninety"};

                if (number < 20)
                {
                    words += unitsMap[number];
                }
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }

        public static string ResizedImageUrl(this HtmlHelper html, string source, int width, int height, int quality = 100, string mode = "crop")
        {
            string querystring = "?width=" + width;
            querystring += "&height=" + height;
            querystring += "&quality=" + quality;
            querystring += "&mode=" + mode;
            querystring += "&scale=both";

            return source + querystring;
        }

    }
}