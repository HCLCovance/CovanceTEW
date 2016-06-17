using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace Covance.TEW_2013.Lists
{
    public class LikesListDefinition : ListDefinition
    {
        public const string UserNameField = "UserName";
        public const string ListIDField = "ListID";
        public const string ItemIDField = "ItemID";


        public LikesListDefinition()
        {
            Title = "Likes";
            Description = "";
        }

        protected override void AddList(SPWeb web)
        {
            Guid id = web.Lists.Add(Title, Description, SPListTemplateType.GenericList);
            List = web.Lists[id];
            List.Hidden = true;

            #if DEBUG
                List.Hidden = false;
            #endif

            List.Update();
        }

        protected override void AddFields()
        {
            KillTitleField();
            AddTextField(UserNameField, true);
            AddTextField(ListIDField, true);
            AddTextField(ItemIDField, true);
        }

        protected override void UpdateDefaultView()
        {
            SPView view = List.DefaultView;
            view.ViewFields.DeleteAll();
            view.ViewFields.Add(UserNameField);
            view.ViewFields.Add(ListIDField);
            view.ViewFields.Add(ItemIDField);
            view.Update();
        }

    }

}
