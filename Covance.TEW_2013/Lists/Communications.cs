using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace Covance.TEW_2013.Lists
{
    public class CommunicationsListDefinition : ListDefinition
    {
        public const string TitleField = "Title";
        public const string DescriptionField = "Description";
        
        public CommunicationsListDefinition()
        {
            Title = "Communications";
            Description = "";
        }

        protected override void AddList(SPWeb web)
        {
            Guid id = web.Lists.Add(Title, Description, SPListTemplateType.DocumentLibrary);
            List = web.Lists[id];
        }

        protected override void AddFields()
        {
            AddMultiLinePlainTextField(DescriptionField, false);
        }

        protected override void UpdateDefaultView()
        {
            SPView view = List.DefaultView;
            view.ViewFields.Add(TitleField);
            view.ViewFields.Add(DescriptionField);
            view.Update();
        }

    }

}
