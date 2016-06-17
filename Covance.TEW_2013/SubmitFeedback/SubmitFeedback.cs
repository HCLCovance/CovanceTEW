using System;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Covance.TEW_2013.Lists;

namespace Covance.TEW_2013.SubmitFeedback
{
    [ToolboxItemAttribute(false)]
    public class SubmitFeedback : WebPart
    {
        //Display strings
        protected static readonly String ChooseProject = "Choose Technology";
        protected static readonly String NullFeedbackMessage = "Enter your message here";
        protected static readonly String FeedbackTypeLabelText = "My message is a: ";
        protected static readonly String MessageTypeRadioGroup = "FeedbackType";
        protected static readonly String Error_NoProject = "Feedback not submitted:<br/>Please choose a project from the 'Choose Project' dropdown";
        protected static readonly String Error_NoFeedback = "Feedback not submitted:<br/> Please enter a message in the text box";
        protected static readonly String Result_Success = "Feedback successfully submitted";

        //SharePoint Data
        SPList FeedbackList = null;
        SPList ProjectList = null;
        SPListItemCollection ProjectItems = null;
        SPQuery oquery = new SPQuery();

        //controls
        DropDownList ddlProject;
        TextBox txtFeedback;
        RadioButton rdoSuggestion;
        RadioButton rdoTip;
        RadioButton rdoQuestion;
        RadioButton rdoComment;
        Label lblResult;

