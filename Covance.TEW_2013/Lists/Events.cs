using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;

namespace Covance.TEW_2013.Lists
{
    public class EventsListDefinition : ListDefinition
    {

        public EventsListDefinition()
        {
            Title = "Events";
            Description = "User this list to store upcoming events";
        }

        protected override void AddList(SPWeb web)
        {
            Guid id = web.Lists.Add(Title, Description, SPListTemplateType.Events);
            List = web.Lists[id];
        }

    }

}
