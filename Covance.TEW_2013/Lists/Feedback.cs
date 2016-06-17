using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace Covance.TEW_2013.Lists
{
    public class FeedbackListDefinition : ListDefinition
    {
        public const string ProjectField = "Project";
        public const string MessageTypeField = "Message Type";
        public const string MessageField = "Message";
        public const string DisplayField = "Display";

        public const string MessageTypeChoiceSuggestion = "Suggestion";
        public const string MessageTypeChoiceTip = "Tip";
        public const string MessageTypeChoiceQuestion= "Question";
        public const string MessageTypeChoiceComment = "Comment";

        public FeedbackListDefinition()
        {
            Title = "Feedback";
            Description = "This list will stored the feedback items sumbmitted through the website";
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

            String[] MessageTypes = new String[]
            {
              MessageTypeChoiceSuggestion, 
              MessageTypeChoiceTip, 
              MessageTypeChoiceQuestion,
              MessageTypeChoiceComment  
            };

            AddLookupFieldFromListOnSameWeb(ProjectField, new ProjectsListDefinition(), ProjectsListDefinition.ProjectTitleField, true);
            AddChoiceDropdownField(MessageTypeField, true, MessageTypes);
            AddMultiLinePlainTextField(MessageField, true);
            AddCheckboxField(DisplayField, false, false);
        }

        protected override void UpdateDefaultView()
        {
            SPView view = List.DefaultView;
            view.ViewFields.DeleteAll();
            view.ViewFields.Add(EditField);
            view.ViewFields.Add(ProjectField);
            view.ViewFields.Add(MessageTypeField);
            view.ViewFields.Add(MessageField);
            view.ViewFields.Add(DisplayField);
            view.ViewFields.Add(CreatedByField);
            view.Update();
        }

    }

}
