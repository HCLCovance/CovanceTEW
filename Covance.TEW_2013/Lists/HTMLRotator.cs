using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace Covance.TEW_2013.Lists
{
    public class HTMLRotatorListDefinition : ListDefinition
    {
        public const string ContentField = "Content";
        public const string ReadMoreField = "Read More";
        public const string OrderField = "Order Number";

        public HTMLRotatorListDefinition()
        {
            Title = "HTMLRotator";
            Description = "This list contains the rotating HTML content for the home page";
        }

        protected override void AddList(SPWeb web)
        {
            Guid id = web.Lists.Add(Title, Description, SPListTemplateType.GenericList);
            List = web.Lists[id];
            List.EnableAttachments = false;
            List.Update();
        }

        protected override void AddFields()
        {
            KillTitleField();
            AddRichTexField(ContentField, false);
            AddTextField(ReadMoreField, false);
            AddNumberField(OrderField, false);
        }

        protected override void UpdateDefaultView()
        {
            SPView view = List.DefaultView;
            view.ViewFields.DeleteAll();
            view.ViewFields.Add(EditField);
            view.ViewFields.Add(ContentField);
            view.ViewFields.Add(ReadMoreField);
            view.ViewFields.Add(OrderField);
            view.Update();
        }

    }

}
