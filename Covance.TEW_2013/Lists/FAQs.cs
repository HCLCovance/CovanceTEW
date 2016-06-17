using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace Covance.TEW_2013.Lists
{
    public class FAQsListDefinition : ListDefinition
    {
        public const string QuestionField = "Question";
        public const string AnswerField = "Answer";
        public const string CategoryField = "Category";
        public const string ProjectField = "Project";
        public const string DidYouKnowField = "Did You Know";
        public const string DidYouKnowHyperLinkField = "Did You Know Link";
        public const string OrderField = "Order Number";
        public const string LikesField = "Likes";

        public FAQsListDefinition()
        {
            Title = "FAQs";
            Description = "Use this list to store FAQs for all projects";
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

            AddLookupFieldFromListOnSameWeb(ProjectField, new ProjectsListDefinition(), ProjectsListDefinition.ProjectTitleField, true);
            AddMultiLinePlainTextField(QuestionField, true);
            AddRichTexField(AnswerField, true);

            AddTextField(DidYouKnowField, false);
            AddHyperLinkField(DidYouKnowHyperLinkField, false);
            AddChoiceDropdownField(CategoryField, false, null);
            AddNumberField(OrderField, false);
            AddNumberField(LikesField, false);

        }

        protected override void UpdateDefaultView()
        {
            SPView view = List.DefaultView;
            view.ViewFields.DeleteAll();
            view.ViewFields.Add(EditField);
            view.ViewFields.Add(ProjectField);
            view.ViewFields.Add(QuestionField);
            view.ViewFields.Add(AnswerField);
            view.ViewFields.Add(CategoryField);
            view.ViewFields.Add(DidYouKnowField);
            view.ViewFields.Add(DidYouKnowHyperLinkField);
            view.ViewFields.Add(OrderField);
            view.Update();
        }

    }

}
