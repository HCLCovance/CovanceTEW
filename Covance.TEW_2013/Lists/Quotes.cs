using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace Covance.TEW_2013.Lists
{
    public class QuotesListDefinition : ListDefinition
    {
        public const string ProjectField = "Project";
        public const string QuoteField = "Quote";
        public const string UserNameField = "User Name";
        public const string RoleField = "Role";
        public const string FromField = "From";

        public const string FromChoiceUser = "User";
        public const string FromChoiceTeam = "Team";


        public QuotesListDefinition()
        {
            Title = "Quotes";
            Description = "Use this list to store quotes from the Users or Teams";
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

            String[] FromChoices = new String[]
            {
              FromChoiceUser,
              FromChoiceTeam
            };

            AddLookupFieldFromListOnSameWeb(ProjectField, new ProjectsListDefinition(), ProjectsListDefinition.ProjectTitleField, true);
            AddTextField(QuoteField, true);
            AddTextField(UserNameField, true);
            AddTextField(RoleField, true);
            AddChoiceDropdownField(FromField, true, FromChoices);
        }

        protected override void UpdateDefaultView()
        {
            SPView view = List.DefaultView;
            view.ViewFields.DeleteAll();
            view.ViewFields.Add(EditField);
            view.ViewFields.Add(ProjectField);
            view.ViewFields.Add(QuoteField);
            view.ViewFields.Add(UserNameField);
            view.ViewFields.Add(RoleField);
            view.ViewFields.Add(FromField);
            view.Update();
        }

    }

}
