using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace Covance.TEW_2013.Lists
{
    public abstract class ListDefinition
    {
        public const String TitleField = "Title";
        public const String LinkTitleField = "LinkTitle";
        public const String CreatedByField = "Created By";
        public const String EditField = "Edit";


        public String Title;
        public String Description;
        protected SPList List;


        public void Create(SPWeb web)
        {

            if (String.IsNullOrEmpty(Title))
            {
                throw new ArgumentException("Title of list to create has not been set");
            }

            if (Description == null)
            {
                throw new ArgumentException("Description of list to create has not been set");
            }

            if (web == null)
            {
                throw new NullReferenceException("Site to create list on is null");
            }


            AddList(web);

            if (List == null)
            {
                throw new NullReferenceException("Could not create list");
            }

            AddFields();
            UpdateDefaultView();
            SetAdditionalProperties();
            List.Update();

        }

        protected abstract void AddList(SPWeb web);

        protected virtual void AddFields() { }

        protected virtual void UpdateDefaultView() { }

        protected virtual void SetAdditionalProperties() { }



        public bool ListExists(SPWeb web)
        {
            if (string.IsNullOrEmpty(Title))
            {
                throw new ArgumentException("Title of list to create has not been set");
            }

            SPList list = null;

            try
            {
                list = web.Lists[Title];
            }
            catch (ArgumentException)
            {
                // list not found
            }

            return list != null ? true : false;
        }

        protected void KillTitleField()
        {
            SPField title = List.Fields.GetField(TitleField);
            title.Required = false;
            title.Hidden = true;
            title.Update();
        }

        protected void AddTextField(String FieldName, Boolean Required)
        {
            List.Fields.Add(FieldName, SPFieldType.Text, Required);
        }

        protected void AddNumberField(String FieldName, Boolean Required)
        {
            List.Fields.Add(FieldName, SPFieldType.Number, Required);
        }

        protected void AddMultiLinePlainTextField(String FieldName, Boolean Required)
        {
            List.Fields.Add(FieldName, SPFieldType.Note, Required);
        }

        protected void AddRichTexField(String FieldName, Boolean Required)
        {
            String RichTextFieldName = List.Fields.Add(FieldName, SPFieldType.Note, Required);
            SPFieldMultiLineText RichTextField = (SPFieldMultiLineText)List.Fields.GetFieldByInternalName(RichTextFieldName);
            RichTextField.RichText = true;
            RichTextField.RichTextMode = SPRichTextMode.FullHtml;
            RichTextField.Update();
        }

        protected void AddCheckboxField(String FieldName, Boolean Required, Boolean DefaultValue)
        {
            String CheckboxFieldName = List.Fields.Add(FieldName, SPFieldType.Boolean, Required);
            SPFieldBoolean CheckBoxField = (SPFieldBoolean)List.Fields.GetFieldByInternalName(CheckboxFieldName);

            String DefaultValueString = DefaultValue == true ? "1" : "0";
            CheckBoxField.DefaultValue = DefaultValueString;
        }

        protected void AddChoiceDropdownField(String FieldName, Boolean Required, String[] Choices)
        {
            String ChoiceFieldName = List.Fields.Add(FieldName, SPFieldType.Choice, Required);
            SPFieldChoice ChoiceField = (SPFieldChoice)List.Fields.GetFieldByInternalName(ChoiceFieldName);
            ChoiceField.EditFormat = SPChoiceFormatType.Dropdown;

            if (Choices != null)
            {
                ChoiceField.Choices.AddRange(Choices);
            }

            ChoiceField.Update();
        }

        protected void AddHyperLinkField(String FieldName, Boolean Required)
        {
            String HyperLinkFieldName = List.Fields.Add(FieldName, SPFieldType.URL, Required);
        }

        protected void AddLookupFieldFromListOnSameWeb(String FieldName, ListDefinition LookUpListDef, String LookUpFieldName, Boolean Required)
        {

            SPList LookupList = null;

            try
            {
                LookupList = List.ParentWeb.Lists[LookUpListDef.Title];
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ArgumentOutOfRangeException("Cannot create lookup column " + FieldName + " on " + List.Title + ": " + LookUpListDef.Title + " list not found on " + List.ParentWeb.Url);
            }

            String FieldInternalName = List.Fields.AddLookup(FieldName, LookupList.ID, Required);

            SPFieldLookup LookupField = (SPFieldLookup)List.Fields.GetFieldByInternalName(FieldInternalName);
            LookupField.LookupField = LookupList.Fields[LookUpFieldName].InternalName;
            LookupField.Update();
        }
    }

}