        public SubmitFeedback()
        {
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            try
            {
                FeedbackList = GetListFromSiteRoot(new FeedbackListDefinition().Title);
                ProjectList = GetListFromSiteRoot(new ProjectsListDefinition().Title);
                oquery.Query = "<OrderBy><FieldRef Name ='Project_x0020_Title' Ascending='TRUE' /></OrderBy>";
                ProjectItems = GetListItems(ProjectList, oquery);
                CreateFeedbackForm();

                if (!Page.IsPostBack)
                {
                    ResetControls();
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }
        }
        protected SPList GetListFromSiteRoot(String ListName)
        {
            SPList List = null;
            try
            {
                List = SPContext.Current.Web.Site.RootWeb.Lists[ListName];
            }
            catch
            {
                throw new Exception("List '" + ListName + "' does not exist on site collection root");
            }

            return List;
        }

        protected String GetInternalFieldName(SPList List, String DisplayName)
        {
            if (List == null) throw new Exception("Null list passed to: GetInternalFieldName()");

            String InternalName = "";

            try
            {
                InternalName = List.Fields.GetField(DisplayName).InternalName;
            }
            catch
            {
                throw new Exception("'" + DisplayName + "' field not found in list " + List.Title + " at " + List.ParentWebUrl);
            }

            return InternalName;

        }

        protected SPListItemCollection GetListItems(SPList List, SPQuery Query)
        {
            if (List == null) throw new Exception("Null list passed to: GetListItems()");

            SPListItemCollection ListItems = null;

            if (Query == null)
            {
                ListItems = List.Items;
            }
            else
            {
                ListItems = List.GetItems(Query);
            }

            if (ListItems == null || ListItems.Count <= 0)
            {
                throw new Exception("No suitable items were found in list '" + List.Title + "' on the site collection root");
            }

            return ListItems;
        }

        void btnSubmit_Click(object sender, EventArgs e)
        {
            SPWeb Web = SPContext.Current.Web;

            try
            {
                Web.AllowUnsafeUpdates = true;

                if (ValidateProject() && ValidateFeedback())
                {
                    //get The Message Type
                    String MessageType = GetMessageType();

                    //get the project (lookup)
                    String ProjectInternalFieldName = GetInternalFieldName(ProjectList, ProjectsListDefinition.ProjectTitleField);
                    SPQuery ProjectQuery = new SPQuery();
                    ProjectQuery.Query = "<Where><Eq><FieldRef Name='" + ProjectInternalFieldName + "'/><Value Type='Text'>" + ddlProject.Text + "</Value></Eq></Where>";

                    SPListItemCollection ProjectItems = ProjectList.GetItems(ProjectQuery);
                    SPFieldLookup ProjectLookupField = ProjectItems[0].Fields.GetFieldByInternalName(ProjectInternalFieldName) as SPFieldLookup;
                    int ProjectLookupID = ProjectItems[0].ID;

                    SPListItem FeedbackItem = FeedbackList.Items.Add();

                    FeedbackItem[FeedbackListDefinition.MessageTypeField] = FeedbackList.Fields[FeedbackListDefinition.MessageTypeField].GetFieldValue(MessageType);
                    FeedbackItem[FeedbackListDefinition.ProjectField] = new SPFieldLookupValue(ProjectLookupID, ddlProject.Text);
                    FeedbackItem[FeedbackListDefinition.MessageField] = txtFeedback.Text;
                    FeedbackItem[FeedbackListDefinition.DisplayField] = "0";

                    FeedbackItem.Update();

                    ResetControls();
                    lblResult.Text = Result_Success;
                    lblResult.CssClass = "success";
                }
            }
            catch (Exception ex)
            {
                DisplayError(ex.Message);
            }
            finally
            {
                Web.AllowUnsafeUpdates = false;
            }
        }


        protected bool ValidateProject()
        {
            bool Valid = true;

            if (String.IsNullOrEmpty(ddlProject.Text) || ddlProject.Text == ChooseProject)
            {
                lblResult.Text = Error_NoProject;
                lblResult.CssClass = "error";
                Valid = false;
            }

            return Valid;
        }

        protected bool ValidateFeedback()
        {
            bool Valid = true;

            if (String.IsNullOrEmpty(txtFeedback.Text) || txtFeedback.Text == NullFeedbackMessage)
            {
                lblResult.Text = Error_NoFeedback;
                lblResult.CssClass = "error";
                Valid = false;
            }

            return Valid;
        }

        protected void ResetControls()
        {
            lblResult.Text = "";
            txtFeedback.Text = NullFeedbackMessage;

            rdoComment.Checked = true;
            rdoQuestion.Checked = false;
            rdoSuggestion.Checked = false;
            rdoTip.Checked = false;

            ddlProject.SelectedIndex = 0;
        }

        protected void CreateFeedbackForm()
        {
            //Project Dropdown

            ddlProject = new DropDownList();
            ddlProject.CssClass = "SubmitFeedbackDropDown";
            ddlProject.Items.Add(ChooseProject);

            foreach (SPListItem ProjectItem in ProjectItems)
            {
                try
                {
                    ddlProject.Items.Add(ProjectItem[ProjectsListDefinition.ProjectTitleField].ToString());
                }
                catch
                { }
            }

            TableCell ProjectColumn = new TableCell();
            ProjectColumn.CssClass = "SubmitFeedbackProjectSelectColumn";
            ProjectColumn.Controls.Add(ddlProject);
            WebControl ProjectRow = new WebControl(System.Web.UI.HtmlTextWriterTag.Tr);
            ProjectRow.CssClass = "SubmitFeedbackProjectSelectRow";
            ProjectRow.Controls.Add(ProjectColumn);

            //Feedback Type Radio Buttons

            Label lblFeedbackType = new Label();
            lblFeedbackType.Text = FeedbackTypeLabelText;
            WebControl FeedbackTypeLabelDIV = new WebControl(System.Web.UI.HtmlTextWriterTag.Div);
            FeedbackTypeLabelDIV.CssClass = "SubmitFeedbackFeedbackLabelArea";
            FeedbackTypeLabelDIV.Controls.Add(lblFeedbackType);

            rdoSuggestion = new RadioButton();
            rdoSuggestion.CssClass = "SubmitFeedbackRadioButton";
            rdoSuggestion.GroupName = MessageTypeRadioGroup;
            rdoSuggestion.Text = FeedbackListDefinition.MessageTypeChoiceSuggestion;
            WebControl FeedbackTypeSuggestionDiv = new WebControl(System.Web.UI.HtmlTextWriterTag.Div);
            FeedbackTypeSuggestionDiv.CssClass = "SubmitFeedbackRadioButtonArea";
            FeedbackTypeSuggestionDiv.Controls.Add(rdoSuggestion);

            rdoTip = new RadioButton();
            rdoTip.CssClass = "SubmitFeedbackRadioButton";
            rdoTip.GroupName = MessageTypeRadioGroup;
            rdoTip.Text = FeedbackListDefinition.MessageTypeChoiceTip;
            WebControl FeedbackTypeTipDiv = new WebControl(System.Web.UI.HtmlTextWriterTag.Div);
            FeedbackTypeTipDiv.CssClass = "SubmitFeedbackRadioButtonArea";
            FeedbackTypeTipDiv.Controls.Add(rdoTip);

            rdoQuestion = new RadioButton();
            rdoQuestion.CssClass = "SubmitFeedbackRadioButton";
            rdoQuestion.GroupName = MessageTypeRadioGroup;
            rdoQuestion.Text = FeedbackListDefinition.MessageTypeChoiceQuestion;
            WebControl FeedbackTypeQuestionDiv = new WebControl(System.Web.UI.HtmlTextWriterTag.Div);
            FeedbackTypeQuestionDiv.CssClass = "SubmitFeedbackRadioButtonArea";
            FeedbackTypeQuestionDiv.Controls.Add(rdoQuestion);

            rdoComment = new RadioButton();
            rdoComment.CssClass = "SubmitFeedbackRadioButton";
            rdoComment.GroupName = MessageTypeRadioGroup;
            rdoComment.Text = FeedbackListDefinition.MessageTypeChoiceComment;
            WebControl FeedbackTypeCommentDiv = new WebControl(System.Web.UI.HtmlTextWriterTag.Div);
            FeedbackTypeCommentDiv.CssClass = "SubmitFeedbackRadioButtonArea";
            FeedbackTypeCommentDiv.Controls.Add(rdoComment);

            WebControl FeedbackTypeColumn = new WebControl(System.Web.UI.HtmlTextWriterTag.Td);
            FeedbackTypeColumn.CssClass = "SubmitFeedbackFeedbackTypeColumn";
            FeedbackTypeColumn.Controls.Add(FeedbackTypeLabelDIV);
            FeedbackTypeColumn.Controls.Add(FeedbackTypeSuggestionDiv);
            FeedbackTypeColumn.Controls.Add(FeedbackTypeTipDiv);
            FeedbackTypeColumn.Controls.Add(FeedbackTypeQuestionDiv);
            FeedbackTypeColumn.Controls.Add(FeedbackTypeCommentDiv);

            WebControl FeedbackTypeRow = new WebControl(System.Web.UI.HtmlTextWriterTag.Tr);
            FeedbackTypeRow.CssClass = "SubmitFeedbackTypeRow";
            FeedbackTypeRow.Controls.Add(FeedbackTypeColumn);


            //feedback textbox

            txtFeedback = new TextBox();
            txtFeedback.CssClass = "SubmitFeedbackTextBox";
            txtFeedback.TextMode = TextBoxMode.MultiLine;
            txtFeedback.Attributes["onclick"] = "if(this.value=='" + NullFeedbackMessage + "')this.value='';";
            TableCell FeedbackTextColumn = new TableCell();
            FeedbackTextColumn.CssClass = "SubmitFeedbackFeedbackTextColumn";
            FeedbackTextColumn.Controls.Add(txtFeedback);
            WebControl FeedbackTextRow = new WebControl(System.Web.UI.HtmlTextWriterTag.Tr);
            FeedbackTextRow.CssClass = "SubmitFeedbackFeedbackTextRow";
            FeedbackTextRow.Controls.Add(FeedbackTextColumn);

            //submit button

            Button btnSubmit = new Button();
            btnSubmit.CssClass = "SubmitFeedbackButton";
            btnSubmit.Text = "Send";
            btnSubmit.Click += new EventHandler(btnSubmit_Click);
            TableCell SubmitButtonColumn = new TableCell();
            SubmitButtonColumn.CssClass = "SubmitFeedbackSubmitButtonColumn";
            SubmitButtonColumn.Controls.Add(btnSubmit);
            WebControl SubmitButtonRow = new WebControl(System.Web.UI.HtmlTextWriterTag.Tr);
            SubmitButtonRow.CssClass = "SubmitFeedbackSubmitButtonRow";
            SubmitButtonRow.Controls.Add(SubmitButtonColumn);

            //result label
            lblResult = new Label();
            WebControl ResultDiv = new WebControl(System.Web.UI.HtmlTextWriterTag.Div);
            ResultDiv.CssClass = "SubmitFeedbackResult";
            ResultDiv.Controls.Add(lblResult);

            //Feedback Form Table

            WebControl FeedbackTable = new WebControl(System.Web.UI.HtmlTextWriterTag.Table);
            FeedbackTable.CssClass = "SubmitFeedbackTable";
            FeedbackTable.Controls.Add(ProjectRow);
            FeedbackTable.Controls.Add(FeedbackTypeRow);
            FeedbackTable.Controls.Add(FeedbackTextRow);
            FeedbackTable.Controls.Add(SubmitButtonRow);


            this.Controls.Add(FeedbackTable);
            this.Controls.Add(ResultDiv);
        }

        protected String GetMessageType()
        {
            string MessageType = "";

            if (rdoTip.Checked)
            {
                MessageType = FeedbackListDefinition.MessageTypeChoiceTip;
            }
            else if (rdoSuggestion.Checked)
            {
                MessageType = FeedbackListDefinition.MessageTypeChoiceSuggestion;
            }
            else if (rdoQuestion.Checked)
            {
                MessageType = FeedbackListDefinition.MessageTypeChoiceQuestion;
            }
            else if (rdoComment.Checked)
            {
                MessageType = FeedbackListDefinition.MessageTypeChoiceComment;
            }

            return MessageType;
        }
        protected void DisplayError(string ErrorText)
        {
            this.Controls.Clear();
            Label lblError = new Label();
            lblError.Text = ErrorText;

            WebControl ErrorContainer = new WebControl(System.Web.UI.HtmlTextWriterTag.Div);
            ErrorContainer.ForeColor = System.Drawing.Color.Red;
            ErrorContainer.Controls.Add(lblError);
            this.Controls.Add(ErrorContainer);
        }



    }
}
