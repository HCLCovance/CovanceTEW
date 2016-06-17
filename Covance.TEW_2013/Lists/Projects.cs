using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace Covance.TEW_2013.Lists
{
    public class ProjectsListDefinition : ListDefinition
    {
        public const string ProjectTitleField = "Project Title";

        public ProjectsListDefinition()
        {
            Title = "Projects";
            Description = "This list stored the all the Covance Projects";
        }

        protected override void AddList(SPWeb web)
        {
            Guid id = web.Lists.Add(Title, Description, SPListTemplateType.GenericList);
            List = web.Lists[id];
            List.EnableAttachments = false;
            List.Hidden = true;

            #if DEBUG
                List.Hidden = false;
            #endif
    
            List.Update();
        }

        protected override void AddFields()
        {
            KillTitleField();
            AddTextField(ProjectTitleField, true);
        }

        protected override void UpdateDefaultView()
        {
            SPView view = List.DefaultView;
            view.ViewFields.DeleteAll();
            view.ViewFields.Add(EditField);
            view.ViewFields.Add(ProjectTitleField);
            view.Update();
        }

    }

}
