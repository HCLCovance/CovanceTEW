using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace Covance.TEW_2013.Lists
{
    public class ProjectContactsListDefinition : ListDefinition
    {
        public const string RoleField = "Role";
        public const string UserNameField = "User Name";
        public const string EmailField = "Email";
        public const string ProjectField = "Project";
        public const string ImageUrlField = "Image URL";


        public ProjectContactsListDefinition()
        {
            Title = "Project Contacts";
            Description = "Use this list to add site contacts for projects";
        }

        protected override void AddList(SPWeb web)
        {
            Guid id = web.Lists.Add(Title, Description, SPListTemplateType.GenericList);
            List = web.Lists[id];
        }

        protected override void AddFields()
        {
            KillTitleField();

            AddLookupFieldFromListOnSameWeb(ProjectField, new ProjectsListDefinition(), ProjectsListDefinition.ProjectTitleField, true);
            AddTextField(RoleField, true);
            AddTextField(UserNameField, true);
            AddTextField(EmailField, true);
            AddTextField(ImageUrlField, false);
        }

        protected override void UpdateDefaultView()
        {
            SPView view = List.DefaultView;
            view.ViewFields.Delete(LinkTitleField);
            view.ViewFields.Add(EditField);
            view.ViewFields.Add(ProjectField);
            view.ViewFields.Add(RoleField);
            view.ViewFields.Add(UserNameField);
            view.ViewFields.Add(EmailField);
            view.ViewFields.Add(ImageUrlField);
            view.Update();
        }

    }

}
