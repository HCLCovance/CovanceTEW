using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace Covance.TEW_2013.Lists
{
    public class SliderListDefinition : ListDefinition
    {
        public const string LeftField = "Left";
        public const string RightField = "Right";
        public const string OrderField = "Order Number";

        public SliderListDefinition()
        {
            Title = "SliderData";
            Description = "This list contains the information for the home page slider, whic displays capabilities and timing";
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
            AddMultiLinePlainTextField(LeftField, false);
            AddMultiLinePlainTextField(RightField, false);
            AddNumberField(OrderField, false);
        }

        protected override void UpdateDefaultView()
        {
            SPView view = List.DefaultView;
            view.ViewFields.DeleteAll();
            view.ViewFields.Add(EditField);
            view.ViewFields.Add(LeftField);
            view.ViewFields.Add(RightField);
            view.ViewFields.Add(OrderField);
            view.Update();
        }

    }

}
